using OpenQuant.API;

namespace OpenQuant.Finam
{
	public class OpenBookReturn
	{
		public BidAsk Side { get; set; }

		public OrderBookAction Action { get; set; }

		public double Price { get; set; }

		public int Size { get; set; }

		public int Position { get; set; }

		public OpenBookReturn(TransaqOpenBook tq, int position, int type)
		{
			if (tq.Sell > 0 || tq.Sell == -1)
			{
				Side = BidAsk.Ask;
				Size = tq.Sell;
			}
			if (tq.Buy > 0 || tq.Buy == -1)
			{
				Side = BidAsk.Bid;
				Size = tq.Buy;
			}
			switch (type)
			{
			case 1:
				Action = OrderBookAction.Delete;
				break;
			case 2:
				Action = OrderBookAction.Insert;
				break;
			case 3:
				Action = OrderBookAction.Update;
				break;
			default:
				Action = OrderBookAction.Undefined;
				break;
			}
			Price = tq.Price;
			Position = position;
		}
	}
}
