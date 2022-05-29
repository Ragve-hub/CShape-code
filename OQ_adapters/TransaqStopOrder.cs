namespace OpenQuant.Finam
{
	public sealed class TransaqStopOrder
	{
		public long TransactionId { get; private set; }

		public long ActiveOrderNo { get; private set; }

		public string Status { get; private set; }

		public string WithdrawTime { get; private set; }

		public string Result { get; private set; }

		public TransaqStoploss StopLoss { get; private set; }

		public TransaqTakeprofit TakeProfit { get; private set; }

		public TransaqStopOrder(string data)
		{
			TransactionId = (ActiveOrderNo = -1L);
			Status = (WithdrawTime = (Result = string.Empty));
			StopLoss = new TransaqStoploss(string.Empty);
			TakeProfit = new TransaqTakeprofit(string.Empty);
			string[] array = data.Split('|');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(';');
				if (array2[0] == "stoporder")
				{
					for (int j = 1; j < array2.Length; j += 2)
					{
						switch (array2[j])
						{
						case "transactionid":
							TransactionId = long.Parse(array2[j + 1]);
							break;
						case "activeorderno":
							ActiveOrderNo = long.Parse(array2[j + 1]);
							break;
						case "status":
							Status = array2[j + 1];
							break;
						case "withdrawtime":
							WithdrawTime = array2[j + 1];
							break;
						case "result":
							Result = array2[j + 1];
							break;
						}
					}
				}
				if (array2[0] == "stoploss")
				{
					StopLoss = new TransaqStoploss(array[i]);
				}
				if (array2[0] == "takeprofit")
				{
					TakeProfit = new TransaqTakeprofit(array[i]);
				}
			}
		}

		public void Update(TransaqStopOrder tso)
		{
			if (tso.TransactionId != -1)
			{
				TransactionId = tso.TransactionId;
			}
			if (tso.ActiveOrderNo != -1)
			{
				ActiveOrderNo = tso.ActiveOrderNo;
			}
			if (tso.Status != string.Empty)
			{
				Status = tso.Status;
			}
			if (tso.WithdrawTime != string.Empty)
			{
				WithdrawTime = tso.WithdrawTime;
			}
			if (tso.Result != string.Empty)
			{
				Result = tso.Result;
			}
			StopLoss.Update(tso.StopLoss);
			TakeProfit.Update(tso.TakeProfit);
		}
	}
}
