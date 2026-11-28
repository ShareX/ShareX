namespace ShareX.ScreenRecordingLib;

/// <summary>Windows picked a video encoder that is not D3D11 hardware accelerated while hardware encoding is required.
/// Derives from <see cref="NotSupportedException"/> so encoder fallbacks keep treating it as "unsupported".</summary>
public sealed class HardwareEncoderUnavailableException(string encoderName)
    : NotSupportedException($"Windows selected '{encoderName}', which does not provide D3D11 hardware video encoding on this GPU. Disable ScreenRecordRequireHardwareEncoder to allow Windows software encoding.")
{
    public string EncoderName { get; } = encoderName;
}
