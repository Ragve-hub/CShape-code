using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;



namespace ConsoleApplication1
{
    class Program
    {
        static void Main()
        { 
        string path0 = @"C:\RIM2\";
            
            Directory.CreateDirectory(path0);

             WebClient webClient = new WebClient();
         


             string path2 = "https://iss.moex.com/iss/engines/futures/markets/forts/securities/RIM2/trades.csv?limit=5000&start=";
             
            
            for (int i = 0; i < 100000; i = i+5000)
              {
                  
                  webClient.DownloadFile(path2 + i, path0+ "trades_"+i +".txt"); 
             }
             
             // delete empty files
             
              //Расширение файла
            string extension = ".txt";
            
            //Размер файла в килобайтах
            int size = 1;
            
            
            foreach (var item in System.IO.Directory.GetFiles(path0))
            {
                System.IO.FileInfo file = new System.IO.FileInfo(item);
                if (System.IO.Path.GetExtension(item) == extension && file.Length / 1024 <= size)
                {
                   
                        file.Delete();
                    }
                }
                
