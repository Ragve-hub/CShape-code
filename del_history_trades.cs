using System;
using System.IO;
using System.Drawing;

using OpenQuant.API;
using OpenQuant.API.Indicators;

public class MyScript : Script
{
   public override void Run()
   {
      string fmt = "";
      
      string symbol = "ESZ8";
      Instrument inst = InstrumentManager.Instruments[symbol];
      if (inst == null) {
         Console.WriteLine("Instrument does not exist. {0}",symbol);         
         return;
      }
      
      // specify date range to delete
      DateTime start = new DateTime(2008,10,22,0,0,0);
      DateTime end = new DateTime (2008,10, 22, 23,59,0);
      
      
      // get the trade series to delete from
      TradeSeries series = DataManager.GetHistoricalTrades(inst,start,end);
      if (series.Count <= 0) {         
         Console.WriteLine("Requested series is empty. {0}", inst.Symbol);
         return;
      }
      // CLAIM: At this point, all trades in our local series should be
      // deleted, since we specified an exact time range of trades to select
      // for deletion. And the count of trades to delete should be non-zero.
      
      // print a summary message before deleting
      int countBefore = series.Count;
      fmt = "{0} series count before deletion is {1}";
      Console.WriteLine (fmt, inst.Symbol,series.Count );
   
      // Loop over all trades of interest and remove the trades      
      fmt = "{0} \tDeleting {1} trades, this can take 10 minutes per thousand...";
      Console.WriteLine (fmt, DateTime.Now, countBefore);
      
      int nDeletedBars = 0;
      int curThousand = 0;
      foreach (Trade foo in series) {
         
         // print a progress message every 10 minutes or so
         if (curThousand >= 1000) {
            fmt = "{0} \tDeleted 1000 trades, now at timestamp {1} with {2} trades to go";
            Console.WriteLine (fmt, DateTime.Now,
               foo.DateTime, countBefore - nDeletedBars );
            curThousand = 0;
         }
         
         // delete one trade and update counters
         DataManager.DeleteTrade(inst,foo.DateTime);   
         nDeletedBars++;
         curThousand++;

      }
      
      // print a summary message
      if (nDeletedBars > 0) {
         TradeSeries tmp = DataManager.GetHistoricalTrades(inst, start, end);
         fmt = "Deleted {0} bars.  {1} series count before= {2}, after deletion = {3}";
         Console.WriteLine (fmt,nDeletedBars, inst.Symbol, countBefore, tmp.Count );
      }
   }
}
