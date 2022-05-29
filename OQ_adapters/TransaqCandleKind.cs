namespace OpenQuant.Finam
{
	public sealed class TransaqCandleKind
	{
		public int Id { get; private set; }

		public long Period { get; private set; }

		public string Name { get; private set; }

		public TransaqCandleKind(string data)
		{
			Name = string.Empty;
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i += 2)
			{
				switch (array[i])
				{
				case "id":
					Id = int.Parse(array[i + 1]);
					break;
				case "period":
					Period = long.Parse(array[i + 1]);
					break;
				case "name":
					Name = array[i + 1];
					break;
				}
			}
		}
	}
}
