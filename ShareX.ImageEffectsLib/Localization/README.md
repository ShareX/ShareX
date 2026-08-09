# Image Effects localization status

Image Effects translations use the shared `Strings.resx` resource set in this directory. Keys are scoped by their owning view or by the image-effect metadata they represent.

## Localized areas

| Area | Resource prefix | Keys | Status |
| --- | --- | ---: | --- |
| Gradient editor | `GradientOptionsPanel_` | 4 | Complete |
| Effect options panel | `ImageEffectOptionsPanel_` | 15 | Complete |
| Effect package window | `ImageEffectPackagerWindow_` | 16 | Complete |
| Confirmation window | `ImageEffectsConfirmationWindow_` | 4 | Complete |
| Main image-effects window | `ImageEffectsWindow_` | 29 | Complete |
| Effect categories | `ImageEffectCategory_` | 4 | Complete |
| Default effect content | `ImageEffectDefault_` | 2 | Complete |
| Effect names | `ImageEffect_` | 51 | Complete |
| Effect property names | `ImageEffectProperty_` | 136 | Complete |
| Effect property descriptions | `ImageEffectPropertyDescription_` | 28 | Complete |
| Effect enum values | `ImageEffectEnum_` | 58 | Complete |

The default resource set contains 347 keys, with matching catalogs for all 23 supported cultures. Runtime metadata localization is handled by `ImageEffectsLocalization`: it maps reflected effect types, property descriptors, descriptions, and enum values to the scoped keys above while retaining the original metadata as a safe fallback.

The repository-level `ValidateTranslations.ps1` checks supported languages, entry counts, key parity, non-empty values, format placeholders, UTF-8 and CRLF formatting, generated designers, source integration, and remaining literal Avalonia UI text across every localized project.

Run the validator from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ValidateTranslations.ps1
```
