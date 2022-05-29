namespace OpenQuant.Finam
{
	public sealed class TransaqFortsCollaterals
	{
		public string Client { get; private set; }

		public string Current { get; private set; }

		public string Blocked { get; private set; }

		public string Free { get; private set; }

		public TransaqFortsCollaterals(string data)
		{
			Client = (Current = (Blocked = (Free = string.Empty)));
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "client":
					Client = array[i + 1];
					break;
				case "current":
					Current = array[i + 1];
					break;
				case "blocked":
					Blocked = array[i + 1];
					break;
				case "free":
					Free = array[i + 1];
					break;
				}
			}
		}

		public void Update(TransaqFortsCollaterals tfc)
		{
			if (tfc.Client != string.Empty)
			{
				Client = tfc.Client;
			}
			if (tfc.Current != string.Empty)
			{
				Current = tfc.Current;
			}
			if (tfc.Blocked != string.Empty)
			{
				Blocked = tfc.Blocked;
			}
			if (tfc.Free != string.Empty)
			{
				Free = tfc.Free;
			}
		}
	}
}
