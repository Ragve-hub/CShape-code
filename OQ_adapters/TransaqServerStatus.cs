namespace OpenQuant.Finam
{
	public sealed class TransaqServerStatus
	{
		public int Id { get; private set; }

		public string Connected { get; private set; }

		public string Recover { get; private set; }

		public string ErrorMessage { get; private set; }

		public TransaqServerStatus(string data)
		{
			Connected = (Recover = (ErrorMessage = string.Empty));
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "id":
					Id = int.Parse(array[i + 1]);
					break;
				case "connected":
					Connected = array[i + 1];
					if (Connected == "error")
					{
						ErrorMessage = array[i + 2];
					}
					break;
				case "recover":
					Recover = array[i + 1];
					break;
				}
			}
		}
	}
}
