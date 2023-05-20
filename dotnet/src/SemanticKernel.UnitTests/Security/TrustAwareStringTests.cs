// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Security;
using Xunit;

namespace SemanticKernel.UnitTests.Security;

/// <summary>
/// Unit tests of <see cref="TrustAwareString"/>.
/// </summary>
public sealed class TrustAwareStringTest
{
    [Fact]
    public void CreateNewWithDefaultTrustedSucceeds()
    {
        // Arrange
        string anyValue = Guid.NewGuid().ToString();

        // Act
        TrustAwareString value = new(anyValue);

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.True(value.IsTrusted);
    }

    [Fact]
    public void CreateEmptySucceeds()
    {
        // Act
        TrustAwareString value = TrustAwareString.Empty;

        // Assert
        Assert.Empty(value.ToString());
        Assert.Empty(value.Value);
        Assert.True(value.IsTrusted);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CreateNewWithIsTrustedValueSucceeds(bool isTrusted)
    {
        // Arrange
        string anyValue = Guid.NewGuid().ToString();

        // Act
        TrustAwareString value = new(anyValue, isTrusted);

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.Equal(isTrusted, value.IsTrusted);
    }

    [Fact]
    public void EqualsAndGetHashCodeSucceeds() {
        // Arrange
        var trustedValue0 = new TrustAwareString("some value 0", true);
        var trustedValue0Copy = new TrustAwareString("some value 0", true);
        var untrustedValue0 = new TrustAwareString("some value 0", false);
        var untrustedValue0Copy = new TrustAwareString("some value 0", false);
        var trustedValue1 = new TrustAwareString("some value 1", true);
        var untrustedValue1 = new TrustAwareString("some value 1", true);
        var stringValue0 = "some value 0";

        // Act and assert
        Assert.True(trustedValue0.Equals(trustedValue0Copy));
        Assert.True(untrustedValue0.Equals(untrustedValue0Copy));
        Assert.True(trustedValue0 == trustedValue0Copy);
        Assert.True(untrustedValue0 == untrustedValue0Copy);

        Assert.False(trustedValue0.Equals(untrustedValue0));
        Assert.False(trustedValue0.Equals(trustedValue1));
        Assert.False(untrustedValue0.Equals(untrustedValue1));
        Assert.True(trustedValue0 != untrustedValue0);
        Assert.True(trustedValue0 != trustedValue1);
        Assert.True(untrustedValue0 != untrustedValue1);

        Assert.False(trustedValue0.Equals(stringValue0));

        Assert.Equal(trustedValue0.GetHashCode(), trustedValue0Copy.GetHashCode());
        Assert.NotEqual(trustedValue0.GetHashCode(), untrustedValue0.GetHashCode());
        Assert.NotEqual(trustedValue0.GetHashCode(), trustedValue1.GetHashCode());
    }
}
