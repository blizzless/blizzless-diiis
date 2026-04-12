---
epoch: 1773011296182
modelKey: "gpt-5|openai"
topic: "Refine Prompt Engineering and Tables"


tags:
  - copilot-conversation
---

**user**: More complexity and clarity. Add good prompt engineering and add informations about the other tables in the markdown text above. Better prompt engineering
[Context: Notes: ZAsset Specs.md]
[Timestamp: 2026/03/08 20:08:16]

**ai**: # ZAsset

ZAsset is a Stock and Asset Management System.

Must be written with latest C# and .NET 10 in Blazor Web App (Server-side). Use Tailwind exclusively:
- Include https://cdn.tailwindcss.com in the host page.
- Remove Bootstrap entirely (packages, links, classes).
- Override all default scaffolding to Tailwind components (forms, tables, dialogs, layout, toasts).
- Prefer semantic HTML with accessible Tailwind patterns.

## Non-Functional Requirements

- Multitenancy by Site: All data access must be scoped by SiteId using EF Core Global Query Filters.
- Security: Microsoft Identity. Role-based authorization (Admin, Manager, Operator, Auditor). Claims: SiteId, IsGlobal.
- Concurrency: Use optimistic concurrency with rowversion on mutable entities (Asset, AssetTransaction, Location).
- Auditing: Full create/update/delete audit trail. Store user, timestamp, IP (when available).
- Localization: English by default; make labels/localizable via resource files.
- Accessibility: WCAG AA compliant. Keyboard navigable modals, focus traps, ARIA.
- Observability: Structured logs (Serilog), distributed tracing-friendly correlation IDs, EF Core logging.
- Performance: Indexes on SiteId + foreign keys + frequently filtered columns. Paginated queries with server-side filtering.
- Validation: FluentValidation for server-side; client-side validation with DataAnnotations and tailored error UI.
- File storage: Store metadata in DB, binary in blob/file store. Virus scan before attach (pluggable provider).

## Authentication and Authorization

- Microsoft Identity with ApplicationUser extended profile: FirstName, LastName, DisplayName, IsGlobal (bool), DefaultSiteId (nullable).
- Login flow:
  - If user.IsGlobal == true: prompt Site selection after login.
  - If user.IsGlobal == false: auto-redirect to user.DefaultSiteId; hide Site switcher.
- Authorization policies:
  - RequireSiteContext: Route/access only if a Site is selected/resolved.
  - SiteScopedWrite: Enforce SiteId equality on writes.

## Domain Model (Entities & Tables)

All entities are Site-scoped unless explicitly stated. Include common fields: Id (GUID), SiteId (GUID, nullable if global), CreatedAtUtc, CreatedBy, UpdatedAtUtc, UpdatedBy, IsDeleted (soft delete where appropriate), RowVersion (rowversion/byte[]).

### Tables Overview |

| Table name | Functionality |
| --- | --- |
| Site | Represents a tenant boundary. Created/managed only through the “Site” page. Users may be global or bound to a default Site. |
| Warehouse | Warehouse within a Site. All navigation must ignore Warehouses from other Sites. |
| Location | Physical location in a Warehouse (format: Rack, Shelf — e.g., R1-S2). Supports hierarchical sublocations optional (e.g., bin). |
| Product | Canonical product definition an Asset can refer to. Has ID (format XX-YY-ZZ-11), SKU, Description, Cost, UoM. |
| Asset | Trackable entity, may have a parent (hierarchy). Can link to multiple Products (many-to-many). Holds state, quantity, and location. |
| AssetTransaction | Movement and lifecycle events for an Asset. Move within same Warehouse → Status.Move. Cross-Site/Warehouse → Status.Transfer. Drives “Goods In” workflow and timeline. |
| Assignee | Association to ApplicationUser for responsibility tracking. Transfers don't have assignees. History-aware (from/to dates). |
| AssetProduct | Join table Asset↔Product with quantities or metadata per association. |
| Attachment | Files attached to Assets or Transactions (images, PDFs, docs). |
| Tag | Flexible tagging for Assets and Products. |
| AuditLog | Immutable log of entity changes. |
| InventoryAdjustment | Manual corrections (gain/loss) with reason codes and approvals. |
| Supplier | Optional vendor/supplier reference for Products and inbound transfers. |
| UnitOfMeasure | Normalized UoM reference (EA, BOX, KG…). |
| Notification | User notifications (transfer received, action required). |
| Setting | Site-level configuration (e.g., location code pattern, mandatory fields). |

### Key Entities (Fields, Rules)

- Site
  - Id, Name (unique), Code (short), IsActive
  - Rules: Cannot delete if Warehouses exist. Unique Code.

- Warehouse
  - Id, SiteId, Name (unique within Site), Code, IsActive
  - Indexes: (SiteId, Name), (SiteId, Code)
  - Rules: Cannot delete if Locations/Assets present.

- Location
  - Id, SiteId, WarehouseId, Code (e.g., R1-S2), Rack, Shelf, Bin (optional), IsActive, RowVersion
  - Unique: (SiteId, WarehouseId, Code)
  - Rules: Code must comply with Setting.LocationPattern if provided.

- UnitOfMeasure
  - Id, Code (EA, KG…), Name
  - Unique: Code
  - Seed common UoMs.

- Product
  - Id (GUID), ProductCode (format: XX-YY-ZZ-11), SKU, Description, DefaultCost, UoMId, IsActive
  - Unique: (SiteId, ProductCode), (SiteId, SKU)
  - Rules: ProductCode format validation; DefaultCost >= 0.

- Asset
  - Id, SiteId, WarehouseId, LocationId (nullable), ParentAssetId (nullable), Name, SerialNumber (nullable), Quantity (decimal), UoMId, State (enum), StatusNote, RowVersion
  - Hierarchy: ParentAssetId optional; siblings inferred via same ParentAssetId.
  - Rules: Quantity > 0; If LocationId null → AssetStockState = NotStored. If ParentAssetId set, WarehouseId/LocationId must match parent unless explicitly overridden by a movement transaction (enforced via service). SerialNumber unique per Site when provided.

- AssetProduct
  - AssetId, ProductId, Quantity (decimal), Notes
  - Unique: (AssetId, ProductId)
  - Rules: Quantity >= 0.

- Assignee
  - Id, AssetId, UserId (ApplicationUser.Id), FromUtc, ToUtc (nullable), Notes
  - Rules: No overlapping assignments for same Asset and User. Current assignee has ToUtc null.

- AssetTransaction
  - Id, SiteId, AssetId, FromWarehouseId, ToWarehouseId (nullable), FromLocationId (nullable), ToLocationId (nullable)
  - Type (enum): Create, Move, Transfer, Receive, Store, Adjust, Assign, Unassign, Split, Merge, Dispose
  - Status (enum): Draft, Pending, InTransit, Received, NotStored, Stored, Completed, Cancelled
  - QuantityDelta (decimal; positive/negative per action)
  - PerformedBy (UserId), PerformedAtUtc
  - CorrelationId (to group multi-step flows, e.g., Transfer + Receive)
  - Notes, RowVersion
  - Indexes: (SiteId, AssetId, PerformedAtUtc DESC), (SiteId, Status)
  - Rules: Enforce valid status transitions (see Workflow).

- InventoryAdjustment
  - Id, SiteId, AssetId, QuantityDelta, ReasonCode, RequestedBy, ApprovedBy (nullable), ApprovedAtUtc (nullable), Status (Requested, Approved, Rejected, Applied)

- Attachment
  - Id, SiteId, EntityType (Asset/AssetTransaction/Product), EntityId, FileName, ContentType, Size, StorageUri, UploadedBy, UploadedAtUtc, Hash

- Tag
  - Id, SiteId, Name
  - TagMap: (TagId, EntityType, EntityId)

- Supplier
  - Id, SiteId, Name, Code, Contact, IsActive

- Notification
  - Id, SiteId, UserId, Type, Title, Body, IsRead, CreatedAtUtc, Link

- Setting
  - Id, SiteId, Key, Value (JSON), UpdatedAtUtc

### Enums

- AssetStockState: NotStored, Stored
- AssetTransactionType: Create, Move, Transfer, Receive, Store, Adjust, Assign, Unassign, Split, Merge, Dispose
- AssetTransactionStatus: Draft, Pending, InTransit, Received, NotStored, Stored, Completed, Cancelled

## Workflows

- Site selection
  - Global user: Select Site after login. Persist selection per session and profile.
  - Non-global user: Auto-redirect to DefaultSiteId, Site switcher hidden.

- Asset movement
  - Within same Warehouse:
    - Create AssetTransaction Type=Move, Status=Pending → Completed, update LocationId to ToLocationId, AssetStockState=Stored.
  - Cross-Warehouse (same Site) or Cross-Site:
    - Create AssetTransaction Type=Transfer, Status=InTransit, FromLocationId set, ToWarehouseId (and optionally ToLocationId null initially).
    - Asset is not available in destination until Receive.

- Goods In
  - Page lists incoming transfers (Status=InTransit, Type=Transfer) filtered by current Site/Warehouse.
  - Operator selects transfer, chooses final Location, confirms:
    - System appends Receive event (Type=Receive, Status=Received) then Store (Type=Store, Status=Stored).
    - AssetStockState transitions: Received → NotStored → Stored.
    - Update Asset WarehouseId and LocationId to destination.
    - Close correlation group as Completed.

- History & timeline
  - Asset detail shows chronological AssetTransaction entries in a Tailwind timeline with badges for Type/Status, locations, operators, notes, and attachments.

- Assignee
  - Assign: Create Assign transaction; Assignee record with FromUtc now; close previous active ToUtc.
  - Unassign: Create Unassign transaction; set ToUtc on current.

- Adjustments
  - InventoryAdjustment requested → approved → applied produces an Adjust transaction with QuantityDelta.

- Hierarchy
  - Parent/child relationships navigable as a tree. Moves of parent prompt to cascade or split children with explicit confirmation.

## UI (Blazor Server + Tailwind)

- Layout
  - Left sidebar: Sites (if global), Warehouses, Pages.
  - Top bar: Search, Quick actions, User menu.
  - Use Heroicons/Phosphor via CDN. Dark mode toggle via Tailwind.

- Pages
  - Site: CRUD, user bindings, settings.
  - Warehouse: CRUD per Site.
  - Location: CRUD with code builder (R/S/B), map/grid visualization, QR/Barcode print.
  - Products: List, details, tags, attachments.
  - Assets: List with filters (Warehouse, Location, Tags, Product, Assignee, State). Bulk actions (move, assign).
  - Asset Detail: Summary, hierarchy tree, products tab, timeline, attachments, assignees, tags.
  - Transactions: Global view with filters and export.
  - Goods In: Receive transfers, assign location, print labels.
  - Adjustments: Request/approve/apply.
  - Admin: Users/Roles, Settings, Audit Logs.

- Components
  - AssetTimeline, AssetTree, LocationPicker (typeahead + map), AssigneePicker, TagChip, FileUpload, Toast/Notification bell, ConfirmDialog.

## Data Access & Rules

- EF Core with Global Query Filter on SiteId for all Site-scoped entities.
- Repositories or direct DbContext with domain services enforcing invariants (status transitions, hierarchy move rules).
- Transactions: Wrap multi-step operations in DbTransaction with outbox pattern for notifications.
- Indexing
  - Common: (SiteId, Code/Name), (SiteId, WarehouseId, Code), (SiteId, AssetId, PerformedAtUtc DESC)
  - Full-text on Product.Description and Asset.Name if available.

## Validation & Status Transitions

- Allowed transitions (simplified):
  - Draft → Pending → InTransit → Received → NotStored → Stored → Completed
  - Pending → Cancelled
  - Adjust: Requested → Approved/Rejected → Applied
- Enforce on server with clear error messages. UI disables invalid actions.

## Tailwind-First Styling Guidance

- Forms: grid-cols-6 responsive layouts, rounded inputs, focus-visible styles.
- Tables: sticky header, zebra rows, responsive overflow-x, row selection.
- Timeline: vertical rule with color-coded badges per Type/Status.
- Badges: Move=blue, Transfer=amber, Receive=green, Store=emerald, Adjust=violet, Cancelled=rose.

## Security

- Roles:
  - Admin: Full access per Site.
  - Manager: CRUD assets/products/locations, approve adjustments, receive transfers.
  - Operator: Create/move/receive assets, attach files.
  - Auditor: Read-only, export.
- Claims:
  - IsGlobal: true/false
  - AllowedSites: list of SiteIds where applicable.
- Row-level enforcement: On write, verify command.SiteId == current Site.

## Logging, Auditing, Notifications

- AuditLog on entity changes with old/new value diffs (property name + values).
- Notifications for:
  - Incoming transfers to a Warehouse.
  - Adjustments awaiting approval.
  - Failed background operations.
- Users can opt-in/out per notification type in Settings.

## Testing

- Unit tests for domain services (status transitions, hierarchy moves).
- Integration tests on EF Core with SQLite in-memory.
- Playwright/BDD specs for Goods In and Move flows.
- Seed data builders for Sites, Warehouses, Locations.

## Prompt Engineering Pack (Copy-Paste Ready)

Use these prompts with your codegen assistant. Always include “Constraints” and “Definition of Done”.

- System prompt: Architecture baseline
  - You are a senior C#/.NET 10 and Blazor Server engineer. Generate production-grade, secure, and testable code. Do not use Bootstrap. Use Tailwind via https://cdn.tailwindcss.com. Follow the provided ZAsset specification. Enforce Site scoping and Identity-based authorization.

- Task: EF Core entity + configuration
  - Goal: Implement the Asset and AssetTransaction entities with Fluent API.
  - Inputs:
    - Asset fields: Id, SiteId, WarehouseId, LocationId?, ParentAssetId?, Name, SerialNumber?, Quantity, UoMId, State, StatusNote, RowVersion.
    - AssetTransaction fields: Id, SiteId, AssetId, FromWarehouseId, ToWarehouseId?, FromLocationId?, ToLocationId?, Type, Status, QuantityDelta, PerformedBy, PerformedAtUtc, CorrelationId, Notes, RowVersion.
  - Constraints:
    - Use EF Core for .NET 10. Enable Global Query Filter on SiteId.
    - Configure required FKs, unique constraints, and indexes listed in the spec.
    - Use decimal(18, 4) for quantities and money. Add rowversion.
  - Definition of Done:
    - Compiles, has Fluent configurations, and passes unit tests for transitions and filters.

- Task: Tailwind Blazor page for Goods In
  - Goal: Build Goods In page listing InTransit transfers and allowing receive/store in one flow.
  - Constraints:
    - No Bootstrap. Use Tailwind components. Include LocationPicker, confirm dialog, toasts.
    - Pagination, search by Asset Name/Code, filter by Warehouse.
    - Disable actions when invalid; show validation errors inline.
  - Definition of Done:
    - On confirm, creates Receive and Store transactions, updates Asset WarehouseId/LocationId, timeline refreshes, success toast.

- Task: Global Site filter
  - Goal: Implement Site selection flow and apply Global Query Filters.
  - Constraints:
    - If user.IsGlobal false → auto select DefaultSiteId, hide selector.
    - Persist Site selection in server session and user profile.
  - Definition of Done:
    - All queries return only current Site data. Writes validated against SiteId.

- Task: Timeline component
  - Goal: Create AssetTimeline with virtualized list and Tailwind badges by Type/Status.
  -
[Timestamp: 2026/03/08 20:10:21]

**user**: continue
[Context: Notes: ZAsset Specs.md]
[Timestamp: 2026/03/08 20:11:34]

**ai**: Continuing…

## Detailed Table Specifications |

Below are the extended definitions for all “other” tables referenced in the spec, with key fields, indexes, invariants, and common operations.

### AssetProduct (Join: Asset ↔ Product)
- Fields
  - AssetId (FK → Asset.Id), ProductId (FK → Product.Id), Quantity decimal(18,4), Notes nvarchar(512), CreatedAtUtc, UpdatedAtUtc
- Keys/Indexes
  - PK: (AssetId, ProductId)
  - FK indexes on (AssetId), (ProductId)
- Rules
  - Quantity >= 0
  - Deleting Asset or Product cascades or is restricted per business rule (prefer Restrict and enforce via service)
- Common ops
  - Upsert product quantities for an Asset
  - List products for an Asset with total cost rollup

### Attachment
- Fields
  - Id, SiteId, EntityType enum(Asset, AssetTransaction, Product), EntityId, FileName, ContentType, Size, StorageUri, Hash (SHA-256), UploadedBy, UploadedAtUtc
- Indexes
  - (SiteId, EntityType, EntityId)
  - (SiteId, UploadedAtUtc DESC)
- Rules
  - Virus scan must pass before StorageUri is considered active
  - Hash to deduplicate optional
- Common ops
  - Upload, preview, download, delete (soft delete recommended)

### Tag and TagMap
- Tag
  - Id, SiteId, Name (unique per Site), Color (optional tailwind token), IsActive
  - Unique: (SiteId, Name)
- TagMap
  - TagId, EntityType enum(Asset, Product), EntityId, CreatedAtUtc
  - PK: (TagId, EntityType, EntityId)
- Rules
  - Only active Tags can be newly attached
- Common ops
  - List tags for entity
  - Filter entities by tag(s)

### AuditLog
- Fields
  - Id, SiteId (nullable for global), EntityType, EntityId, Action enum(Create, Update, Delete), UserId, TimestampUtc, Properties JSON (key → [old,new]), CorrelationId
- Indexes
  - (SiteId, EntityType, EntityId, TimestampUtc DESC)
  - (CorrelationId)
- Rules
  - Immutable; writes append-only
- Common ops
  - Show diffs on Asset/Transaction details
  - Export for compliance

### InventoryAdjustment
- Fields
  - Id, SiteId, AssetId, QuantityDelta decimal(18,4), ReasonCode, RequestedBy, RequestedAtUtc, ApprovedBy (nullable), ApprovedAtUtc (nullable), Status enum(Requested, Approved, Rejected, Applied), Notes
- Indexes
  - (SiteId, AssetId)
  - (SiteId, Status)
- Rules
  - Applied produces an AssetTransaction(Type=Adjust) and updates Asset.Quantity atomically
- Common ops
  - Request → Approve/Reject → Apply flow with role checks

### Supplier
- Fields
  - Id, SiteId, Name, Code, ContactName, Email, Phone, Address JSON, IsActive
- Indexes
  - (SiteId, Code unique)
  - (SiteId, Name)
- Common ops
  - Attach supplier to Products
  - Look up by Code in import flows

### UnitOfMeasure
- Fields
  - Id, Code (EA, KG...), Name, IsActive
- Indexes
  - Code unique
- Common ops
  - Seed defaults; map Product and Asset quantities

### Notification
- Fields
  - Id, SiteId, UserId, Type enum(TransferIncoming, AdjustmentAwaitingApproval, System), Title, Body, Link, IsRead, CreatedAtUtc, ReadAtUtc (nullable)
- Indexes
  - (UserId, IsRead, CreatedAtUtc DESC)
- Rules
  - Respect user opt-in/out settings
- Common ops
  - Bell dropdown, mark-as-read, deep-link to item

### Setting
- Fields
  - Id, SiteId, Key (string), Value JSON, UpdatedAtUtc
- Indexes
  - (SiteId, Key unique)
- Examples
  - LocationPattern: regex for codes (e.g., ^R\d+-S\d+(-B\d+)?$)
  - QuantityPrecision: integer per Site/UoM policy

### Assignee
- Fields
  - Id, AssetId, UserId, FromUtc, ToUtc (nullable), Notes
- Indexes
  - (AssetId, FromUtc DESC)
  - (UserId, FromUtc DESC)
- Rules
  - No overlapping active periods for same Asset and User
  - Transfers cannot have assignees
- Common ops
  - Current assignee = latest with ToUtc null

## Validation Matrix (Key Rules) |

| Entity | Validation rules |
| --- | --- |
| Site | Name required, Code unique, cannot delete with children. |
| Warehouse | Unique Name/Code per Site; cannot delete with Assets/Locations. |
| Location | Unique Code per (Site, Warehouse); matches Setting.LocationPattern if present. |
| Product | ProductCode and SKU unique per Site; DefaultCost >= $0$; UoM required. |
| Asset | Quantity > $0$; Serial unique per Site if provided; if ParentAssetId set, default Warehouse/Location inherited or move via transaction. |
| AssetProduct | Quantity >= $0$; unique per (Asset, Product). |
| AssetTransaction | Valid Type/Status transitions; Site and FK scoping consistent; negative QuantityDelta only allowed for Adjust/Dispose/Split. |
| InventoryAdjustment | Status progression valid; Apply only when Approved; delta not $0$. |
| Attachment | ContentType whitelisted; Size within limits; virus scan pass required for activation. |
| Tag | Unique Name per Site; IsActive required. |
| Notification | Type valid; Link points to in-app route. |
| Setting | Unique Key per Site; Value valid JSON schema per key. |

## State Machines (Allowed Transitions) |

- AssetTransactionStatus
  - Draft → Pending → InTransit → Received → NotStored → Stored → Completed
  - Pending → Cancelled
  - Any non-terminal → Cancelled if no side effects have been applied

- InventoryAdjustment.Status
  - Requested → Approved → Applied
  - Requested → Rejected
  - Approved → Applied
  - Rejected/Applied are terminal

Server must enforce with descriptive errors and UI disables unavailable actions.

## Search, Filter, and Indexing

- Global Query Filter on SiteId for scoped entities.
- Common filters
  - Asset: WarehouseId, LocationId, TagIds, ProductId, Assignee, State, Quantity range, UpdatedAt
  - Transactions: Type, Status, Date range, Warehouse, Operator
- Index recommendations
  - Asset: (SiteId, WarehouseId, LocationId), (SiteId, Name), (SiteId, SerialNumber)
  - Product: (SiteId, ProductCode), (SiteId, SKU)
  - Transaction: (SiteId, AssetId, PerformedAtUtc DESC), (SiteId, Status)
- Optional
  - Full-text on Asset.Name, Product.Description
  - Search normalization for codes (strip hyphens, uppercase)

## Import/Export

- CSV/XLSX importers
  - Products: ProductCode, SKU, Description, DefaultCost, UoMCode, Tags[], SupplierCode
  - Locations: WarehouseCode, Code, Rack, Shelf, Bin
  - Assets: Name, Serial, WarehouseCode, LocationCode, Quantity, UoMCode, ProductCodes[]
- Guardrails
  - Validate referential keys (create report for missing references)
  - Dry-run mode with errors/warnings
  - Idempotency by Code+Site keys
- Export
  - Assets, Products, Transactions with applied filters; paginate large exports

## Labels, Barcodes, and QR

- Generate printable labels for Assets and Locations (ZPL and PDF)
- Encoded data
  - Asset: asset:{AssetId}; human-readable Name/Serial/Location
  - Location: loc:{LocationId}; Code
- Tailwind print layouts; test 2x1 and 3x1 inch labels

## File and Attachment Security

- Antivirus scanning hook before finalize
- Enforce max size per Setting (e.g., 25 MB)
- Allowed mime types whitelist
- Store metadata in DB; binary in S3/Azure Blob/local adapter
- Signed URLs for external storage where applicable

## Observability and Diagnostics

- CorrelationId propagation per HTTP circuit
- Structured logs with minimum fields: UserId, SiteId, Route, Action, EntityType, EntityId
- Health checks
  - DB connectivity
  - Storage connectivity
  - Background workers
- Admin page for recent errors with drill-down

## Background Jobs

- Notification dispatcher (periodic)
- Data cleanup (soft-deleted pruning per retention Setting)
- Re-index full-text as needed
- Stuck transaction detector (e.g., InTransit beyond SLA)

## Pages: Acceptance Criteria (DoD) |

- Assets
  - Server-side paging/sorting
  - Filter chips (removable) persist in URL
  - Bulk move/assign guarded by confirmation
  - Accessible keyboard nav and focus management

- Asset Detail
  - Summary, Products, Hierarchy, Timeline, Attachments, Assignees tabs
  - Timeline virtualized with lazy fetch, badges by Type/Status
  - Inline tag editor and assignee picker

- Goods In
  - Lists InTransit transfers scoped to current Warehouse
  - Receive+Store flow in one confirm dialog
  - Success toast; timeline refresh; printable label

- Locations
  - Code builder (Rack/Shelf/Bin)
  - Grid/map visualization; QR print batch
  - Validation against Setting.LocationPattern

- Adjustments
  - Request form with ReasonCode
  - Approve/Reject with notes
  - Apply produces Adjust transaction atomically

## Security and Permissions Matrix |

| Role | Sites | Warehouses | Locations | Products | Assets | Transactions | Adjustments | Settings | Users/Roles |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Admin | CRUD | CRUD | CRUD | CRUD | CRUD | CRUD | Approve/Apply | CRUD | CRUD |
| Manager | R | CRUD | CRUD | CRUD | CRUD | CRUD | Approve/Apply | R | R |
| Operator | R | R | R | R | CRUD (within Site) | Create/Move/Receive | Request | R | — |
| Auditor | R | R | R | R | R | R | R | R | — |

R = Read. All operations are Site-scoped unless IsGlobal user and global admin route.

## DTOs and Mapping

- CreateAssetRequest
  - Name, SerialNumber?, Quantity, UoMCode, WarehouseCode, LocationCode?, ParentAssetId?, ProductCodes[]?
- MoveAssetRequest
  - AssetId, ToWarehouseCode?, ToLocationCode?
- TransferAssetRequest
  - AssetId, ToSiteCode, ToWarehouseCode, ToLocationCode?
- ReceiveStoreRequest
  - TransactionId, ToWarehouseCode, ToLocationCode
- AdjustmentRequest
  - AssetId, QuantityDelta, ReasonCode, Notes

Automapper or manual mappers; validate all external inputs via FluentValidation.

## Data Seeding

- Units: EA, BOX, KG, L, M
- Settings defaults: LocationPattern, QuantityPrecision=2
- Demo: 1 Site with 2 Warehouses, 12 Locations, 10 Products, 15 Assets, sample Transfers

## EF Core Configuration Checklist

- Use decimal(18,2) for money; decimal(18,4) for quantities
- RowVersion concurrency on mutable tables (Asset, AssetTransaction, Location)
- Global Query Filter by SiteId for scoped entities
- Unique constraints and composite indexes as specified
- Soft delete via IsDeleted where appropriate; exclude from queries by filter

## Error Handling and UX

- Standard error envelope for service methods:
  - Code, Message, Details[], CorrelationId, ValidationErrors[]
- UI patterns
  - Inline validation messages below fields
  - Toast for transient successes/failures
  - ConfirmDialog for destructive or cross-warehouse moves

## Prompt Engineering Library (Enhanced)

Use these ready-to-paste prompts with your codegen assistant. Always include “Constraints” and “Definition of Done” and request a brief “Self-check” at the end where the assistant verifies requirements.

- System prompt: ZAsset guardrails
  - You are a principal .NET 10 and Blazor Server engineer. Produce secure, testable, production-grade code. Use Tailwind via https://cdn.tailwindcss.com and no Bootstrap. Enforce Site scoping, Identity authZ, and all rules from the ZAsset specification. Prefer small composable services with unit tests.

- Task: EF Core model + config for “other tables”
  - Goal: Implement Attachment, Tag/TagMap, AuditLog, InventoryAdjustment, Supplier, UnitOfMeasure, Notification, Setting, AssetProduct.
  - Constraints:
    - Global Query Filter on SiteId where applicable.
    - Composite keys and unique constraints per spec.
    - RowVersion where concurrency matters (InventoryAdjustment).
    - Use decimal(18,4) for QuantityDelta.
  - Definition of Done:
    - Entities, Fluent configurations, migrations generated, unit tests for constraints and filters.
  - Self-check:
    - List created indexes and uniques; confirm Site filters present.

- Task: InventoryAdjustment service + validator
  - Goal: Implement request/approve/apply with role checks and Adjust transaction emission.
  - Constraints:
    - Approve only by Manager/Admin.
    - Apply only when Approved; update Asset.Quantity atomically.
    - Audit all state changes.
  - Definition of Done:
    - Unit tests for valid/invalid transitions, concurrency conflict test.
  - Self-check:
    - Verify Adjust transaction fields and timeline entry.

- Task: Notification subsystem
  - Goal: Create in-app notifications for incoming transfers and pending approvals.
  - Constraints:
    - Background job scans and creates notifications.
    - Respect user opt-in/out in Settings.
  - Definition of Done:
    - UI bell component, mark-as-read, deep links.
  - Self-check:
    - Show sample payload and UI state update.

- Task: Tagging UI + filters
  - Goal: Add TagChip editor and filters on Assets and Products.
  - Constraints:
    - Only active tags attachable; create-new gated by permission.
  - Definition of Done:
    - Filters persist in URL; combine multiple tags (AND/OR modes).
  - Self-check:
    - Confirm indexes used by queries.

- Task: Attachment upload component
  - Goal: Secure file uploads with virus scan and preview.
  - Constraints:
    - Size/type restrictions; scan before activation.
  - Definition of Done:
    - FileUpload component with progress, error states, preview for images/PDF.
  - Self-check:
    - Confirm metadata stored and StorageUri set post-scan.

- Task: Location code builder
  - Goal: Create Location form with Rack/Shelf/Bin builder and regex validation.
  - Constraints:
    - Enforce Setting.LocationPattern when present.
  - Definition of Done:
    - Real-time preview of Code; validation errors inline.
  - Self-check:
    - Provide example codes passing/failing regex.

- Task: Playwright E2E for Goods In flow
  - Goal: Automate end-to-end transfer receive/store scenario.
  - Constraints:
    - Seed data, simulate InTransit → Receive → Store, assert timeline entries.
  - Definition of Done:
    - Test passes in CI; screenshots on failure.
  - Self-check:
    - Validate Site scoping in assertions.

## Development Workflow

- Branching: feature/* with PR checks (build, tests, lint)
- CI
  - dotnet build/test, EF migrations validation, Playwright
- CD
  - Blue/green deploy; run migrations on startup gated by role/approval
- Feature flags via Settings for risky features

## Risk and Edge Cases

- Transfers stuck InTransit: background detector, notification to Manager after SLA breach
- Hierarchy moves: cascade or split decision prompt; deny partial invalid states
- Concurrency: rowversion conflict → UI shows “Record updated, please review changes”
- Soft deletes: prevent orphan references; offer restore window

This continues and deepens the original specification, adds clarity for the “other tables,” and provides a stronger prompt engineering toolkit to drive consistent, compliant code generation across the stack.
[Timestamp: 2026/03/08 20:13:01]