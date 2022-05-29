using System;
using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqTakeprofit
	{
		public double ActivationPrice { get; private set; }

		public DateTime GuardTime { get; private set; }

		public string BrokerRef { get; private set; }

		public int Quantity { get; private set; }

		public string Extremum { get; private set; }

		public string Level { get; private set; }

		public string Correction { get; private set; }

		public string GuardSpread { get; private set; }

		public TransaqTakeprofit(string data)
		{
			ActivationPrice = -1.0;
			GuardTime = DateTime.MinValue;
			Quantity = -1;
			BrokerRef = (Extremum = (Level = (Correction = (GuardSpread = string.Empty))));
			string[] array = data.Split(';');
			for (int i = 1; i < array.Length; i += 2)
			{
				switch (array[i])
				{
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
				case "extremum":
					Extremum = array[i + 1];
					break;
				case "level":
					Level = array[i + 1];
					break;
				case "correction":
					Correction = array[i + 1];
					break;
				case "guardspread":
					GuardSpread = array[i + 1];
					break;
				}
			}
		}

		public void Update(TransaqTakeprofit tt)
		{
			if (tt.ActivationPrice != -1.0)
			{
				ActivationPrice = tt.ActivationPrice;
			}
			if (tt.GuardTime != DateTime.MinValue)
			{
				GuardTime = tt.GuardTime;
			}
			if (tt.BrokerRef != string.Empty)
			{
				BrokerRef = tt.BrokerRef;
			}
			if (tt.Quantity != -1)
			{
				Quantity = tt.Quantity;
			}
			if (tt.Extremum != string.Empty)
			{
				Extremum = tt.Extremum;
			}
			if (tt.Level != string.Empty)
			{
				Level = tt.Level;
			}
			if (tt.Correction != string.Empty)
			{
				Correction = tt.Correction;
			}
			if (tt.GuardSpread != string.Empty)
			{
				GuardSpread = tt.GuardSpread;
			}
		}
	}
}
