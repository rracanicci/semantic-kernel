// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SemanticKernel.Orchestration;
using Xunit;

namespace SemanticKernel.UnitTests.Orchestration;

/// <summary>
/// Unit tests of <see cref="ContextVariables"/>.
/// </summary>
public class ContextVariablesTests
{
    [Fact]
    public void EnumerationOfContextVariableVariablesSucceeds()
    {
        // Arrange
        string firstName = Guid.NewGuid().ToString();
        string firstValue = Guid.NewGuid().ToString();
        string secondName = Guid.NewGuid().ToString();
        string secondValue = Guid.NewGuid().ToString();

        // Act
        ContextVariables target = new();
        target.Set(firstName, firstValue);
        target.Set(secondName, secondValue);

        // Assert
        var items = target.ToArray();

        Assert.Single(items.Where(i => i.Key == firstName && i.Value == firstValue));
        Assert.Single(items.Where(i => i.Key == secondName && i.Value == secondValue));
    }

    [Fact]
    public void IndexGetAfterIndexSetSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyValue = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target[anyName] = anyValue;

        // Assert
        Assert.Equal(anyValue, target[anyName]);
    }

    [Fact]
    public void IndexGetWithoutSetThrowsKeyNotFoundException()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act,Assert
        Assert.Throws<KeyNotFoundException>(() => target[anyName]);
    }

    [Fact]
    public void IndexSetAfterIndexSetSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyValue = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target[anyName] = anyValue;
        target[anyName] = anyValue;

        // Assert
        Assert.Equal(anyValue, target[anyName]);
    }

    [Fact]
    public void IndexSetWithoutGetSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyValue = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target[anyName] = anyValue;

        // Assert
        Assert.Equal(anyValue, target[anyName]);
    }

    [Fact]
    public void SetAfterIndexSetSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target[anyName] = anyContent;
        target.Set(anyName, anyContent);

        // Assert
        Assert.True(target.Get(anyName, out string _));
    }

    [Fact]
    public void SetAfterSetSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target.Set(anyName, anyContent);
        target.Set(anyName, anyContent);

        // Assert
        Assert.True(target.Get(anyName, out string _));
    }

    [Fact]
    public void SetBeforeIndexSetSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target.Set(anyName, anyContent);
        target[anyName] = anyContent;

        // Assert
        Assert.True(target.Get(anyName, out string _));
    }

    [Fact]
    public void SetBeforeSetSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target.Set(anyName, anyContent);
        target.Set(anyName, anyContent);

        // Assert
        Assert.True(target.Get(anyName, out string _));
    }

    [Fact]
    public void SetWithoutGetSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target.Set(anyName, anyContent);

        // Assert
        Assert.True(target.Get(anyName, out string _));
    }

    [Fact]
    public void SetWithoutLabelSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target.Set(anyName, anyContent);

        // Assert
        Assert.True(target.Get(anyName, out string _));
    }

    [Fact]
    public void CreateWithInputDefaultIsTrustedSucceeds()
    {
        // Arrange
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new(anyContent);

        // Assert
        Assert.Equal(anyContent, target.Input);
        AssertContextVariable(target, ContextVariables.MainKey, anyContent, true);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CreateWithInputIsTrustedValueSucceeds(bool isTrusted)
    {
        // Arrange
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new(anyContent, isTrusted);

        // Assert
        Assert.Equal(anyContent, target.Input);
        AssertContextVariable(target, ContextVariables.MainKey, anyContent, isTrusted);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void KeepInputContentAndUpdateIsTrustedSucceeds(bool isTrusted)
    {
        // Arrange
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new(anyContent, !isTrusted);

        // Assert
        Assert.Equal(anyContent, target.Input);
        AssertContextVariable(target, ContextVariables.MainKey, anyContent, !isTrusted);

        // Act
        target.Update(target.Input, isTrusted);

        // Assert
        Assert.Equal(anyContent, target.Input);
        AssertContextVariable(target, ContextVariables.MainKey, anyContent, isTrusted);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UpdateInputContentAndUpdateIsTrustedSucceeds(bool isTrusted)
    {
        // Arrange
        string anyContent = Guid.NewGuid().ToString();
        string newContent = Guid.NewGuid().ToString();
        ContextVariables target = new(anyContent, !isTrusted);

        // Assert
        Assert.Equal(anyContent, target.Input);
        AssertContextVariable(target, ContextVariables.MainKey, anyContent, !isTrusted);

        // Act
        target.Update(newContent, isTrusted);

        // Assert
        Assert.Equal(newContent, target.Input);
        AssertContextVariable(target, ContextVariables.MainKey, newContent, isTrusted);
    }

    [Fact]
    public void UpdateAndKeepTrustSucceeds()
    {
        // Arrange
        string trustedContent = Guid.NewGuid().ToString();
        string untrustedContent = Guid.NewGuid().ToString();
        ContextVariables trustedVar = new(Guid.NewGuid().ToString(), true);
        ContextVariables untrustedVar = new(Guid.NewGuid().ToString(), false);

        // Act
        trustedVar.Update(trustedContent);
        untrustedVar.Update(untrustedContent);

        // Assert
        AssertContextVariable(trustedVar, ContextVariables.MainKey, trustedContent, true);
        AssertContextVariable(untrustedVar, ContextVariables.MainKey, untrustedContent, false);
    }

    [Fact]
    public void UpdateUntrustedSucceeds()
    {
        // Arrange
        string trustedContent = Guid.NewGuid().ToString();
        string untrustedContent = Guid.NewGuid().ToString();
        ContextVariables trustedVar = new(Guid.NewGuid().ToString(), true);
        ContextVariables untrustedVar = new(Guid.NewGuid().ToString(), false);

        // Act
        trustedVar.UpdateUntrusted(trustedContent);
        untrustedVar.UpdateUntrusted(untrustedContent);

        // Assert
        AssertContextVariable(trustedVar, ContextVariables.MainKey, trustedContent, false);
        AssertContextVariable(untrustedVar, ContextVariables.MainKey, untrustedContent, false);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SetWithTrustSucceeds(bool isTrusted)
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        target.Set(anyName, anyContent, isTrusted);

        // Assert
        AssertContextVariable(target, anyName, anyContent, isTrusted);
    }

    [Fact]
    public void GetNameThatDoesNotExistReturnsFalse()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        ContextVariables target = new();

        // Act
        var exists = target.Get(anyName, out string value, out bool isTrusted);

        // Assert
        Assert.False(exists);
        Assert.Empty(value);
        Assert.True(isTrusted);
    }

    [Fact]
    public void UpdateOriginalDoesNotAffectClonedSucceeds()
    {
        // Arrange
        string mainContent = Guid.NewGuid().ToString();
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        string someOtherMainContent = Guid.NewGuid().ToString();
        string someOtherContent = Guid.NewGuid().ToString();
        ContextVariables target = new();
        ContextVariables original = new(mainContent, false);

        original.Set(anyName, anyContent, false);

        // Act
        // Clone original into target
        target.Update(original);
        // Update original
        original.Update(someOtherMainContent, true);
        original.Set(anyName, someOtherContent, true);

        // Assert
        // Target should be the same as the original before the update
        AssertContextVariable(target, ContextVariables.MainKey, mainContent, false);
        AssertContextVariable(target, anyName, anyContent, false);
        // Original should have been updated
        AssertContextVariable(original, ContextVariables.MainKey, someOtherMainContent, true);
        AssertContextVariable(original, anyName, someOtherContent, true);
    }

    [Fact]
    public void CallIsAllTrustedSucceeds()
    {
        // Arrange
        string mainContent = Guid.NewGuid().ToString();
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new(mainContent, true);

        // Act
        target.Set(anyName, anyContent, true);

        // Assert
        Assert.True(target.IsAllTrusted());

        // Act
        target.Set(anyName, anyContent, false);

        // Assert
        Assert.False(target.IsAllTrusted());
    }

    [Fact]
    public void CallUntrustAllSucceeds()
    {
        // Arrange
        string mainContent = Guid.NewGuid().ToString();
        string anyName0 = Guid.NewGuid().ToString();
        string anyContent0 = Guid.NewGuid().ToString();
        string anyName1 = Guid.NewGuid().ToString();
        string anyContent1 = Guid.NewGuid().ToString();
        ContextVariables target = new(mainContent, true);

        // Act
        target.Set(anyName0, anyContent0, true);
        target.Set(anyName1, anyContent1, true);

        // Assert
        // Assert everything is trusted
        Assert.True(target.IsAllTrusted());
        AssertContextVariable(target, ContextVariables.MainKey, mainContent, true);
        AssertContextVariable(target, anyName0, anyContent0, true);
        AssertContextVariable(target, anyName1, anyContent1, true);

        // Act
        target.UntrustAll();

        // Assert
        // Assert everything is now untrusted
        Assert.False(target.IsAllTrusted());
        AssertContextVariable(target, ContextVariables.MainKey, mainContent, false);
        AssertContextVariable(target, anyName0, anyContent0, false);
        AssertContextVariable(target, anyName1, anyContent1, false);
    }

    private static void AssertContextVariable(ContextVariables variables, string name, string expectedValue, bool expectedIsTrusted)
    {
        var exists = variables.Get(name, out var value, out bool isTrusted);

        // Assert the variable exists
        Assert.True(exists);
        // Assert the value matches
        Assert.Equal(expectedValue, value);
        // Assert isTrusted matches
        Assert.Equal(expectedIsTrusted, isTrusted);
    }
}
