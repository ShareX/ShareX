# Video thumbnailer

The window queries file-media support using its configured executable, independently of desktop recording. Unsupported controls show the existing translated FFmpeg-unavailable reason. Pickers and drag/drop cannot replace the video or output folder after support loss or closure, and saved options are retained while editing is unavailable.

Start rechecks support before dispatch and the production worker checks again before creating its engine. Closing ends new commands and queued UI updates; an already accepted worker keeps ownership of its engine and output callback, preserving completion behavior. Settings are disabled during the job.

`ShareX.Tools.Tests/FileMediaSupportTests.cs` uses generated existence fixtures and injected workers to verify these boundaries without an external engine, native dialog or upload. Actual Windows/Linux desktop verification remains required.
