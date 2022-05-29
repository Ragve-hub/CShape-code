using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqSecPosition
	{
		public int SecId { get; private set; }

		public int Market { get; private set; }

		public string SecCode { get; private set; }

		public string Register { get; private set; }

		public string Client { get; private set; }

		public string ShortName { get; private set; }

		public string SaldoIn { get; private set; }

		public string SaldoMin { get; private set; }

		public int Bought { get; private set; }

		public int Sold { get; private set; }

		public double Saldo { get; private set; }

		public string OrderBuy { get; private set; }

		public string OrderSell { get; private set; }

		public TransaqSecPosition(string data)
		{
			SecId = (Market = (Bought = (Sold = -1)));
			Register = "default";
			SecCode = (Client = (ShortName = (SaldoIn = (SaldoMin = (OrderBuy = (OrderSell = string.Empty))))));
			Saldo = double.MaxValue;
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "secid":
					SecId = int.Parse(array[i + 1]);
					break;
				case "market":
					Market = int.Parse(array[i + 1]);
					break;
				case "seccode":
					SecCode = array[i + 1];
					break;
				case "register":
					Register = array[i + 1];
					break;
				case "client":
					Client = array[i + 1];
					break;
				case "shortname":
					ShortName = array[i + 1];
					break;
				case "saldoin":
					SaldoIn = array[i + 1];
					break;
				case "saldomin":
					SaldoMin = array[i + 1];
					break;
				case "bought":
					Bought = int.Parse(array[i + 1]);
					break;
				case "sold":
					Sold = int.Parse(array[i + 1]);
					break;
				case "saldo":
					Saldo = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "orderbuy":
					OrderBuy = array[i + 1];
					break;
				case "ordersell":
					OrderSell = array[i + 1];
					break;
				}
			}
		}

		public void Update(TransaqSecPosition tsp)
		{
			if (tsp.SecId != -1)
			{
				SecId = tsp.SecId;
			}
			if (tsp.Client != string.Empty)
			{
				Client = tsp.Client;
			}
			if (tsp.ShortName != string.Empty)
			{
				ShortName = tsp.ShortName;
			}
			if (tsp.SaldoIn != string.Empty)
			{
				SaldoIn = tsp.SaldoIn;
			}
			if (tsp.SaldoMin != string.Empty)
			{
				SaldoMin = tsp.SaldoMin;
			}
			if (tsp.Bought != -1)
			{
				Bought = tsp.Bought;
			}
			if (tsp.Sold != -1)
			{
				Sold = tsp.Sold;
			}
			if (tsp.Saldo != double.MaxValue)
			{
				Saldo = tsp.Saldo;
			}
			if (tsp.OrderBuy != string.Empty)
			{
				OrderBuy = tsp.OrderBuy;
			}
			if (tsp.OrderSell != string.Empty)
			{
				OrderSell = tsp.OrderSell;
			}
		}
	}
}
