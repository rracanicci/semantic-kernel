// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Security;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace SemanticKernel.UnitTests;

public class KernelTests
{
    [Fact]
    public void ItProvidesAccessToFunctionsViaSkillCollection()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        kernel.Config.AddTextCompletionService(factory.Object);

        var nativeSkill = new MySkill();
        kernel.CreateSemanticFunction(promptTemplate: "Tell me a joke", functionName: "joker", skillName: "jk", description: "Nice fun");
        kernel.ImportSkill(nativeSkill, "mySk");

        // Act
        FunctionsView data = kernel.Skills.GetFunctionsView();

        // Assert - 3 functions, var name is not case sensitive
        Assert.True(data.IsSemantic("jk", "joker"));
        Assert.True(data.IsSemantic("JK", "JOKER"));
        Assert.False(data.IsNative("jk", "joker"));
        Assert.False(data.IsNative("JK", "JOKER"));
        Assert.True(data.IsNative("mySk", "sayhello"));
        Assert.True(data.IsNative("MYSK", "SayHello"));
        Assert.True(data.IsNative("mySk", "ReadSkillCollectionAsync"));
        Assert.True(data.IsNative("MYSK", "readskillcollectionasync"));
        Assert.Single(data.SemanticFunctions["Jk"]);
        Assert.Equal(3, data.NativeFunctions["mySk"].Count);
    }

    [Fact]
    public async Task ItProvidesAccessToFunctionsViaSKContextAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        kernel.Config.AddTextCompletionService(factory.Object);

        var nativeSkill = new MySkill();
        kernel.CreateSemanticFunction("Tell me a joke", functionName: "joker", skillName: "jk", description: "Nice fun");
        var skill = kernel.ImportSkill(nativeSkill, "mySk");

        // Act
        SKContext result = await kernel.RunAsync(skill["ReadSkillCollectionAsync"]);

        // Assert - 3 functions, var name is not case sensitive
        Assert.Equal("Nice fun", result["jk.joker"]);
        Assert.Equal("Nice fun", result["JK.JOKER"]);
        Assert.Equal("Just say hello", result["mySk.sayhello"]);
        Assert.Equal("Just say hello", result["mySk.SayHello"]);
        Assert.Equal("Export info.", result["mySk.ReadSkillCollectionAsync"]);
        Assert.Equal("Export info.", result["mysk.readskillcollectionasync"]);
    }

    [Fact]
    public async Task RunAsyncDoesNotRunWhenCancelledAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        var nativeSkill = new MySkill();
        var skill = kernel.ImportSkill(nativeSkill, "mySk");

        using CancellationTokenSource cts = new();
        cts.Cancel();

        // Act
        SKContext result = await kernel.RunAsync(cts.Token, skill["GetAnyValue"]);

        // Assert
        Assert.True(string.IsNullOrEmpty(result.Result));
        Assert.True(result.ErrorOccurred);
        Assert.True(result.LastException is OperationCanceledException);
    }

    [Fact]
    public async Task RunAsyncRunsWhenNotCancelledAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        var nativeSkill = new MySkill();
        kernel.ImportSkill(nativeSkill, "mySk");

        using CancellationTokenSource cts = new();

        // Act
        SKContext result = await kernel.RunAsync(cts.Token, kernel.Func("mySk", "GetAnyValue"));

        // Assert
        Assert.False(string.IsNullOrEmpty(result.Result));
        Assert.False(result.ErrorOccurred);
        Assert.False(result.LastException is OperationCanceledException);
    }

    [Fact]
    public void ItImportsSkillsNotCaseSensitive()
    {
        // Act
        IDictionary<string, ISKFunction> skill = KernelBuilder.Create().ImportSkill(new MySkill(), "test");

        // Assert
        Assert.Equal(3, skill.Count);
        Assert.True(skill.ContainsKey("GetAnyValue"));
        Assert.True(skill.ContainsKey("getanyvalue"));
        Assert.True(skill.ContainsKey("GETANYVALUE"));
    }

    [Fact]
    public void ItAllowsToImportSkillsInTheGlobalNamespace()
    {
        // Arrange
        var kernel = KernelBuilder.Create();

        // Act
        IDictionary<string, ISKFunction> skill = kernel.ImportSkill(new MySkill());

        // Assert
        Assert.Equal(3, skill.Count);
        Assert.True(kernel.Skills.TryGetFunction("GetAnyValue", out ISKFunction? functionInstance));
        Assert.NotNull(functionInstance);
    }

    [Fact]
    public void ItAllowsToImportTheSameSkillMultipleTimes()
    {
        // Arrange
        var kernel = KernelBuilder.Create();

        // Act - Assert no exception occurs
        kernel.ImportSkill(new MySkill());
        kernel.ImportSkill(new MySkill());
        kernel.ImportSkill(new MySkill());
    }

    [Fact]
    public void ItFailsIfTextCompletionServiceConfigIsNotSet()
    {
        // Arrange
        var kernel = KernelBuilder.Create();

        var exception = Assert.Throws<KernelException>(
            () => kernel.CreateSemanticFunction(promptTemplate: "Tell me a joke", functionName: "joker", skillName: "jk", description: "Nice fun"));
    }

    [Fact]
    public void ItConfiguresCustomTrustServiceInFunctionsBySettingKernelDefaultTrustService()
    {
        // Arrange
        ITrustService trustService = new CustomTrustService();
        var kernel = Kernel.Builder.WithDefaultTrustService(trustService).Build();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        var promptTemplateConfig = new PromptTemplateConfig();
        var promptTemplate = new PromptTemplate("Tell me a joke", promptTemplateConfig, kernel);
        var semanticFunctionConfig = new SemanticFunctionConfig(promptTemplateConfig, promptTemplate, isSensitive: true);

        kernel.Config.AddTextCompletionService(factory.Object);

        // Act
        var nativeSkill = kernel.ImportSkill(new MySkill(), "mySk");
        var semanticSkill0 = kernel.RegisterSemanticFunction(functionName: "joker0", semanticFunctionConfig);
        var semanticSkill1 = kernel.RegisterSemanticFunction(skillName: "jk", functionName: "joker1", semanticFunctionConfig);
        var semanticSkill2 = kernel.CreateSemanticFunction("Tell me a joke", functionName: "joker2", skillName: "jk", description: "Nice fun");
        var sayHelloFunc = (SKFunction)kernel.Skills.GetFunction("mySk", "SayHello");
        var jokerFunc0 = (SKFunction)kernel.Skills.GetFunction("joker0");
        var jokerFunc1 = (SKFunction)kernel.Skills.GetFunction("jk", "joker1");
        var jokerFunc2 = (SKFunction)kernel.Skills.GetFunction("jk", "joker2");

        // Assert
        Assert.NotNull(kernel.DefaultTrustService);
        Assert.NotNull(sayHelloFunc.TrustService);
        Assert.NotNull(jokerFunc0.TrustService);
        Assert.NotNull(jokerFunc1.TrustService);
        Assert.NotNull(jokerFunc1.TrustService);
        Assert.Equal(trustService, kernel.DefaultTrustService);
        Assert.Equal(trustService, sayHelloFunc.TrustService);
        Assert.Equal(trustService, jokerFunc0.TrustService);
        Assert.Equal(trustService, jokerFunc1.TrustService);
        Assert.Equal(trustService, jokerFunc2.TrustService);
    }

    [Fact]
    public void ItConfiguresCustomTrustServiceInFunctionsByFunctionCreationParameter()
    {
        // Arrange
        ITrustService trustService = new CustomTrustService();
        var kernel = Kernel.Builder.Build();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        var promptTemplateConfig = new PromptTemplateConfig();
        var promptTemplate = new PromptTemplate("Tell me a joke", promptTemplateConfig, kernel);
        var semanticFunctionConfig = new SemanticFunctionConfig(promptTemplateConfig, promptTemplate, isSensitive: true);

        kernel.Config.AddTextCompletionService(factory.Object);

        // Act
        var nativeSkill = kernel.ImportSkill(new MySkill(), "mySk", trustService: trustService);
        var semanticSkill0 = kernel.RegisterSemanticFunction(functionName: "joker0", semanticFunctionConfig, trustService: trustService);
        var semanticSkill1 = kernel.RegisterSemanticFunction(skillName: "jk", functionName: "joker1", semanticFunctionConfig, trustService: trustService);
        var semanticSkill2 = kernel.CreateSemanticFunction("Tell me a joke", functionName: "joker2", skillName: "jk", description: "Nice fun", trustService: trustService);
        var sayHelloFunc = (SKFunction)kernel.Skills.GetFunction("mySk", "SayHello");
        var jokerFunc0 = (SKFunction)kernel.Skills.GetFunction("joker0");
        var jokerFunc1 = (SKFunction)kernel.Skills.GetFunction("jk", "joker1");
        var jokerFunc2 = (SKFunction)kernel.Skills.GetFunction("jk", "joker2");

        // Assert
        Assert.Null(kernel.DefaultTrustService);
        Assert.NotNull(sayHelloFunc.TrustService);
        Assert.NotNull(jokerFunc0.TrustService);
        Assert.NotNull(jokerFunc1.TrustService);
        Assert.NotNull(jokerFunc1.TrustService);
        Assert.Equal(trustService, sayHelloFunc.TrustService);
        Assert.Equal(trustService, jokerFunc0.TrustService);
        Assert.Equal(trustService, jokerFunc1.TrustService);
        Assert.Equal(trustService, jokerFunc2.TrustService);
    }

    [Fact]
    public void ItUsesDefaultTrustServiceInFunctionsIfNoneIsProvided()
    {
        // Arrange
        var kernel = Kernel.Builder.Build();
        var factory = new Mock<Func<IKernel, ITextCompletion>>();
        var promptTemplateConfig = new PromptTemplateConfig();
        var promptTemplate = new PromptTemplate("Tell me a joke", promptTemplateConfig, kernel);
        var semanticFunctionConfig = new SemanticFunctionConfig(promptTemplateConfig, promptTemplate, isSensitive: true);

        kernel.Config.AddTextCompletionService(factory.Object);

        // Act
        var nativeSkill = kernel.ImportSkill(new MySkill(), "mySk");
        var semanticSkill0 = kernel.RegisterSemanticFunction(functionName: "joker0", semanticFunctionConfig);
        var semanticSkill1 = kernel.RegisterSemanticFunction(skillName: "jk", functionName: "joker1", semanticFunctionConfig);
        var semanticSkill2 = kernel.CreateSemanticFunction("Tell me a joke", functionName: "joker2", skillName: "jk", description: "Nice fun");
        var sayHelloFunc = (SKFunction)kernel.Skills.GetFunction("mySk", "SayHello");
        var jokerFunc0 = (SKFunction)kernel.Skills.GetFunction("joker0");
        var jokerFunc1 = (SKFunction)kernel.Skills.GetFunction("jk", "joker1");
        var jokerFunc2 = (SKFunction)kernel.Skills.GetFunction("jk", "joker2");

        // Assert
        Assert.Null(kernel.DefaultTrustService);
        Assert.NotNull(sayHelloFunc.TrustService);
        Assert.NotNull(jokerFunc0.TrustService);
        Assert.NotNull(jokerFunc1.TrustService);
        Assert.NotNull(jokerFunc1.TrustService);
        Assert.IsType<DefaultTrustService>(sayHelloFunc.TrustService);
        Assert.IsType<DefaultTrustService>(jokerFunc0.TrustService);
        Assert.IsType<DefaultTrustService>(jokerFunc1.TrustService);
        Assert.IsType<DefaultTrustService>(jokerFunc2.TrustService);
    }

    public class MySkill
    {
        [SKFunction("Return any value.")]
        public string GetAnyValue()
        {
            return Guid.NewGuid().ToString();
        }

        [SKFunction("Just say hello")]
        public void SayHello()
        {
            Console.WriteLine("Hello folks!");
        }

        [SKFunction("Export info.")]
        public async Task<SKContext> ReadSkillCollectionAsync(SKContext context)
        {
            await Task.Delay(0);

            if (context.Skills == null)
            {
                Assert.Fail("Skills collection is missing");
            }

            FunctionsView procMem = context.Skills.GetFunctionsView();

            foreach (KeyValuePair<string, List<FunctionView>> list in procMem.SemanticFunctions)
            {
                foreach (FunctionView f in list.Value)
                {
                    context[$"{list.Key}.{f.Name}"] = f.Description;
                }
            }

            foreach (KeyValuePair<string, List<FunctionView>> list in procMem.NativeFunctions)
            {
                foreach (FunctionView f in list.Value)
                {
                    context[$"{list.Key}.{f.Name}"] = f.Description;
                }
            }

            return context;
        }
    }

    private sealed class CustomTrustService : ITrustService
    {
        public Task<bool> ValidateContextAsync(ISKFunction func, SKContext context)
        {
            throw new NotImplementedException();
        }

        public Task<SensitiveString> ValidatePromptAsync(ISKFunction func, SKContext context, string prompt)
        {
            throw new NotImplementedException();
        }
    }
}
