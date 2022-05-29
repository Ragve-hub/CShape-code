using System;
using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqStoploss
	{
		public string UseCredit { get; private set; }

		public double ActivationPrice { get; private set; }

		public DateTime GuardTime { get; private set; }

		public string BrokerRef { get; private set; }

		public int Quantity { get; private set; }

		public string ByMarket { get; private set; }

		public double OrderPrice { get; private set; }

		public TransaqStoploss(string data)
		{
			UseCredit = (BrokerRef = (ByMarket = string.Empty));
			ActivationPrice = (OrderPrice = -1.0);
			GuardTime = DateTime.MinValue;
			Quantity = -1;
			string[] array = data.Split(';');
			for (int i = 1; i < array.Length; i += 2)
			{
				switch (array[i])
				{
				case "usecredit":
					UseCredit = array[i + 1];
					break;
				case "activationprice":
					ActivationPrice = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "guardtime":
					GuardTime = DateTime.ParseExact(array[i + 1], "d.M.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
					break;
				case "brokerref":
					BrokerRef = array[i + 1];
					break;
				case "quantity":
					Quantity = int.Parse(array[i + 1]);
					break;
				case "bymarket":
					ByMarket = array[i + 1];
					break;
				case "orderprice":
					OrderPrice = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				}
			}
		}

		public void Update(TransaqStoploss ts)
		{
			if (ts.UseCredit != string.Empty)
			{
				UseCredit = ts.UseCredit;
			}
			if (ts.ActivationPrice != -1.0)
			{
				ActivationPrice = ts.ActivationPrice;
			}
			if (ts.GuardTime != DateTime.MinValue)
			{
				GuardTime = ts.GuardTime;
			}
			if (ts.BrokerRef != string.Empty)
			{
				BrokerRef = ts.BrokerRef;
			}
			if (ts.Quantity != -1)
			{
				Quantity = ts.Quantity;
			}
			if (ts.ByMarket != string.Empty)
			{
				ByMarket = ts.ByMarket;
			}
			if (ts.OrderPrice != -1.0)
			{
				OrderPrice = ts.OrderPrice;
			}
		}
	}
}
