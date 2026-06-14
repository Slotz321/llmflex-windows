# LLMFlex Windows

LLMFlex Windows is a Windows-native adaptation and early prototype inspired by Futureformed's original LLMFlex concept: make it easier to switch AI coding tools between provider/model profiles without hand-editing configuration every time.

The first working path is intentionally narrow:

- A .NET CLI foundation for Windows.
- Profile storage under `%APPDATA%\LLMFlexWindows`.
- OpenRouter-style profile support for Codex-compatible configuration.
- Codex `config.toml` managed-block writing.
- Codex `auth.json` API-key mode writing.
- Prototype local API-key storage.
- Interactive API-key prompt support.
- Snapshot/restore support for prior Codex auth/config state.
- Key removal and key presence checks.

This repository is not a replacement for Futureformed's original project. It is a Windows prototype inspired by that idea, with appreciation and attribution to Futureformed's LLMFlex work.

## Current Status

This project is prototype software. The CLI can create profiles, store and remove API keys, preview a Codex config block, apply a profile to Codex, snapshot Codex files, and restore previously captured Codex state.

The current implementation uses these paths:

- `%USERPROFILE%\.codex\config.toml`
- `%USERPROFILE%\.codex\auth.json`
- `%APPDATA%\LLMFlexWindows\profiles.json`
- `%APPDATA%\LLMFlexWindows\secrets.local.json`
- `%APPDATA%\LLMFlexWindows\snapshots\`

Use `status` before experimenting so you can see exactly where the tool will read or write local state.

## Intentionally Not Done Yet

The following are not implemented in this prototype pass:

- GUI or tray app.
- Windows DPAPI integration.
- Windows Credential Manager integration.
- Production-grade encrypted secret storage.
- Broad provider expansion.
- Installer, signing, auto-update, or distribution packaging.
- Major runtime refactors.
- Production support guarantees.

## Requirements

- Windows.
- .NET SDK capable of building `net10.0`.
- Codex installed and configured separately if you want to apply profiles to Codex.
- An OpenRouter API key, or another intentionally configured compatible endpoint/key.

## Build

From the repository root:

```powershell
dotnet build
```

Run the CLI through the project:

```powershell
dotnet run --project .\LLMFlexWin -- status
```

## Commands

Show paths and local state counts:

```powershell
dotnet run --project .\LLMFlexWin -- status
```

List saved profiles and whether each has a stored key:

```powershell
dotnet run --project .\LLMFlexWin -- list
```

Add or update a profile:

```powershell
dotnet run --project .\LLMFlexWin -- add-profile "openrouter-sonnet" openrouter "https://openrouter.ai/api/v1" "anthropic/claude-sonnet-4"
```

Store a key interactively:

```powershell
dotnet run --project .\LLMFlexWin -- set-key-prompt "openrouter-sonnet"
```

Store a key as a command argument:

```powershell
dotnet run --project .\LLMFlexWin -- set-key "openrouter-sonnet" "YOUR_API_KEY"
```

Prefer `set-key-prompt` for local use. Passing keys as command arguments can expose them through shell history, terminal logs, or process inspection.

Check, remove, and list key state:

```powershell
dotnet run --project .\LLMFlexWin -- has-key "openrouter-sonnet"
dotnet run --project .\LLMFlexWin -- remove-key "openrouter-sonnet"
dotnet run --project .\LLMFlexWin -- list
```

Preview the Codex `config.toml` managed block without modifying files:

```powershell
dotnet run --project .\LLMFlexWin -- preview "openrouter-sonnet"
```

Capture snapshots of existing Codex files, if present:

```powershell
dotnet run --project .\LLMFlexWin -- snapshot
```

Apply a profile to Codex:

```powershell
dotnet run --project .\LLMFlexWin -- apply "openrouter-sonnet"
```

Restore previously captured Codex files:

```powershell
dotnet run --project .\LLMFlexWin -- restore
```

## What `apply` Does

`apply <profileName>` finds a saved profile and writes a managed block to `%USERPROFILE%\.codex\config.toml`.

The managed block uses:

- `model_provider = "llmflex"`
- `model = "<profile model>"`
- `[model_providers.llmflex]`
- `base_url = "<profile base URL>"`
- `env_key = "OPENAI_API_KEY"`
- `wire_api = "responses"`

The writer removes prior LLM Flex managed blocks and older top-level `model` and `model_provider` keys before inserting the current managed block.

If a key exists for the selected profile, `apply` also writes `%USERPROFILE%\.codex\auth.json` with:

- `OPENAI_API_KEY`
- `auth_mode = "apikey"`

Before writing either Codex file, the tool captures a snapshot if one does not already exist.

## Safety Notes

- Do not commit API keys.
- Do not upload `%USERPROFILE%\.codex\auth.json` if it contains a key.
- Review `%USERPROFILE%\.codex\config.toml` before sharing it.
- Do not upload `%APPDATA%\LLMFlexWindows\secrets.local.json`.
- Treat screenshots, terminal logs, and issue attachments as potentially sensitive.
- Prefer `set-key-prompt` over `set-key <profileName> <apiKey>`.
- Use `preview` before `apply` when testing config changes.
- Use `snapshot` before experiments if you want a restore point.

Current key storage is prototype-level local JSON, not hardened Windows secret storage. Secret storage hardening is planned work, not a completed feature.

## Collaboration Areas

Good contribution areas include:

- Documentation improvements and examples.
- Focused tests around config/auth writing.
- Snapshot and restore behavior tests.
- Safer secret storage design using Windows-native primitives.
- Guardrails for Codex auth/config safety.
- GUI/tray app planning notes.
- Provider compatibility research.
- Packaging and distribution planning.

Please read [CONTRIBUTING.md](CONTRIBUTING.md), [ROADMAP.md](ROADMAP.md), and [SECURITY.md](SECURITY.md) before opening a pull request.

This repository is the current public-facing construction and collaboration surface. Public coordination should happen through visible GitHub issues, roadmap updates, and pull requests.

## Attribution

This Windows prototype is inspired by Futureformed's original LLMFlex concept and project direction. Futureformed's work established the core idea of switching AI coding tool model/provider profiles with less manual config editing. This repository explores a Windows-native path for that idea.

## License

MIT. See [LICENSE](LICENSE).
