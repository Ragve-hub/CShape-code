using System.Globalization;

namespace OpenQuant.Finam
{
	public sealed class TransaqOrder
	{
		public long TransactionId { get; private set; }

		public long OrderNo { get; private set; }

		public string Status { get; private set; }

		public double Balance { get; private set; }

		public double Price { get; private set; }

		public int Quantity { get; private set; }

		public string WithdrawTime { get; private set; }

		public string Result { get; private set; }

		public TransaqOrder(string data)
		{
			TransactionId = (OrderNo = (Quantity = -1));
			Status = (WithdrawTime = (Result = string.Empty));
			Balance = (Price = -1.0);
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "transactionid":
					TransactionId = long.Parse(array[i + 1]);
					break;
				case "orderno":
					OrderNo = long.Parse(array[i + 1]);
					break;
				case "status":
					Status = array[i + 1];
					break;
				case "balance":
					Balance = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "price":
					Price = double.Parse(array[i + 1], CultureInfo.InvariantCulture);
					break;
				case "quantity":
					Quantity = int.Parse(array[i + 1]);
					break;
				case "withdrawtime":
					WithdrawTime = array[i + 1];
					break;
				case "result":
					Result = array[i + 1];
					break;
				}
			}
		}

		public void Update(TransaqOrder to)
		{
			if (to.TransactionId != -1)
			{
				TransactionId = to.TransactionId;
			}
			if (to.OrderNo != -1)
			{
				OrderNo = to.OrderNo;
			}
			if (to.Status != string.Empty)
			{
				Status = to.Status;
			}
			if (to.Balance != -1.0)
			{
				Balance = to.Balance;
			}
			if (to.Price != -1.0)
			{
				Price = to.Price;
			}
			if (to.Quantity != -1)
			{
				Quantity = to.Quantity;
			}
			if (to.WithdrawTime != string.Empty)
			{
				WithdrawTime = to.WithdrawTime;
			}
			if (to.Result != string.Empty)
			{
				Result = to.Result;
			}
		}
	}
}
