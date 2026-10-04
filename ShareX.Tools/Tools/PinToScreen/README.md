# Pin to screen

The chooser keeps selection busy through the hide delay and the actual capture/file/clipboard callback. Capture support is checked before opening the region selector and again after it returns. File and clipboard sources remain independent of desktop capture support.

A close request disables commands and source/error/restoration callbacks immediately. If a selector is pending, the window hides and waits for `SelectionFinished` before its final close. The existing selector contract does not offer cancellation; retaining the chooser until completion prevents it from releasing its lifetime while work is outstanding. Successful selection preserves the source location and pin/notification order before closing. Window delegates are released on final closure.

`ShareX.Tools.Tests/PinToScreenSourceSupportTests.cs` covers busy/unsupported selection, capability loss, close during delays and all three source waits, reentrant successful close, late errors and completion ownership. Fixtures use generated one-pixel PNGs and controlled callbacks, without constructing windows, reading the clipboard or capturing a desktop. R21 still tracks the test project's portable target; actual Windows 10/11/Linux desktop and R12 coordinate verification remain open.
