﻿using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AuthenticationSdk.util
{
    public class LogUtility
    {
        private static Dictionary<string, string> sensitiveTags = new Dictionary<string, string>();
        private static Dictionary<string, string> authenticationTags = new Dictionary<string, string>();

        public LogUtility()
        {
            if (!loaded)
            {
                LoadSensitiveDataConfiguration();
            }
        }

        /// <summary>
        /// mutex to ensure that the operation is thread safe
        /// </summary>
        private static readonly object mutex = new object();

        /// <summary>
        /// check if the dictionaries have already been loaded
        /// </summary>
        private static bool loaded = false;

        private void LoadSensitiveDataConfiguration()
        {
            lock(mutex)
            {
                sensitiveTags.Clear();
                authenticationTags.Clear();

                sensitiveTags = SensitiveTags.getSensitiveTags();
                authenticationTags = AuthenticationTags.getAuthenticationTags();

                loaded = true;
            }
        }

        public string MaskSensitiveData(string str)
        {
            try
            {
                foreach (KeyValuePair<string, string> tag in sensitiveTags)
                {
                    str = Regex.Replace(str, tag.Key, tag.Value);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                foreach (KeyValuePair<string, string> tag in authenticationTags)
                {
                    str = Regex.Replace(str, tag.Key, tag.Value);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return str;
        }

        public bool IsMaskingEnabled(Logger logger)
        {
            if (!(logger.Factory.Configuration?.Variables?.ContainsKey("enableMasking") ?? false))
            {
                logger.Warn("NLog configuration is missing key/value pair: enableMasking. Assuming true");
                return true;
            }

            return logger.Factory.Configuration.Variables["enableMasking"].ToString().ToLower().Contains("true");
        }

        public string ConvertDictionaryToString(Dictionary<string, string> dict)
        {
            var stringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, string> kvp in dict)
            {
                stringBuilder.Append($"{kvp.Key} = {kvp.Value}\n");
            }

            return stringBuilder.ToString();
        }
    }
}
