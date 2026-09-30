using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

/// <summary>System clipboard: Win32, NSPasteboard, X11 selections or Wayland data control.</summary>
/// <remarks>Images are exchanged as PNG bytes so the interface does not depend on System.Drawing, Avalonia or SkiaSharp.</remarks>
public interface IClipboardService
{
    FeatureSupport Support { get; }

    Task<bool> SetTextAsync(string text, CancellationToken cancellationToken = default);

    Task<string?> GetTextAsync(CancellationToken cancellationToken = default);

    Task<bool> SetImageAsync(byte[] png, CancellationToken cancellationToken = default);

    /// <summary>Returns the clipboard image encoded as PNG, or null when the clipboard does not hold an image.</summary>
    Task<byte[]?> GetImageAsync(CancellationToken cancellationToken = default);

    Task<bool> SetFilesAsync(IReadOnlyList<string> paths, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetFilesAsync(CancellationToken cancellationToken = default);

    Task<bool> ClearAsync(CancellationToken cancellationToken = default);
}
