using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqFortsPosition
	{
		public int SecId { get; private set; }

		public string SecCode { get; private set; }

		public string Client { get; private set; }

		public string StartNet { get; private set; }

		public string OpenBuys { get; private set; }

		public string OpenSells { get; private set; }

		public double TotalNet { get; private set; }

		public string TodayBuy { get; private set; }

		public string TodaySell { get; private set; }

		public string OptMargin { get; private set; }

		public string VarMargin { get; private set; }

		public string ExpirationPos { get; private set; }

		public string UsedSellSpotLimit { get; private set; }

		public string SellSpotLimit { get; private set; }

		public string Netto { get; private set; }

		public string Kgo { get; private set; }

		public TransaqFortsPosition(string data)
		{
			SecId = -1;
			TotalNet = double.MaxValue;
			SecCode = (Client = (StartNet = (OpenBuys = (OpenSells = (TodayBuy = (TodaySell = string.Empty))))));
			OptMargin = (VarMargin = (ExpirationPos = (UsedSellSpotLimit = (SellSpotLimit = (Netto = (Kgo = string.Empty))))));
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "secid":
					SecId = int.Parse(array[i + 1]);
					break;
				case "seccode":
					SecCode = array[i + 1];
					break;
				case "client":
					Client = array[i + 1];
					break;
				case "startnet":
					StartNet = array[i + 1];
					break;
				case "openbuys":
					OpenBuys = array[i + 1];
					break;
				case "opensells":
					OpenSells = array[i + 1];
					break;
				case "totalnet":
					TotalNet = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "todaybuy":
					TodayBuy = array[i + 1];
					break;
				case "todaysell":
					TodaySell = array[i + 1];
					break;
				case "optmargin":
					OptMargin = array[i + 1];
					break;
				case "varmargin":
					VarMargin = array[i + 1];
					break;
				case "expirationpos":
					ExpirationPos = array[i + 1];
					break;
				case "usedsellspotlimit":
					UsedSellSpotLimit = array[i + 1];
					break;
				case "sellspotlimit":
					SellSpotLimit = array[i + 1];
					break;
				case "netto":
					Netto = array[i + 1];
					break;
				case "kgo":
					Kgo = array[i + 1];
					break;
				}
			}
		}

		public void Update(TransaqFortsPosition tfp)
		{
			if (tfp.SecId != -1)
			{
				SecId = tfp.SecId;
			}
			if (tfp.Client != string.Empty)
			{
				Client = tfp.Client;
			}
			if (tfp.StartNet != string.Empty)
			{
				StartNet = tfp.StartNet;
			}
			if (tfp.OpenBuys != string.Empty)
			{
				OpenBuys = tfp.OpenBuys;
			}
			if (tfp.OpenSells != string.Empty)
			{
				OpenSells = tfp.OpenSells;
			}
			if (tfp.TotalNet != double.MaxValue)
			{
				TotalNet = tfp.TotalNet;
			}
			if (tfp.TodayBuy != string.Empty)
			{
				TodayBuy = tfp.TodayBuy;
			}
			if (tfp.TodaySell != string.Empty)
			{
				TodaySell = tfp.TodaySell;
			}
			if (tfp.OptMargin != string.Empty)
			{
				OptMargin = tfp.OptMargin;
			}
			if (tfp.VarMargin != string.Empty)
			{
				VarMargin = tfp.VarMargin;
			}
			if (tfp.ExpirationPos != string.Empty)
			{
				ExpirationPos = tfp.ExpirationPos;
			}
			if (tfp.UsedSellSpotLimit != string.Empty)
			{
				UsedSellSpotLimit = tfp.UsedSellSpotLimit;
			}
			if (tfp.SellSpotLimit != string.Empty)
			{
				SellSpotLimit = tfp.SellSpotLimit;
			}
			if (tfp.Netto != string.Empty)
			{
				Netto = tfp.Netto;
			}
			if (tfp.Kgo != string.Empty)
			{
				Kgo = tfp.Kgo;
			}
		}
	}
}
