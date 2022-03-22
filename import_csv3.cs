
public override void Run()
        {
         Instrument instrument = InstrumentManager["AAPL2"];
         string filepath = "F:\\dataSample.txt";
         using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
         {
            sr.ReadLine(); //header
            
            while(sr.Peek() >= 0)
            {
               string[] parts = sr.ReadLine().Split(',');
               
               //reducing fraction part to be able use in formatter
               string dt_ = parts[0].Substring(0, parts[0].Length-2);
               
               DateTime dt = DateTime.ParseExact(dt_, "yyyy-MM-dd HH:mm:ss.fffffff", null);
               
               if (parts[1] != "NaN") //parse trade
               {
                  double tradePrice = double.Parse(parts[1]);
                  int tradeSize = int.Parse(parts[2]);
                  
                  Trade trade = new Trade(dt, 0, instrument.Id, tradePrice, tradeSize);
                  
                  DataManager.Save(trade);
               }
                  
               if (parts[4] != "NaN") //parse bid
               {
                  double bidPrice = double.Parse(parts[4]);
                  int bidSize = int.Parse(parts[5]);
                  
                  Bid bid = new Bid(dt, 0, instrument.Id, bidPrice, bidSize);
                  
                  DataManager.Save(bid);
               }
               
               if (parts[6] != "NaN") //parse ask
               {
                  double askPrice = double.Parse(parts[6]);
                  int askSize = int.Parse(parts[7]);
                  
                  Ask ask = new Ask(dt, 0, instrument.Id, askPrice, askSize);
                  
                  DataManager.Save(ask);
               }
            }
         }
         
         Console.WriteLine("Finishedd");
        }
       
