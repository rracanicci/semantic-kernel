// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Security;
using Xunit;

namespace SemanticKernel.UnitTests.Security;

/// <summary>
/// Unit tests of <see cref="SensitiveString"/>.
/// </summary>
public class SensitiveStringTest
{
    [Fact]
    public void CreateNewWithDefaultsSucceeds()
    {
        // Arrange
        string anyValue = Guid.NewGuid().ToString();

        // Act
        SensitiveString value = new SensitiveString(anyValue);

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.True(value.IsTrusted);
        Assert.Equal(anyValue.Length, value.Value.Length);
    }

    [Fact]
    public void CreateNewWithNotTrustedContentSucceeds()
    {
        // Arrange
        string anyValue = Guid.NewGuid().ToString();

        // Act
        SensitiveString value = new SensitiveString(anyValue, false);

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.False(value.IsTrusted);
        Assert.Equal(anyValue.Length, value.Value.Length);
    }
}
