// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.Orchestration;

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// Base interface used to handle trust events in sensitive functions.
/// </summary>
public interface ISensitiveHandler
{
    /// <summary>
    /// Called when the kernel identifies an attempt to run a sensitive function
    /// with untrusted content. This runs before the function itself.
    /// </summary>
    /// <param name="skillName">Name of the skill to be executed</param>
    /// <param name="functionName">Name of the function to be executed</param>
    /// <param name="isSemantic">Whether the function to be executed is semantic (true) or native (false)</param>
    /// <param name="context">The current execution context</param>
    /// <param name="prompt">The current rendered prompt to be executed (for semantic functions)</param>
    void OnSensitiveFunctionWithUntrustedContent(
        string skillName, string functionName, bool isSemantic, SKContext context, string? prompt);
}
