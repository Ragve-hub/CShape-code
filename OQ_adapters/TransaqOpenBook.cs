using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqOpenBook
	{
		public int SecId { get; private set; }

		public string SecCode { get; private set; }

		public string Board { get; private set; }

		public double Price { get; private set; }

		public int Buy { get; private set; }

		public int Sell { get; private set; }

		public string SecCodeBoard { get; private set; }

		public TransaqOpenBook(string data)
		{
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length - 1; i += 2)
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
				case "buy":
					Buy = int.Parse(array[i + 1]);
					break;
				case "sell":
					Sell = int.Parse(array[i + 1]);
					break;
				}
			}
			SecCodeBoard = SecCode + "|" + Board;
		}
	}
}
