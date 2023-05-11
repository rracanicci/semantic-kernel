// Copyright (c) Microsoft. All rights reserved.

using System.Linq;
using System.Text.RegularExpressions;

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

    /// <summary>
    /// Creates a copy of the string, keeping the string content but updating the trust flag value.
    /// </summary>
    /// <param name="isTrusted">New trust value</param>
    /// <returns>The new updated sensitive string</returns>
    public SensitiveString UpdateIsTrusted(bool isTrusted)
    {
        return new SensitiveString(this.Value, isTrusted);
    }

    public override string ToString()
    {
        return this.Value;
    }
}
