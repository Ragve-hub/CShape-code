using System;
using System.Drawing;
using System.Collections;

using OpenQuant.API;
using OpenQuant.API.Indicators;

public class MyStrategy : Strategy
{
   static Hashtable indicators = new Hashtable();
   static Instrument MSFT;
   static Instrument AAPL;
   static TimeSeries mean;
   
   public override void OnStrategyStart()
   {
      Console.WriteLine("Adding " + Instrument);
      
      MSFT = Instruments["MSFT"];
      AAPL = Instruments["AAPL"];
            
      indicators[Instrument] = new MACD(Bars, 7, 14);
      
      if (Instrument == AAPL)
      {
         mean = new TimeSeries("Mean", Color.White);
         
         Draw(mean, 2);
      }
   }

   public override void OnBarSlice(long size)
   {
      if (Instrument == AAPL)
      {
         foreach(Instrument instrument in indicators.Keys)
         {
            Indicator indicator = indicators[instrument] as Indicator;
            
            //if (indicator.Count != 0)
            //   Console.WriteLine(instrument + "  " + indicator.Last);
         }
         
         //Console.WriteLine("=====");
         
         mean.Add(MSFT.Bar.DateTime, (MSFT.Bar.Close + AAPL.Bar.Close) / 2);
      }
   }
}
