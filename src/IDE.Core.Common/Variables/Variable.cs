using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IDE.Core.Common.Variables
{
    public class Variable
    {
        public Variable(string key, object value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; }

        public object Value { get; }

        public override string ToString()
        {
            return $"{Key}: {Value}";
        }
    }
}
