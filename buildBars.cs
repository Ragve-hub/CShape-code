
using System;
using System.Drawing;
using OpenQuant.API;
using OpenQuant.API.Indicators;

public class MyStrategy : Strategy
{
   public override void OnTrade(Trade trade)
   {
      if(trade.Price > 0 && trade.Size > 0) {
         BuildBarsFromTrades(BarType.Time,60,Trades,Bars);
         //BuildBarsFromTrades(BarType.Tick,100,Trades,Bars);
         //BuildBarsFromTrades(BarType.Volume,100,Trades,Bars);
      }
   }

   public void BuildBarsFromTrades(BarType barType, long barSize, TradeSeries trades, BarSeries bars)
   {
      switch(barType)
      {
         case BarType.Time:
            BuildTimeBarsFromTrades(barSize,trades.Last,bars);
            break;
         case BarType.Tick:
            BuildTickBarsFromTrades(barSize,trades,bars);
            break;
         case BarType.Volume:      
            BuildVolumeBarsFromTrades(barSize,trades.Last,bars);
            break;
      }
   
   }            
         
   public void BuildTimeBarsFromTrades(long barSize, Trade trade, BarSeries bars)
   {
      if(barSize < 1){
         throw(new Exception("barSize must be > 0"));
      }

      DateTime nextBarEndTime = new DateTime();
      if(bars.Count > 0){   //calculate the end time of the last bar
         long lastBarEndInSeconds = bars.Last.EndTime.Ticks / 10000000;
         long nextBarEndInSeconds = ((lastBarEndInSeconds + barSize)/barSize)*barSize;
         nextBarEndTime = nextBarEndTime.AddSeconds(nextBarEndInSeconds);
      }
      if(trade.DateTime < nextBarEndTime)
      {    //merge bar into previous bar
         bars.Add(BarType.Time,barSize,bars.Last.BeginTime, trade.DateTime,
            bars.Last.Open,
            Math.Max(bars.Last.High,trade.Price),
            Math.Min(bars.Last.Low,trade.Price),
            Trade.Price,
            bars.Last.Volume + trade.Size,
            0);  //Replaces the last bar
      }else{
         //Add new bar   
         bars.Add(BarType.Time,barSize,
            trade.DateTime,trade.DateTime,
            trade.Price,trade.Price,trade.Price,trade.Price,
            trade.Size,0);
      }
   }
   
   public void BuildTickBarsFromTrades(long barSize, TradeSeries trades, BarSeries bars)
   {
      Trade trade = trades.Last;
      if(barSize < 1){
         throw(new Exception("barSize must be > 0"));
      }
      //check if the last bar is full

      if(bars.Count > 0 && (((trades.Count / barSize)*barSize)< trades.Count))
      {    //merge bar into previous bar
         bars.Add(BarType.Tick,barSize,bars.Last.BeginTime, trade.DateTime,
            bars.Last.Open,
            Math.Max(bars.Last.High,trade.Price),
            Math.Min(bars.Last.Low,trade.Price),
            trade.Price,
            bars.Last.Volume + trade.Size,
            0);  //Replaces the last bar
      }else{
         //Add a small amount of time to the new bar
         //new bar must be later than last bar or it will replace the last bar.
         DateTime newTradeTime = trade.DateTime;
         if(bars.Count > 0 && bars.Last.BeginTime >= trade.DateTime){
            newTradeTime = bars.Last.BeginTime.AddTicks(1);   
         }else{
            newTradeTime = trade.DateTime;
         }         
         //Add new bar   
         bars.Add(BarType.Tick,barSize,
            newTradeTime,newTradeTime,
            trade.Price,trade.Price,trade.Price,trade.Price,
            trade.Size,0);
      }            
   }

   public void BuildVolumeBarsFromTrades(long barSize, Trade trade, BarSeries bars)
   {
      if(barSize < 1){
         throw(new Exception("barSize must be > 0"));
      }

      //make sure not to access bars.Last if there are no bars
      //volumeRemaining is the unfilled portion of the last bar
      DateTime newTradeTime = trade.DateTime;
      long volumeRemaining = 0;
      if(bars.Count > 0) {
         //new trade must be later than previous bar or it won't add.
         if(bars.Last.BeginTime >= trade.DateTime){
            newTradeTime = bars.Last.BeginTime.AddTicks(1);   
         }         
         volumeRemaining =  barSize - bars.Last.Volume;
      }   
      //merge with last volume bar
      long volumeAdd = 0;
      if(volumeRemaining > 0){
         volumeAdd = Math.Min(volumeRemaining,trade.Size);
         bars.Add(BarType.Volume,barSize,bars.Last.BeginTime, trade.DateTime,
            bars.Last.Open,
            Math.Max(bars.Last.High,trade.Price),
            Math.Min(bars.Last.Low,trade.Price),
            trade.Price,
            bars.Last.Volume + volumeAdd,
            0);  //Replaces the last bar
      }
      //deduct the volume that was just added from volumeRemaining
      volumeRemaining = trade.Size - volumeAdd;
      //add new volume bars until there is no more volumeRemaining
      while(volumeRemaining > 0){
         volumeAdd = Math.Min(volumeRemaining,barSize);
         bars.Add(BarType.Volume,barSize,
            newTradeTime,newTradeTime,
            trade.Price,trade.Price,trade.Price,trade.Price,
            volumeAdd,0);
         volumeRemaining = volumeRemaining - volumeAdd;
         //new trade must be later than previous bar or it won't add.
         newTradeTime = bars.Last.BeginTime.AddTicks(1);   
      }                  
   }
} //MyStrategy()
