# Contributing

Thanks for helping make LLMFlex Windows safer and more useful. This project is early, so focused contributions are much easier to review than broad rewrites.

## Workflow

1. Open an issue first for behavior changes, security-sensitive changes, provider expansion, GUI work, or anything that changes how Codex files are written.
2. Fork the repository.
3. Create a branch in your fork.
4. Make one focused change.
5. Run local validation.
6. Open a pull request against this repository.

Direct collaborator access is not given by default. The normal contribution path is fork plus pull request.

## Keep Pull Requests Focused

Good pull request scopes:

- Add tests for one command or writer behavior.
- Improve documentation for one setup path.
- Add a guardrail around one file-writing operation.
- Propose a design note for secret storage hardening.

Avoid combining unrelated runtime changes, provider expansion, GUI work, and documentation edits in one pull request.

## No Secrets

Never commit:

- API keys.
- `.env` files with real values.
- `%USERPROFILE%\.codex\auth.json`.
- Codex config files that expose credentials or private endpoints.
- `%APPDATA%\LLMFlexWindows\secrets.local.json`.
- Screenshots or logs containing keys.

If a secret is accidentally committed or posted, rotate or revoke it immediately. Do not rely only on deleting it from a later commit.

## Testing Before Submitting

At minimum, run:

```powershell
dotnet build
```

For changes that touch command behavior, also run relevant CLI commands against a safe local test profile. Prefer `preview` before `apply`.

Suggested smoke path:

```powershell
dotnet run --project .\LLMFlexWin -- status
dotnet run --project .\LLMFlexWin -- add-profile "test-openrouter" openrouter "https://openrouter.ai/api/v1" "anthropic/claude-sonnet-4"
dotnet run --project .\LLMFlexWin -- preview "test-openrouter"
dotnet run --project .\LLMFlexWin -- list
```

Do not include real API keys in test logs, screenshots, commits, or pull request descriptions.

## Areas That Need Discussion First

Please open an issue before starting:

- Secret storage hardening.
- DPAPI or Credential Manager integration.
- Codex `auth.json` behavior changes.
- GUI or tray app work.
- Provider expansion.
- Packaging, signing, or installer work.
- Major refactoring.

## Code Style

- Keep changes small and readable.
- Match the existing .NET style.
- Prefer explicit safety checks around file writes.
- Do not broaden runtime behavior unless the issue describes the need.

## Pull Request Checklist

- The change is focused.
- `dotnet build` passes.
- No secrets, auth files, local config files, or sensitive screenshots are included.
- Docs are updated when command behavior changes.
- The pull request explains how the change was tested.
