using System;
using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqTick
	{
		public int SecId { get; private set; }

		public string SecCode { get; private set; }

		public string Board { get; private set; }

		public DateTime TradeTime { get; private set; }

		public double Price { get; private set; }

		public int Quantity { get; private set; }

		public string SecCodeBoard { get; private set; }

		public TransaqTick(string data)
		{
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i += 2)
			{
				switch (array[i])
				{
				case "secid":
					SecId = int.Parse(array[i + 1]);
					break;
				case "seccode":
					SecCode = array[i + 1];
					break;
				case "board":
					Board = array[i + 1];
					break;
				case "tradetime":
					TradeTime = DateTime.ParseExact(array[i + 1], "d.M.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
					break;
				case "price":
					Price = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "quantity":
					Quantity = int.Parse(array[i + 1]);
					break;
				}
			}
			SecCodeBoard = SecCode + "|" + Board;
		}
	}
}
