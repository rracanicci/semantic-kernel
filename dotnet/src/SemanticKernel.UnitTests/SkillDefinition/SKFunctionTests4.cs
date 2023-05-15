// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Security;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using Xunit;

namespace SemanticKernel.UnitTests.SkillDefinition;

public sealed class SKFunctionTests4
{
    [Fact]
    public async Task SemanticSensitiveFunctionShouldNotFailWithTrustedInputAsync()
    {
        // Arrange
        ITrustService trustService = new DefaultTrustService();
        var kernel = Kernel.Builder.WithDefaultTrustService(trustService).Build();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        var aiService = new Mock<ITextCompletion>();
        var context = new ContextVariables("my input");

        factory.Setup(x => x.Invoke(kernel)).Returns(aiService.Object);
        kernel.Config.AddTextCompletionService(factory.Object);

        var func = kernel.CreateSemanticFunction(
            "Tell me a joke",
            functionName: "joker",
            skillName: "jk",
            description: "Nice fun",
            isSensitive: true
        );

        // Act
        var result = await kernel.RunAsync(context, func);

        // Assert
        Assert.False(result.ErrorOccurred);
    }

    [Fact]
    public async Task SemanticSensitiveFunctionShouldFailWithUntrustedInputAsync()
    {
        // Arrange
        ITrustService trustService = new DefaultTrustService();
        var kernel = Kernel.Builder.WithDefaultTrustService(trustService).Build();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        var aiService = new Mock<ITextCompletion>();
        var context = new ContextVariables("my input", false);

        factory.Setup(x => x.Invoke(kernel)).Returns(aiService.Object);
        kernel.Config.AddTextCompletionService(factory.Object);

        var func = kernel.CreateSemanticFunction(
            "Tell me a joke",
            functionName: "joker",
            skillName: "jk",
            description: "Nice fun",
            isSensitive: true
        );

        // Act
        var result = await kernel.RunAsync(context, func);

        // Assert
        Assert.True(result.ErrorOccurred);
        Assert.IsType<UntrustedContentException>(result.LastException);
        Assert.Equal(
            UntrustedContentException.ErrorCodes.SensitiveFunctionWithUntrustedContent,
            ((UntrustedContentException)result.LastException).ErrorCode
        );
    }

    [Fact]
    public async Task SemanticSensitiveFunctionWithDefaultUntrustedAsync()
    {
        // Arrange
        ITrustService trustService = new DefaultTrustService(defaultTrusted: false);
        var kernel = Kernel.Builder.WithDefaultTrustService(trustService).Build();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        var aiService = new Mock<ITextCompletion>();
        var context = new ContextVariables("my input");

        factory.Setup(x => x.Invoke(kernel)).Returns(aiService.Object);
        kernel.Config.AddTextCompletionService(factory.Object);

        var func = kernel.CreateSemanticFunction(
            "Tell me a joke",
            functionName: "joker",
            skillName: "jk",
            description: "Nice fun",
            isSensitive: true
        );

        // Act
        var result = await kernel.RunAsync(context, func);

        // Assert
        Assert.False(result.ErrorOccurred);
        Assert.False(result.IsTrusted);
    }

    [Fact]
    public async Task SemanticSensitiveFunctionShouldFailWithUntrustedTemplateRenderAsync()
    {
        // Arrange
        var trustService = new DefaultTrustService();
        var promptTemplateConfig = new PromptTemplateConfig();
        var promptTemplate = new Mock<IPromptTemplate>();

        // Mock this to make the context untrusted when the template is rendered
        promptTemplate.Setup(x => x.RenderAsync(It.IsAny<SKContext>()))
            .Callback<SKContext>(ctx => ctx.IsTrusted = false)
            .ReturnsAsync("unsafe prompt");

        promptTemplate
            .Setup(x => x.GetParameters())
            .Returns(new List<ParameterView>());

        var functionConfig = new SemanticFunctionConfig(promptTemplateConfig, promptTemplate.Object, isSensitive: true);
        var func = SKFunction.FromSemanticConfig(
            "exampleSkill",
            "exampleFunction",
            functionConfig,
            trustService
        );
        var aiService = new Mock<ITextCompletion>();

        func.SetAIService(() => aiService.Object);

        // Act
        var result = await func.InvokeAsync();

        // Assert
        Assert.True(result.ErrorOccurred);
        Assert.IsType<UntrustedContentException>(result.LastException);
        Assert.Equal(
            UntrustedContentException.ErrorCodes.SensitiveFunctionWithUntrustedContent,
            ((UntrustedContentException)result.LastException).ErrorCode
        );
    }

    [Fact]
    public async Task NativeSensitiveFunctionShouldNotFailWithTrustedInputAsync()
    {
        // Arrange
        ITrustService trustService = new DefaultTrustService();
        var kernel = Kernel.Builder.WithDefaultTrustService(trustService).Build();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        var aiService = new Mock<ITextCompletion>();
        var context = new ContextVariables("my input");

        factory.Setup(x => x.Invoke(kernel)).Returns(aiService.Object);
        kernel.Config.AddTextCompletionService(factory.Object);

        var func = kernel.ImportSkill(new MySkill(), nameof(MySkill))["Function1"];

        // Act
        var result = await kernel.RunAsync(context, func);

        // Assert
        Assert.False(result.ErrorOccurred);
    }

    [Fact]
    public async Task NativeSensitiveFunctionShouldFailWithUntrustedInputAsync()
    {
        // Arrange
        ITrustService trustService = new DefaultTrustService();
        var kernel = Kernel.Builder.WithDefaultTrustService(trustService).Build();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        var aiService = new Mock<ITextCompletion>();
        var context = new ContextVariables("my input", false);

        factory.Setup(x => x.Invoke(kernel)).Returns(aiService.Object);
        kernel.Config.AddTextCompletionService(factory.Object);

        var func = kernel.ImportSkill(new MySkill(), nameof(MySkill))["Function1"];

        // Act
        var result = await kernel.RunAsync(context, func);

        // Assert
        Assert.True(result.ErrorOccurred);
        Assert.IsType<UntrustedContentException>(result.LastException);
        Assert.Equal(
            UntrustedContentException.ErrorCodes.SensitiveFunctionWithUntrustedContent,
            ((UntrustedContentException)result.LastException).ErrorCode
        );
    }

    private sealed class MySkill
    {
        [SKFunction("Function1", isSensitive: true)]
        public void Function1()
        { }
    }
}
