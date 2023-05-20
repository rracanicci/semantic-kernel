// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Security;

namespace Microsoft.SemanticKernel.Orchestration;

/// <summary>
/// Context Variables is a data structure that holds temporary data while a task is being performed.
/// It is accessed and manipulated by functions in the pipeline.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DebuggerTypeProxy(typeof(ContextVariables.TypeProxy))]
public sealed class ContextVariables : IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>
    /// In the simplest scenario, the data is an input string, stored here.
    /// </summary>
    public string Input => this._variables[MainKey].Value;

    /// <summary>
    /// Constructor for context variables.
    /// </summary>
    /// <param name="content">Optional value for the main variable of the context.</param>
    /// <param name="isTrusted">Whether the content is trusted or not (default true).</param>
    public ContextVariables(string? content = null, bool isTrusted = true)
    {
        this._variables[MainKey] = new TrustAwareString(content ?? string.Empty, isTrusted);
    }

    /// <summary>
    /// Updates the main input text with the new value after a function is complete.
    /// </summary>
    /// <param name="content">The new input value, for the next function in the pipeline, or as a result for the user
    /// if the pipeline reached the end.</param>
    /// <param name="isTrusted">Whether the content is trusted or not (default true).</param>
    /// <returns>The current instance</returns>
    public ContextVariables Update(string? content, bool isTrusted = true)
    {
        this._variables[MainKey] = new TrustAwareString(content ?? string.Empty, isTrusted);
        return this;
    }

    /// <summary>
    /// Updates all the local data with new data, merging the two datasets.
    /// Do not discard old data
    /// </summary>
    /// <param name="newData">New data to be merged</param>
    /// <param name="merge">Whether to merge and keep old data, or replace. False == discard old data.</param>
    /// <returns>The current instance</returns>
    public ContextVariables Update(ContextVariables newData, bool merge = true)
    {
        if (!object.ReferenceEquals(this, newData))
        {
            // If requested, discard old data and keep only the new one.
            if (!merge) { this._variables.Clear(); }

            foreach (KeyValuePair<string, TrustAwareString> varData in newData._variables)
            {
                this._variables[varData.Key] = varData.Value;
            }
        }

        return this;
    }

    /// <summary>
    /// This method allows to store additional data in the context variables, e.g. variables needed by functions in the
    /// pipeline. These "variables" are visible also to semantic functions using the "{{varName}}" syntax, allowing
    /// to inject more information into prompt templates.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Value to store. If the value is NULL the variable is deleted.</param>
    /// <param name="isTrusted">Whether the value's content is trusted or not (default true).</param>
    /// TODO: support for more complex data types, and plan for rendering these values into prompt templates.
    public void Set(string name, string? value, bool isTrusted = true)
    {
        Verify.NotNullOrWhiteSpace(name);
        if (value != null)
        {
            this._variables[name] = new TrustAwareString(value, isTrusted);
        }
        else
        {
            this._variables.TryRemove(name, out _);
        }
    }

    /// <summary>
    /// Fetch a variable value and if its content is trusted from the context variables.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Value</param>
    /// <param name="isTrusted">Whether the variable value is trusted or not</param>
    /// <returns>Whether the value exists in the context variables</returns>
    public bool Get(string name, out string value, out bool isTrusted)
    {
        TrustAwareString result;

        if (this._variables.TryGetValue(name, out result!))
        {
            value = result.Value;
            isTrusted = result.IsTrusted;
            return true;
        }

        value = string.Empty;
        isTrusted = true;

        return false;
    }

    /// <summary>
    /// Fetch a variable value from the context variables.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Value</param>
    /// <returns>Whether the value exists in the context variables</returns>
    public bool Get(string name, out string value)
    {
        return this.Get(name, out value, out _);
    }

    /// <summary>
    /// Array of all variables in the context variables.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <returns>The value of the variable.</returns>
    public string this[string name]
    {
        get => this._variables[name].Value;
        set
        {
            // This will be trusted by default for now.
            // TODO: we could plan to replace string usages in the kernel
            // with TrustAwareString, so here "value" could directly be a trust aware string
            // including trust information
            this._variables[name] = new TrustAwareString(value);
        }
    }

    /// <summary>
    /// Returns true if there is a variable with the given name
    /// </summary>
    /// <param name="key"></param>
    /// <returns>True if there is a variable with the given name, false otherwise</returns>
    public bool ContainsKey(string key)
    {
        return this._variables.ContainsKey(key);
    }

    /// <summary>
    /// True if all the stored variables have trusted content.
    /// </summary>
    /// <returns></returns>
    public bool IsAllTrusted()
    {
        return this._variables.Values.All(v => v.IsTrusted);
    }

    /// <summary>
    /// Make all the variables stored in the context untrusted.
    /// </summary>
    public void MakeAllUntrusted()
    {
        // Create a copy of the variables map iterator with ToList to avoid
        // iterating in the map while updating it
        foreach (var item in this._variables.ToList())
        {
            // Note: we don't use an internal setter for better multi-threading
            this._variables[item.Key] = new TrustAwareString(item.Value.Value, false);
        }
    }

    /// <summary>
    /// Print the processed input, aka the current data after any processing occurred.
    /// </summary>
    /// <returns>Processed input, aka result</returns>
    public override string ToString()
    {
        return this.Input;
    }

    /// <summary>
    /// Get an enumerator that iterates through the context variables.
    /// </summary>
    /// <returns>An enumerator that iterates through the context variables</returns>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        // For now, return an iterator that does not directly expose the trust aware string
        foreach (KeyValuePair<string, TrustAwareString> kv in this._variables)
        {
            yield return new KeyValuePair<string, string>(kv.Key, kv.Value.Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._variables.GetEnumerator();
    }

    /// <summary>
    /// Create a copy of the current instance with a copy of the internal data
    /// </summary>
    /// <returns>Copy of the current instance</returns>
    public ContextVariables Clone()
    {
        var clone = new ContextVariables();
        foreach (KeyValuePair<string, TrustAwareString> x in this._variables)
        {
            clone.Set(x.Key, x.Value.Value, x.Value.IsTrusted);
        }

        return clone;
    }

    internal const string MainKey = "INPUT";

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal string DebuggerDisplay =>
        this._variables.TryGetValue(MainKey, out var input) && !string.IsNullOrEmpty(input.Value) ?
            $"Variables = {this._variables.Count}, Input = {input.Value}" :
            $"Variables = {this._variables.Count}";

    /// <summary>
    /// Updates the main input text with the new value after a function is complete.
    /// Retain the trust boolean, e.g. don't allow to switch from Not Trusted to Trusted.
    /// </summary>
    /// <param name="content">The new input value, for the next function in the pipeline, or as a result for the user
    /// if the pipeline reached the end.</param>
    /// <param name="isTrusted">Whether the content is trusted or not (default true).</param>
    /// <returns>The current instance</returns>
    internal ContextVariables UpdateWithTrustCheck(string? content, bool isTrusted)
    {
        var currentTrust = this._variables[MainKey].IsTrusted;
        this._variables[MainKey] = new TrustAwareString(content ?? string.Empty, currentTrust && isTrusted);
        return this;
    }

    #region private ================================================================================

    /// <summary>
    /// Important: names are case insensitive
    /// </summary>
    private readonly ConcurrentDictionary<string, TrustAwareString> _variables = new(StringComparer.OrdinalIgnoreCase);

    private sealed class TypeProxy
    {
        private readonly ContextVariables _variables;

        public TypeProxy(ContextVariables variables) => _variables = variables;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, string>[] Items => _variables._variables.ToDictionary(kv => kv.Key, kv => kv.Value.Value).ToArray();
    }

    #endregion
}
