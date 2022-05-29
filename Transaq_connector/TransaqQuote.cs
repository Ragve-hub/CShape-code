using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqQuote
	{
		public int SecId { get; private set; }

		public string SecCode { get; private set; }

		public string Board { get; private set; }

		public double Bid { get; private set; }

		public double Offer { get; private set; }

		public double BidSize { get; private set; }

		public double OfferSize { get; private set; }

		public string SecCodeBoard { get; private set; }

		public TransaqQuote(string data)
		{
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i += 2)
			{
				switch (array[i])
				{
				case "secid":
					SecId = int.Parse(array[i + 1]);
					break;
				case "seccode":
					SecCode = array[i + 1];
					break;
				case "board":
					Board = array[i + 1];
					break;
				case "bid":
					Bid = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "offer":
					Offer = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "biddepth":
					BidSize = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "offerdepth":
					OfferSize = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				}
			}
			SecCodeBoard = SecCode + "|" + Board;
		}
	}
}
