# Translation batch format

`Scripts/Resx.ps1 export` writes this versioned JSON structure:

```json
{
  "version": 1,
  "entries": [
    {
      "project": "ShareX.Tools",
      "key": "ExampleWindow_Frame",
      "source": "Frame {0} of {1}",
      "existing": {},
      "translations": {
        "fr": "Image {0} sur {1}",
        "tr": "Kare {0} / {1}"
      }
    }
  ]
}
```

- `project` is the directory containing `Localization/Strings.resx`.
- `key` is an exact, case-sensitive English resource key.
- `source` is the exported English value. Import rejects the entry if it changed on disk.
- `existing` contains the previous values of the translations included in the batch. An absent culture means its resource was missing. Keep this snapshot unchanged, including empty values.
- `translations` maps catalog culture names to translated strings. Null values are skipped. Omit unrequested cultures or leave them null for later batches.
- `comment`, when exported, supplies developer context.
- `reuseCandidates`, when exported, contains exact-English resource matches and their valid translations. These are suggestions requiring a context check.

For a reviewed reuse candidate, add this sibling property to the entry:

```json
"reuseFrom": {
  "project": "ShareX.Tools",
  "key": "NetworkMonitorWindow_Pause"
}
```

The referenced English value must exactly equal `source`. The importer reads its current localized values for null translation cells, validates each value against the destination key, and honors any explicit translations. An unapproved English-equivalent term still needs an allowlist entry for the destination resource.

For intentional revisions, export with `-IncludeExisting`. The previous values populate `existing`, and `translations` starts with null cells for the agent to fill. If another edit changed a previous value, the importer rejects the batch before writing any catalog. If the requested value is already on disk, the entry is a no-op; this permits resuming or re-importing a batch.

The importer supports string entries in the shared `Localization/Strings*.resx` catalogs. Legacy per-form or `Properties/Resources*.resx` catalogs are outside this helper's scope. It does not regenerate designer code, translate automatically, or update the English allowlist.
