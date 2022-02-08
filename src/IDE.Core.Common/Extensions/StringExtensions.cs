using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IDE.Core
{
    public static class StringExtensions
    {
        /// <summary>
        /// http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/791963c8-9e20-4e9e-b184-f0e592b943b0
        /// </summary>
        /// <returns>Ex: casedWordHTTPWriter becomes "Cased Word HTTP Writer", HotMomma becomes "Hot Momma"</returns>
        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        /// <remarks>This was written pre-linq</remarks>
        public static string ToDelimitedString(this List<string> list, string separator = ":", bool insertSpaces = false, string delimiter = "")
        {
            var result = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                string initialStr = list[i];
                var currentString = (delimiter == string.Empty) ? initialStr : string.Format("{1}{0}{1}", initialStr, delimiter);
                if (i < list.Count - 1)
                {
                    currentString += separator;
                    if (insertSpaces)
                    {
                        currentString += ' ';
                    }
                }
                result.Append(currentString);
            }
            return result.ToString();
        }

        public static string GetNextIndexedName(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return "1";

            var isNumber = int.TryParse(inputString, out int number);
            if (isNumber)
                return (number + 1).ToString();

            var matches = Regex.Matches(inputString, @"[0-9]+");

            if (matches.Count == 0)
                return $"{inputString}1";

            var lastMatch = matches[matches.Count - 1];
            number = int.Parse(lastMatch.Value) + 1;

            return $"{inputString.Substring(0, lastMatch.Index)}{number}{inputString.Substring(lastMatch.Index + lastMatch.Length)  }";
        }

        public static void SplitName(this string inputString, out string Prefix, out int number)
        {
            Prefix = null;
            number = -1;

            var matches = Regex.Matches(inputString, @"[0-9]+");

            if (matches.Count == 0)
                Prefix = inputString;
            else
            {
                var lastMatch = matches[matches.Count - 1];
                number = int.Parse(lastMatch.Value);
                Prefix = inputString.Substring(0, lastMatch.Index);

              //  return $"{inputString.Substring(0, lastMatch.Index)}{number}{inputString.Substring(lastMatch.Index + lastMatch.Length)  }";

            }

        }
    }
}
