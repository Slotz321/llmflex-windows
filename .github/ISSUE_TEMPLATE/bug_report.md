---
name: Bug report
about: Report something that is broken or unsafe
title: "[Bug]: "
labels: bug
assignees: ""
---

## Summary

Describe the bug clearly.

## Command

Paste the command with any secrets replaced by `REDACTED`.

```powershell
dotnet run --project .\LLMFlexWin -- status
```

## Expected Behavior

What did you expect to happen?

## Actual Behavior

What happened instead?

## Environment

- Windows version:
- .NET SDK version:
- Repository commit or branch:
- Codex installed: yes/no

## Files Involved

Check any that apply:

- [ ] `%USERPROFILE%\.codex\config.toml`
- [ ] `%USERPROFILE%\.codex\auth.json`
- [ ] `%APPDATA%\LLMFlexWindows\profiles.json`
- [ ] `%APPDATA%\LLMFlexWindows\secrets.local.json`
- [ ] Snapshot files
- [ ] Not sure

## Safety Check

- [ ] I removed API keys, tokens, and private endpoints.
- [ ] I did not attach raw auth/config files with secrets.
- [ ] I replaced sensitive values with `REDACTED`.

## Additional Context

Add any other context that may help reproduce the issue.
