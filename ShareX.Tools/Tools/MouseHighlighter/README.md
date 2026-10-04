# Mouse highlighter rendering

The UI-thread overlay reads the existing service state and draws its circles, fades, ripples and release crosshairs with Skia. `Rendering/MouseHighlighterRenderer.cs` contains the drawing and effect bounds; the backend-facing overlay constructor and `Refresh`/`Dispose` calls are unchanged.

Every frame clears the platform's premultiplied BGRA buffer, translates desktop coordinates into its native area and presents that area. A larger reusable buffer is cleared too, so shrinking or removing an effect cannot leave old pixels in unused rows or columns. Reallocation replaces the Skia wrapper. Renderer disposal releases that wrapper before disposing the platform surface, once; late refresh calls are ignored. The Windows surface rejects buffer/presentation access after disposal and makes hide/repeated disposal harmless.

Six proper `ShareX.Tools.Tests/MouseHighlighterDrawingTests.cs` cases verify BGRA/alpha, clipping at negative origins, circle delay/fade/zero duration, ripple/crosshair paths, reused/reallocated buffer clearing and disposal ownership. One Windows-only case renders synthetic pixels into the real native DIB while intercepting presentation, keeping the fixture-owned overlay hidden. No global mouse hook, user input, desktop capture, visible overlay or compositor changes are used.

These fixtures prove buffer/drawing integration on this Windows host, not visible desktop interaction or Linux execution. R21 still tracks the portable Tools test target. Real Windows 10/11 and composited Linux desktop mouse-highlighter verification remains open.
