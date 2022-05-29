namespace OpenQuant.Finam
{
	public sealed class TransaqClient
	{
		public string Id { get; private set; }

		public string Remove { get; private set; }

		public string Type { get; private set; }

		public string Currency { get; private set; }

		public string Ml_intraday { get; private set; }

		public string Ml_overnight { get; private set; }

		public string Ml_restrict { get; private set; }

		public string Ml_call { get; private set; }

		public string Ml_close { get; private set; }

		public TransaqClient(string data)
		{
			Id = (Remove = (Type = (Currency = string.Empty)));
			Ml_intraday = (Ml_overnight = (Ml_restrict = (Ml_call = (Ml_close = string.Empty))));
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i += 2)
			{
				switch (array[i])
				{
				case "id":
					Id = array[i + 1];
					break;
				case "remove":
					Remove = array[i + 1];
					break;
				case "type":
					Type = array[i + 1];
					break;
				case "currency":
					Currency = array[i + 1];
					break;
				case "ml_intraday":
					Ml_intraday = array[i + 1];
					break;
				case "ml_overnight":
					Ml_overnight = array[i + 1];
					break;
				case "ml_restrict":
					Ml_restrict = array[i + 1];
					break;
				case "ml_call":
					Ml_call = array[i + 1];
					break;
				case "ml_close":
					Ml_close = array[i + 1];
					break;
				}
			}
		}
	}
}
