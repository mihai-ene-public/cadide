using System;
using System.Collections.Generic;

namespace IDE.Core.Common.Variables
{
    public class VariablesContext : List<Variable>
    {

        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

        #region Utilities

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string
        /// </summary>
        /// <param name="original">Original string</param>
        /// <param name="pattern">The string to be replaced</param>
        /// <param name="replacement">The string to replace all occurrences of pattern string</param>
        /// <returns>A string that is equivalent to the current string except that all instances of pattern are replaced with replacement string</returns>
        protected string Replace(string original, string pattern, string replacement)
        {
            //for case sensitive comparison use base string.Replace() method
            var stringComparison = StringComparison;
            if (stringComparison == StringComparison.Ordinal)
                return original.Replace(pattern, replacement);

            //or do some routine work here
            var count = 0;
            var position0 = 0;
            int position1;

            var inc = original.Length / pattern.Length * (replacement.Length - pattern.Length);
            var chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = original.IndexOf(pattern, position0, stringComparison)) != -1)
            {
                for (var i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (var i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }

            if (position0 == 0)
                return original;

            for (var i = position0; i < original.Length; ++i)
                chars[count++] = original[i];

            return new string(chars, 0, count);
        }

        /// <summary>
        /// Replace tokens
        /// </summary>
        /// <param name="template">The template with token keys inside</param>
        /// <param name="tokens">The sequence of tokens to use</param>
        /// <param name="htmlEncode">The value indicating whether tokens should be HTML encoded</param>
        /// <param name="stringWithQuotes">The value indicating whether string token values should be wrapped in quotes</param>
        /// <returns>Text with all token keys replaces by token value</returns>
        protected string ReplaceTokens(string template)
        {
            foreach (var token in this)
            {
                var tokenValue = token.Value ?? string.Empty;

                template = Replace(template, $@"{{{token.Key}}}", tokenValue.ToString());
            }

            return template;
        }


        #endregion

        public string Replace(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
                throw new ArgumentNullException(nameof(inputString));


            //replace tokens
            inputString = ReplaceTokens(inputString);

            return inputString;
        }
    }
}
