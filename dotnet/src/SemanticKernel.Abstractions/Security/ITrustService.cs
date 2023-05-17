// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// Base interface used to handle trust events and validation.
/// </summary>
public interface ITrustService
{
    /// <summary>
    /// Called to validate the context before:
    /// - Semantic Functions: rendering the prompt used for the text completion client.
    /// - Native Functions: calling the native function.
    /// </summary>
    /// <param name="func">Instance of the function being called</param>
    /// <param name="context">The current execution context</param>
    /// <returns>Should return true if the context is to be considered trusted, or false otherwise.</returns>
    Task<bool> ValidateContextAsync(ISKFunction func, SKContext context);

    /// <summary>
    /// Called to validate the rendered prompt before executing the text completion client
    /// (only for Semantic Functions).
    /// </summary>
    /// <param name="func">Instance of the function being called</param>
    /// <param name="context">The current execution context</param>
    /// <param name="prompt">The current rendered prompt to be used with the completion client.</param>
    /// <returns>Should return a SensitiveString representing the final prompt to be used for the completion client.
    /// The SensitiveString includes trust information.</returns>
    Task<SensitiveString> ValidatePromptAsync(ISKFunction func, SKContext context, string prompt);
}
