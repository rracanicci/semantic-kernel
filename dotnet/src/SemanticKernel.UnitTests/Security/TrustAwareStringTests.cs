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
        TrustAwareString value = new TrustAwareString(anyValue);

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
        TrustAwareString value = new TrustAwareString(anyValue, isTrusted);

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.Equal(isTrusted, value.IsTrusted);
    }
}
