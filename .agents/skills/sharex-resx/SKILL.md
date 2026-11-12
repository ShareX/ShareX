---
name: sharex-resx
description: Translate or update ShareX UI strings in Localization/Strings.resx catalogs using the repository's RESX batch helpers. Use for ShareX localization requests and newly added windows or controls needing translations.
---

# ShareX RESX translations

Use the maintained helpers in `Scripts/Resx.ps1`; generate translation data rather than a new importer. Run commands from the ShareX repository root, containing `ShareX.sln` and `Scripts`. PowerShell 7.2 or newer is required (`pwsh`). When only Windows PowerShell 5.1 is installed, install a local copy into the git-ignored `artifacts` folder with `dotnet tool install PowerShell --tool-path artifacts/pwsh` and run `artifacts/pwsh/pwsh.exe`.

## Identify the work

Find the owning project's `Localization/Strings.resx` keys and their source references with a targeted `rg` search. Include related menu, context-menu, and hotkey strings when the requested feature introduced them in other projects. Existing localized resource references may already cover the requested labels.

If new English keys are required, add scoped keys to `Strings.resx`, add their accessors to `Strings.Designer.cs`, and use those references in the source. The batch helper translates existing English keys; it preserves typed and binary RESX entries. `Scripts/FormatTranslations.ps1` sorts and normalizes catalogs after hand edits.

The Visual Studio resource generator does not run in command-line builds. After adding or removing English keys, sync the accessors:

```powershell
pwsh -File Scripts/Resx.ps1 designer -Project ShareX.Tools
```

It appends missing accessors in the style of the file's latest accessor and removes accessors whose keys no longer exist; other lines are untouched.

Keep stable identifiers separate from display text. A label that is also persisted, compared, or used as a command or recipe id needs its own display property. Message text must not drive control flow: replace checks such as `ex.Message.StartsWith(...)` with a dedicated exception type before localizing the message. Build sentences from format strings with placeholders, not from concatenated fragments.

### A project without a catalog

1. Copy the 14-line `resheader` prologue of an existing `Localization/Strings.resx` into `Localization/Strings.resx` and every culture's `Localization/Strings.<culture>.resx`, closing each with `</root>`, then run `Scripts/FormatTranslations.ps1`.
2. In the `.csproj`, mark `Localization\Strings.resx` with `PublicResXFileCodeGenerator`, make `Strings.Designer.cs` depend on it, and import the `Localization` namespace where the source uses `Strings`.
3. Register the project in the `$projects` list in `Scripts/ValidateTranslations.ps1` with its `ResourceBaseName`.
4. Run `designer` to create `Strings.Designer.cs`.

## Export a small batch

```powershell
pwsh -File Scripts/Resx.ps1 export -Project ShareX.Tools -Prefix AnimatedGifTrimmer_ -Path artifacts/gif-translations.json
```

The exporter discovers projects and catalog cultures from disk. It selects missing, empty, invalid, or unapproved English-equivalent translations, exports at most 40 keys by default, and reports how many keys remain. Use `-Key`, multiple `-Prefix` values, `-Culture`, or `-MaxEntries` to adjust the working set. Use `-IncludeExisting` only when intentionally reviewing or revising existing translations.

Read the batch, not whole catalogs. When the batch is too large, work on smaller key or culture groups. Completed values stay on disk, and re-exporting the same scope selects the remaining work.

## Translate and reuse

Fill the `translations` values by culture and resource key. Keep `source`, `existing`, `project`, and `key` unchanged: they protect against stale batches. A null translation is left pending, so partial batches can be imported and resumed.

The batch includes source comments and up to two exact-English `reuseCandidates`. Review their original key and UI context before reusing wording. Add `reuseFrom` with the chosen `project` and `key` to copy its translations for null cells; explicit translations take precedence. For additional terminology lookup:

```powershell
pwsh -File Scripts/Resx.ps1 find -Text 'Pause' -Culture tr,fr -MaxEntries 5
```

Preserve format placeholders, command placeholders, menu access keys (one `_` per menu header; CJK locales use a `(_F)` suffix), product names, and file extensions. Translate surrounding prose naturally. Do not copy a complete English phrase into a locale merely to fill a missing key.

Intentional English-equivalent terms (brands, acronyms, same-spelling cognates such as German `Navigator`) require a reviewed entry in `Scripts/TranslationEnglishAllowlist.txt`. A value copied through `reuseFrom` needs its own entry, because approvals are per key. Import reports every English-equivalent value with its source hash; after reviewing them, record the approvals for the current English source:

```powershell
pwsh -File Scripts/Resx.ps1 approve -Project ShareX.Tools -Key AnimatedGifTrimmerWindow_Navigator -Culture de,nl-NL
```

Approve only values that are correct as written; translate the others instead.

See [the batch format](references/batch-format.md) for a small JSON example and update/reuse semantics.

## Apply and verify

```powershell
pwsh -File Scripts/Resx.ps1 import -Path artifacts/gif-translations.json
pwsh -File Scripts/Resx.ps1 check -Project ShareX.Tools -Prefix AnimatedGifTrimmer_
```

Import validates the entire batch before writing, lists every invalid translation in one run, preserves the established XML/UTF-8/CRLF format, and reports changed catalogs. `-WhatIf` previews the same validated changes when a preview is useful. A source or translation conflict means the disk values changed: re-export and review those entries instead of changing the saved snapshot to force an overwrite. Re-importing an already applied value is a no-op.

Repeat export/import/check until the requested scope has no remaining work across all catalog cultures, unless the user requested particular languages. Run `Scripts/ValidateTranslations.ps1` once at completion for the repository-wide checks and formatting. If source or designer references changed, also build the affected application project using the repository's normal build configuration.

Report the translated scope and validation results. Do not expand a feature translation request into unrelated catalog cleanup.
