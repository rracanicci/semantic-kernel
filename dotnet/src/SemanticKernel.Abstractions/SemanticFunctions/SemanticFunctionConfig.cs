// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.SemanticFunctions;

/// <summary>
/// Semantic function configuration.
/// </summary>
public sealed class SemanticFunctionConfig
{
    /// <summary>
    /// Prompt template configuration.
    /// </summary>
    public PromptTemplateConfig PromptTemplateConfig { get; }

    /// <summary>
    /// Prompt template.
    /// </summary>
    public IPromptTemplate PromptTemplate { get; }

    /// <summary>
    /// Whether the function is set to be sensitive (default false).
    /// </summary>
    public bool IsSensitive { get; }

    /// <summary>
    /// Constructor for SemanticFunctionConfig.
    /// </summary>
    /// <param name="config">Prompt template configuration.</param>
    /// <param name="template">Prompt template.</param>
    /// <param name="isSensitive">Whether the function is set to be sensitive (default false).</param>
    public SemanticFunctionConfig(
        PromptTemplateConfig config,
        IPromptTemplate template,
        bool isSensitive = false)
    {
        this.PromptTemplateConfig = config;
        this.PromptTemplate = template;
        this.IsSensitive = isSensitive;
    }
}
