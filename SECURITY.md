# Security Policy

LLMFlex Windows is an early prototype. Treat it as development software, especially around API-key handling and Codex auth/config changes.

## Do Not Report Secrets Publicly

Do not post API keys, tokens, private endpoints, full `auth.json` files, or sensitive config snippets in GitHub issues, pull requests, screenshots, or logs.

Do not upload:

- `%USERPROFILE%\.codex\auth.json`
- `%USERPROFILE%\.codex\config.toml` without reviewing it carefully
- `%APPDATA%\LLMFlexWindows\secrets.local.json`
- Terminal logs that include command-line API keys

If you accidentally expose a key, revoke or rotate it with the provider immediately.

## Current Secret Storage

The current local secret store is prototype-level. API keys are stored in `%APPDATA%\LLMFlexWindows\secrets.local.json`.

This is not yet hardened with DPAPI, Windows Credential Manager, or another encrypted-at-rest secret storage mechanism. Do not treat this version as production-grade secret management.

## Reporting Security Concerns

For security concerns, avoid sharing exploit details or secrets publicly.

Preferred path:

1. Use GitHub's private vulnerability reporting or Security Advisory feature if it is available for this repository.
2. If private reporting is not available, open a public issue with a minimal high-level description only.
3. Do not include keys, full file contents, repro data with secrets, or private environment details.
4. State that you can provide sensitive details through a private channel if the maintainer enables one.

## Safe Bug Reports

When reporting security-sensitive behavior, include:

- The command you ran, with secrets replaced by `REDACTED`.
- The expected behavior.
- The observed behavior.
- Whether `config.toml`, `auth.json`, or the local secret store was involved.
- The relevant version or commit.

Do not attach raw auth/config files unless all secrets and private values have been removed.
