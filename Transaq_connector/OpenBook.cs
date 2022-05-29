using System.Collections.Generic;
using System.Linq;

namespace OpenQuant.Finam
{
	public class OpenBook
	{
		private List<OpenBookData> openBookBid;

		private List<OpenBookData> openBookAsk;

		public OpenBook()
		{
			openBookBid = new List<OpenBookData>();
			openBookAsk = new List<OpenBookData>();
		}

		public OpenBookReturn UpdateOpenBook(TransaqOpenBook tob)
		{
			if (tob.Sell == -1)
			{
				return Delete(ref tob, ref openBookAsk);
			}
			if (tob.Buy == -1)
			{
				return Delete(ref tob, ref openBookBid);
			}
			if (tob.Sell > 0)
			{
				return UpdateOrInsert(ref tob, ref openBookAsk, "ASK");
			}
			if (tob.Buy > 0)
			{
				return UpdateOrInsert(ref tob, ref openBookBid, "DESC");
			}
			return new OpenBookReturn(tob, -1, 0);
		}

		private OpenBookReturn Delete(ref TransaqOpenBook tob, ref List<OpenBookData> openBookData)
		{
			bool flag = true;
			if (openBookData.Count == 0)
			{
				return new OpenBookReturn(tob, -1, 0);
			}
			if (openBookData.Count > 0 && flag)
			{
				for (int i = 0; i < openBookData.Count; i++)
				{
					if (openBookData[i].Price == tob.Price)
					{
						openBookData.RemoveAt(i);
						return new OpenBookReturn(tob, i, 1);
					}
				}
				flag = false;
			}
			if (openBookData.Count > 0 && !flag)
			{
				return new OpenBookReturn(tob, -1, 0);
			}
			return new OpenBookReturn(tob, -1, 0);
		}

		private OpenBookReturn UpdateOrInsert(ref TransaqOpenBook tob, ref List<OpenBookData> openBookData, string sortType)
		{
			for (int i = 0; i < openBookData.Count; i++)
			{
				if (openBookData[i].Price == tob.Price)
				{
					openBookData.RemoveAt(i);
					openBookData.Insert(i, new OpenBookData(tob.Price, (tob.Buy == 0) ? tob.Sell : tob.Buy));
					return new OpenBookReturn(tob, i, 3);
				}
			}
			if (openBookData.Count == 0)
			{
				openBookData.Insert(0, new OpenBookData(tob.Price, (tob.Buy == 0) ? tob.Sell : tob.Buy));
				return new OpenBookReturn(tob, 0, 2);
			}
			if (openBookData.Count > 0 && sortType == "ASK")
			{
				if (tob.Price < openBookData.First().Price)
				{
					openBookData.Insert(0, new OpenBookData(tob.Price, (tob.Buy == 0) ? tob.Sell : tob.Buy));
					return new OpenBookReturn(tob, 0, 2);
				}
				if (tob.Price > openBookData.Last().Price)
				{
					openBookData.Add(new OpenBookData(tob.Price, (tob.Buy == 0) ? tob.Sell : tob.Buy));
					return new OpenBookReturn(tob, openBookData.Count - 1, 2);
				}
				for (int j = 0; j < openBookData.Count - 1; j++)
				{
					if (openBookData[j].Price < tob.Price && tob.Price < openBookData[j + 1].Price)
					{
						openBookData.Insert(j + 1, new OpenBookData(tob.Price, (tob.Buy == 0) ? tob.Sell : tob.Buy));
						return new OpenBookReturn(tob, j + 1, 2);
					}
				}
			}
			if (openBookData.Count > 0 && sortType == "DESC")
			{
				if (tob.Price > openBookData.First().Price)
				{
					openBookData.Insert(0, new OpenBookData(tob.Price, (tob.Buy == 0) ? tob.Sell : tob.Buy));
					return new OpenBookReturn(tob, 0, 2);
				}
				if (tob.Price < openBookData.Last().Price)
				{
					openBookData.Add(new OpenBookData(tob.Price, (tob.Buy == 0) ? tob.Sell : tob.Buy));
					return new OpenBookReturn(tob, openBookData.Count - 1, 2);
				}
				for (int k = 0; k < openBookData.Count - 1; k++)
				{
					if (openBookData[k].Price > tob.Price && tob.Price > openBookData[k + 1].Price)
					{
						openBookData.Insert(k + 1, new OpenBookData(tob.Price, (tob.Buy == 0) ? tob.Sell : tob.Buy));
						return new OpenBookReturn(tob, k + 1, 2);
					}
				}
			}
			return new OpenBookReturn(tob, -1, 0);
		}
	}
}
