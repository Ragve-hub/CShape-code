using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqClientTrade
	{
		public long TradeNo { get; private set; }

		public long OrderNo { get; private set; }

		public double Commission { get; private set; }

		public double Price { get; private set; }

		public int Quantity { get; private set; }

		public TransaqClientTrade(string data)
		{
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length - 1; i++)
			{
				switch (array[i])
				{
				case "tradeno":
					TradeNo = long.Parse(array[i + 1]);
					break;
				case "orderno":
					OrderNo = long.Parse(array[i + 1]);
					break;
				case "comission":
					Commission = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "price":
					Price = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "quantity":
					Quantity = int.Parse(array[i + 1]);
					break;
				}
			}
		}
	}
}
