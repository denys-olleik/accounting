using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Common
{
    public static class TaskStatusNames
    {
        public const string Open = "open";
        public const string Closed = "closed";
        public const string Completed = "completed";

        private static readonly List<string> _all = new List<string>();

        static TaskStatusNames()
        {
            var fields = typeof(TaskStatusNames).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string) && field.GetValue(null) is string value)
                {
                    _all.Add(value);
                }
            }
        }

        public static IReadOnlyList<string> All => _all.AsReadOnly();
    }
}