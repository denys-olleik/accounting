using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
    public class Claim : IIdentifiable<int>
    {
        public int ClaimID { get; set; }
        public int UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public DateTime Created { get; set; }
        public int Identifiable => this.ClaimID;

        public static class CustomClaimTypeConstants
        {
            public const string Password = "password";
            public const string Role = "role";
            public const string OrganizationId = "organizationId";
            public const string OrganizationName = "organizationName";

            private static readonly List<string> _all = new List<string>();

            static CustomClaimTypeConstants()
            {
                var fields = typeof(CustomClaimTypeConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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
}