enum CalcMethod
	{
		FIFO,
		LIFO
	}
	
	private double GetAvgEntryPrice(CalcMethod method)
	{
		// calculate total buy and sell volume
		double sellVolume = 0;
		double buyVolume = 0;
         
		foreach (Transaction transaction in Position.Transactions)
		{
			if (transaction.Side == TransactionSide.Sell)
				sellVolume += transaction.Qty;
			else
				buyVolume += transaction.Qty;
		}
      
		double x = 0;
		double cumQty = 0;
		double openVolume;
      
		if (Position.Side == PositionSide.Long)
		{   
			openVolume = buyVolume - sellVolume;
         
			if (method == CalcMethod.LIFO)
			{
				for (int i = 0; i < Position.Transactions.Count; i++)
				{
					Transaction transaction = Position.Transactions[i];
               
					if (transaction.Side == TransactionSide.Sell)
						continue;
               
					double qty = Math.Min(transaction.Qty, openVolume);
               
					cumQty += qty;
					x += qty * transaction.Price;
               
					openVolume -= qty;
               
					if (openVolume == 0)
						break;
				}
			}
         
			if (method == CalcMethod.FIFO)
			{
				for (int i = Position.Transactions.Count - 1; i >= 0 ; i--)
				{
					Transaction transaction = Position.Transactions[i];
               
					if (transaction.Side == TransactionSide.Sell)
						continue;
               
					double qty = Math.Min(transaction.Qty, openVolume);
               
					cumQty += qty;
					x += qty * transaction.Price;
               
					openVolume -= qty;
               
					if (openVolume== 0)
						break;
				}
			}
		}      
		else
		{   
			openVolume = sellVolume - buyVolume;
         
			if (method == CalcMethod.LIFO)
			{
				for (int i = 0; i < Position.Transactions.Count; i++)
				{
					Transaction transaction = Position.Transactions[i];
               
					if (transaction.Side == TransactionSide.Buy)
						continue;
               
					double qty = Math.Min(transaction.Qty, openVolume);
               
					cumQty += qty;
					x += qty * transaction.Price;
               
					openVolume -= qty;
               
					if (openVolume == 0)
						break;
				}
			}
         
			if (method == CalcMethod.FIFO)
			{
				for (int i = Position.Transactions.Count - 1; i >= 0 ; i--)
				{
					Transaction transaction = Position.Transactions[i];
               
					if (transaction.Side == TransactionSide.Buy)
						continue;
               
					double qty = Math.Min(transaction.Qty, openVolume);
               
					cumQty += qty;
					x += qty * transaction.Price;
               
					openVolume -= qty;
               
					if (openVolume == 0)
						break;
				}
			}
		}            
      
		return x / cumQty;
		
	}
