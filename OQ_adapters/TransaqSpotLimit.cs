namespace OpenQuant.Finam
{
	public sealed class TransaqSpotLimit
	{
		public string Client { get; private set; }

		public string BuyLimit { get; private set; }

		public string BuyLimitUsed { get; private set; }

		public TransaqSpotLimit(string data)
		{
			Client = (BuyLimit = (BuyLimitUsed = string.Empty));
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "client":
					Client = array[i + 1];
					break;
				case "buylimit":
					BuyLimit = array[i + 1];
					break;
				case "buylimitused":
					BuyLimitUsed = array[i + 1];
					break;
				}
			}
		}

		public void Update(TransaqSpotLimit tsl)
		{
			if (tsl.Client != string.Empty)
			{
				Client = tsl.Client;
			}
			if (tsl.BuyLimit != string.Empty)
			{
				BuyLimit = tsl.BuyLimit;
			}
			if (tsl.BuyLimitUsed != string.Empty)
			{
				BuyLimitUsed = tsl.BuyLimitUsed;
			}
		}
	}
}
