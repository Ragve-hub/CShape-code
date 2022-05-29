using System;
using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqCandle
	{
		public DateTime Date { get; private set; }

		public double Open { get; private set; }

		public double High { get; private set; }

		public double Low { get; private set; }

		public double Close { get; private set; }

		public int Volume { get; private set; }

		public int OI { get; private set; }

		public TransaqCandle(string data)
		{
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length - 1; i += 2)
			{
				switch (array[i])
				{
				case "date":
					Date = DateTime.ParseExact(array[i + 1], "d.M.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
					break;
				case "open":
					Open = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "high":
					High = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "low":
					Low = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "close":
					Close = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "volume":
					Volume = int.Parse(array[i + 1]);
					break;
				case "oi":
					OI = int.Parse(array[i + 1]);
					break;
				}
			}
		}
	}
}
