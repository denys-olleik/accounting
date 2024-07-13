using System.Reflection;

namespace Accounting.Common
{
    public class UnitTypes
    {
        public const string Piece = "piece";
        public const string Kilogram = "kilogram";
        public const string Meter = "meter";
        public const string Liter = "liter";

        private static readonly List<string> _all = new List<string>();

        static UnitTypes()
        {
            var fields = typeof(UnitTypes).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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