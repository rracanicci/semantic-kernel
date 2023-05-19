// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Security;
using Xunit;

namespace SemanticKernel.UnitTests.Security;

/// <summary>
/// Unit tests of <see cref="SensitiveString"/>.
/// </summary>
public sealed class SensitiveStringTest
{
    [Fact]
    public void CreateNewWithDefaultTrustedSucceeds()
    {
        // Arrange
        string anyValue = Guid.NewGuid().ToString();

        // Act
        SensitiveString value = new SensitiveString(anyValue);

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.True(value.IsTrusted);
    }

    [Fact]
    public void CreateEmptySucceeds()
    {
        // Act
        SensitiveString value = SensitiveString.Empty;

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
        SensitiveString value = new SensitiveString(anyValue, isTrusted);

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.Equal(isTrusted, value.IsTrusted);
    }
}
