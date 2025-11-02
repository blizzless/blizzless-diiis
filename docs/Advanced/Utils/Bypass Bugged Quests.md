## Purpose

When a quest step is flagged as broken, the server can advance past it automatically so you can keep playing. Any quest page referencing “Bypass Bugged Quests” indicates such a step. See [[Known Bugs]] for the current list.

## How it works

- If `BypassBuggedQuests` is enabled, entering a known-bugged step will auto-advance to the next step.
- The bypass only changes quest state; it doesn’t “fix” the underlying content. It is made for testing the Act and annotating bugs.
- Some downstream triggers (doors, portals, followers, conversations) will be run as their source step is duplicated to formulate a similar behavior. Consult the quest notes for any manual fixes.

## Configuration

Set the option in config.ini:

```ini
[Game Server]
BypassBuggedQuests = true
```

- Default should be false for normal play; enable only for testing or when a quest is known to be blocked.
- Changes apply server-side and affect all players on the server.

## Manual control

- Advance the current quest step: !quest advance
- Inspect the current quest: !quest info

Use manual advancing sparingly; if you need multiple advances in a row, the quest is likely expecting intermediate triggers that are currently broken.

## Caveats

- Progress continuity: If a bypass skips a conversation or unlock, you may need to trigger equivalents manually (see the quest’s “Notes & Potential Issues”).
- Traceability: When in doubt, cross-check [[Known Bugs]] and the specific quest page to understand what was skipped and why.

If you find yourself relying on bypasses regularly, the real bug deserves attention. The bypass is a tool, not a solution.