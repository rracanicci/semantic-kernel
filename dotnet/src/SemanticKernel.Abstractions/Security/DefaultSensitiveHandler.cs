// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.Orchestration;

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// Default implementation of the sensitive handler.
/// When set, throws an exception to stop execution when sensitive functions try
/// to run with untrusted content.
/// </summary>
public class DefaultSensitiveHandler : ISensitiveHandler
{
    /// <inheritdoc/>
    public void OnSensitiveFunctionWithUntrustedContent(
        string skillName, string functionName, bool isSemantic, SKContext context, string? prompt)
    {
        throw new UntrustedContentException(
            UntrustedContentException.ErrorCodes.SensitiveFunctionWithUntrustedContent,
             $"Could not run {skillName}.{functionName}, the function is sensitive and the input untrusted"
        );
    }
}
