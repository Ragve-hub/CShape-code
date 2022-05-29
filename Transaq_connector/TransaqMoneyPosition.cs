namespace OpenQuant.Finam
{
	public sealed class TransaqMoneyPosition
	{
		public string Register { get; private set; }

		public string Asset { get; private set; }

		public string Client { get; private set; }

		public string ShortName { get; private set; }

		public string SaldoIn { get; private set; }

		public string Bought { get; private set; }

		public string Sold { get; private set; }

		public string Saldo { get; private set; }

		public string OrdBuy { get; private set; }

		public string OrdBuyCond { get; private set; }

		public string Comission { get; private set; }

		public TransaqMoneyPosition(string data)
		{
			Register = "default";
			Asset = (Client = (ShortName = (SaldoIn = (Bought = string.Empty))));
			Sold = (Saldo = (OrdBuy = (OrdBuyCond = (Comission = string.Empty))));
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "register":
					Register = array[i + 1];
					break;
				case "asset":
					Asset = array[i + 1];
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
				case "bought":
					Bought = array[i + 1];
					break;
				case "sold":
					Sold = array[i + 1];
					break;
				case "saldo":
					Saldo = array[i + 1];
					break;
				case "ordbuy":
					OrdBuy = array[i + 1];
					break;
				case "ordbuycond":
					OrdBuyCond = array[i + 1];
					break;
				case "comission":
					Comission = array[i + 1];
					break;
				}
			}
		}

		public void Update(TransaqMoneyPosition tmpNew)
		{
			if (tmpNew.Asset != string.Empty)
			{
				Asset = tmpNew.Asset;
			}
			if (tmpNew.Client != string.Empty)
			{
				Client = tmpNew.Client;
			}
			if (tmpNew.ShortName != string.Empty)
			{
				ShortName = tmpNew.ShortName;
			}
			if (tmpNew.SaldoIn != string.Empty)
			{
				SaldoIn = tmpNew.SaldoIn;
			}
			if (tmpNew.Bought != string.Empty)
			{
				Bought = tmpNew.Bought;
			}
			if (tmpNew.Sold != string.Empty)
			{
				Sold = tmpNew.Sold;
			}
			if (tmpNew.Saldo != string.Empty)
			{
				Saldo = tmpNew.Saldo;
			}
			if (tmpNew.OrdBuy != string.Empty)
			{
				OrdBuy = tmpNew.OrdBuy;
			}
			if (tmpNew.OrdBuyCond != string.Empty)
			{
				OrdBuyCond = tmpNew.OrdBuyCond;
			}
			if (tmpNew.Comission != string.Empty)
			{
				Comission = tmpNew.Comission;
			}
		}
	}
}
