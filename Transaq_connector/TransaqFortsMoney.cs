namespace OpenQuant.Finam
{
	public sealed class TransaqFortsMoney
	{
		public string Client { get; private set; }

		public string Current { get; private set; }

		public string Blocked { get; private set; }

		public string Free { get; private set; }

		public string VarMargin { get; private set; }

		public TransaqFortsMoney(string data)
		{
			Client = (Current = (Blocked = (Free = (VarMargin = string.Empty))));
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
				case "varmargin":
					VarMargin = array[i + 1];
					break;
				}
			}
		}

		public void Update(TransaqFortsMoney tfm)
		{
			if (tfm.Client != string.Empty)
			{
				Client = tfm.Client;
			}
			if (tfm.Current != string.Empty)
			{
				Current = tfm.Current;
			}
			if (tfm.Blocked != string.Empty)
			{
				Blocked = tfm.Blocked;
			}
			if (tfm.Free != string.Empty)
			{
				Free = tfm.Free;
			}
			if (tfm.VarMargin != string.Empty)
			{
				VarMargin = tfm.VarMargin;
			}
		}
	}
}
