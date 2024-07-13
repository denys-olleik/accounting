using Dapper;

namespace Accounting.Database
{
  public class QueryBuilder
  {
    public DynamicParameters Parameters { get; private set; }
    public List<string> Conditions { get; private set; }

    public QueryBuilder()
    {
      Parameters = new DynamicParameters();
      Conditions = new List<string>();
    }

    public void AddParameter(string name, object value)
    {
      Parameters.Add(name, value);
    }

    public void AddSearchCondition(string column, string keyword)
    {
      string parameterName = $"Keyword_{Conditions.Count}";
      Parameters.Add(parameterName, "%" + keyword + "%");
      Conditions.Add($"{column} LIKE @{parameterName}");
    }

    public string BuildSearchLogic()
    {
      return Conditions.Count > 0 ? "AND (" + string.Join(" OR ", Conditions) + ")" : "";
    }
  }
}