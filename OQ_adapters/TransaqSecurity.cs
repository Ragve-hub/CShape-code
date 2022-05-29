using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqSecurity
	{
		public int SecId { get; private set; }

		public bool Active { get; private set; }

		public string SecCode { get; private set; }

		public string Board { get; private set; }

		public int Market { get; private set; }

		public string ShortName { get; private set; }

		public int Decimals { get; private set; }

		public double MinStep { get; private set; }

		public int LotSize { get; private set; }

		public double PointCost { get; private set; }

		public bool UseCredit { get; private set; }

		public bool ByMarket { get; private set; }

		public bool NoSplit { get; private set; }

		public bool ImmOrCancel { get; private set; }

		public bool CancelBalance { get; private set; }

		public string SecType { get; private set; }

		public string SecCodeBoard { get; private set; }

		public string SecCodeMarket { get; private set; }

		public TransaqSecurity(string data)
		{
			SecCode = (Board = (ShortName = (SecType = string.Empty)));
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length - 1; i++)
			{
				switch (array[i])
				{
				case "secid":
					SecId = int.Parse(array[i + 1]);
					break;
				case "active":
					Active = bool.Parse(array[i + 1]);
					break;
				case "seccode":
					SecCode = array[i + 1];
					break;
				case "board":
					Board = array[i + 1];
					break;
				case "market":
					Market = int.Parse(array[i + 1]);
					break;
				case "shortname":
					ShortName = array[i + 1];
					break;
				case "decimals":
					Decimals = int.Parse(array[i + 1]);
					break;
				case "minstep":
					MinStep = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "lotsize":
					LotSize = int.Parse(array[i + 1]);
					break;
				case "point_cost":
					PointCost = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "usecredit":
					UseCredit = ((array[i + 1] == "yes") ? true : false);
					break;
				case "bymarket":
					ByMarket = ((array[i + 1] == "yes") ? true : false);
					break;
				case "nosplit":
					NoSplit = ((array[i + 1] == "yes") ? true : false);
					break;
				case "immorcancel":
					ImmOrCancel = ((array[i + 1] == "yes") ? true : false);
					break;
				case "cancelbalance":
					CancelBalance = ((array[i + 1] == "yes") ? true : false);
					break;
				case "sectype":
					SecType = array[i + 1];
					break;
				}
			}
			SecCodeBoard = SecCode + "|" + Board;
			SecCodeMarket = SecCode + Market;
		}
	}
}
