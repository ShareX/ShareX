namespace ShareX.Platform;

/// <summary>Reports whether a platform feature works in the current session, and why not when it does not.</summary>
public sealed record FeatureSupport(bool IsSupported, string? Reason = null)
{
    public static FeatureSupport Supported { get; } = new FeatureSupport(true);

    public static FeatureSupport NotSupported(string reason) => new FeatureSupport(false, reason);

    /// <summary>The feature works once the user installs the named external tool or package.</summary>
    public static FeatureSupport RequiresTool(string toolDescription) => new FeatureSupport(false, $"Install {toolDescription} to enable this feature.");

    public override string ToString() => IsSupported ? "Supported" : $"Not supported: {Reason}";
}

public enum PermissionState
{
    /// <summary>The platform has no permission gate for this feature.</summary>
    NotRequired,
    Granted,
    Denied,
    /// <summary>The user has not been asked yet, or the state cannot be queried without prompting.</summary>
    Unknown
}
