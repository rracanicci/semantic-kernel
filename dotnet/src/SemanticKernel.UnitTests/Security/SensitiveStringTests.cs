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
        Assert.Equal(anyValue.Length, value.Length);
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
        Assert.Equal(anyValue.Length, value.Length);
    }

    [Fact]
    public void CallToTrustedSucceeds()
    {
        // Arrange
        string anyValue = Guid.NewGuid().ToString();
        SensitiveString value = new SensitiveString(anyValue, false);

        // Act
        SensitiveString trustedValue = value.ToTrusted();

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.False(value.IsTrusted);
        Assert.Equal(anyValue.Length, value.Length);

        Assert.Equal(anyValue, trustedValue.ToString());
        Assert.Equal(anyValue, trustedValue.Value);
        Assert.True(trustedValue.IsTrusted);
        Assert.Equal(anyValue.Length, trustedValue.Length);
    }

    [Fact]
    public void CallToUntrustedSucceeds()
    {
        // Arrange
        string anyValue = Guid.NewGuid().ToString();
        SensitiveString value = new SensitiveString(anyValue, true);

        // Act
        SensitiveString untrustedValue = value.ToUntrusted();

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.True(value.IsTrusted);
        Assert.Equal(anyValue.Length, value.Length);

        Assert.Equal(anyValue, untrustedValue.ToString());
        Assert.Equal(anyValue, untrustedValue.Value);
        Assert.False(untrustedValue.IsTrusted);
        Assert.Equal(anyValue.Length, untrustedValue.Length);
    }

    [Fact]
    public void CallUpdateValueSucceeds()
    {
        // Arrange
        string anyValue = Guid.NewGuid().ToString();
        string newValue = Guid.NewGuid().ToString();
        SensitiveString value = new SensitiveString(anyValue, true);

        // Act
        SensitiveString updatedValue = value.UpdateValue(newValue);

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.True(value.IsTrusted);
        Assert.Equal(anyValue.Length, value.Length);

        Assert.Equal(newValue, updatedValue.ToString());
        Assert.Equal(newValue, updatedValue.Value);
        Assert.True(updatedValue.IsTrusted);
        Assert.Equal(newValue.Length, updatedValue.Length);
    }

    [Fact]
    public void CallUpdateIsTrustedSucceeds()
    {
        // Arrange
        string anyValue = Guid.NewGuid().ToString();
        SensitiveString value = new SensitiveString(anyValue, true);

        // Act
        SensitiveString updatedValue = value.UpdateIsTrusted(false);

        // Assert
        Assert.Equal(anyValue, value.ToString());
        Assert.Equal(anyValue, value.Value);
        Assert.True(value.IsTrusted);
        Assert.Equal(anyValue.Length, value.Length);

        Assert.Equal(anyValue, updatedValue.ToString());
        Assert.Equal(anyValue, updatedValue.Value);
        Assert.False(updatedValue.IsTrusted);
        Assert.Equal(anyValue.Length, updatedValue.Length);
    }
}
