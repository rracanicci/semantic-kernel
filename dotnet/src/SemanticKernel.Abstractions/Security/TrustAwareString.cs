// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// A string wrapper that carries trust information.
/// All field are readonly.
/// </summary>
public class TrustAwareString
{
    /// <summary>
    /// Create a new empty trust aware string (default trusted).
    /// </summary>
    public static TrustAwareString Empty => new(string.Empty, true);

    /// <summary>
    /// The raw string value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Whether the current value is trusted or not.
    /// </summary>
    public bool IsTrusted { get; }

    /// <summary>
    /// Create a new trust aware string.
    /// </summary>
    /// <param name="value">The raw string value</param>
    /// <param name="isTrusted">Whether the raw string value is trusted or not</param>
    public TrustAwareString(string value, bool isTrusted = true)
    {
        this.Value = value;
        this.IsTrusted = isTrusted;
    }

    public override string ToString()
    {
        return this.Value;
    }
}
