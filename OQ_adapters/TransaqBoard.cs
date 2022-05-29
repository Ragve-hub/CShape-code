namespace OpenQuant.Finam
{
	public sealed class TransaqBoard
	{
		public string Id { get; private set; }

		public string Name { get; private set; }

		public int Market { get; private set; }

		public int Type { get; private set; }

		public TransaqBoard(string data)
		{
			Id = (Name = string.Empty);
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "id":
					Id = array[i + 1];
					break;
				case "name":
					Name = array[i + 1];
					break;
				case "market":
					Market = int.Parse(array[i + 1]);
					break;
				case "type":
					Type = int.Parse(array[i + 1]);
					break;
				}
			}
		}
	}
}
