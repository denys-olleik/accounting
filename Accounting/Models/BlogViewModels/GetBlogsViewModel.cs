﻿namespace Accounting.Models.BlogViewModels
{
  public class GetBlogsViewModel : PaginatedViewModel
  {
    public List<BlogViewModel>? Blogs { get; set; }

    public class BlogViewModel
    {
      public int RowNumber { get; set; }
      public int BlogID { get; set; }
      public string? PublicId { get; set; }
      public string? Title { get; set; }
      public string? Content { get; set; }
    }
  }
}