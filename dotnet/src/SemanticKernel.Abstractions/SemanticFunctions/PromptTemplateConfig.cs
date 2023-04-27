// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.SemanticFunctions;

/// <summary>
/// Prompt template configuration.
/// </summary>
public class PromptTemplateConfig
{
    /// <summary>
    /// Completion configuration parameters.
    /// </summary>
    public class CompletionConfig
    {
        /// <summary>
        /// Sampling temperature to use, between 0 and 2. Higher values will make the output more random.
        /// Lower values will make it more focused and deterministic.
        /// </summary>
        [JsonPropertyName("temperature")]
        [JsonPropertyOrder(1)]
        public double Temperature { get; set; } = 0.0f;

        /// <summary>
        /// Cut-off of top_p probability mass of tokens to consider.
        /// For example, 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// </summary>
        [JsonPropertyName("top_p")]
        [JsonPropertyOrder(2)]
        public double TopP { get; set; } = 0.0f;

        /// <summary>
        /// Lowers the probability of a word appearing if it already appeared in the predicted text.
        /// Unlike the frequency penalty, the presence penalty does not depend on the frequency at which words
        /// appear in past predictions.
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        [JsonPropertyOrder(3)]
        public double PresencePenalty { get; set; } = 0.0f;

        /// <summary>
        /// Controls the model’s tendency to repeat predictions. The frequency penalty reduces the probability
        /// of words that have already been generated. The penalty depends on how many times a word has already
        /// occurred in the prediction.
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        [JsonPropertyOrder(4)]
        public double FrequencyPenalty { get; set; } = 0.0f;

        /// <summary>
        /// Maximum number of tokens that can be generated.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        [JsonPropertyOrder(5)]
        public int MaxTokens { get; set; } = 256;

        /// <summary>
        /// Stop sequences are optional sequences that tells the AI model when to stop generating tokens.
        /// </summary>
        [JsonPropertyName("stop_sequences")]
        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> StopSequences { get; set; } = new();
    }

    /// <summary>
    /// Input parameter for semantic functions.
    /// </summary>
    public class InputParameter
    {
        /// <summary>
        /// Name of the parameter to pass to the function.
        /// e.g. when using "{{$input}}" the name is "input", when using "{{$style}}" the name is "style", etc.
        /// </summary>
        [JsonPropertyName("name")]
        [JsonPropertyOrder(1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Parameter description for UI apps and planner. Localization is not supported here.
        /// </summary>
        [JsonPropertyName("description")]
        [JsonPropertyOrder(2)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Default value when nothing is provided.
        /// </summary>
        [JsonPropertyName("defaultValue")]
        [JsonPropertyOrder(3)]
        public string DefaultValue { get; set; } = string.Empty;
    }

    /// <summary>
    /// Input configuration (list of all input parameters for a semantic function).
    /// </summary>
    public class InputConfig
    {
        [JsonPropertyName("parameters")]
        [JsonPropertyOrder(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<InputParameter> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Schema - Not currently used.
    /// </summary>
    [JsonPropertyName("schema")]
    [JsonPropertyOrder(1)]
    public int Schema { get; set; } = 1;

    /// <summary>
    /// Type, such as "completion", "embeddings", etc.
    /// </summary>
    /// <remarks>TODO: use enum</remarks>
    [JsonPropertyName("type")]
    [JsonPropertyOrder(2)]
    public string Type { get; set; } = "completion";

    /// <summary>
    /// Description
    /// </summary>
    [JsonPropertyName("description")]
    [JsonPropertyOrder(3)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Completion configuration parameters.
    /// </summary>
    [JsonPropertyName("completion")]
    [JsonPropertyOrder(4)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CompletionConfig Completion { get; set; } = new();

    /// <summary>
    /// Default AI services to use.
    /// </summary>
    [JsonPropertyName("default_services")]
    [JsonPropertyOrder(5)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> DefaultServices { get; set; } = new();

    /// <summary>
    /// Input configuration (that is, list of all input parameters).
    /// </summary>
    [JsonPropertyName("input")]
    [JsonPropertyOrder(6)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InputConfig Input { get; set; } = new();

    /// <summary>
    /// If true, the prompt template will be considered trusted (default true).
    /// This is just for the prompt itself, context variables such as the input
    /// are handled separetely.
    /// Untrusted prompts should only be allowed to run with not sensitive functions.
    /// </summary>
    [JsonPropertyName("is_prompt_trusted")]
    [JsonPropertyOrder(7)]
    public bool IsPromptTrusted { get; set; } = true;

    /// <summary>
    /// If true, the output of the function will always be considered to be untrusted
    /// regardless of the input (default false).
    /// </summary>
    [JsonPropertyName("force_output_to_be_untrusted")]
    [JsonPropertyOrder(8)]
    public bool ForceOutputToBeUntrusted { get; set; } = false;

    /// <summary>
    /// Whether the function is set to be sensitive (default false).
    /// Sensitive functions should not be allowed to run with untrusted input.
    /// </summary>
    [JsonPropertyName("is_sensitive")]
    [JsonPropertyOrder(9)]
    public bool IsSensitive { get; set; } = false;

    /// <summary>
    /// Remove some default properties to reduce the JSON complexity.
    /// </summary>
    /// <returns>Compacted prompt template configuration.</returns>
    public PromptTemplateConfig Compact()
    {
        if (this.Completion.StopSequences.Count == 0)
        {
            this.Completion.StopSequences = null!;
        }

        if (this.DefaultServices.Count == 0)
        {
            this.DefaultServices = null!;
        }

        return this;
    }

    /// <summary>
    /// Creates a prompt template configuration from JSON.
    /// </summary>
    /// <param name="json">JSON of the prompt template configuration.</param>
    /// <returns>Prompt template configuration.</returns>
    public static PromptTemplateConfig FromJson(string json)
    {
        var result = Json.Deserialize<PromptTemplateConfig>(json);
        return result ?? throw new ArgumentException("Unable to deserialize prompt template config from argument. The deserialization returned null.", nameof(json));
    }
}
