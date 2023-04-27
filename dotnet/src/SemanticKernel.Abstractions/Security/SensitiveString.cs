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
    /// The length of the wrapped string.
    /// </summary>
    public int Length => this.Value.Length;

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
    /// Creates a copy of the string value with trusted content.
    /// This only updates the internal trust flag.
    /// </summary>
    /// <returns>The new sensitive string with trusted content</returns>
    public SensitiveString ToTrusted()
    {
        return new SensitiveString(this.Value, true);
    }

    /// <summary>
    /// Creates a copy of the string value with untrusted content.
    /// This only updates the internal trust flag.
    /// </summary>
    /// <returns>The new sensitive string with untrusted content</returns>
    public SensitiveString ToUntrusted()
    {
        return new SensitiveString(this.Value, false);
    }

    /// <summary>
    /// Creates a copy of the string, keeping the trust flag but updating the string value.
    /// </summary>
    /// <param name="value">New string value</param>
    /// <returns>The new updated sensitive string</returns>
    public SensitiveString UpdateValue(string value)
    {
        return new SensitiveString(value, this.IsTrusted);
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

    #region StringOperations

    public SensitiveString Trim()
    {
        return new SensitiveString(this.Value.Trim(), this.IsTrusted);
    }

    public SensitiveString[] Split(params char[] separator)
    {
        return this.Value.Split(separator).Select(s => new SensitiveString(s, this.IsTrusted)).ToArray();
    }

    public SensitiveString Substring(int startIndex, int length)
    {
        return new SensitiveString(this.Value.Substring(startIndex, length), this.IsTrusted);
    }

    public SensitiveString Substring(int startIndex)
    {
        return new SensitiveString(this.Value.Substring(startIndex), this.IsTrusted);
    }

    public bool IsMatch(string pattern)
    {
        return Regex.IsMatch(this.Value, pattern);
    }

    public char this[int index]
    {
        get => this.Value[index];
    }

    public override string ToString()
    {
        return this.Value;
    }

    public static bool IsNullOrEmpty(SensitiveString? str)
    {
        return string.IsNullOrEmpty(str?.Value);
    }

    #endregion
}
