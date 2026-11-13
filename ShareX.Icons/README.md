# ShareX.Icons

The icons used across ShareX, in two packs that the user picks between in Application settings.

| Folder | Pack | Style |
| --- | --- | --- |
| `Modern` | `IconPack.Modern` | Line art drawn in `currentColor`, so it follows the theme's text color. |
| `Legacy` | `IconPack.Legacy` | Full-colour icons with gradients, outlines and highlights. |

Both folders are embedded in the assembly. Open `Preview.html` in a browser to see every icon of either pack.

## File names

Each icon is an SVG file named after its key, a constant of `AppIcons`, `ImageEditorIcons`, `ImageEffectIcons`,
`PhotoEditorIcons` or `PixelEditorIcons`, with `/` replaced by `.`: the key `app/camera` is `app.camera.svg` in both folders.

A Legacy icon can have a version for dark backgrounds with the `-white` suffix (`app.camera-white.svg`). Modern icons
need none, since they are drawn in the text color.

## Every Modern icon has a Legacy icon, except the modern-only ones

When a key has no file in `Legacy`, the Legacy pack shows its Modern icon. Every key should have a Legacy icon, except
these, which stay Modern in both packs. Do not create Legacy icons for them; one added to the folder is not used.

| Keys | Files | Why |
| --- | --- | --- |
| `AppIcons.CaptionMoon`, `CaptionSun`, `CaptionPin`, `CaptionUnpin`, `CaptionMinimize`, `CaptionMaximize`, `CaptionRestore`, `CaptionClose` | `app.caption-*.svg` | The main window's title bar buttons match the window chrome. |
| All `PhotoEditorIcons` | `photo-editor.*.svg` | The photo editor keeps its own look whatever pack the app uses. |
| All `PixelEditorIcons` | `pixel-editor.*.svg` | The pixel editor shares the photo editor's look; its icons are pixel art. |

The list is enforced by `LegacyIcons.IsModernOnly`, and the photo editor and pixel editor windows also set their
`IconPackScope` to `IconPack.Modern` (`ShareX.PhotoEditor/UI/Theme/PhotoEditorTheme.cs`). Update this table and
`IsModernOnly` together.

## Drawing new icons

- **Modern:** see the remarks on `ModernIcons` for the grids, stroke widths and shading rules. Use `currentColor` for
  line work, `opacity` for shading, and a `<mask>` to cut the gap under a badge or slash.
- **Legacy:** a 16x16 `viewBox`, in the style of the existing Legacy icons: gradient fills, a darker outline, a light
  highlight, and badges (green plus, red minus, and so on) in the bottom-right corner.
- Only the SVG features that `SvgIconDrawing` reads are supported: basic shapes and paths, solid and gradient paints,
  group opacity, transforms and masks.
- Add the new file name to the icon lists in `Preview.html`.
