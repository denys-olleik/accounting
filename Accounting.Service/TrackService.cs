//using Accounting.Business;
//using Accounting.Common;
//using Accounting.Database;

//namespace Accounting.Service
//{
//  public class InvoiceService : BaseService
//  {
//    private readonly JournalService _journalService;
//    private readonly JournalInvoiceInvoiceLineService _journalInvoiceInvoiceLineService;

//    public InvoiceService(
//      JournalService journalService,
//      JournalInvoiceInvoiceLineService journalInvoiceInvoiceLineService) : base()
//    {
//      _journalService = journalService;
//      _journalInvoiceInvoiceLineService = journalInvoiceInvoiceLineService;
//    }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Service
{
  public class TrackService : BaseService
  {
    public TrackService() : base()
    {

    }
    public TrackService(string databaseName, string databasePassword) 
      : base(databaseName, databasePassword)
    {

    }

    public async Task<int> 
  }
}