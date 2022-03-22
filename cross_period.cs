using System;
using System.Drawing;

using OpenQuant.API;
using OpenQuant.API.Indicators;

public class MyStrategy : Strategy
{
   SMA sma1;
   SMA sma2;
   
   public override void OnStrategyStart()
   {
      BarSeries series1 = GetSeries(BarType.Time, 100);
      BarSeries series2 = GetSeries(BarType.Time, 200);
      
      sma1 = new SMA(series1, 14, Color.Blue);
      sma2 = new SMA(series2, 14, Color.Red);
      
      Draw(sma1);
      Draw(sma2);
   }

   public override void OnBar(Bar bar)
   {
      if (bar.Size == 100)
         Console.WriteLine("Got 100 second bar");
      
      if (bar.Size == 200)
         Console.WriteLine("Got 200 second bar");
      
      if (sma1.CrossesAbove(sma2, bar))
         Buy(100, "Entry");
      
      if (sma1.CrossesBelow(sma2, bar))
         Sell(100, "Exit");
   }
}
