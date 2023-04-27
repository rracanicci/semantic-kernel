// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using Xunit;

namespace SemanticKernel.UnitTests.SkillDefinition;

public sealed class SKFunctionTests2
{
    private readonly Mock<ILogger> _log;
    private readonly Mock<IReadOnlySkillCollection> _skills;

    private static string s_expected = string.Empty;
    private static string s_canary = string.Empty;

    public SKFunctionTests2()
    {
        this._log = new Mock<ILogger>();
        this._skills = new Mock<IReadOnlySkillCollection>();

        s_canary = "";
        s_expected = Guid.NewGuid().ToString("D");
    }

    [Fact]
    public void ItHasDefaultTrustSettings()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static void Test()
        {
            s_canary = s_expected;
        }

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);

        // Assert
        Assert.NotNull(function);
        Assert.False(function.ForceOutputToBeUntrusted);
        Assert.False(function.IsSensitive);
    }

    [Fact]
    public void ItSetsTrustSettings()
    {
        // Arrange
        [SKFunction("Test", forceOutputToBeUntrusted: true, isSensitive: true)]
        [SKFunctionName("Test")]
        static void Test()
        {
            s_canary = s_expected;
        }

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);

        // Assert
        Assert.NotNull(function);
        Assert.True(function.ForceOutputToBeUntrusted);
        Assert.True(function.IsSensitive);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType1Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static void Test()
        {
            s_canary = s_expected;
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(1);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType2Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static string Test()
        {
            s_canary = s_expected;
            return s_expected;
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(2);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, result.Result);
        Assert.Equal(s_expected, context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType3Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static Task<string> Test()
        {
            s_canary = s_expected;
            return Task.FromResult(s_expected);
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(3);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context.Result);
        Assert.Equal(s_expected, result.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType4Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static void Test(SKContext cx)
        {
            s_canary = s_expected;
            cx["canary"] = s_expected;
        }

        var context = this.MockContext("xy", isTrusted);
        context["someVar"] = "qz";

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(4);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType5Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static string Test(SKContext cx)
        {
            s_canary = cx["someVar"];
            return "abc";
        }

        var context = this.MockContext("", isTrusted);
        context["someVar"] = s_expected;

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(5);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal("abc", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Fact]
    public async Task ItSupportsType5NullableAsync()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        string? Test(SKContext cx)
        {
            s_canary = cx["someVar"];
            return "abc";
        }

        var context = this.MockContext("");
        context["someVar"] = s_expected;

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(5);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal("abc", context.Result);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType6Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        Task<string> Test(SKContext cx)
        {
            s_canary = s_expected;
            cx.Variables["canary"] = s_expected;
            return Task.FromResult(s_expected);
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(6);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_canary, context.Result);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType7Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        Task<SKContext> Test(SKContext cx)
        {
            s_canary = s_expected;
            cx.Variables.Update("foo");
            cx["canary"] = s_expected;
            return Task.FromResult(cx);
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(7);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal("foo", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Fact]
    public async Task ItKeepsInputTrustType7Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        Task<SKContext> Test(SKContext cx)
        {
            s_canary = s_expected;
            // Setting this variable as untrusted
            cx.Variables.Update("foo", false);
            cx["canary"] = s_expected;
            return Task.FromResult(cx);
        }

        var context = this.MockContext("");

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(7);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal("foo", context.Result);
        // This should result in an untrusted output
        Assert.False(result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsAsyncType7Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        async Task<SKContext> TestAsync(SKContext cx)
        {
            await Task.Delay(0);
            s_canary = s_expected;
            cx.Variables.Update("foo");
            cx["canary"] = s_expected;
            return cx;
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(TestAsync), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(7);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal("foo", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType8Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        void Test(string input)
        {
            s_canary = s_expected + input;
        }

        var context = this.MockContext(".blah", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(8);
        Assert.Equal(s_expected + ".blah", s_canary);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType9Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        string Test(string input)
        {
            s_canary = s_expected;
            return "foo-bar";
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(9);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal("foo-bar", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType10Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        Task<string> Test(string input)
        {
            s_canary = s_expected;
            return Task.FromResult("hello there");
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(10);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal("hello there", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType11Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        void Test(string input, SKContext cx)
        {
            s_canary = s_expected;
            cx.Variables.Update("x y z");
            cx["canary"] = s_expected;
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(11);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal("x y z", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType12Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static string Test(string input, SKContext cx)
        {
            s_canary = s_expected;
            cx["canary"] = s_expected;
            cx.Variables.Update("x y z");
            // This value should overwrite "x y z"
            return "new data";
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(12);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal("new data", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType13Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static Task<string> Test(string input, SKContext cx)
        {
            s_canary = s_expected;
            cx["canary"] = s_expected;
            cx.Variables.Update("x y z");
            // This value should overwrite "x y z"
            return Task.FromResult("new data");
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(13);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal("new data", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType14Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static Task<SKContext> Test(string input, SKContext cx)
        {
            s_canary = s_expected;
            cx["canary"] = s_expected;
            cx.Variables.Update("x y z");

            // This value should overwrite "x y z". Contexts are merged.
            var newCx = new SKContext(
                new ContextVariables(input),
                skills: new Mock<IReadOnlySkillCollection>().Object);

            newCx.Variables.Update("new data");
            newCx["canary2"] = "222";

            return Task.FromResult(newCx);
        }

        var oldContext = this.MockContext("", isTrusted);
        oldContext["legacy"] = "something";

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext newContext = await function.InvokeAsync(oldContext);

        // Assert
        Assert.False(oldContext.ErrorOccurred);
        Assert.False(newContext.ErrorOccurred);
        this.VerifyFunctionTypeMatch(14);

        Assert.Equal(s_expected, s_canary);

        Assert.True(oldContext.Variables.ContainsKey("canary"));
        Assert.False(oldContext.Variables.ContainsKey("canary2"));

        Assert.False(newContext.Variables.ContainsKey("canary"));
        Assert.True(newContext.Variables.ContainsKey("canary2"));

        Assert.Equal(s_expected, oldContext["canary"]);
        Assert.Equal("222", newContext["canary2"]);

        Assert.True(oldContext.Variables.ContainsKey("legacy"));
        Assert.False(newContext.Variables.ContainsKey("legacy"));

        Assert.Equal("x y z", oldContext.Result);
        Assert.Equal("new data", newContext.Result);
        Assert.Equal(expectedTrustResult, newContext.IsTrusted);
    }

    [Fact]
    public async Task ItKeepsInputTrustType14Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static Task<SKContext> Test(string input, SKContext cx)
        {
            s_canary = s_expected;
            cx["canary"] = s_expected;
            cx.Variables.Update("x y z");

            // This value should overwrite "x y z". Contexts are merged.
            var newCx = new SKContext(
                new ContextVariables(input),
                skills: new Mock<IReadOnlySkillCollection>().Object);

            // Setting the trust of the input to be false
            newCx.Variables.Update("new data", false);
            newCx["canary2"] = "222";

            return Task.FromResult(newCx);
        }

        var oldContext = this.MockContext("");
        oldContext["legacy"] = "something";

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext newContext = await function.InvokeAsync(oldContext);

        // Assert
        Assert.False(oldContext.ErrorOccurred);
        Assert.False(newContext.ErrorOccurred);
        this.VerifyFunctionTypeMatch(14);

        Assert.Equal(s_expected, s_canary);

        Assert.True(oldContext.Variables.ContainsKey("canary"));
        Assert.False(oldContext.Variables.ContainsKey("canary2"));

        Assert.False(newContext.Variables.ContainsKey("canary"));
        Assert.True(newContext.Variables.ContainsKey("canary2"));

        Assert.Equal(s_expected, oldContext["canary"]);
        Assert.Equal("222", newContext["canary2"]);

        Assert.True(oldContext.Variables.ContainsKey("legacy"));
        Assert.False(newContext.Variables.ContainsKey("legacy"));

        Assert.Equal("x y z", oldContext.Result);
        Assert.Equal("new data", newContext.Result);
        // Check if the trust of the input is still false
        Assert.False(newContext.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType15Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static Task Test(string input)
        {
            s_canary = s_expected;
            return Task.CompletedTask;
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(15);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType16Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static Task Test(SKContext cx)
        {
            s_canary = s_expected;
            cx["canary"] = s_expected;
            cx.Variables.Update("x y z");
            return Task.CompletedTask;
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(16);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal("x y z", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType17Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static Task Test(string input, SKContext cx)
        {
            s_canary = s_expected;
            cx["canary"] = s_expected;
            cx.Variables.Update(input + "x y z");
            return Task.CompletedTask;
        }

        var context = this.MockContext("input:", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(17);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal("input:x y z", context.Result);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    public async Task ItSupportsType18Async(bool isTrusted, bool forceOutputToBeUntrusted, bool expectedTrustResult)
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static Task Test()
        {
            s_canary = s_expected;
            return Task.CompletedTask;
        }

        var context = this.MockContext("", isTrusted);

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        function.ForceOutputToBeUntrusted = forceOutputToBeUntrusted;
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(18);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(expectedTrustResult, result.IsTrusted);
    }

    private static MethodInfo Method(Delegate method)
    {
        return method.Method;
    }

    private SKContext MockContext(string input, bool isTrusted = true)
    {
        return new SKContext(
            new ContextVariables(input, isTrusted),
            skills: this._skills.Object,
            logger: this._log.Object);
    }

    private void VerifyFunctionTypeMatch(int typeNumber)
    {
#pragma warning disable CS8620
        // Verify that the expected function has been called
        this._log.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Trace),
                It.Is<EventId>(eventId => eventId.Id == typeNumber),
                It.Is<It.IsAnyType>((@object, @type) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // Verify that unexpected functions have not been called (e.g. missing a return statement)
        // TODO: check for "Executing function type" instead of using "eventId.Id != 0"
        // Note: eventId.Id != 0 is used to avoid catching other Log.Trace events and failing the tests
        this._log.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Trace),
                It.Is<EventId>(eventId => eventId.Id != 0 && eventId.Id != typeNumber),
                It.Is<It.IsAnyType>((@object, @type) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
#pragma warning restore CS8620
    }
}
