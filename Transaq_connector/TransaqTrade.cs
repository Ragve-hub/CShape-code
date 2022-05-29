using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqTrade
	{
		public int SecId { get; private set; }

		public string SecCode { get; private set; }

		public string Board { get; private set; }

		public double Price { get; private set; }

		public int Quantity { get; private set; }

		public string SecCodeBoard { get; private set; }

		public TransaqTrade(string data)
		{
			SecId = (Quantity = -1);
			Price = -1.0;
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length - 1; i++)
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
