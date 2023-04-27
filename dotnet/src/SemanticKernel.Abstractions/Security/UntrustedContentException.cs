// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Security;

/// <summary>
/// Untrusted content exception.
/// </summary>
public class UntrustedContentException : Exception<UntrustedContentException.ErrorCodes>
{
    /// <summary>
    /// Pure error message without error codes.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error codes for <see cref="UntrustedContentException"/>.
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// Unknown error.
        /// </summary>
        UnknownError = -1,

        /// <summary>
        /// Sensitive function was called with untrusted content.
        /// </summary>
        SensitiveFunctionWithUntrustedContent = 0,
    }

    /// <summary>
    /// Gets the error code of the exception.
    /// </summary>
    public ErrorCodes ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UntrustedContentException"/> class.
    /// </summary>
    /// <param name="errCode">The error code.</param>
    /// <param name="message">The message.</param>
    public UntrustedContentException(ErrorCodes errCode, string? message = null) : base(errCode, message)
    {
        this.ErrorCode = errCode;
        this.ErrorMessage = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UntrustedContentException"/> class.
    /// </summary>
    /// <param name="errCode">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="e">The inner exception.</param>
    public UntrustedContentException(ErrorCodes errCode, string message, Exception? e) : base(errCode, message, e)
    {
        this.ErrorCode = errCode;
        this.ErrorMessage = message;
    }

    #region private ================================================================================

    private UntrustedContentException()
    {
        // Not allowed, error code is required
    }

    private UntrustedContentException(string message) : base(message)
    {
        // Not allowed, error code is required
    }

    private UntrustedContentException(string message, Exception innerException) : base(message, innerException)
    {
        // Not allowed, error code is required
    }

    #endregion
}
