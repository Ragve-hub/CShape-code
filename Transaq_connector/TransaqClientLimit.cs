namespace OpenQuant.Finam
{
	public sealed class TransaqClientLimit
	{
		public string Client { get; private set; }

		public string CbplLimit { get; private set; }

		public string CbplUsed { get; private set; }

		public string CbplPlanned { get; private set; }

		public string Fob_VarMargin { get; private set; }

		public string Coverage { get; private set; }

		public string Liquidity_C { get; private set; }

		public string Profit { get; private set; }

		public string Money_Current { get; private set; }

		public string Money_Blocked { get; private set; }

		public string Money_Free { get; private set; }

		public string Options_Premium { get; private set; }

		public string Exchange_Fee { get; private set; }

		public string Forts_VarMargin { get; private set; }

		public string VarMargin { get; private set; }

		public string PclMargin { get; private set; }

		public string Options_VM { get; private set; }

		public string Spot_Buy_Limit { get; private set; }

		public string Used_Spot_Buy_Limit { get; private set; }

		public string Collat_Current { get; private set; }

		public string Collat_Blocked { get; private set; }

		public string Collat_Free { get; private set; }

		public TransaqClientLimit(string data)
		{
			Client = (CbplLimit = (CbplUsed = (CbplPlanned = (Fob_VarMargin = (Coverage = (Liquidity_C = (Profit = string.Empty)))))));
			Money_Current = (Money_Blocked = (Money_Free = (Options_Premium = (Exchange_Fee = (Forts_VarMargin = string.Empty)))));
			VarMargin = (PclMargin = (Options_VM = (Spot_Buy_Limit = (Used_Spot_Buy_Limit = string.Empty))));
			Collat_Current = (Collat_Blocked = (Collat_Free = string.Empty));
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case "client":
					Client = array[i + 1];
					break;
				case "cbplimit":
					CbplLimit = array[i + 1];
					break;
				case "cbplused":
					CbplUsed = array[i + 1];
					break;
				case "cbplplanned":
					CbplPlanned = array[i + 1];
					break;
				case "fob_varmargin":
					Fob_VarMargin = array[i + 1];
					break;
				case "coverage":
					Coverage = array[i + 1];
					break;
				case "liquidity_c":
					Liquidity_C = array[i + 1];
					break;
				case "profit":
					Profit = array[i + 1];
					break;
				case "money_current":
					Money_Current = array[i + 1];
					break;
				case "money_blocked":
					Money_Blocked = array[i + 1];
					break;
				case "money_free":
					Money_Free = array[i + 1];
					break;
				case "options_premium":
					Options_Premium = array[i + 1];
					break;
				case "exchange_fee":
					Exchange_Fee = array[i + 1];
					break;
				case "forts_varmargin":
					Forts_VarMargin = array[i + 1];
					break;
				case "varmargin":
					VarMargin = array[i + 1];
					break;
				case "pclmargin":
					PclMargin = array[i + 1];
					break;
				case "options_vm":
					Options_VM = array[i + 1];
					break;
				case "spot_buy_limit":
					Spot_Buy_Limit = array[i + 1];
					break;
				case "used_stop_buy_limit":
					Used_Spot_Buy_Limit = array[i + 1];
					break;
				case "collat_current":
					Collat_Current = array[i + 1];
					break;
				case "collat_blocked":
					Collat_Blocked = array[i + 1];
					break;
				case "collat_free":
					Collat_Free = array[i + 1];
					break;
				}
			}
		}
	}
}
