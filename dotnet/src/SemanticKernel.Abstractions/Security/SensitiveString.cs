// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// A string wrapper that carries trust information.
/// All field are readonly.
/// </summary>
public class SensitiveString
{
    /// <summary>
    /// Create a new empty sensitive string (default trusted).
    /// </summary>
    public static SensitiveString Empty => new SensitiveString(string.Empty, true);

    /// <summary>
    /// The raw string value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Whether the current value is trusted or not.
    /// </summary>
    public bool IsTrusted { get; }

    /// <summary>
    /// Create a new sensitive string.
    /// </summary>
    /// <param name="value">The raw string value</param>
    /// <param name="isTrusted">Whether the raw string value is trusted or not</param>
    public SensitiveString(string value, bool isTrusted = true)
    {
        this.Value = value;
        this.IsTrusted = isTrusted;
    }

    public override string ToString()
    {
        return this.Value;
    }
}
