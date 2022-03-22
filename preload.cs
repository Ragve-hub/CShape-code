GetHistoricalBars("IB", datetime1, datetime2, BarType.Time, 60);


using System;
using System.Drawing;

using OpenQuant.API;
using OpenQuant.API.Indicators;

public class MyStrategy : Strategy
{
   SMA sma;
   
   DateTime datetime1;
   DateTime datetime2;
       
   public override void OnStrategyStart()
   {
      datetime2 = DateTime.Now;
      datetime1 = datetime2.AddDays(-5); 

      foreach (Bar bar in GetHistoricalBars(datetime1, datetime2, BarType.Time, 60))
         Bars.Add(bar);             
       
      sma = new SMA(Bars, 128);

      Draw(sma);
   }
}
