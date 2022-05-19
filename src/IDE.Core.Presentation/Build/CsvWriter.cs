using IDE.Documents.Views;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Build
{
    public class CsvWriter
    {

        public CsvWriter(string separator = ",")
        {
            _separator = separator;
        }

        private string _separator;

        public Task WriteCsv(DynamicList list, string savePath)
        {
            return Task.Run(() =>
            {
                var csv = new StringBuilder();

                if (string.IsNullOrEmpty(_separator))
                    _separator = ",";

                //write header
                var properties = list.GetItemProperties(null);
                var pNames = properties.Cast<PropertyDescriptor>()
                                       .Select(p => @$"""{ (string.IsNullOrEmpty(p.DisplayName) ? p.Name : p.DisplayName)}""")
                                       .ToArray();
                
                csv.AppendLine(string.Join(_separator, pNames));

                //write lines
                foreach (var bomItem in list)
                {
                    var values = new List<string>();
                    foreach (var bomProp in bomItem.GetProperties().Cast<PropertyDescriptor>())
                    {
                        var propValue = bomProp.GetValue(bomItem);
                        if (propValue == null)
                            propValue = string.Empty;
                        values.Add($@"""{propValue}""");
                    }

                    csv.AppendLine(string.Join(_separator, values.ToArray()));
                }

                File.WriteAllText(savePath, csv.ToString());

            });
        }
    }
}
