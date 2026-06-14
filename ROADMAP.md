# Roadmap

This roadmap is phased so the Windows prototype can become safer and more contributor-ready without taking on too much at once.

## Public Work Coordination

Roadmap items should become GitHub issues when they are ready for outside contribution. Use issue labels to indicate type, scope, and difficulty where useful.

Active work should avoid hidden duplication. Check open issues and pull requests before starting, and reflect completed work in commits, closed issues, or updated documentation.

This public roadmap applies only to this repository unless another repository is explicitly published. Private/internal projects are not part of this public roadmap.

## Phase 1: Documentation And Tests

- Keep README commands aligned with `Program.cs`.
- Add command-level smoke tests as the CLI stabilizes.
- Add unit tests for Codex config managed-block behavior.
- Add tests for `auth.json` writing behavior with redacted or fake keys.
- Add tests for snapshot capture and restore behavior.
- Document safe local testing patterns that do not require real API keys.

## Phase 2: Secret Storage Hardening

- Replace prototype local JSON key storage with Windows-native protected storage.
- Evaluate DPAPI and Windows Credential Manager options.
- Define migration behavior from prototype `secrets.local.json`.
- Add tests for missing, removed, migrated, and invalid key states.
- Document what is encrypted, what is not, and where secrets live.

## Phase 3: Config/Auth Safety

- Add stronger validation before writing Codex files.
- Improve detection of existing Codex auth modes.
- Make restore behavior easier to inspect before applying.
- Add safer dry-run output for both config and auth changes.
- Protect unrelated user-managed config sections.

## Phase 4: GUI/Tray App Direction

- Define the intended Windows UX.
- Design profile creation, key entry, apply, preview, and restore flows.
- Keep the CLI as a testable core path.
- Avoid GUI work until file safety and secret storage are better defined.

## Phase 5: Provider Expansion

- Document provider compatibility expectations.
- Add provider metadata only when it maps cleanly to supported targets.
- Test provider-specific base URLs, model names, and auth assumptions.
- Avoid broad provider lists without verified behavior.

## Phase 6: Packaging And Distribution

- Decide on packaging format for early Windows builds.
- Add release build guidance.
- Evaluate signing and installer requirements.
- Add upgrade and rollback notes.
- Prepare end-user documentation once the storage and safety model is mature.
