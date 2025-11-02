# Known Bugs

This page tracks blocking and high‑impact issues observed in the current build. When a bug prevents progression, see “Workarounds” at the bottom or enable Bypass Bugged Quests.

## Global

#### DRLG Emu failures (map templates not loading)
- Symptom: Certain dungeon variants fail to load or connect via portals; progression soft‑locks.
- Evidence: “Portal's tagged starting point does not exist (Tag = 172)” when entering trdun_crypt_falsepassage_01.
- Impact: First observed in Act I, Shattered Crown step 37. Subsequent tombs may also fail after a restart.
- Scope: Template/SNO specific. See Diablo Random Level Generation (DRLG Emu) and SNO (Data Asset Reference).
- Workaround: Enable Bypass Bugged Quests; if testing tombs, restart server between attempts.

#### Follower/NPC teleport sync
- Symptom: Followers/NPCs occasionally fail to appear at the new map’s entry or are teleported to unreachable spots.
- Impact: Interactions break or followers cannot be (re)acquired.
- Known cases: Leah (multiple steps), Haedrig (cellar during Shattered Crown).
- Workaround: Reload area or quest step; if blocked, use Bypass.

#### Quest state desync on load
- Symptom: In‑progress loads sometimes report an incorrect quest state (e.g., step −1 or quest ID as the step), causing required actors to be hidden or not following.
- Known cases: Fallen Star step 66 (Leah remains hidden).
- Workaround: Advance a step and back, or restart the quest segment.

#### NPC portal ordering
- Symptom: Entering an NPC‑spawned portal before the NPC can prevent the NPC from teleporting.
- Known cases: Rescue Cain step 17; entering early may strand Cain.
- Workaround: Wait for the NPC to teleport first.

## Act I

### 1. Fallen Star (ID 87700)
- Leah hidden/not following on step 66 when loading mid‑quest due to state misreporting.
- Workaround: Nudge quest state or reload; ensure Leah is visible before proceeding.

### 2. Rescue Cain (ID 72095)
- Leah can teleport to an unreachable, non‑interactable position in the cellar after killing Captain Daltyn (step 51).
- Cain portal ordering: entering before Cain (step 17) can prevent his teleport.
- Workarounds: Wait for Cain to port first. If Leah becomes stuck, reload or use Bypass.

### 3. Shattered Crown (72221) — CRITICAL
- Step 37: Chancellor’s tombs can fail to load; second/third tomb often soft‑lock with DRLG Emu portal tag errors.
- Log snippet: “Portal's tagged starting point does not exist (Tag = 172)” to trdun_crypt_falsepassage_01.
- Downstream steps 59/61 also blocked because step 37 cannot be completed.
- Rare: Haedrig teleported to unreachable spot in cellar (step 41).
- Workarounds: Enable Bypass Bugged Quests; restarting between tomb attempts may not reliably fix.

### 4. Reign of Black King (72061) — CRITICAL
- Sealed doors in the King’s Crypt fail to open. Temporary unblock with `!doors near X`.
- Skeleton King encounter issues:
  - Portal behind him is open at mission start (should be gated).
  - Crown interaction fails (“impossible to interact”).
  - Portal closes after boss death (likely inverted logic).
  - Boss health/damage excessively high; appears “invincible”.
  - Often requires !quest advance twice and !powerful to progress.
- Templar cleanup inconsistency (step 44): follower not destroyed as commented; confirm intended behavior.
- Workarounds: Use admin commands noted above; otherwise enable Bypass.

## Workarounds

- Bypass blocked steps: see Bypass Bugged Quests and enable in server config.
- Admin commands used during testing:
  - !quest advance — nudge quest state if stuck.
  - !doors near 50 — temporarily open sealed doors during crypt issues.
  - !powerful — only as a last resort for over-tuned encounters.
- General: Avoid entering NPC portals before the NPC. If followers/NPCs are missing, reload the area/quest before advancing, or force advance quest.