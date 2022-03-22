
using System;
using System.IO;

using OpenQuant.API;

public class MyScript : Script
{
   public override void Run()
   {
      const string dataDir = @"D:\Data\CSV\";

      long dailyBarSize = 86400;
      
      string[] filenames = new string[]
         {
            "AAPL.csv",
            "CSCO.csv",
            "MSFT.csv"
         };

      // CSV data files have invariant date/number format
      System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
      
      foreach (string filename in filenames)
      {
         Console.WriteLine();
         
         string path = dataDir + filename;

         // check file exists
         if (!File.Exists(path))
         {
            Console.WriteLine(string.Format("File {0} does not exists.", path));
            
            continue;
         }
         
         Console.WriteLine(string.Format("Processing file {0} ...", path));
         
         // get instrument
         string symbol = filename.Substring(0, filename.IndexOf('.'));

         Instrument instrument = InstrumentManager.Instruments[symbol];
         
         if (instrument == null)
         {
            Console.WriteLine(string.Format("Instrument {0} does not exist.", symbol));
            
            continue;
         }
         
         // read file and parse data
         StreamReader reader = new StreamReader(path);
         
         reader.ReadLine(); // skip CSV header
         
         string line = null;
         
         while ((line = reader.ReadLine()) != null)
         {
            string[] items = line.Split(',');

            // parse data
            DateTime date = DateTime.ParseExact(items[0], "yyyy-M-d", culture);

            double high  = double.Parse(items[1], culture);
            double low   = double.Parse(items[2], culture);
            double open  = double.Parse(items[3], culture);
            double close = double.Parse(items[4], culture);
            
            long volume  = long.Parse(items[5], culture);

            // add daily bar
            DataManager.Add(
               instrument,
               date,
               open,
               high,
               low,
               close,
               volume,
               dailyBarSize);
         }
         
         reader.Close();
         
         //
         Console.WriteLine(string.Format("CSV data for {0} was successfully imported.", instrument.Symbol));
      }
   }
}
