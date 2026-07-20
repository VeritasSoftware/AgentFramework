using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intellectus.AIAgent.Framework
{
    public class AgentBuilder
    {
        private AgentSettings _settings = new();

        public AgentBuilder AddTool(ITool tool)
        {
            if (_settings.Tools == null) _settings.Tools = new List<ITool>();

            _settings.Tools.Add(tool);
            return this;
        }

        public AgentBuilder AddOpenAIAPIKey(string key)
        {
            _settings.OpenAIAPIKey = key;
            return this;
        }

        public AgentBuilder AddOpenAILLM(string llm)
        {
            _settings.OpenAILLMModel = llm;
            return this;
        }

        public AgentBuilder AddReasoningResult(string reasoningResult)
        {
            _settings.ReasoningResult = reasoningResult;
            return this;
        }

        public Agent ToAgent()
        {
            if (string.IsNullOrEmpty(_settings.OpenAIAPIKey))
            {
                throw new ApplicationException($"{nameof(_settings.OpenAIAPIKey)} is not provided.");
            }
            if (string.IsNullOrEmpty(_settings.OpenAILLMModel))
            {
                throw new ApplicationException($"{nameof(_settings.OpenAILLMModel)} is not provided.");
            }
            if (string.IsNullOrEmpty(_settings.ReasoningResult))
            {
                throw new ApplicationException($"{nameof(_settings.ReasoningResult)} is not provided.");
            }
            if (_settings.Tools == null || !_settings.Tools.Any())
            {
                throw new ApplicationException($"{nameof(_settings.Tools)} is not provided. Use {nameof(this.AddTool)} to add tools.");
            }

            var client = new ChatClient(_settings.OpenAILLMModel, _settings.OpenAIAPIKey);

            return new Agent(_settings, client);
        }
    }
}
