#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;

namespace ShareX.Tools;

public static class MouseHighlighterManager
{
    private static MouseHighlighterService? _service;
    private static MouseHighlighterOptions? _manualOptions;
    private static RecordingSession? _recordingSession;
    private static MouseHighlighterWindow? _settingsWindow;
    private static volatile bool _shutdown;

    public static bool IsManuallyActive => _manualOptions != null;
    public static bool IsRecordingActive => _recordingSession != null;
    public static event Action? StateChanged;

    public static void ShowWindow(MouseHighlighterOptions options, Action? settingsChanged = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            if (_shutdown) return;
            if (_settingsWindow != null)
            {
                _settingsWindow.Activate();
                return;
            }
            _settingsWindow = new MouseHighlighterWindow(options, settingsChanged);
            _settingsWindow.Closed += (_, _) => _settingsWindow = null;
            _settingsWindow.Show();
        });
    }

    public static void Toggle(MouseHighlighterOptions options)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            try { SetManualActive(!IsManuallyActive, options); }
            catch (Exception ex)
            {
                ToolsDiagnostics.ReportWarning(nameof(MouseHighlighterManager), "Could not start mouse highlighting.", ex);
                ShareX.AvaloniaUI.MessageBox.Show(Localization.Strings.MouseHighlighter_StartFailed + " " + ex.Message,
                    "ShareX", ShareX.AvaloniaUI.MessageBoxButtons.OK, ShareX.AvaloniaUI.MessageBoxIcon.Error);
            }
        });
    }

    public static void ActivateOnStartup(MouseHighlighterOptions options)
    {
        if (options.AutoActivate) Toggle(options);
    }

    internal static void SetManualActive(bool active, MouseHighlighterOptions options)
    {
        if (_shutdown) return;
        MouseHighlighterOptions? previous = _manualOptions;
        _manualOptions = active ? options : null;
        try { ApplyState(); }
        catch
        {
            _manualOptions = previous;
            throw;
        }
    }

    public static void RefreshOptions(MouseHighlighterOptions options)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_service != null && ReferenceEquals(_service.Options, options))
            {
                _service.UpdateOptions(options);
            }
        });
    }

    // The recording worker awaits creation so the overlay is ready before the
    // first captured frame. Disposing the lease also waits for UI cleanup.
    public static async Task<IDisposable> BeginRecordingAsync(MouseHighlighterOptions options)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (_shutdown) throw new InvalidOperationException("ShareX is closing.");
            if (_recordingSession != null) throw new InvalidOperationException("Mouse highlighting is already recording.");
            RecordingSession session = new(options);
            _recordingSession = session;
            try { ApplyState(); }
            catch
            {
                _recordingSession = null;
                throw;
            }
            return (IDisposable)session;
        });
    }

    public static void Shutdown()
    {
        void Close()
        {
            _shutdown = true;
            _manualOptions = null;
            _recordingSession = null;
            _service?.Dispose();
            _service = null;
            _settingsWindow?.Close();
        }
        if (Dispatcher.UIThread.CheckAccess()) Close();
        else Dispatcher.UIThread.Invoke(Close);
    }

    internal static void StopOnError(MouseHighlighterService service, Exception exception)
    {
        if (!ReferenceEquals(_service, service)) return;
        ToolsDiagnostics.ReportWarning(nameof(MouseHighlighterManager), "Mouse highlighting failed.", exception);
        _service.Dispose();
        _service = null;
        _manualOptions = null;
        _recordingSession = null;
        StateChanged?.Invoke();
        ShareX.AvaloniaUI.MessageBox.Show(Localization.Strings.MouseHighlighter_StartFailed + " " + exception.Message,
            "ShareX", ShareX.AvaloniaUI.MessageBoxButtons.OK, ShareX.AvaloniaUI.MessageBoxIcon.Error);
    }

    private static void ApplyState()
    {
        MouseHighlighterOptions? options = _recordingSession?.Options ?? _manualOptions;
        if (options == null)
        {
            _service?.Dispose();
            _service = null;
        }
        else if (_service == null)
        {
            _service = new MouseHighlighterService(options);
        }
        else if (!ReferenceEquals(_service.Options, options))
        {
            _service.UpdateOptions(options);
        }
        StateChanged?.Invoke();
    }

    private sealed class RecordingSession(MouseHighlighterOptions options) : IDisposable
    {
        public MouseHighlighterOptions Options { get; } = options;

        public void Dispose()
        {
            if (_shutdown) return;
            void End()
            {
                if (!ReferenceEquals(_recordingSession, this)) return;
                _recordingSession = null;
                ApplyState();
            }
            if (Dispatcher.UIThread.CheckAccess()) End();
            else Dispatcher.UIThread.Invoke(End);
        }
    }
}
