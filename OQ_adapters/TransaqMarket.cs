namespace OpenQuant.Finam
{
	public sealed class TransaqMarket
	{
		public int Id { get; private set; }

		public string Name { get; private set; }

		public TransaqMarket(string data)
		{
			Name = string.Empty;
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "market":
					Name = array[i + 1];
					break;
				case "id":
					Id = int.Parse(array[i + 1]);
					break;
				}
			}
		}
	}
}
