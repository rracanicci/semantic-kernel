// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// Default implementation of the trust handler.
/// When set, throws an exception to stop execution when sensitive functions try
/// to run with untrusted content.
/// </summary>
public class DefaultTrustHandler : ITrustHandler
{
    /// <summary>
    /// If set to false, will cause ValidateInput to always return false.
    /// </summary>
    public bool DefaultTrusted { set; get; }

    /// <summary>
    /// Creates a new default trust handler.
    /// </summary>
    /// <param name="defaultTrusted">If set to false, will cause ValidateInput to always return false (default true).</param>
    public DefaultTrustHandler(bool defaultTrusted = true)
    {
        this.DefaultTrusted = defaultTrusted;
    }

    /// <summary>
    /// Returns true if the context is already trusted and DefaultTrusted is true.
    /// Otherwise, returns false.
    /// </summary>
    /// <param name="func">Instance of the function being called</param>
    /// <param name="context">The current execution context</param>
    /// <param name="prompt">The current rendered prompt to be executed (for semantic functions)</param>
    /// <returns>Should return true if the context/prompts are to be considered trusted, or false otherwise.</returns>
    /// <exception cref="UntrustedContentException">Raised when the context is unstrusted and the function is sensitive.</exception>
    public bool ValidateInput(ISKFunction func, SKContext context, string? prompt)
    {
        if (func.IsSensitive && !context.IsTrusted)
        {
            throw new UntrustedContentException(
                UntrustedContentException.ErrorCodes.SensitiveFunctionWithUntrustedContent,
                 $"Could not run {func.SkillName}.{func.Name}, the function is sensitive and the input untrusted"
            );
        }

        return context.IsTrusted && this.DefaultTrusted;
    }
}
