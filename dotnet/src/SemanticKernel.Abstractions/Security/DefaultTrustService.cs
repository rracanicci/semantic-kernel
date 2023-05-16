// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// Default implementation of the trust service.
/// When set, throws an exception to stop execution when sensitive functions try
/// to run with untrusted content.
/// </summary>
public class DefaultTrustService : ITrustService
{
    /// <summary>
    /// If set to false, will cause ValidateInput to always return false.
    /// </summary>
    private readonly bool _defaultTrusted;

    /// <summary>
    /// Creates a new default trust handler.
    /// </summary>
    /// <param name="defaultTrusted">If set to false, will cause ValidateInput to always return false (default true).</param>
    public DefaultTrustService(bool defaultTrusted = true)
    {
        this._defaultTrusted = defaultTrusted;
    }

    /// <summary>
    /// Returns true if the context is already trusted and DefaultTrusted is true.
    /// Otherwise, returns false.
    /// </summary>
    /// <param name="func">Instance of the function being called</param>
    /// <param name="context">The current execution context</param>
    /// <returns>Should return true if the context is to be considered trusted, or false otherwise.</returns>
    /// <exception cref="UntrustedContentException">Raised when the context is untrusted and the function is sensitive.</exception>
    public Task<bool> ValidateInputAsync(ISKFunction func, SKContext context)
    {
        if (func.IsSensitive && !context.IsTrusted)
        {
            throw new UntrustedContentException(
                UntrustedContentException.ErrorCodes.SensitiveFunctionWithUntrustedContent,
                 $"Could not run {func.SkillName}.{func.Name}, the function is sensitive and the input untrusted"
            );
        }
        return Task.FromResult(context.IsTrusted && this._defaultTrusted);
    }

    /// <summary>
    /// Returns the prompt wrapped in a SensitiveString. The SensitiveString will
    /// be trusted if the context is already trusted and DefaultTrusted is true.
    /// </summary>
    /// <param name="func">Instance of the function being called</param>
    /// <param name="context">The current execution context</param>
    /// <param name="prompt">The current rendered prompt to be used with the completion client.</param>
    /// <returns>Should return a SensitiveString representing the final prompt to be used with the completion client.
    /// The SensitiveString includes trust information.</returns>
    /// <exception cref="UntrustedContentException">Raised when the context is unstrusted and the function is sensitive.</exception>
    public async Task<SensitiveString> ValidatePromptAsync(ISKFunction func, SKContext context, string prompt)
    {
        return new SensitiveString(
            prompt,
            isTrusted: await this.ValidateInputAsync(func, context).ConfigureAwait(false)
        );
    }
}
