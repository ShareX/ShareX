# Video converter

The window keeps settings and drag/drop actions disabled while choosing an input/output path or encoding. Commands and setters also check that state, so a delayed picker cannot start a competing conversion or change saved settings after close. Storage callbacks check closure again after resolving the suggested folder, before opening a dialog.

Each conversion retains its cancellation token until the handler actually completes. Stop cancels the current job and leaves editing disabled until completion; closing rejects further UI callbacks and cancels the job once. Queued progress is accepted only for the current, uncancelled job, so an earlier conversion cannot overwrite a completed result or a later job. Completion uses the output path captured in the conversion request.

`ShareX.Tools.Tests/VideoConverterLifetimeTests.cs` covers these paths with controlled picker/job callbacks, a queued synchronization context and fixture-owned temporary input files. It never invokes a media engine or a native dialog. The test project's portable target change remains tracked in R21.

R41 separately tracks file conversion/trimming/thumbnail capabilities and R22's Linux dependency rule. File tools must retain their Windows behavior independently of desktop recording support. These UI lifetime changes do not establish Linux feature support or actual desktop verification.
