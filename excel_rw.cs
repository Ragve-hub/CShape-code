To build the sample you should add a reference to the Interop.Excel.dll, using the following steps:

locate the Interop.Excel.dll file at “[Installation Directory]\Framework\bin” folder
copy it to the “[Installation Directory]\bin” folder
from the main menu go to Tools -> Options, in the opened Options dialog click “Projects and Solutions” node and select “Build” node. Click “Add” button, choose “.NET…” option and choose the Interop.Excel.dll file.
using System;
using System.Drawing;

using OpenQuant.API;
using OpenQuant.API.Indicators;

using Interop.Excel;

public class MyStrategy : Strategy
{
   SMA sma1;
   SMA sma2;
   
   Workbook workbook;
   Worksheet worksheet;
   
   public override void OnStrategyStart()
   {
      // open Excel file
      Application excel = new Application();

      workbook = excel.Workbooks.Open(@"C:\Users\s.b\Documents\SampleBook.xlsx", 0, false, 5, "", "", true, Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, null, null);

      Sheets sheets = workbook.Worksheets;
      worksheet = (Worksheet)sheets.get_Item(1);

      // read A1 and A2 values
      double length1 = (double)worksheet.get_Range("A1", "A1").Value2;
      double length2 = (double)worksheet.get_Range("A2", "A2").Value2;
      
      //      
      sma1 = new SMA(Bars, (int)length1);
      sma2 = new SMA(Bars, (int)length2);
   }

   public override void OnBar(Bar bar)
   {
      worksheet.get_Range("B1", "B1").Value2 = bar.Close;      
   }

   public override void OnStrategyStop()
   {
      // save and close the workbook
      workbook.Save();
      workbook.Close(true, null, null);
   }
}
