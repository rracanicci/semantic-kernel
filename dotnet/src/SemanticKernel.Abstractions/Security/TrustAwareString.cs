// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// A string wrapper that carries trust information.
/// All field are readonly.
/// </summary>
public class TrustAwareString : IEquatable<TrustAwareString>
{
    /// <summary>
    /// Create a new empty trust aware string (default trusted).
    /// </summary>
    public static TrustAwareString Empty => new TrustAwareString(string.Empty, true);

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

    public override bool Equals(object obj)
    {
        if (obj is null) return false;

        return (obj is TrustAwareString other) && this.Equals(other);
    }

    public bool Equals(TrustAwareString? other)
    {
        if (other is null) return false;

        return this.Value == other.Value && this.IsTrusted == other.IsTrusted;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.IsTrusted);
    }

    public static bool operator ==(TrustAwareString? left, TrustAwareString? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(TrustAwareString? left, TrustAwareString? right)
    {
        return !(left == right);
    }
}
