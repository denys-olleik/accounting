using static Accounting.Business.BusinessEntity;

namespace Accounting.Business
{
    public class Helpers
    {
        public static string FormatBusinessEntityName(string firstName, string lastName, string companyName, string customerType)
        {
            string formattedName = "";

            if (customerType == CustomerTypeConstants.Individual)
            {
                formattedName = $"{firstName} {lastName}";
                if (!string.IsNullOrEmpty(companyName))
                {
                    formattedName += $" ({companyName})";
                }
            }
            else if (customerType == CustomerTypeConstants.Company)
            {
                formattedName = companyName;
                string personName = string.Join(" ", new string[] { firstName, lastName }.Where(s => !string.IsNullOrEmpty(s)));
                if (!string.IsNullOrEmpty(personName))
                {
                    formattedName += $" ({personName})";
                }
            }

            return formattedName;
        }
    }
}