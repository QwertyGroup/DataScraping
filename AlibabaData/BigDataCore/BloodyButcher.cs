using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BigDataCore
{
    public class BloodyButcher // csv parser
    {
        public List<string> ToCSV(List<(string brand, List<List<(string field, string val)>> models)> data)
        {
            var csv = new List<string>();
            var allfields = GatherAllFields(data);

            // Add headings to csv
            csv.Add(new Func<string>(() =>
            {
                var fields = allfields.Select(f => f.Key);
                var line = string.Empty;
                foreach (var f in fields)
                    if (f != fields.Last())
                        line += $"{f}☭";
                    else line += f;
                return line;
            }).Invoke());

            // Add rows to csv
            foreach (var brand in data)
                foreach (var model in brand.models)
                {
                    if (!model.Where(m => m.field == "Link").First().val.Contains("http://www.chrono24.com")) continue;
                    if (model.Where(m => m.field == "Brand").ToList().Count < 1) continue;

                    var str = string.Empty;
                    var fields = new Dictionary<string, string>(allfields);
                    foreach (var field in model)
                    {
                        if (fields[field.field].Length == 0)
                        {
                            if (field.field == "Model")
                            {
                                var kek = model.Where(m => m.field == "Brand");
                                //if (kek == null || kek.ToList().Count == 0) continue;
                                var bname = kek.First().val;
                                fields[field.field] = field.val.Replace(bname.Replace(" ", string.Empty), $"{bname} ");
                            }
                            else if (field.field == "Price")
                            {
                                var rgx = new Regex(@"(\$[0-9]*\,[0-9]*\,[0-9]*\,[0-9]*)|(\$[0-9]*\,[0-9]*\,[0-9]*)|(\$[0-9]*\,[0-9]*)|(\$[0-9]*)");
                                var res = rgx.Matches(field.val);
                                if (res.Count > 0)
                                    fields[field.field] = res[res.Count - 1].Value.Replace("$", string.Empty).Replace(",", string.Empty);
                                else
                                    fields[field.field] = string.Empty;
                            }
                            else
                            {
                                fields[field.field] = field.val;
                            }
                        }
                        else
                            if ((fields[field.field] != field.val) &&
                            (fields[field.field].Trim() != string.Empty))
                            fields[field.field] += $"; {field.val}";
                    }

                    // to string
                    foreach (var pair in fields)
                        if (pair.Key != fields.Last().Key)
                            str += pair.Value + "☭";
                        else
                            str += pair.Value;
                    csv.Add(str);
                }

            return csv;
        }

        private Dictionary<string, string> GatherAllFields(List<(string brand, List<List<(string field, string val)>> models)> data)
        {
            var fields = new Dictionary<string, int>();
            foreach (var brand in data)
                foreach (var model in brand.models)
                    foreach (var field in model)
                        if (!fields.ContainsKey(field.field)) fields.Add(field.field, 1);
                        else fields[field.field]++;
            var purefields = fields.OrderBy(f => f.Value).Select(f => f.Key).Reverse().ToList();
            var toreturn = new Dictionary<string, string>();
            foreach (var f in purefields) toreturn.Add(f, string.Empty);
            return toreturn;
        }
    }
}
