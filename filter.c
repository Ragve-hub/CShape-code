public class MyScenario : Scenario
{
   public override void Run()
   {   
      // get reference to strategy project
      
      Project project = Solution.Projects[0];
      
      // clear project instrument list
      
      project.ClearInstruments();
      
      // add most liquid stocks to instrument list
      
      foreach (Instrument instrument in InstrumentManager.Instruments)
         if (instrument.Type == InstrumentType.Stock)
         {
            BarSeries series = DataManager.GetHistoricalBars(instrument, BarType.Time, 86400);
         
            if (series.Count != 0 && series.Last.Volume > 50000000)
            {
               Console.WriteLine("Adding " + instrument);
            
               project.AddInstrument(instrument);
            }
         }
      
      // start backtest
      
      Start();
   }
}
