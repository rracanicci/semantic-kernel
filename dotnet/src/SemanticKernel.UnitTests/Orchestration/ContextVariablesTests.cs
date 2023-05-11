// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Security;
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
        target.SensitiveInput = target.SensitiveInput.UpdateIsTrusted(false);

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
        SensitiveString? entry = target.Get(anyName);

        Assert.NotNull(entry);
        Assert.Equal(anyContent, entry.Value);
        Assert.False(entry.IsTrusted);
    }

    [Fact]
    public void GetNullSucceeds()
    {
        // Arrange
        string anyName = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables();

        // Assert
        SensitiveString? entry = target.Get(anyName);

        Assert.Null(entry);
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
        SensitiveString? targetEntry = target.Get(anyName);

        Assert.Equal(target.Input, mainContent);
        Assert.False(target.IsInputTrusted);

        Assert.NotNull(targetEntry);
        Assert.Equal(anyContent, targetEntry.Value);
        Assert.False(targetEntry.IsTrusted);

        Assert.Equal(mainContent, toBeCloned.Input);
        Assert.True(toBeCloned.IsInputTrusted);
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
    public void CallIsAnyUntrustedSucceeds()
    {
        // Arrange
        string mainContent = Guid.NewGuid().ToString();
        string anyName = Guid.NewGuid().ToString();
        string anyContent = Guid.NewGuid().ToString();
        ContextVariables target = new ContextVariables(mainContent, true);

        // Act
        target.Set(anyName, anyContent, true);

        // Assert
        Assert.False(target.IsAnyUntrusted());

        // Act
        target.Update(anyContent, false);

        // Assert
        Assert.True(target.IsAnyUntrusted());
    }
}
