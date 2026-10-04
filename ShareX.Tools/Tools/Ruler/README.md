# Ruler overlay

The ruler captures before showing its window, including when a desktop requires an asynchronous permission prompt. Its screen pixel buffer uses the returned capture bounds, keeps measurements in desktop pixels and stores managed samples. Temporary Skia objects used for PNG decoding are disposed during conversion.

The window retains those returned bounds and divides the physical dimensions by its actual Avalonia `RenderScaling`. Geometry is refreshed on open and after `ScalingChanged`, so the scale of the monitor under the virtual desktop's top-left point does not determine the size of the whole overlay. Stored measurements continue to use `PointToScreen`/`PointToClient`; scaling changes redraw them without changing the capture pixels or origin. Queued geometry callbacks skip a closed window.

Existing proper `ShareX.Tools.Tests/RulerCaptureTests.cs` covers returned bounds, pixel/PNG sampling, scaled buffers, cancellation and unsupported capture. The full solution gate checks this UI geometry edit against those regressions. Actual mixed-monitor Windows 10/11/Linux overlay placement and R12 backend coordinate verification remain open; scalar conversion alone does not prove those desktop flows. R21 still tracks the portable Tools test target.
