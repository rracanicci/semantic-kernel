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
    public void CreateWithDefaultIsTrustedSucceeds()
    {
        // Arrange
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables(anyContent);

        // Assert
        Assert.Equal(anyContent, target.Input);
        Assert.True(target.IsInputTrusted);
    }

    [Fact]
    public void CreateWithNotTrustedValueSucceeds()
    {
        // Arrange
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables(anyContent, false);

        // Assert
        Assert.Equal(anyContent, target.Input);
        Assert.False(target.IsInputTrusted);
    }

    [Fact]
    public void SetNotTrustedSucceeds()
    {
        // Arrange
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables(anyContent);

        // Assert
        Assert.Equal(anyContent, target.Input);
        Assert.True(target.IsInputTrusted);

        // Act
        target.Update(target.Input, false);

        // Assert
        Assert.Equal(anyContent, target.Input);
        Assert.False(target.IsInputTrusted);
    }

    [Fact]
    public void UpdateWithNotTrustedSucceeds()
    {
        // Arrange
        string anyContent = Guid.NewGuid().ToString();
        string newContent = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables(anyContent);

        // Assert
        Assert.Equal(anyContent, target.Input);
        Assert.True(target.IsInputTrusted);

        // Act
        target.Update(newContent, false);

        // Assert
        Assert.Equal(newContent, target.Input);
        Assert.False(target.IsInputTrusted);
    }

    [Fact]
    public void SetWithNotTrustedSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables();

        // Act
        target.Set(anyName, anyContent, false);

        // Assert
        AssertContextVariable(target, anyName, anyContent, false);
    }

    [Fact]
    public void GetDoesNotExistSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables();

        // Assert
        var exists = target.Get(anyName, out string value, out bool isTrusted);

        Assert.False(exists);
        Assert.Empty(value);
        Assert.True(isTrusted);
    }

    [Fact]
    public void GetExistSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        string anyValue = Guid.NewGuid().ToString();
        bool anyTrust = false;
        ContextVariables target = new ContextVariables();

        target.Set(anyName, anyValue, anyTrust);

        // Assert
        var exists = target.Get(anyName, out string value, out bool isTrusted);

        Assert.True(exists);
        Assert.Equal(anyValue, value);
        Assert.Equal(anyTrust, isTrusted);
    }

    [Fact]
    public void UpdateCloneSucceeds()
    {
        // Arrange
        string mainContent = Guid.NewGuid().ToString();
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables();
        ContextVariables toBeCloned = new ContextVariables(mainContent, false);

        // Act
        toBeCloned.Set(anyName, anyContent, false);
        target.Update(toBeCloned);
        toBeCloned.Update(mainContent, true);

        // Assert
        AssertContextVariable(target, ContextVariables.MainKey, mainContent, false);
        AssertContextVariable(target, anyName, anyContent, false);
        AssertContextVariable(toBeCloned, ContextVariables.MainKey, mainContent, true);
        AssertContextVariable(toBeCloned, anyName, anyContent, false);
    }

    [Fact]
    public void CallIsAllTrustedSucceeds()
    {
        // Arrange
        string mainContent = Guid.NewGuid().ToString();
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables(mainContent, true);

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
    public void CallMakeAllUntrustedSucceeds()
    {
        // Arrange
        string mainContent = Guid.NewGuid().ToString();
        string anyName0 = Guid.NewGuid().ToString();
        string anyContent0 = Guid.NewGuid().ToString();
        string anyName1 = Guid.NewGuid().ToString();
        string anyContent1 = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables(mainContent, true);

        // Act
        target.Set(anyName0, anyContent0, true);
        target.Set(anyName1, anyContent1, true);

        // Assert
        Assert.True(target.IsAllTrusted());
        AssertContextVariable(target, ContextVariables.MainKey, mainContent, true);
        AssertContextVariable(target, anyName0, anyContent0, true);
        AssertContextVariable(target, anyName1, anyContent1, true);

        // Act
        target.MakeAllUntrusted();

        // Assert
        Assert.False(target.IsAllTrusted());
        AssertContextVariable(target, ContextVariables.MainKey, mainContent, false);
        AssertContextVariable(target, anyName0, anyContent0, false);
        AssertContextVariable(target, anyName1, anyContent1, false);
    }

    private static void AssertContextVariable(ContextVariables variables, string name, string expectedValue, bool expectedIsTrusted)
    {
        var exists = variables.Get(name, out var value, out bool isTrusted);

        Assert.True(exists);
        Assert.Equal(expectedValue, value);
        Assert.Equal(expectedIsTrusted, isTrusted);
    }
}
