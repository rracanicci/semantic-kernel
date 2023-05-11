// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// Base interface used to handle trust events and validation.
/// </summary>
public interface ITrustHandler
{
    /// <summary>
    /// Called to validate the input context before: executing a completion for a semantic function;
    /// or calling a native function.
    /// 
    /// - For semantic functions there will be two calls to this, one with the raw context, before the
    /// template is rendered into a prompt (prompt will be null). And other after the template is
    /// rendered into a prompt (prompt will be rendered template).
    /// - For native functions the prompt parameter will always be null.
    /// </summary>
    /// <param name="func">Instance of the function being called</param>
    /// <param name="context">The current execution context</param>
    /// <param name="prompt">The current rendered prompt to be executed (for semantic functions)</param>
    /// <returns>Should return true if the context/prompts are to be considered trusted, or false otherwise.</returns>
    bool ValidateInput(ISKFunction func, SKContext context, string? prompt);
}
