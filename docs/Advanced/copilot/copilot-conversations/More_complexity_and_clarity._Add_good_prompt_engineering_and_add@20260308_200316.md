---
epoch: 1773010996439
modelKey: "gpt-5|openai"
topic: "Refine Prompt Engineering and Tables"


tags:
  - copilot-conversation
---

**user**: More complexity and clarity. Add good prompt engineering and add informations about the other tables in the markdown text above. Better prompt engineering
[Context: Notes: ZAsset Specs.md]
[Timestamp: 2026/03/08 20:03:16]

**ai**: # ZAsset

ZAsset is a Stock and Asset Management System.

- Tech stack: Latest C# with .NET 10, Blazor Web App (Server-side).
- UI: Use Tailwind CSS via https://cdn.tailwindcss.com. Remove Bootstrap entirely. All components and layouts must be Tailwind-first and responsive.
- Multitenant model: Everything is scoped by Site. All queries and actions must respect the current Site context.

## Authentication and Authorization

- Use ASP.NET Core Identity (ApplicationUser).
- On login:
  - If user has parameter Global: false, immediately route to their predestined Site (no site picker).
  - Otherwise, show a Site picker. The chosen Site becomes the active scope for the session.
- Authorization model:
  - Roles: Admin, Manager, Operator, Auditor (extendable).
  - Policies:
    - SiteScoped: Requires the user to have access to the active Site (via claim/role mapping).
    - GlobalAdmin: Full cross-site privileges.
  - Claims:
    - site_access:[SiteId] for site-level access
    - global:[true|false]
- Identity integration:
  - Display names in UI as FirstName LastName - email (e.g., Foo Bar - foo.bar@email.com).
  - Assignee selection sources ApplicationUser; Transfer transactions do not have assignees.

## Assets & Products

We use Site > Warehouse > Location hierarchy.

- Invariants:
  - A Warehouse belongs to exactly one Site.
  - A Location belongs to exactly one Warehouse (and implicitly one Site).
  - All lookups and filters must be Site-scoped; cross-site access is never implicit.
- Moves vs Transfers:
  - Intra-warehouse moves: Status.Move.
  - Cross-site transfers: Status.Transfer.
    - When a Transfer is initiated, the AssetTransaction is created with Transfer status.
    - The target Site receives the item via Goods In page, where an Operator must place it into a Location to complete the transfer.

## Processes & Workflows

- Receive products/assets at a Site (initial inbound).
- Create Asset(s) referencing one or more Products.
- Perform Location Moves within a Warehouse.
- Perform Site Transfers, completed through Goods In at destination Site.
- Conduct Inventory Counts (full or cycle), reconcile discrepancies with Adjustment transactions.
- Assign users (assignees) to assets (non-transfer actions only).
- Maintain a complete, human-friendly timeline per Asset including all AssetTransaction events.

## Tables

| Table name | Functionality |
| | |
| Site | Created and managed through a dedicated "Site" page. Active Site is chosen on login unless Global: false directs the user. |
| Warehouse | A warehouse within a Site. All navigation and queries must ignore warehouses from other Sites. |
| Location | Physical location (e.g., Rack-Shelve format R1-S2). Belongs to a Warehouse. |
| Product | A distinct catalog item that assets can refer to. Includes properties like SKU, UoM, Manufacturer, Category. |
| Asset | A tangible/virtual tracked item. Supports parent-child-sibling relationships. An Asset can reference many Products (via AssetProduct). |
| AssetProduct | Join table mapping Assets to one or more Products (for kits/bundles or BOM-like composition). |
| AssetTransaction | Immutable event log for asset stock/history (Move, Transfer, Receive, Issue, Adjust, Audit, Dispose, Maintain). Transfer triggers Goods In at destination. Drives the timeline UI. |
| Assignee | Logical selection of ApplicationUser as the responsible party for an Asset; Transfers never have an Assignee. |
| ProductCategory | Categorizes Products for navigation, reporting, and policy. Hierarchical support recommended. |
| UnitOfMeasure | Defines measurement units (EA, BOX, KG, L) and conversions if needed. |
| Manufacturer | Source manufacturer for Products (name, code, contact data). |
| Supplier | Vendors for purchasing or replenishment. |
| StockLot | Optional lots/batches for Products. Tracks expirations and compliance attributes. Site- and Location-scoped. |
| SerialNumber | Per-item serial tracking when Product requires serialization. |
| TransferOrder | Represents cross-site transfer intent/authorization. Links to one or more Transfer lines and results in AssetTransaction entries and subsequent Goods In. |
| GoodsIn | Destination-site receipt document for Transfer or Purchase receipts; assigns final Location and completes Transfer. Includes lines and finalization data. |
| InventoryCountSession | A counting session (full or cycle) in a Site/Warehouse/Zone scope. |
| InventoryCountLine | Count entries for Product/Asset/Location with variance and reconciliation state. |
| Attachment | File attachments to Assets, Products, Transactions, or Documents (photos, PDFs, manuals). |
| Tag | Freeform or controlled tags on Assets/Products/Locations for quick filtering. Stored in note properties as needed. |
| AttributeDefinition | Custom attribute schema (name, data type, scope) for extensibility. |
| AttributeValue | Instance values attached to entities (Asset/Product/Location/etc.) per AttributeDefinition. |
| AuditLog | System audit trail for sensitive changes (who, what, when, before/after). |
| Notification | User-targeted notifications for approvals, exceptions, or incoming Goods In. |
| SiteUser | Mapping of Users to Sites including per-site Role(s), plus Global flag and default Site preference. |
| WarehouseZone | Optional subdivision within a Warehouse (e.g., Ambient, Cold Storage, Mezzanine). |
| LocationType | Classification for Locations (Rack, Bin, Floor, Dock) with behavioral flags (staging, quarantine, cycle-count frequency). |

## Core Entities & Key Fields

| Entity | Key fields |
| | |
| Site | Id, Name, Code, IsActive, SettingsJson, CreatedAt, UpdatedAt |
| Warehouse | Id, SiteId, Name, Code, ZoneStrategy, IsActive |
| WarehouseZone | Id, WarehouseId, Name, Code, TemperatureRange, IsActive |
| Location | Id, WarehouseId, ZoneId?, Code (e.g., R1-S2), TypeId, IsStaging, IsQuarantine, IsActive |
| LocationType | Id, Name, Code, IsStaging, IsQuarantine, AllowCounting |
| Product | Id, SKU, Name, CategoryId, UoMId, ManufacturerId?, IsSerialized, IsLotTracked, Barcode, MinStock, MaxStock, IsActive |
| ProductCategory | Id, ParentId?, Name, Code, Path, IsActive |
| UnitOfMeasure | Id, Name, Code, BaseFactor, Precision |
| Manufacturer | Id, Name, Code, ContactEmail, Phone, Url |
| Supplier | Id, Name, Code, ContactEmail, Phone, Terms, IsActive |
| Asset | Id, SiteId, Code, ParentAssetId?, Status, PrimaryLocationId?, AssigneeUserId?, CreatedAt |
| AssetProduct | Id, AssetId, ProductId, Quantity, UoMId |
| SerialNumber | Id, ProductId, Serial, AssetId?, CurrentSiteId, CurrentLocationId?, Status |
| StockLot | Id, ProductId, LotCode, ExpiryDate?, AttributesJson, CurrentQty, SiteId |
| TransferOrder | Id, FromSiteId, ToSiteId, RequestedByUserId, Status, CreatedAt, ApprovedAt?, ShippedAt?, ReceivedAt? |
| GoodsIn | Id, SiteId, SourceType (PO/Transfer), SourceId, ReceivedByUserId, Status, CreatedAt, CompletedAt? |
| InventoryCountSession | Id, SiteId, WarehouseId?, ZoneId?, StartedByUserId, Mode (Full/Cycle), Status, StartedAt, CompletedAt? |
| InventoryCountLine | Id, SessionId, ProductId?, AssetId?, LocationId, ExpectedQty, CountedQty, Variance, ReconciledByUserId?, ReconciledAt? |
| AssetTransaction | Id, AssetId, Type, FromSiteId?, ToSiteId?, FromWarehouseId?, ToWarehouseId?, FromLocationId?, ToLocationId?, QuantityDelta, ProductId?, ReasonCode?, Note, CreatedByUserId, CreatedAt, CorrelationId |
| Attachment | Id, EntityType, EntityId, FileName, MimeType, SizeBytes, Url, UploadedByUserId, UploadedAt |
| Tag | Id, Name, Color?, Scope (Asset/Product/Location), IsControlled |
| AttributeDefinition | Id, Name, Code, DataType (Text/Number/Date/Json/Bool), Scope, IsRequired |
| AttributeValue | Id, DefinitionId, EntityType, EntityId, ValueText?, ValueNumber?, ValueDate?, ValueBool?, ValueJson? |
| AuditLog | Id, EntityType, EntityId, Action, UserId, At, BeforeJson, AfterJson, Ip, UserAgent |
| Notification | Id, UserId, Title, Message, Severity, IsRead, CreatedAt |
| SiteUser | Id, SiteId, UserId, Roles (csv/json), IsDefaultForLogin, Global (bool) |

Indexes and constraints:
- All Site-bound tables must have SiteId or derive Site via upstream references. Add composite indexes on (SiteId, Code/SKU/Name) for fast lookup.
- Enforce uniqueness where relevant: Product.SKU per Site or globally, Location.Code per Warehouse, Asset.Code per Site.

## Transactions, Statuses, and Events

- AssetTransaction.Type enum:
  - Receive, Move, Transfer, Issue, Adjust, Audit, Dispose, Maintain
- Asset.Status enum:
  - Active, InMaintenance, Retired, Lost, InTransit, Quarantined
- Transfer lifecycle:
  - Draft → Approved → Shipped → InTransit → GoodsIn (Received) → Completed
- Goods In:
  - Assigns final Location (required).
  - Validates Site and optionally Warehouse constraints.
- Timeline:
  - Render a reverse-chronological timeline per Asset using AssetTransaction and related documents (TransferOrder, GoodsIn, InventoryCount adjustments, Attachments, Assignee changes).

## UI/UX and Tailwind Standards

- Include Tailwind via https://cdn.tailwindcss.com in the Blazor Server host page. Remove Bootstrap references.
- Design system:
  - Typography: Tailwind font-sans with responsive scaling.
  - Components: Cards, Tables, Badges, Modals, Tabs, Steppers for workflows (Transfer/Goods In).
  - Accessibility: Focus rings, ARIA labels, color contrast AA+.
  - State badges: Move, Transfer, Receive, Adjust, etc. with distinct colors.
- Layout:
  - Left navigation by module (Dashboard, Assets, Products, Warehouses, Transfers, Goods In, Inventory, Reports, Admin).
  - Top bar for Site switcher, search, notifications, user menu.

## Pages (Blazor) to Implement

- Auth & Site Selection:
  - Login, Site Picker (unless Global: false).
- Admin:
  - Site management, SiteUser mapping, Roles/Policies, AttributeDefinition, Tags.
- Master Data:
  - Products (CRUD), Categories, UoM, Manufacturer, Supplier.
  - Warehouses (CRUD), Zones, Locations (grid + map-like visualization).
- Operations:
  - Assets (grid, details with timeline, attachments, attributes, relationships).
  - Moves (intra-warehouse), Transfers (create/approve/ship), Goods In (receive/place).
  - Inventory Count (start, count by location/zone, reconcile).
- Reporting:
  - Stock by Site/Warehouse/Location.
  - Transaction history and exceptions (negative stock attempts, expiries).
- Utilities:
  - Barcode/QR print for Assets/Locations.
  - Notifications center.

## Validation and Invariants

- Site scoping: Every query must filter by active Site unless GlobalAdmin performs cross-site reporting explicitly.
- Location changes:
  - Move requires same Warehouse; Transfer requires different Site.
- Serialized products:
  - Quantity operations must be per serial; no bulk without enumerating serials.
- Lot-tracked products:
  - Expiry and lot attributes must persist across transactions.
- Goods In cannot complete without Location assignment.
- Inventory reconciliation creates Adjust transactions with reason codes.

## Auditing, Logging, and Concurrency

- Use optimistic concurrency (rowversion/timestamp) on mutable aggregates (Asset, Product, Location).
- Write AuditLog for CRUD and workflow state changes.
- Include CorrelationId across multi-step flows (Transfer → Goods In).
- Soft-delete where practical, with IsActive flag and DeletedAt.

## Prompt Engineering Library

Use these templates to generate consistent code, pages, and tests. Always mention:
- Entity scope (Site/Warehouse/Location)
- Authorization policy
- Validation rules
- Tailwind UI expectations
- Test cases and sample data

- Scaffold Entity + EF Core
  - Prompt: Generate an EF Core entity and configuration for [EntityName] with fields: [list]. Add required indexes: [list]. Add relationships: [list]. Enforce Site scoping via [SiteId/foreign keys]. Create a migration named [yyyymmddhhmm_[EntityName]]. Include seed data for dev. Use nullable reference types and DateTimeOffset. Add rowversion for concurrency.

- Scaffold Repository/Service
  - Prompt: Create an application service for [EntityName] with methods: [list]. All queries must filter by SiteId. Enforce authorization policy [PolicyName]. Return paged/sorted results. Validate invariants: [list]. Include unit tests with in-memory provider.

- Scaffold Blazor Page (Tailwind)
  - Prompt: Build a Blazor Server page for [EntityName] list + detail. Use Tailwind for layout and responsive tables. Add search, filters (Site, Warehouse, Location, Status), paging, and action buttons. Show badges for statuses. Include optimistic UI updates and toast notifications. No Bootstrap. Add accessibility attributes.

- Timeline Component
  - Prompt: Implement a reusable Timeline component for AssetTransaction with grouping by date, icons per Type, and expandable details (From/To, Reason, User, CorrelationId). Use Tailwind and dark mode friendly colors. Provide skeleton loaders.

- Workflow (Transfer → Goods In)
  - Prompt: Implement TransferOrder creation, approval, shipment, and Goods In receipt. Enforce Site and Location constraints. Generate AssetTransaction events at each step. Add validation and error messages. Provide unit/integration tests and seed data.

- Inventory Count
  - Prompt: Build InventoryCountSession start/scan/reconcile flow. Ensure counting by Location and Zone. Variance creates Adjust transactions with reason codes. Add UI for exception handling and audit logs.

- Authorization & Claims
  - Prompt: Configure policies SiteScoped and GlobalAdmin. Map SiteUser roles to policies. On login, set active Site context from SiteUser default or picker. Protect all pages with SiteScoped. Include unit tests for policy behavior.

- Seeding & Fixtures
  - Prompt: Seed Sites, Warehouses, Zones, Locations (R1-S1..S10 pattern), Products (serialized and lot-tracked), and demo Assets. Include Transfer and Goods In demo flow. Provide barcodes/QRs for sample Assets.

- QA Checklist
  - Prompt: Provide a test checklist covering: Site scoping, role access, serialized product handling, lot expiry, move vs transfer rules, Goods In completion, audit logs, optimistic concurrency conflict handling, and accessibility (keyboard nav, focus, contrast).

## Open Questions and Decisions to Finalize

- Is Product.SKU unique globally or per Site?
- Do we support cross-warehouse moves within the same Site as Move or Transfer? (Current rule: Move = same Warehouse; Transfer = cross-Site.)
- Attachments storage: local disk, blob storage, or external document service?
- Custom attributes inheritance: Should Asset inherit Product attributes by default?

Once confirmed, update this note and seed prompts accordingly.
[Timestamp: 2026/03/08 20:06:05]

**ai**: # ZAsset

ZAsset is a Stock and Asset Management System.

- Tech stack: Latest C# and .NET 10 with Blazor Web App (Server-side).
- Styling: Include https://cdn.tailwindcss.com and use Tailwind for all UI (no Blazorise or other component libraries).
- Data scoping: Multi-tenant by Site. All queries, navigations, and operations are site-scoped by default.

## Authentication and Authorization

- Use ASP.NET Identity (ApplicationUser).
- Upon login:
  - If the user has Global: true, present a "Select Site" modal/page to choose the active Site for the session.
  - If Global: false and the user is bound to exactly one Site, redirect to that Site automatically.
  - If Global: false and the user has multiple allowed Sites, present the "Select Site" modal but restrict choices to assigned Sites.
- Claims/Properties:
  - ApplicationUser has: Global (bool), AllowedSites (collection via UserSite), FirstName, LastName.
  - Current Site persisted in session and a SiteId claim for server-side scoping.
- Roles (suggested):
  - GlobalAdmin (create Sites, manage users, cross-site reporting),
  - SiteAdmin (manage Warehouses, Locations, Products, Assets for a Site),
  - Operator (perform moves/transfers, Goods In, adjustments),
  - Viewer (read-only).
- Authorization rules:
  - Site creation only available to GlobalAdmin via page "Site".
  - All entity CRUD must enforce SiteId scoping (e.g., Warehouse must belong to Current Site).
  - Asset transfers across Sites require permission: Asset.Transfer.

## Assets & Products

- Physical model: Site > Warehouse > Location.
- Location format: Rack, Shelf as R{number}-S{number}, e.g., R1-S2.
  - Validation regex: ^R[1-9][0-9]*-S[1-9][0-9]*$.
- Product: Catalog item (SKU) that Assets can carry/contain.
- Asset: A container/physical unit that can be:
  - Hierarchical (parent/children/siblings).
  - Associated with many Products (with quantities).
- Movement semantics:
  - Move: within the same Warehouse (Location change, same WarehouseId).
  - Transfer: across Sites (creates an outbound transfer record; receipt is completed via Goods In at destination Site).

## Tables (Overview) |

| Table name | Functionality |
| --- | --- |
| Site | Tenant boundary. Selected at login. Created only through page "Site" by GlobalAdmin. |
| Warehouse | Warehouse inside a Site. All navigations must exclude Warehouses from other Sites. |
| Location | Physical coordinates within a Warehouse (format Rn-Sm). Holds Assets and stock. |
| Product | Catalog entity (SKU, description, UoM, category). |
| UnitOfMeasure | Defines measurement units and conversions (e.g., EA, BOX). |
| ProductCategory | Hierarchical categorization of Products. |
| Asset | Physical container or tracked unit. Can have parent/children. Can hold many Products (via AssetProduct). |
| AssetProduct | Join table to track which Products and how much are inside an Asset. |
| AssetTransaction | Immutable stock history for Asset and Product movements: Move or Transfer. Drives the timeline UI. |
| TransferOrder | Outbound record for inter-Site transfers; consumed by Goods In at destination. |
| GoodsIn | Destination-side receipt of TransferOrder; assigns Location and completes the transfer. |
| InventoryAdjustment | Manual correction entries (gain/loss) per Product per Location or Asset. |
| InventorySnapshot | Periodic snapshot for reporting and audit. |
| Attachment | Files (images, docs) linked to Assets, Products, or Transactions. |
| Tag | Free-form tags assignable to Assets or Products. |
| Assignee | Virtual projection of ApplicationUser (FirstName LastName - email) for assignment UIs. |
| AssetAssignee | History of assignments of an Asset to a user (who/when). Transfers have no assignee. |
| UserSite | Mapping table of which Sites a user can access. Used when Global is false or for scoping. |
| AuditLog | Append-only audit trail (who, when, what changed). |

## Entity Field Specs

### Site |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| Name | string(200) | Unique |
| Code | string(50) | Unique, URL-safe slug |
| IsActive | bool | Default true |
| CreatedAtUtc | datetime |  |
| UpdatedAtUtc | datetime |  |

### Warehouse |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | FK → Site |
| Name | string(200) | Unique per Site |
| Code | string(50) | Unique per Site |
| IsActive | bool |  |
| CreatedAtUtc | datetime |  |
| UpdatedAtUtc | datetime |  |

- Unique index: (SiteId, Name), (SiteId, Code).

### Location |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | FK → Site (denormalized for fast scoping) |
| WarehouseId | Guid | FK → Warehouse |
| Code | string(50) | Format Rn-Sm, unique per Warehouse |
| Description | string(200) | Optional |
| IsActive | bool |  |
| CreatedAtUtc | datetime |  |
| UpdatedAtUtc | datetime |  |

- Check constraint: Code matches ^R[1-9][0-9]*-S[1-9][0-9]*$.
- Unique index: (WarehouseId, Code).

### UnitOfMeasure |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| Name | string(100) | e.g., Each, Box |
| Symbol | string(10) | e.g., EA, BOX |
| BaseRatio | decimal(18,6) | Ratio to base UoM (1 for base) |
| IsBase | bool | Exactly one base per category (if used) |

### ProductCategory |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| ParentId | Guid? | Self-FK for hierarchy |
| Name | string(200) | Unique per parent |
| Code | string(50) | Optional, unique per tree |
| IsActive | bool |  |

### Product |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | FK → Site |
| SKU | string(100) | Unique per Site |
| Name | string(200) |  |
| CategoryId | Guid? | FK → ProductCategory |
| DefaultUomId | Guid | FK → UnitOfMeasure |
| Barcode | string(100) | Optional, unique per Site |
| Description | string(1000) | Optional |
| IsActive | bool |  |
| CreatedAtUtc | datetime |  |
| UpdatedAtUtc | datetime |  |

- Unique index: (SiteId, SKU), (SiteId, Barcode).

### Asset |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | FK → Site |
| WarehouseId | Guid | FK → Warehouse |
| LocationId | Guid | FK → Location |
| ParentAssetId | Guid? | Self-FK for hierarchy |
| Code | string(100) | Unique per Site (e.g., ASSET-000123) |
| Name | string(200) |  |
| Status | tinyint | Enum AssetStatus |
| IsMovable | bool |  |
| Notes | string(2000) | Optional |
| CreatedAtUtc | datetime |  |
| UpdatedAtUtc | datetime |  |
| ConcurrencyToken | rowversion | EF Core concurrency |

- Indexes: (SiteId, Code) unique; (WarehouseId, LocationId).

### AssetProduct |

| Column | Type | Notes |
| --- | --- | --- |
| AssetId | Guid | PK part, FK → Asset |
| ProductId | Guid | PK part, FK → Product |
| Quantity | decimal(18,6) | Non-negative |
| UomId | Guid | FK → UnitOfMeasure |
| LastCountedAtUtc | datetime? | For cycle count |

### AssetTransaction |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | FK → Site (of the origin) |
| AssetId | Guid | FK → Asset |
| ProductId | Guid? | Optional if moving container only |
| TransactionType | tinyint | Enum: Move=1, Transfer=2, Adjustment=3, Receipt=4 |
| Quantity | decimal(18,6)? | Required for product movements |
| FromWarehouseId | Guid? | Required on Move/Transfer |
| FromLocationId | Guid? | Required on Move/Transfer |
| ToWarehouseId | Guid? | Required on Move |
| ToLocationId | Guid? | Required on Move |
| TransferOrderId | Guid? | Links to TransferOrder on Transfer/Receipt |
| Reason | string(500) | Optional (e.g., cycle count) |
| PerformedByUserId | string | FK → AspNetUsers |
| PerformedAtUtc | datetime |  |
| MetaJson | nvarchar(max) | Extensible metadata |

- Immutable; do not update after insert (append-only).
- Drives the Asset timeline UI.

### TransferOrder |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| FromSiteId | Guid | FK → Site |
| ToSiteId | Guid | FK → Site |
| AssetId | Guid | FK → Asset (asset leaving origin) |
| CreatedByUserId | string | FK → AspNetUsers |
| CreatedAtUtc | datetime |  |
| Status | tinyint | Enum: Draft, Dispatched, InTransit, Received, Cancelled |
| DispatchedAtUtc | datetime? |  |
| ReceivedAtUtc | datetime? | Set by Goods In at destination |
| Notes | string(1000) | Optional |

### GoodsIn |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | Destination Site, FK → Site |
| TransferOrderId | Guid | FK → TransferOrder |
| ReceivedByUserId | string | FK → AspNetUsers |
| AssignedWarehouseId | Guid | FK → Warehouse (destination) |
| AssignedLocationId | Guid | FK → Location (destination) |
| ReceivedAtUtc | datetime |  |
| Notes | string(1000) | Optional |

- Operation: On save, create AssetTransaction of type Receipt; update Asset.SiteId, WarehouseId, LocationId; update TransferOrder.Status to Received.

### InventoryAdjustment |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | FK → Site |
| AssetId | Guid? | Optional if location-only |
| ProductId | Guid | FK → Product |
| LocationId | Guid? | If not in Asset |
| QuantityDelta | decimal(18,6) | Positive/negative |
| Reason | string(500) | Required |
| PerformedByUserId | string |  |
| PerformedAtUtc | datetime |  |

### InventorySnapshot |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | FK → Site |
| TakenAtUtc | datetime |  |
| SnapshotJson | nvarchar(max) | Denormalized counts per Product/Location/Asset |

### Attachment |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | FK → Site |
| EntityType | tinyint | Enum: Asset, Product, Transaction |
| EntityId | Guid |  |
| FileName | string(255) |  |
| ContentType | string(100) |  |
| BlobUrl | string(1000) | Or local path/provider key |
| UploadedByUserId | string |  |
| UploadedAtUtc | datetime |  |

### Tag |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid | FK → Site |
| Name | string(100) | Unique per Site |
| ColorHex | string(7) | e.g., #10B981 |

- Bridge tables (optional): AssetTag(AssetId, TagId), ProductTag(ProductId, TagId).

### Assignee and AssetAssignee |

- Assignee is not persisted separately; it is a projection over ApplicationUser: FirstName LastName - email.
- AssetAssignee table:

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| AssetId | Guid | FK → Asset |
| UserId | string | FK → AspNetUsers |
| AssignedAtUtc | datetime |  |
| UnassignedAtUtc | datetime? | Null means currently assigned |

- Rule: Transfers don't set an assignee.

### UserSite |

| Column | Type | Notes |
| --- | --- | --- |
| UserId | string | PK part |
| SiteId | Guid | PK part |
| RoleOverride | string? | Optional, if per-site role differs |

### AuditLog |

| Column | Type | Notes |
| --- | --- | --- |
| Id | Guid | PK |
| SiteId | Guid? | Optional if global event |
| EntityType | string(100) | e.g., Asset |
| EntityId | string(100) |  |
| Action | string(50) | Created, Updated, Deleted |
| UserId | string |  |
| OccurredAtUtc | datetime |  |
| DiffJson | nvarchar(max) | Before/after |

## Enums

- AssetStatus: Active=1, Inactive=2, InTransit=3, Damaged=4, Lost=5.
- TransactionType: Move=1, Transfer=2, Adjustment=3, Receipt=4.
- TransferOrderStatus: Draft=1, Dispatched=2, InTransit=3, Received=4, Cancelled=5.

## Business Rules

- Site scoping:
  - All reads/writes constrained to Current Site unless Global feature explicitly requires cross-site.
  - EF Core global query filters on SiteId for entities with SiteId.
- Movement vs Transfer:
  - Move: FromLocationId and ToLocationId required; WarehouseId must be same.
  - Transfer: FromSiteId != ToSiteId. On dispatch, create TransferOrder and AssetTransaction (Transfer). Asset marked InTransit until Goods In.
  - Goods In: Required to finalize Transfer; assigns Warehouse/Location in destination Site, creates Receipt transaction, sets AssetStatus to Active.
- Location format: Enforce regex and unique per Warehouse. Provide quick-create wizard for grid of racks/shelves.
- Asset hierarchy:
  - ParentAssetId cannot create cycles. Enforce via application rule.
  - Moving a parent Asset cascades implied moves of child Assets (or restrict: “children inherit parent’s location”).
- Quantities:
  - AssetProduct.Quantity cannot be negative.
  - UoM conversions must be applied consistently for Quantity deltas.

## UI/UX and Pages

- Global layout:
  - Tailwind CDN: add <script src="https://cdn.tailwindcss.com"></script> to _Host.cshtml.
  - Use Tailwind components for modals, tables, tabs, timelines, and forms.
- Pages:
  - Login + Site Selection:
    - If Global, show Site picker with search; remember last-used Site.
    - If single Site and Global: false → auto-redirect.
  - Site (admin-only): CRUD Sites.
  - Warehouse: Site-scoped CRUD.
  - Location:
    - Create with validation for Rn-Sm.
    - Bulk create: define ranges for R and S.
  - Product:
    - CRUD with Category, UoM, Tags, Attachments.
  - Asset:
    - CRUD with parent selector; visual tree view for hierarchy.
    - Tabs: Details, Products, Timeline, Attachments, Tags, Assignees.
    - Timeline: Beautiful, dense Tailwind vertical timeline; shows AssetTransaction entries and Goods In receipts with icons.
    - Move wizard (within Warehouse); Transfer wizard (across Sites).
  - Transactions:
    - Recent activity feed with filters by Site/Warehouse/Type.
  - Goods In:
    - Lists pending TransferOrders to current Site; operator selects one, chooses Warehouse/Location, and receives.
- Usability:
  - Barcode/QR support for locating Assets/Products.
  - Autosuggest for Locations and Products.
  - Toasts for success/failure; inline validation errors.

## Persistence and Performance

- EF Core:
  - DbContext with OnModelCreating applying HasQueryFilter(e => e.SiteId == CurrentSiteId) where applicable.
  - Concurrency: rowversion on mutable aggregates (Asset).
  - Soft-delete optional: add IsDeleted with filter where needed.
- Indexing:
  - Unique constraints as specified.
  - Add nonclustered indexes on (SiteId, Code/SKU), (WarehouseId, Code), and common filter columns.
- Auditing:
  - SaveChanges interceptor to append AuditLog entries for tracked entities.
- Migrations:
  - Naming: yyyyMMddHHmm_description.
  - Seed: Base Uo
[Timestamp: 2026/03/08 20:06:28]