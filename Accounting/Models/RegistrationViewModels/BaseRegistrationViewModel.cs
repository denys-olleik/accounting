namespace Accounting.Models.RegistrationViewModels
{
  public abstract class BaseRegistrationViewModel
  {
    private string? _email;
    private string? _firstName;
    private string? _lastName;
    private string? _password;

    public string? Email
    {
      get => _email;
      set => _email = value?.Trim();
    }
    public string? FirstName
    {
      get => _firstName;
      set => _firstName = value?.Trim();
    }
    public string? LastName
    {
      get => _lastName;
      set => _lastName = value?.Trim();
    }
    public string? Password
    {
      get => _password;
      set => _password = value?.Trim();
    }
    public FluentValidation.Results.ValidationResult? ValidationResult { get; set; } = new();

    protected static readonly char[] _disallowedCharacters = { ';', '&', '|', '>', '<', '$', '\\', '`', '"', '\'', '/', '%', '*' };

    public static bool DoesNotContainDisallowedCharacters(string? input)
    {
      if (string.IsNullOrEmpty(input))
        return true;

      foreach (var ch in _disallowedCharacters)
      {
        if (input.Contains(ch))
        {
          return false;
        }
      }

      return true;
    }
  }
}