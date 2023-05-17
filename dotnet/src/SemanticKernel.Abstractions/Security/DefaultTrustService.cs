// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// Default implementation of the trust service.
///
/// This is just a simple example implementation that will be used by default if no other is provided.
///
/// When set, throws an exception to stop execution when sensitive functions try to run with untrusted content.
/// </summary>
public class DefaultTrustService : ITrustService
{
    /// <summary>
    /// Creates a new DefaultTrustService with default parameters.
    /// </summary>
    public static DefaultTrustService Default => new DefaultTrustService();

    /// <summary>
    /// If set to false, will cause ValidateContextAsync to always return false and ValidatePromptAsync
    /// to always flag the prompt as untrusted.
    /// </summary>
    private readonly bool _defaultTrusted;

    /// <summary>
    /// Creates a new default trust handler.
    /// </summary>
    /// <param name="defaultTrusted">If set to false, will cause ValidateContextAsync to always return false and ValidatePromptAsync
    /// to always flag the prompt as untrusted (default true).</param>
    public DefaultTrustService(bool defaultTrusted = true)
    {
        this._defaultTrusted = defaultTrusted;
    }

    /// <summary>
    /// It will check if the provided context is trusted or not by only analysing the trust flag of all the variables stored on it. It will not look at the actual
    /// content of the variables, only their trust flags. In case any of the variables is untrusted and the function is tagged as sensitive,
    /// an UntrustedContentException will be thrown to stop execution.
    ///
    /// This will return true if all the variables in context are flagged as trusted and if the service was created with defaultTrusted = true.
    /// Meaning that if we want to force the result of a function to be unstrusted, we could create this service with defaultTrusted = false.
    ///
    /// NOTE: This is only a simple example implementation that propagates the trust checks using the trust flags from
    /// the variables. Another implementations might consider analyzing the content of the variables to decide if the context is trusted or not, or even, update
    /// the content of the variable to something considered trusted (sanitization).
    /// </summary>
    /// <param name="func">Instance of the function being called</param>
    /// <param name="context">The current execution context</param>
    /// <returns>Should return true if the context is to be considered trusted, or false otherwise.</returns>
    /// <exception cref="UntrustedContentException">Raised when the context is untrusted and the function is sensitive.</exception>
    public Task<bool> ValidateContextAsync(ISKFunction func, SKContext context)
    {
        if (func.IsSensitive && !context.IsTrusted)
        {
            throw new UntrustedContentException(
                UntrustedContentException.ErrorCodes.SensitiveFunctionWithUntrustedContent,
                $"Could not run {func.SkillName}.{func.Name}, the function is sensitive and the input untrusted"
            );
        }
        // If defaultTrusted == false, will always return false
        return Task.FromResult(context.IsTrusted && this._defaultTrusted);
    }

    /// <summary>
    /// This is a only a sample implementation that uses the trust information of the variables in the context to decide
    /// if the current prompt is considered to be trusted or not.
    ///
    /// If any of the variables in the context is untrusted or if defaultTrusted is set to false at the service creation,
    /// the resulting prompt returned will have the same content as the prompt parameter, but will be flagged as untrusted.
    ///
    /// Since this is called after the template is rendered, the context could had been considered trusted when ValidateContextAsync was called,
    /// but might have became untrusted by a function call included as part of the prompt template. So this sample implementation will
    /// check the context again using the sample ValidateContextAsync implementation.
    ///
    /// Returns the prompt wrapped in a SensitiveString.
    ///
    /// NOTE: This is only a simple example implementation that propagates the trust checks using the trust flags from
    /// the variables. Another implementations might consider analyzing the content of the rendered prompt and the variables to decide if the prompt
    /// is trusted or not, or even, update the content of the variable/return a new prompt string with something considered trusted (sanitization).
    /// </summary>
    /// <param name="func">Instance of the function being called</param>
    /// <param name="context">The current execution context</param>
    /// <param name="prompt">The current rendered prompt to be used with the completion client.</param>
    /// <returns>Should return a SensitiveString representing the final prompt to be used with the completion client.
    /// The SensitiveString includes trust information.</returns>
    /// <exception cref="UntrustedContentException">Raised when the context is untrusted and the function is sensitive.</exception>
    public async Task<SensitiveString> ValidatePromptAsync(ISKFunction func, SKContext context, string prompt)
    {
        return new SensitiveString(
            // This is only a sample implementation that directly returns the prompt
            prompt,
            // The content of the prompt will not be used in this example validation
            isTrusted: await this.ValidateContextAsync(func, context).ConfigureAwait(false)
        );
    }
}
