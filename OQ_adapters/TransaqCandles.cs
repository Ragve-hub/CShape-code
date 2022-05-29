using System.Collections.Generic;

namespace OpenQuant.Finam
{
	public sealed class TransaqCandles
	{
		public int SecId { get; private set; }

		public string SecCode { get; private set; }

		public string Board { get; private set; }

		public int Period { get; private set; }

		public int Status { get; private set; }

		public string SecCodeBoard { get; private set; }

		public List<TransaqCandle> TransaqCandleList { get; private set; }

		public TransaqCandles(string data)
		{
			TransaqCandleList = new List<TransaqCandle>();
			string[] array = data.Split('|');
			string[] array2 = array[0].Split(';');
			for (int i = 0; i < array2.Length - 1; i += 2)
			{
				switch (array2[i])
				{
				case "secid":
					SecId = int.Parse(array2[i + 1]);
					break;
				case "seccode":
					SecCode = array2[i + 1];
					break;
				case "board":
					Board = array2[i + 1];
					break;
				case "period":
					Period = int.Parse(array2[i + 1]);
					break;
				case "status":
					Status = int.Parse(array2[i + 1]);
					break;
				}
			}
			SecCodeBoard = SecCode + "|" + Board;
			for (int j = 1; j < array.Length; j++)
			{
				TransaqCandleList.Add(new TransaqCandle(array[j]));
			}
		}
	}
}
