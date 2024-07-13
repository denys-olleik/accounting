namespace Accounting.Common
{
  public interface IIdentifiable<out TKey>
  {
    TKey Identifiable { get; }
  }
}