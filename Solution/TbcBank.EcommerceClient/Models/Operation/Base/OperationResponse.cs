﻿using System;
using System.Collections.Generic;

namespace TbcBank.EcommerceClient
{
    public abstract class OperationResponse
    {
        private const char _lineSeparator = '\n';
        private const char _keyValueSeparator = ':';

        private IDictionary<string, string> _keyValuePairs;

        public string RawResponse { get; private set; }
        public string ErrorMessage { get; private set; }

        public virtual bool IsError => ErrorMessage != null;

        public OperationResponse(HttpRequestResult httpResult)
        {
            if (httpResult == null) { throw new ArgumentNullException(nameof(httpResult)); }
            if (httpResult.RawResponse == null) { throw new ArgumentOutOfRangeException(nameof(httpResult.RawResponse)); }

            RawResponse = httpResult.RawResponse;
            if (RawResponse != null)
            {
                ParseRawResponseKeyValues();
            }
        }

        protected abstract void AssignModelValues();

        protected string GetResponseKeyValue(string key)
        {
            return _keyValuePairs.ContainsKey(key)
                 ? _keyValuePairs[key]
                 : null;
        }

        private void ParseRawResponseKeyValues()
        {
            _keyValuePairs = new Dictionary<string, string>();

            if (RawResponse.StartsWith("error:"))
            {
                ErrorMessage = RawResponse;
                return;
            }

            string[] lines = RawResponse.Split(_lineSeparator);
            if (lines.Length == 0) { return; }

            foreach (var line in lines)
            {
                string[] kvp = line.Split(_keyValueSeparator);
                if (kvp.Length < 2) { continue; }

                _keyValuePairs.Add(kvp[0].Trim(), kvp[1].Trim());
            }

            AssignModelValues();
        }
    }
}
