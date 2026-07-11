#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

namespace ShareX.ImageEditor.Integration.Diagnostics;

public enum EditorDiagnosticLevel
{
    Information,
    Warning,
    Error,
    /// <summary>
    /// Verbose troubleshooting (e.g. effect preview pipeline). Hosts may filter or write to debug log only.
    /// </summary>
    Debug
}

/// <summary>
/// Immutable diagnostic payload emitted by ImageEditor and consumed by host apps.
/// </summary>
public sealed class EditorDiagnosticEvent
{
    public EditorDiagnosticEvent(EditorDiagnosticLevel level, string source, string message, Exception? exception = null)
    {
        Level = level;
        Source = source;
        Message = message;
        ExceptionText = exception?.ToString();
        TimestampUtc = DateTimeOffset.UtcNow;
    }

    public EditorDiagnosticLevel Level { get; }
    public string Source { get; }
    public string Message { get; }
    public string? ExceptionText { get; }
    public DateTimeOffset TimestampUtc { get; }
}

/// <summary>
/// Host-provided sink for ImageEditor diagnostics and exception telemetry.
/// </summary>
public interface IEditorDiagnosticsSink
{
    void Report(EditorDiagnosticEvent diagnosticEvent);
}

public sealed class DelegateEditorDiagnosticsSink : IEditorDiagnosticsSink
{
    private readonly Action<EditorDiagnosticEvent> _handler;

    public DelegateEditorDiagnosticsSink(Action<EditorDiagnosticEvent> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public void Report(EditorDiagnosticEvent diagnosticEvent)
    {
        _handler(diagnosticEvent);
    }
}