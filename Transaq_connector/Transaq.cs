using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Threading;
using System.Xml;
using OpenQuant.API;
using OpenQuant.API.Plugins;
using OpenQuant.Finam.Design;

namespace OpenQuant.Finam
{
	public class Transaq : UserProvider
	{
		private const string CATEGORY_ACCOUNT = "Account";

		private const string CATEGORY_LOGGING = "Logging";

		private const string CATEGORY_CONNECTION = "Connection";

		private const string CATEGORY_SESSION = "Session";

		private const string CATEGORY_PROXY = "Proxy";

		private const DllSelector DEFAULT_DLLSELECTOR = DllSelector.txmlconnector_dll;

		private const SecuritySelector DEFAULT_SECURITYSELECTOR = SecuritySelector.seccode;

		private const string DEFAULT_HOST = "127.0.0.1";

		private const int DEFAULT_PORT = 0;

		private const string DEFAULT_USERNAME = "";

		private const string DEFAULT_PASSWORD = "";

		private const string DEFAULT_LOGLEVELCONN = "0";

		private const string DEFAULT_LOGLEVELINIT = "2";

		private const int DEFAULT_RQDELAY = 100;

		private const bool DEFAULT_MICEX_REGISTERS = false;

		private const int DEFAULT_SESSION_TIMEOUT = 120;

		private const int DEFAULT_REQUEST_TIMEOUT = 20;

		private const string DEFAULT_PROXYUSING = "";

		private const string DEFAULT_PROXYTYPE = "";

		private const string DEFAULT_PROXYHOST = "";

		private const int DEFAULT_PROXYPORT = 0;

		private const string DEFAULT_PROXYUSERNAME = "";

		private const string DEFAULT_PROXYPASSWORD = "";

		private DllSelector selectedDll;

		private SecuritySelector selectedSecurity;

		private string host;

		private int port;

		private string username;

		public string proxyUsing;

		public string proxyType;

		private string proxyHost;

		private int proxyPort;

		private string proxyUsername;

		private string proxyPassword;

		private string logLevelConn;

		private string logLevelInit;

		private int rqdelay;

		private bool micexRegisters;

		private int sessionTimeout;

		private int requestTimeout;

		private bool isInitialize;

		private ITransaqConnector connector;

		private TransaqPositions positions;

		private Dictionary<int, TransaqMarket> marketById;

		private Dictionary<string, TransaqBoard> boardById;

		private Dictionary<long, TransaqCandleKind> candleKindByPeriod;

		private Dictionary<string, Instrument> instrumentBySymbol;

		private Dictionary<int, TransaqSecurity> securityBySecId;

		private Dictionary<string, TransaqSecurity> securityBySCB;

		private Dictionary<int, DateTime> tickEndTimeBySecId;

		private Dictionary<string, DateTime> tickEndTimeBySCB;

		private Dictionary<int, HistoricalDataRequest> hdrBySecId;

		private Dictionary<string, HistoricalDataRequest> hdrBySCB;

		private Dictionary<int, OpenBook> openBookBySecId;

		private Dictionary<string, OpenBook> openBookBySCB;

		private Dictionary<int, BidOfferPair> bidOfferPairBySecId;

		private Dictionary<string, BidOfferPair> bidOfferPairBySCB;

		private ReaderWriterLockSlim rwClientById;

		private ReaderWriterLockSlim rwClientLimits;

		private Dictionary<string, TransaqClient> clientById;

		private Dictionary<string, TransaqClientLimit> clientLimits;

		private Dictionary<string, BrokerAccount> brokerAccountByClientId;

		private ReaderWriterLockSlim rwOrderByOrdId;

		private ReaderWriterLockSlim rwOrderByOrdNo;

		private ReaderWriterLockSlim rwTransaqOrderByTrId;

		private ReaderWriterLockSlim rwTransaqStopOrderByTrId;

		private Dictionary<string, Order> orderByOrdId;

		private Dictionary<long, Order> orderByOrdNo;

		private Dictionary<long, TransaqOrder> transaqOrderByTrId;

		private Dictionary<long, TransaqStopOrder> transaqStopOrderByTrId;

		private Dictionary<long, object> clientTradeNo;

		private Dictionary<long, double> balanceByOrderNoForCancel;

		[Category("Account")]
		[Editor(typeof(AccountSelectorEditor), typeof(UITypeEditor))]
		public string DefaultAccount { get; set; }

		[Description("Click ellipse(...) to view available instruments")]
		[Category("Session")]
		[Editor(typeof(SessionDataTypeEditor), typeof(UITypeEditor))]
		public object SessionData => null;

		[Category("Logging")]
		[Editor(typeof(FileBrowserTypeEditor), typeof(UITypeEditor))]
		public string LogFilesDir { get; set; }

		[Category("Logging")]
		[DefaultValue("2")]
		[Description("LogLevelInit must be 1, 2 or 3")]
		public string LogLevelInit
		{
			get
			{
				return logLevelInit;
			}
			set
			{
				switch (value)
				{
				case "1":
				case "2":
				case "3":
					logLevelInit = value;
					break;
				}
			}
		}

		[DefaultValue("0")]
		[Category("Logging")]
		[Description("LogLevelConn must be 0, 1 or 2")]
		public string LogLevelConn
		{
			get
			{
				return logLevelConn;
			}
			set
			{
				switch (value)
				{
				case "0":
				case "1":
				case "2":
					logLevelConn = value;
					break;
				}
			}
		}

		[DefaultValue(true)]
		[Category("Logging")]
		[Description("Output error in \"Provider Errors\" if order is unknown")]
		public bool OutputUnknownOrderError { get; set; }

		[Category("Connection")]
		[DefaultValue(DllSelector.txmlconnector_dll)]
		public DllSelector SelectedDll
		{
			get
			{
				return selectedDll;
			}
			set
			{
				selectedDll = value;
			}
		}

		[DefaultValue(SecuritySelector.seccode)]
		[Category("Connection")]
		public SecuritySelector SelectedSecurity
		{
			get
			{
				return selectedSecurity;
			}
			set
			{
				selectedSecurity = value;
			}
		}

		[DefaultValue("127.0.0.1")]
		[Category("Connection")]
		public string Host
		{
			get
			{
				return host;
			}
			set
			{
				host = value;
			}
		}

		[DefaultValue(0)]
		[Category("Connection")]
		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				port = value;
			}
		}

		[DefaultValue("")]
		[Category("Connection")]
		public string Username
		{
			get
			{
				return username;
			}
			set
			{
				username = value;
			}
		}

		[DefaultValue("")]
		[Editor(typeof(PasswordChangingTypeEditor), typeof(UITypeEditor))]
		[PasswordPropertyText(true)]
		[Description("Click (...) to change password")]
		[Category("Connection")]
		public string Password { get; set; }

		[DefaultValue(100)]
		[Category("Connection")]
		[Description("Should be equal or greater than 100")]
		public int RQDelay
		{
			get
			{
				return rqdelay;
			}
			set
			{
				rqdelay = value;
			}
		}

		[Category("Connection")]
		[DefaultValue(false)]
		public bool MicexRegisters
		{
			get
			{
				return micexRegisters;
			}
			set
			{
				micexRegisters = value;
			}
		}

		[Category("Connection")]
		[Description("Must be greater than RequestTimeout")]
		[DefaultValue(120)]
		public int SessionTimeout
		{
			get
			{
				return sessionTimeout;
			}
			set
			{
				if (value > requestTimeout)
				{
					sessionTimeout = value;
				}
			}
		}

		[Category("Connection")]
		[DefaultValue(20)]
		[Description("Must be less than SessionTimeout")]
		public int RequestTimeout
		{
			get
			{
				return requestTimeout;
			}
			set
			{
				if (value < sessionTimeout)
				{
					requestTimeout = value;
				}
			}
		}

		[Description("Use proxy - True or False")]
		[Category("Proxy")]
		[Editor(typeof(ProxyUsingSelectorEditor), typeof(UITypeEditor))]
		public string ProxyUsing { get; set; }

		[Category("Proxy")]
		[Editor(typeof(ProxyTypeSelectorEditor), typeof(UITypeEditor))]
		public string ProxyType { get; set; }

		[Category("Proxy")]
		[DefaultValue("")]
		public string ProxyHost
		{
			get
			{
				return proxyHost;
			}
			set
			{
				proxyHost = value;
			}
		}

		[DefaultValue(0)]
		[Category("Proxy")]
		public int ProxyPort
		{
			get
			{
				return proxyPort;
			}
			set
			{
				proxyPort = value;
			}
		}

		[DefaultValue("")]
		[Category("Proxy")]
		public string ProxyUsername
		{
			get
			{
				return proxyUsername;
			}
			set
			{
				proxyUsername = value;
			}
		}

		[PasswordPropertyText(true)]
		[Category("Proxy")]
		[DefaultValue("")]
		public string ProxyPassword
		{
			get
			{
				return proxyPassword;
			}
			set
			{
				proxyPassword = value;
			}
		}

		protected override BrokerInfo GetBrokerInfo()
		{
			BrokerInfo brokerInfo = new BrokerInfo();
			if (!isConnected)
			{
				return brokerInfo;
			}
			try
			{
				rwClientById.EnterReadLock();
				foreach (string key in clientById.Keys)
				{
					TransaqClient transaqClient = clientById[key];
					string cmd = "<command id=\"get_client_limits\" client=\"" + transaqClient.Id + "\"/>";
					SendCommandLine(cmd);
				}
				foreach (string key2 in clientById.Keys)
				{
					TransaqClient transaqClient2 = clientById[key2];
					BrokerAccount brokerAccount = brokerInfo.AddAccount(transaqClient2.Id);
					brokerAccount.AddField("Client: Client Id", transaqClient2.Id);
					brokerAccount.AddField("Client: Type", transaqClient2.Type);
					brokerAccount.AddField("Client: Currency", transaqClient2.Currency);
					brokerAccount.AddField("Client: Ml_intraday", transaqClient2.Ml_intraday);
					brokerAccount.AddField("Client: Ml_overnight", transaqClient2.Ml_overnight);
					brokerAccount.AddField("Client: Ml_restrict", transaqClient2.Ml_restrict);
					brokerAccount.AddField("Client: Ml_call", transaqClient2.Ml_call);
					brokerAccount.AddField("Client: Ml_close", transaqClient2.Ml_close);
					Dictionary<string, TransaqMoneyPosition> value = null;
					lock (positions.lockerMoneyPosition)
					{
						if (positions.MoneyPosition.TryGetValue(transaqClient2.Id, out value))
						{
							foreach (string key3 in value.Keys)
							{
								TransaqMoneyPosition transaqMoneyPosition = value[key3];
								brokerAccount.AddField($"MoneyPos Register={transaqMoneyPosition.Register}: Asset", transaqMoneyPosition.Asset);
								brokerAccount.AddField($"MoneyPos Register={transaqMoneyPosition.Register}: ShortName", transaqMoneyPosition.ShortName);
								brokerAccount.AddField($"MoneyPos Register={transaqMoneyPosition.Register}: SaldoIn", transaqMoneyPosition.SaldoIn);
								brokerAccount.AddField($"MoneyPos Register={transaqMoneyPosition.Register}: Saldo", transaqMoneyPosition.Saldo);
								brokerAccount.AddField($"MoneyPos Register={transaqMoneyPosition.Register}: Bought", transaqMoneyPosition.Bought);
								brokerAccount.AddField($"MoneyPos Register={transaqMoneyPosition.Register}: Sold", transaqMoneyPosition.Sold);
								brokerAccount.AddField($"MoneyPos Register={transaqMoneyPosition.Register}: OrdBuy", transaqMoneyPosition.OrdBuy);
								brokerAccount.AddField($"MoneyPos Register={transaqMoneyPosition.Register}: OrdBuyCond", transaqMoneyPosition.OrdBuyCond);
								brokerAccount.AddField($"MoneyPos Register={transaqMoneyPosition.Register}: Comission", transaqMoneyPosition.Comission);
							}
						}
					}
					TransaqFortsMoney value2 = null;
					lock (positions.lockerFortsMoney)
					{
						if (positions.FortsMoney.TryGetValue(transaqClient2.Id, out value2))
						{
							brokerAccount.AddField("FortsMoney: Current", value2.Current);
							brokerAccount.AddField("FortsMoney: Blocked", value2.Blocked);
							brokerAccount.AddField("FortsMoney: Free", value2.Free);
							brokerAccount.AddField("FortsMoney: VarMargin", value2.VarMargin);
						}
					}
					TransaqFortsCollaterals value3 = null;
					lock (positions.lockerFortsCollaterals)
					{
						if (positions.FortsCollaterals.TryGetValue(transaqClient2.Id, out value3))
						{
							brokerAccount.AddField("FortsCollaterals: Client", value3.Client);
							brokerAccount.AddField("FortsCollaterals: Current", value3.Current);
							brokerAccount.AddField("FortsCollaterals: Blocked", value3.Blocked);
							brokerAccount.AddField("FortsCollaterals: Free", value3.Free);
						}
					}
					TransaqSpotLimit value4 = null;
					lock (positions.lockerSpotLimit)
					{
						if (positions.SpotLimit.TryGetValue(transaqClient2.Id, out value4))
						{
							brokerAccount.AddField("SpotLimit: Client", value4.Client);
							brokerAccount.AddField("SpotLimit: BuyLimit", value4.BuyLimit);
							brokerAccount.AddField("SpotLimit: BuyLimitUsed", value4.BuyLimitUsed);
						}
					}
					TransaqClientLimit value5 = null;
					try
					{
						rwClientLimits.EnterReadLock();
						if (clientLimits.TryGetValue(transaqClient2.Id, out value5))
						{
							brokerAccount.AddField("ClientLimit: Client", value5.Client);
							brokerAccount.AddField("ClientLimit: CbplLimit", value5.CbplLimit);
							brokerAccount.AddField("ClientLimit: CbplUsed", value5.CbplUsed);
							brokerAccount.AddField("ClientLimit: CbplPlanned", value5.CbplPlanned);
							brokerAccount.AddField("ClientLimit: Fob_VarMargin", value5.Fob_VarMargin);
							brokerAccount.AddField("ClientLimit: Coverage", value5.Coverage);
							brokerAccount.AddField("ClientLimit: Liquidity_C", value5.Liquidity_C);
							brokerAccount.AddField("ClientLimit: Profit", value5.Profit);
							brokerAccount.AddField("ClientLimit: Money_Current", value5.Money_Current);
							brokerAccount.AddField("ClientLimit: Money_Blocked", value5.Money_Blocked);
							brokerAccount.AddField("ClientLimit: Money_Free", value5.Money_Free);
							brokerAccount.AddField("ClientLimit: Options_Premium", value5.Options_Premium);
							brokerAccount.AddField("ClientLimit: Exchange_Fee", value5.Exchange_Fee);
							brokerAccount.AddField("ClientLimit: Forts_VarMargin", value5.Forts_VarMargin);
							brokerAccount.AddField("ClientLimit: PclMargin", value5.PclMargin);
							brokerAccount.AddField("ClientLimit: Options_VM", value5.Options_VM);
							brokerAccount.AddField("ClientLimit: Spot_Buy_Limit", value5.Spot_Buy_Limit);
							brokerAccount.AddField("ClientLimit: Used_Spot_Buy_Limit", value5.Used_Spot_Buy_Limit);
							brokerAccount.AddField("ClientLimit: Collat_Current", value5.Collat_Current);
							brokerAccount.AddField("ClientLimit: Collat_Blocked", value5.Collat_Blocked);
							brokerAccount.AddField("ClientLimit: Collat_Free", value5.Collat_Free);
						}
					}
					finally
					{
						rwClientLimits.ExitReadLock();
					}
					if (!brokerAccountByClientId.ContainsKey(transaqClient2.Id))
					{
						brokerAccountByClientId.Add(transaqClient2.Id, brokerAccount);
					}
					Dictionary<string, TransaqSecPosition> value6 = null;
					lock (positions.lockerSecPosition)
					{
						if (positions.SecPosition.TryGetValue(transaqClient2.Id, out value6))
						{
							foreach (string key4 in value6.Keys)
							{
								TransaqSecPosition transaqSecPosition = value6[key4];
								BrokerPosition brokerPosition = brokerAccount.AddPosition();
								brokerPosition.Symbol = transaqSecPosition.SecCode;
								if (transaqSecPosition.Saldo > 0.0)
								{
									brokerPosition.LongQty = transaqSecPosition.Saldo;
								}
								else
								{
									brokerPosition.ShortQty = 0.0 - transaqSecPosition.Saldo;
								}
								brokerPosition.Qty = transaqSecPosition.Saldo;
								brokerPosition.AddCustomField("SecId", transaqSecPosition.SecId.ToString());
								brokerPosition.AddCustomField("Market", transaqSecPosition.Market.ToString());
								brokerPosition.AddCustomField("SecCode", transaqSecPosition.SecCode);
								brokerPosition.AddCustomField("Register", transaqSecPosition.Register);
								brokerPosition.AddCustomField("Client", transaqSecPosition.Client);
								brokerPosition.AddCustomField("ShortName", transaqSecPosition.ShortName);
								brokerPosition.AddCustomField("SaldoIn", transaqSecPosition.SaldoIn);
								brokerPosition.AddCustomField("SaldoMin", transaqSecPosition.SaldoMin);
								brokerPosition.AddCustomField("Bought", transaqSecPosition.Bought.ToString());
								brokerPosition.AddCustomField("Sold", transaqSecPosition.Sold.ToString());
								brokerPosition.AddCustomField("Saldo", transaqSecPosition.Saldo.ToString());
								brokerPosition.AddCustomField("OrderBuy", transaqSecPosition.OrderBuy);
								brokerPosition.AddCustomField("OrderSell", transaqSecPosition.OrderSell);
							}
						}
					}
					Dictionary<string, TransaqFortsPosition> value7 = null;
					lock (positions.lockerFortsPosition)
					{
						if (!positions.FortsPosition.TryGetValue(transaqClient2.Id, out value7))
						{
							continue;
						}
						foreach (string key5 in value7.Keys)
						{
							TransaqFortsPosition transaqFortsPosition = value7[key5];
							BrokerPosition brokerPosition2 = brokerAccount.AddPosition();
							brokerPosition2.Symbol = transaqFortsPosition.SecCode;
							if (transaqFortsPosition.TotalNet > 0.0)
							{
								brokerPosition2.LongQty = transaqFortsPosition.TotalNet;
							}
							else
							{
								brokerPosition2.ShortQty = 0.0 - transaqFortsPosition.TotalNet;
							}
							brokerPosition2.Qty = transaqFortsPosition.TotalNet;
							brokerPosition2.AddCustomField("SecId", transaqFortsPosition.SecId.ToString());
							brokerPosition2.AddCustomField("SecCode", transaqFortsPosition.SecCode);
							brokerPosition2.AddCustomField("Client", transaqFortsPosition.Client);
							brokerPosition2.AddCustomField("StartNet", transaqFortsPosition.StartNet);
							brokerPosition2.AddCustomField("OpenBuys", transaqFortsPosition.OpenBuys);
							brokerPosition2.AddCustomField("OpenSells", transaqFortsPosition.OpenSells);
							brokerPosition2.AddCustomField("TotalNet", transaqFortsPosition.TotalNet.ToString());
							brokerPosition2.AddCustomField("TodayBuy", transaqFortsPosition.TodayBuy);
							brokerPosition2.AddCustomField("TodaySell", transaqFortsPosition.TodaySell);
							brokerPosition2.AddCustomField("OptMargin", transaqFortsPosition.OptMargin);
							brokerPosition2.AddCustomField("VarMargin", transaqFortsPosition.VarMargin);
							brokerPosition2.AddCustomField("ExpirationPos", transaqFortsPosition.ExpirationPos);
							brokerPosition2.AddCustomField("UsedSellSpotLimit", transaqFortsPosition.UsedSellSpotLimit);
							brokerPosition2.AddCustomField("SellSpotLimit", transaqFortsPosition.SellSpotLimit);
							brokerPosition2.AddCustomField("Netto", transaqFortsPosition.Netto);
							brokerPosition2.AddCustomField("Kgo", transaqFortsPosition.Kgo);
						}
					}
				}
				return brokerInfo;
			}
			finally
			{
				rwClientById.ExitReadLock();
			}
		}

		protected override void RequestHistoricalData(HistoricalDataRequest request)
		{
			if (!isConnected)
			{
				EmitHistoricalDataError(request, "Provider is not connected");
				return;
			}
			if (request.DataType == DataType.Quote)
			{
				EmitHistoricalDataError(request, "Only Trade, Bar and Daily");
				return;
			}
			TransaqSecurity transaqSecurity = null;
			string symbol = request.Instrument.GetSymbol(name);
			if (selectedSecurity == SecuritySelector.seccode)
			{
				foreach (int key in securityBySecId.Keys)
				{
					TransaqSecurity transaqSecurity2 = securityBySecId[key];
					if (transaqSecurity2.SecCode == symbol)
					{
						transaqSecurity = transaqSecurity2;
						break;
					}
				}
			}
			else if (selectedSecurity == SecuritySelector.seccode_board)
			{
				foreach (string key2 in securityBySCB.Keys)
				{
					TransaqSecurity transaqSecurity3 = securityBySCB[key2];
					if (transaqSecurity3.SecCodeBoard == symbol)
					{
						transaqSecurity = transaqSecurity3;
						break;
					}
				}
			}
			if (transaqSecurity == null)
			{
				EmitHistoricalDataError(request, $"Unknown instrument {symbol} for get historical {request.DataType}");
				return;
			}
			long num = -1L;
			long num2 = 86400L;
			long num3 = -1L;
			if (request.DataType == DataType.Daily)
			{
				if (!candleKindByPeriod.ContainsKey(num2))
				{
					EmitHistoricalDataError(request, $"BarSize {num2} second is unavailable");
					return;
				}
				num = candleKindByPeriod[num2].Id;
				num3 = (long)Math.Ceiling((Clock.Now - request.BeginDate).TotalSeconds / (double)num2);
			}
			else if (request.DataType == DataType.Bar)
			{
				if (!candleKindByPeriod.ContainsKey(request.BarSize))
				{
					EmitHistoricalDataError(request, $"BarSize {num} second is unavailable");
					return;
				}
				num = candleKindByPeriod[request.BarSize].Id;
				num3 = (long)Math.Ceiling((Clock.Now - request.BeginDate).TotalSeconds / (double)request.BarSize);
			}
			if (selectedSecurity == SecuritySelector.seccode)
			{
				string empty = string.Empty;
				if (request.DataType == DataType.Trade)
				{
					empty += "<command id=\"subscribe_ticks\">";
					object obj = empty;
					empty = string.Concat(obj, "<security secid=\"", transaqSecurity.SecId, "\" tradeno=\"1\"/>");
					empty += "</command>";
					tickEndTimeBySecId.Add(transaqSecurity.SecId, Clock.Now);
				}
				else
				{
					object obj2 = empty;
					empty = string.Concat(obj2, "<command id=\"gethistorydata\" secid=\"", transaqSecurity.SecId, "\" period=\"", num, "\" count=\"", num3, "\" reset=\"true\"/>");
				}
				if (SendCommandLine(empty).Success)
				{
					hdrBySecId.Add(transaqSecurity.SecId, request);
				}
				else
				{
					EmitHistoricalDataError(request, "Error in request");
				}
			}
			else if (selectedSecurity == SecuritySelector.seccode_board)
			{
				string empty2 = string.Empty;
				if (request.DataType == DataType.Trade)
				{
					empty2 += "<command id=\"subscribe_ticks\">";
					string text = empty2;
					empty2 = text + "<security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security>";
					empty2 += "<tradeno>1</tradeno>";
					empty2 += "</command>";
					tickEndTimeBySCB.Add(transaqSecurity.SecCodeBoard, Clock.Now);
				}
				else
				{
					empty2 += "<command id=\"gethistorydata\">";
					string text2 = empty2;
					empty2 = text2 + "<security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security>";
					object obj3 = empty2;
					empty2 = string.Concat(obj3, "<period>", num, "</period>");
					object obj4 = empty2;
					empty2 = string.Concat(obj4, "<count>", num3, "</count>");
					empty2 += "<reset>true</reset>";
					empty2 += "</command>";
				}
				if (SendCommandLine(empty2).Success)
				{
					hdrBySCB.Add(transaqSecurity.SecCodeBoard, request);
				}
				else
				{
					EmitHistoricalDataError(request, "Error in request");
				}
			}
		}

		private TransaqResult SendCommandLine(string cmd)
		{
			cmd += '\0';
			TransaqResult transaqResult = new TransaqResult();
			try
			{
				string resultString = GetResultString(connector.SendCommand(cmd));
				transaqResult = new TransaqResult(resultString);
			}
			catch (Exception ex)
			{
				EmitError(ex.Message);
			}
			if (!transaqResult.Success)
			{
				if (!string.IsNullOrWhiteSpace(transaqResult.Message))
				{
					EmitError(transaqResult.Message);
				}
				if (!string.IsNullOrWhiteSpace(transaqResult.Error))
				{
					EmitError(transaqResult.Error);
				}
			}
			return transaqResult;
		}

		private string GetResultString(string xmlString)
		{
			string text = string.Empty;
			XmlReader xmlReader = XmlReader.Create(new StringReader(xmlString));
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element)
				{
					if (xmlReader.Name == "result")
					{
						if (xmlReader.MoveToAttribute("success"))
						{
							string text2 = text;
							text = text2 + xmlReader.Name + ";" + xmlReader.Value + ";";
						}
						if (xmlReader.MoveToAttribute("transactionid"))
						{
							string text3 = text;
							text = text3 + xmlReader.Name + ";" + xmlReader.Value + ";";
						}
					}
					else
					{
						text = text + xmlReader.Name + ";";
					}
				}
				else if (xmlReader.NodeType == XmlNodeType.Text)
				{
					text = text + xmlReader.Value + ";";
				}
				else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "result")
				{
					return text;
				}
			}
			return text;
		}

		private void ReceiveMessage(object sender, NewDataEventArgs e)
		{
			XmlReader xmlReader = XmlReader.Create(new StringReader(e.Data));
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element)
				{
					switch (xmlReader.Name)
					{
					case "markets":
						ProcessMarkets2(xmlReader);
						break;
					case "boards":
						ProcessBoards2(xmlReader);
						break;
					case "candlekinds":
						ProcessCandleKinds2(xmlReader);
						break;
					case "securities":
						ProcessSecurities2(xmlReader);
						break;
					case "client":
						ProcessClient2(xmlReader);
						break;
					case "positions":
						ProcessPositions2(xmlReader);
						break;
					case "clientlimits":
						ProcessClientLimits2(xmlReader);
						break;
					case "server_status":
						ProcessServerStatus2(xmlReader);
						break;
					case "quotations":
						ProcessQuotations2(xmlReader);
						break;
					case "alltrades":
						ProcessAllTrades2(xmlReader);
						break;
					case "quotes":
						ProcessQuotes2(xmlReader);
						break;
					case "candles":
						ProcessCandles2(xmlReader);
						break;
					case "ticks":
						ProcessTicks2(xmlReader);
						break;
					case "orders":
						ProcessOrders2(xmlReader);
						break;
					case "trades":
						ProcessClientTrades2(xmlReader);
						break;
					}
				}
			}
		}

		private XmlReader ProcessMarkets2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "market" && reader.MoveToAttribute("id"))
					{
						text = text + "id;" + reader.Value + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + "market;" + reader.Value;
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "market")
					{
						AddMarket(text);
						text = string.Empty;
					}
					else if (reader.Name == "markets")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessBoards2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "board")
					{
						if (reader.MoveToAttribute("id"))
						{
							text = text + "id;" + reader.Value + ";";
						}
					}
					else
					{
						text = text + reader.Name + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "board")
					{
						AddBoard(text);
						text = string.Empty;
					}
					else if (reader.Name == "boards")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessCandleKinds2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "id")
					{
						text += "id;";
					}
					else if (reader.Name == "period")
					{
						text += "period;";
					}
					else if (reader.Name == "name")
					{
						text += "name;";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "kind")
					{
						AddCandleKind(text);
						text = string.Empty;
					}
					else if (reader.Name == "candlekinds")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessSecurities2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "security")
					{
						if (reader.MoveToAttribute("secid"))
						{
							text = text + "secid;" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("active"))
						{
							text = text + "active;" + reader.Value + ";";
						}
					}
					else if (reader.Name == "opmask")
					{
						if (reader.MoveToAttribute("usecredit"))
						{
							text = text + "usecredit;" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("bymarket"))
						{
							text = text + "bymarket;" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("nosplit"))
						{
							text = text + "nosplit;" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("immorcancel"))
						{
							text = text + "immorcancel;" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("cancelbalance"))
						{
							text = text + "cancelbalance;" + reader.Value + ";";
						}
					}
					else
					{
						text = text + reader.Name + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "security")
					{
						AddSecurity(text);
						text = string.Empty;
					}
					else if (reader.Name == "securities")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessClient2(XmlReader reader)
		{
			string text = string.Empty;
			if (reader.Name == "client")
			{
				if (reader.MoveToAttribute("id"))
				{
					text = text + "id;" + reader.Value + ";";
				}
				if (reader.MoveToAttribute("remove"))
				{
					text = text + "remove;" + reader.Value + ";";
				}
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					text = text + reader.Name + ";";
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "client")
				{
					UpdateClient(text);
					text = string.Empty;
					return reader;
				}
			}
			return reader;
		}

		private XmlReader ProcessPositions2(XmlReader reader)
		{
			bool flag = false;
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "markets")
					{
						flag = true;
					}
					else if (reader.Name == "market")
					{
						flag = false;
					}
					if (!flag)
					{
						text = text + reader.Name + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					if (!flag)
					{
						text = text + reader.Value + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "markets")
					{
						flag = false;
					}
					else if (reader.Name == "money_position")
					{
						positions.AddMoneyPositionInfo(text);
						text = string.Empty;
					}
					else if (reader.Name == "sec_position")
					{
						positions.AddSecPositionInfo(text);
						text = string.Empty;
					}
					else if (reader.Name == "forts_position")
					{
						positions.AddFortsPositionInfo(text);
						text = string.Empty;
					}
					else if (reader.Name == "forts_money")
					{
						positions.AddFortsMoneyInfo(text);
						text = string.Empty;
					}
					else if (reader.Name == "forts_collaterals")
					{
						positions.AddFortsCollateralsInfo(text);
						text = string.Empty;
					}
					else if (reader.Name == "spot_limit")
					{
						positions.AddSpotLimitInfo(text);
						text = string.Empty;
					}
					else if (reader.Name == "positions")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessClientLimits2(XmlReader reader)
		{
			string text = string.Empty;
			if (reader.MoveToAttribute("client"))
			{
				text = text + "client;" + reader.Value + ";";
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					text = text + reader.Name + ";";
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else
				{
					if (reader.NodeType != XmlNodeType.EndElement || !(reader.Name == "clientlimits"))
					{
						continue;
					}
					TransaqClientLimit transaqClientLimit = new TransaqClientLimit(text);
					try
					{
						rwClientLimits.EnterUpgradeableReadLock();
						if (!clientLimits.ContainsKey(transaqClientLimit.Client))
						{
							rwClientLimits.EnterWriteLock();
							clientLimits.Add(transaqClientLimit.Client, transaqClientLimit);
							return reader;
						}
						rwClientLimits.EnterWriteLock();
						clientLimits[transaqClientLimit.Client] = transaqClientLimit;
						return reader;
					}
					finally
					{
						if (rwClientLimits.IsWriteLockHeld)
						{
							rwClientLimits.ExitWriteLock();
						}
						rwClientLimits.ExitUpgradeableReadLock();
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessServerStatus2(XmlReader reader)
		{
			string text = string.Empty;
			if (reader.MoveToAttribute("id"))
			{
				text = text + "id;" + reader.Value + ";";
			}
			if (reader.MoveToAttribute("connected"))
			{
				text = text + "connected;" + reader.Value + ";";
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "server_status")
				{
					UpdateServerStatus(text);
					text = string.Empty;
					return reader;
				}
			}
			UpdateServerStatus(text);
			text = string.Empty;
			return reader;
		}

		private XmlReader ProcessQuotations2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "quotation" && reader.MoveToAttribute("secid"))
					{
						string text2 = text;
						text = text2 + reader.Name + ";" + reader.Value + ";";
					}
					else
					{
						text = text + reader.Name + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "quotation")
					{
						AddQuotation(text);
						text = string.Empty;
					}
					else if (reader.Name == "quotations")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessAllTrades2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "trade" && reader.MoveToAttribute("secid"))
					{
						string text2 = text;
						text = text2 + reader.Name + ";" + reader.Value + ";";
					}
					else
					{
						text = text + reader.Name + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "trade")
					{
						AddAllTrade(text);
						text = string.Empty;
					}
					else if (reader.Name == "alltrades")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessQuotes2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "quote" && reader.MoveToAttribute("secid"))
					{
						string text2 = text;
						text = text2 + reader.Name + ";" + reader.Value + ";";
					}
					else
					{
						text = text + reader.Name + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "quote")
					{
						AddQuote(text);
						text = string.Empty;
					}
					else if (reader.Name == "quotes")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessCandles2(XmlReader reader)
		{
			string text = string.Empty;
			if (reader.MoveToAttribute("secid"))
			{
				string text2 = text;
				text = text2 + reader.Name + ";" + reader.Value + ";";
			}
			if (reader.MoveToAttribute("board"))
			{
				string text3 = text;
				text = text3 + reader.Name + ";" + reader.Value + ";";
			}
			if (reader.MoveToAttribute("seccode"))
			{
				string text4 = text;
				text = text4 + reader.Name + ";" + reader.Value + ";";
			}
			if (reader.MoveToAttribute("period"))
			{
				string text5 = text;
				text = text5 + reader.Name + ";" + reader.Value + ";";
			}
			if (reader.MoveToAttribute("status"))
			{
				string text6 = text;
				text = text6 + reader.Name + ";" + reader.Value + ";";
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "candle")
					{
						text += "|";
						if (reader.MoveToAttribute("date"))
						{
							string text7 = text;
							text = text7 + reader.Name + ";" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("open"))
						{
							string text8 = text;
							text = text8 + reader.Name + ";" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("close"))
						{
							string text9 = text;
							text = text9 + reader.Name + ";" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("high"))
						{
							string text10 = text;
							text = text10 + reader.Name + ";" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("low"))
						{
							string text11 = text;
							text = text11 + reader.Name + ";" + reader.Value + ";";
						}
						if (reader.MoveToAttribute("volume"))
						{
							string text12 = text;
							text = text12 + reader.Name + ";" + reader.Value + ";";
						}
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "candles")
				{
					AddCandle(text);
					text = string.Empty;
					return reader;
				}
			}
			return reader;
		}

		private XmlReader ProcessTicks2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name != "tick")
					{
						text = text + reader.Name + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "tick")
					{
						AddTick(text);
						text = string.Empty;
					}
					else if (reader.Name == "ticks")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessOrders2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "order")
					{
						if (reader.MoveToAttribute("transactionid"))
						{
							string text2 = text;
							text = text2 + reader.Name + ";" + reader.Value + ";";
						}
					}
					else if (reader.Name == "stoporder")
					{
						text += "|stoporder;";
						if (reader.MoveToAttribute("transactionid"))
						{
							string text3 = text;
							text = text3 + reader.Name + ";" + reader.Value + ";";
						}
					}
					else if (reader.Name == "stoploss")
					{
						text += "|stoploss;";
						if (reader.MoveToAttribute("usecredit"))
						{
							string text4 = text;
							text = text4 + reader.Name + ";" + reader.Value + ";";
						}
					}
					else
					{
						text = ((!(reader.Name == "takeprofit")) ? (text + reader.Name + ";") : (text + "|takeprofit;"));
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "order")
					{
						UpdateOrder(text);
						text = string.Empty;
					}
					else if (reader.Name == "stoporder")
					{
						UpdateStopOrder(text);
						text = string.Empty;
					}
					else if (reader.Name == "orders")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private XmlReader ProcessClientTrades2(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name != "trade")
					{
						text = text + reader.Name + ";";
					}
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					text = text + reader.Value + ";";
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "trade")
					{
						AddClientTrade(text);
						text = string.Empty;
					}
					else if (reader.Name == "trades")
					{
						return reader;
					}
				}
			}
			return reader;
		}

		private void AddMarket(string data)
		{
			TransaqMarket transaqMarket = new TransaqMarket(data);
			if (!marketById.ContainsKey(transaqMarket.Id))
			{
				marketById.Add(transaqMarket.Id, transaqMarket);
			}
			if (!SessionDataTypeEditor.markets.ContainsKey(transaqMarket.Id))
			{
				SessionDataTypeEditor.markets.Add(transaqMarket.Id, transaqMarket.Name);
			}
		}

		private void AddBoard(string data)
		{
			TransaqBoard transaqBoard = new TransaqBoard(data);
			if (!boardById.ContainsKey(transaqBoard.Id))
			{
				boardById.Add(transaqBoard.Id, transaqBoard);
			}
		}

		private void AddCandleKind(string data)
		{
			TransaqCandleKind transaqCandleKind = new TransaqCandleKind(data);
			if (!candleKindByPeriod.ContainsKey(transaqCandleKind.Period))
			{
				candleKindByPeriod.Add(transaqCandleKind.Period, transaqCandleKind);
			}
		}

		private void AddSecurity(string data)
		{
			TransaqSecurity transaqSecurity = new TransaqSecurity(data);
			if (!securityBySecId.ContainsKey(transaqSecurity.SecId))
			{
				securityBySecId.Add(transaqSecurity.SecId, transaqSecurity);
			}
			if (!securityBySCB.ContainsKey(transaqSecurity.SecCodeBoard))
			{
				securityBySCB.Add(transaqSecurity.SecCodeBoard, transaqSecurity);
			}
			if (!SessionDataTypeEditor.instruments.ContainsKey(transaqSecurity.SecCodeBoard))
			{
				SessionDataTypeEditor.instruments.Add(transaqSecurity.SecCodeBoard, transaqSecurity);
			}
		}

		private void UpdateClient(string data)
		{
			TransaqClient transaqClient = new TransaqClient(data);
			try
			{
				rwClientById.EnterUpgradeableReadLock();
				if (transaqClient.Remove == "false" && !clientById.ContainsKey(transaqClient.Id))
				{
					rwClientById.EnterWriteLock();
					clientById.Add(transaqClient.Id, transaqClient);
					AccountSelectorEditor.clients.Add(transaqClient.Id);
				}
				if (transaqClient.Remove == "true" && clientById.ContainsKey(transaqClient.Id))
				{
					rwClientById.EnterWriteLock();
					clientById.Remove(transaqClient.Id);
					AccountSelectorEditor.clients.Remove(transaqClient.Id);
				}
			}
			finally
			{
				if (rwClientById.IsWriteLockHeld)
				{
					rwClientById.ExitWriteLock();
				}
				rwClientById.ExitUpgradeableReadLock();
			}
		}

		private void UpdateServerStatus(string data)
		{
			TransaqServerStatus transaqServerStatus = new TransaqServerStatus(data);
			if (transaqServerStatus.Connected == "true")
			{
				try
				{
					string.Format("{0}LogFile{1}.txt", LogFilesDir.EndsWith("\\") ? LogFilesDir : (LogFilesDir + "\\"), Clock.Now.ToString("yyMMdd"));
				}
				catch (Exception ex)
				{
					EmitError(ex.Message);
				}
				isConnected = true;
				EmitConnected();
			}
			else if (transaqServerStatus.Connected == "false")
			{
				candleKindByPeriod.Clear();
				tickEndTimeBySecId.Clear();
				tickEndTimeBySCB.Clear();
				hdrBySecId.Clear();
				hdrBySCB.Clear();
				openBookBySecId.Clear();
				openBookBySCB.Clear();
				bidOfferPairBySecId.Clear();
				securityBySecId.Clear();
				securityBySCB.Clear();
				instrumentBySymbol.Clear();
				try
				{
					rwClientById.EnterWriteLock();
					clientById.Clear();
				}
				finally
				{
					rwClientById.ExitWriteLock();
				}
				brokerAccountByClientId.Clear();
				marketById.Clear();
				boardById.Clear();
				AccountSelectorEditor.clients.Clear();
				SessionDataTypeEditor.instruments.Clear();
				SessionDataTypeEditor.markets.Clear();
				if (isInitialize)
				{
					isInitialize = false;
					connector.NewData -= ReceiveMessage;
				}
				connector.Stop();
				isConnected = false;
				EmitDisconnected();
			}
			else if (transaqServerStatus.Connected == "error")
			{
				candleKindByPeriod.Clear();
				tickEndTimeBySecId.Clear();
				tickEndTimeBySCB.Clear();
				hdrBySecId.Clear();
				hdrBySCB.Clear();
				openBookBySecId.Clear();
				openBookBySCB.Clear();
				bidOfferPairBySecId.Clear();
				securityBySecId.Clear();
				securityBySCB.Clear();
				instrumentBySymbol.Clear();
				try
				{
					rwClientById.EnterWriteLock();
					clientById.Clear();
				}
				finally
				{
					rwClientById.ExitWriteLock();
				}
				brokerAccountByClientId.Clear();
				marketById.Clear();
				boardById.Clear();
				AccountSelectorEditor.clients.Clear();
				SessionDataTypeEditor.instruments.Clear();
				SessionDataTypeEditor.markets.Clear();
				if (isInitialize)
				{
					isInitialize = false;
					connector.NewData -= ReceiveMessage;
				}
				connector.Stop();
				isConnected = false;
				EmitError(transaqServerStatus.ErrorMessage);
				EmitDisconnected();
			}
		}

		private void AddQuotation(string data)
		{
			TransaqQuote transaqQuote = new TransaqQuote(data);
			BidOfferPair bidOfferPair = null;
			if (selectedSecurity == SecuritySelector.seccode)
			{
				if (!bidOfferPairBySecId.ContainsKey(transaqQuote.SecId))
				{
					bidOfferPairBySecId.Add(transaqQuote.SecId, new BidOfferPair());
				}
				bidOfferPair = bidOfferPairBySecId[transaqQuote.SecId];
			}
			else if (selectedSecurity == SecuritySelector.seccode_board)
			{
				if (!bidOfferPairBySCB.ContainsKey(transaqQuote.SecCodeBoard))
				{
					bidOfferPairBySCB.Add(transaqQuote.SecCodeBoard, new BidOfferPair());
				}
				bidOfferPair = bidOfferPairBySCB[transaqQuote.SecCodeBoard];
			}
			bool flag = false;
			if (bidOfferPair.Bid != transaqQuote.Bid && transaqQuote.Bid != 0.0)
			{
				bidOfferPair.Bid = transaqQuote.Bid;
				if (transaqQuote.BidSize != 0.0)
				{
					bidOfferPair.BidSize = transaqQuote.BidSize;
				}
				flag = true;
			}
			if (bidOfferPair.Offer != transaqQuote.Offer && transaqQuote.Offer != 0.0)
			{
				bidOfferPair.Offer = transaqQuote.Offer;
				if (transaqQuote.OfferSize != 0.0)
				{
					bidOfferPair.OfferSize = transaqQuote.OfferSize;
				}
				flag = true;
			}
			if ((bidOfferPair.Bid == transaqQuote.Bid || transaqQuote.Bid == 0.0) && bidOfferPair.BidSize != transaqQuote.BidSize && transaqQuote.BidSize != 0.0)
			{
				bidOfferPair.BidSize = transaqQuote.BidSize;
				flag = true;
			}
			if ((bidOfferPair.Offer == transaqQuote.Offer || transaqQuote.Offer == 0.0) && bidOfferPair.OfferSize != transaqQuote.OfferSize && transaqQuote.OfferSize != 0.0)
			{
				bidOfferPair.OfferSize = transaqQuote.OfferSize;
				flag = true;
			}
			if (flag)
			{
				if (selectedSecurity == SecuritySelector.seccode)
				{
					EmitNewQuote(instrumentBySymbol[transaqQuote.SecCode], Clock.Now, bidOfferPair.Bid, (int)bidOfferPair.BidSize, bidOfferPair.Offer, (int)bidOfferPair.OfferSize);
					bidOfferPairBySecId[transaqQuote.SecId] = bidOfferPair;
				}
				else if (selectedSecurity == SecuritySelector.seccode_board)
				{
					EmitNewQuote(instrumentBySymbol[transaqQuote.SecCodeBoard], Clock.Now, bidOfferPair.Bid, (int)bidOfferPair.BidSize, bidOfferPair.Offer, (int)bidOfferPair.OfferSize);
					bidOfferPairBySCB[transaqQuote.SecCodeBoard] = bidOfferPair;
				}
			}
		}

		private void AddAllTrade(string data)
		{
			TransaqTrade transaqTrade = new TransaqTrade(data);
			if (selectedSecurity == SecuritySelector.seccode)
			{
				EmitNewTrade(instrumentBySymbol[transaqTrade.SecCode], Clock.Now, transaqTrade.Price, transaqTrade.Quantity);
			}
			else if (selectedSecurity == SecuritySelector.seccode_board)
			{
				EmitNewTrade(instrumentBySymbol[transaqTrade.SecCodeBoard], Clock.Now, transaqTrade.Price, transaqTrade.Quantity);
			}
		}

		private void AddQuote(string data)
		{
			TransaqOpenBook transaqOpenBook = new TransaqOpenBook(data);
			if (selectedSecurity == SecuritySelector.seccode)
			{
				OpenBookReturn openBookReturn = openBookBySecId[transaqOpenBook.SecId].UpdateOpenBook(transaqOpenBook);
				if (instrumentBySymbol.ContainsKey(transaqOpenBook.SecCode))
				{
					EmitNewOrderBookUpdate(instrumentBySymbol[transaqOpenBook.SecCode], Clock.Now, openBookReturn.Side, openBookReturn.Action, openBookReturn.Price, openBookReturn.Size, openBookReturn.Position);
				}
			}
			else if (selectedSecurity == SecuritySelector.seccode_board)
			{
				OpenBookReturn openBookReturn2 = openBookBySCB[transaqOpenBook.SecCodeBoard].UpdateOpenBook(transaqOpenBook);
				if (instrumentBySymbol.ContainsKey(transaqOpenBook.SecCodeBoard))
				{
					EmitNewOrderBookUpdate(instrumentBySymbol[transaqOpenBook.SecCodeBoard], Clock.Now, openBookReturn2.Side, openBookReturn2.Action, openBookReturn2.Price, openBookReturn2.Size, openBookReturn2.Position);
				}
			}
		}

		private void AddCandle(string data)
		{
			TransaqCandles transaqCandles = new TransaqCandles(data);
			if (selectedSecurity == SecuritySelector.seccode)
			{
				if (!hdrBySecId.ContainsKey(transaqCandles.SecId))
				{
					return;
				}
				switch (transaqCandles.Status)
				{
				case 0:
				case 1:
					foreach (TransaqCandle transaqCandle in transaqCandles.TransaqCandleList)
					{
						if (hdrBySecId[transaqCandles.SecId].BeginDate <= transaqCandle.Date && transaqCandle.Date < hdrBySecId[transaqCandles.SecId].EndDate)
						{
							EmitNewHistoricalBar(hdrBySecId[transaqCandles.SecId], transaqCandle.Date, transaqCandle.Open, transaqCandle.High, transaqCandle.Low, transaqCandle.Close, transaqCandle.Volume);
						}
					}
					EmitHistoricalDataCompleted(hdrBySecId[transaqCandles.SecId]);
					hdrBySecId.Remove(transaqCandles.SecId);
					break;
				case 2:
					foreach (TransaqCandle transaqCandle2 in transaqCandles.TransaqCandleList)
					{
						if (hdrBySecId[transaqCandles.SecId].BeginDate <= transaqCandle2.Date && transaqCandle2.Date < hdrBySecId[transaqCandles.SecId].EndDate)
						{
							EmitNewHistoricalBar(hdrBySecId[transaqCandles.SecId], transaqCandle2.Date, transaqCandle2.Open, transaqCandle2.High, transaqCandle2.Low, transaqCandle2.Close, transaqCandle2.Volume);
						}
					}
					break;
				case 3:
					EmitHistoricalDataError(hdrBySecId[transaqCandles.SecId], "Historical data is unavailable. Candle.");
					break;
				}
			}
			else
			{
				if (selectedSecurity != SecuritySelector.seccode_board || !hdrBySCB.ContainsKey(transaqCandles.SecCodeBoard))
				{
					return;
				}
				switch (transaqCandles.Status)
				{
				case 0:
				case 1:
					foreach (TransaqCandle transaqCandle3 in transaqCandles.TransaqCandleList)
					{
						if (hdrBySCB[transaqCandles.SecCodeBoard].BeginDate <= transaqCandle3.Date && transaqCandle3.Date < hdrBySCB[transaqCandles.SecCodeBoard].EndDate)
						{
							EmitNewHistoricalBar(hdrBySCB[transaqCandles.SecCodeBoard], transaqCandle3.Date, transaqCandle3.Open, transaqCandle3.High, transaqCandle3.Low, transaqCandle3.Close, transaqCandle3.Volume);
						}
					}
					EmitHistoricalDataCompleted(hdrBySCB[transaqCandles.SecCodeBoard]);
					hdrBySCB.Remove(transaqCandles.SecCodeBoard);
					break;
				case 2:
					foreach (TransaqCandle transaqCandle4 in transaqCandles.TransaqCandleList)
					{
						if (hdrBySCB[transaqCandles.SecCodeBoard].BeginDate <= transaqCandle4.Date && transaqCandle4.Date < hdrBySCB[transaqCandles.SecCodeBoard].EndDate)
						{
							EmitNewHistoricalBar(hdrBySCB[transaqCandles.SecCodeBoard], transaqCandle4.Date, transaqCandle4.Open, transaqCandle4.High, transaqCandle4.Low, transaqCandle4.Close, transaqCandle4.Volume);
						}
					}
					break;
				case 3:
					EmitHistoricalDataError(hdrBySCB[transaqCandles.SecCodeBoard], "Historical data is unavailable. Candle.");
					break;
				}
			}
		}

		private void AddTick(string data)
		{
			TransaqTick transaqTick = new TransaqTick(data);
			if (selectedSecurity == SecuritySelector.seccode)
			{
				if (!hdrBySecId.ContainsKey(transaqTick.SecId))
				{
					return;
				}
				if (transaqTick.TradeTime >= tickEndTimeBySecId[transaqTick.SecId] || transaqTick.TradeTime > hdrBySecId[transaqTick.SecId].EndDate)
				{
					if (SendCommandLine("<command id=\"subscribe_ticks\" filter=\"true\" />\0").Success)
					{
						EmitHistoricalDataCompleted(hdrBySecId[transaqTick.SecId]);
						hdrBySecId.Remove(transaqTick.SecId);
						tickEndTimeBySecId.Remove(transaqTick.SecId);
					}
					else
					{
						EmitHistoricalDataError(hdrBySecId[transaqTick.SecId], "Historical data is unavailable. Tick.");
					}
				}
				if (hdrBySecId[transaqTick.SecId].BeginDate <= transaqTick.TradeTime && transaqTick.TradeTime <= hdrBySecId[transaqTick.SecId].EndDate)
				{
					EmitNewHistoricalTrade(hdrBySecId[transaqTick.SecId], transaqTick.TradeTime, transaqTick.Price, transaqTick.Quantity);
				}
			}
			else
			{
				if (selectedSecurity != SecuritySelector.seccode_board || !hdrBySCB.ContainsKey(transaqTick.SecCodeBoard))
				{
					return;
				}
				if (transaqTick.TradeTime >= tickEndTimeBySCB[transaqTick.SecCodeBoard] || transaqTick.TradeTime > hdrBySCB[transaqTick.SecCodeBoard].EndDate)
				{
					if (SendCommandLine("<command id=\"subscribe_ticks\" filter=\"true\" />\0").Success)
					{
						EmitHistoricalDataCompleted(hdrBySCB[transaqTick.SecCodeBoard]);
						hdrBySCB.Remove(transaqTick.SecCodeBoard);
						tickEndTimeBySCB.Remove(transaqTick.SecCodeBoard);
					}
					else
					{
						EmitHistoricalDataError(hdrBySCB[transaqTick.SecCodeBoard], "Historical data is unavailable. Tick.");
					}
				}
				if (hdrBySCB[transaqTick.SecCodeBoard].BeginDate <= transaqTick.TradeTime && transaqTick.TradeTime <= hdrBySCB[transaqTick.SecCodeBoard].EndDate)
				{
					EmitNewHistoricalTrade(hdrBySCB[transaqTick.SecCodeBoard], transaqTick.TradeTime, transaqTick.Price, transaqTick.Quantity);
				}
			}
		}

		private void UpdateOrder(string data)
		{
			if (!isConnected)
			{
				return;
			}
			TransaqOrder transaqOrder = new TransaqOrder(data);
			TransaqOrder value = null;
			Order value2 = null;
			try
			{
				rwTransaqOrderByTrId.EnterUpgradeableReadLock();
				if (!transaqOrderByTrId.TryGetValue(transaqOrder.TransactionId, out value))
				{
					rwTransaqOrderByTrId.EnterWriteLock();
					transaqOrderByTrId.Add(transaqOrder.TransactionId, transaqOrder);
				}
				else
				{
					value.Update(transaqOrder);
					transaqOrder = value;
					rwTransaqOrderByTrId.EnterWriteLock();
					transaqOrderByTrId[transaqOrder.TransactionId] = transaqOrder;
				}
			}
			finally
			{
				if (rwTransaqOrderByTrId.IsWriteLockHeld)
				{
					rwTransaqOrderByTrId.ExitWriteLock();
				}
				rwTransaqOrderByTrId.ExitUpgradeableReadLock();
			}
			try
			{
				rwOrderByOrdId.EnterReadLock();
				orderByOrdId.TryGetValue(transaqOrder.TransactionId.ToString(), out value2);
			}
			finally
			{
				rwOrderByOrdId.ExitReadLock();
			}
			if (value2 == null)
			{
				try
				{
					rwOrderByOrdNo.EnterReadLock();
					orderByOrdNo.TryGetValue(transaqOrder.OrderNo, out value2);
				}
				finally
				{
					rwOrderByOrdNo.ExitReadLock();
				}
				if (value2 == null)
				{
					if (OutputUnknownOrderError)
					{
						EmitError($"Unknown order. TransactionID={transaqOrder.TransactionId} OrderNo={transaqOrder.OrderNo}");
					}
					return;
				}
				value2.OrderID = transaqOrder.TransactionId.ToString();
				try
				{
					rwOrderByOrdId.EnterWriteLock();
					orderByOrdId.Add(value2.OrderID, value2);
				}
				finally
				{
					rwOrderByOrdId.ExitWriteLock();
				}
			}
			if (transaqOrder.OrderNo != 0)
			{
				try
				{
					rwOrderByOrdNo.EnterUpgradeableReadLock();
					if (!orderByOrdNo.ContainsKey(transaqOrder.OrderNo))
					{
						rwOrderByOrdNo.EnterWriteLock();
						orderByOrdNo.Add(transaqOrder.OrderNo, value2);
					}
				}
				finally
				{
					if (rwOrderByOrdNo.IsWriteLockHeld)
					{
						rwOrderByOrdNo.ExitWriteLock();
					}
					rwOrderByOrdNo.ExitUpgradeableReadLock();
				}
			}
			if (string.IsNullOrWhiteSpace(transaqOrder.Status))
			{
				return;
			}
			switch (transaqOrder.Status)
			{
			case "active":
			case "wait":
			case "watching":
				if (value2.Status == OrderStatus.PendingNew && transaqOrder.OrderNo != 0)
				{
					EmitAccepted(value2);
				}
				break;
			case "cancelled":
				if (transaqOrder.WithdrawTime == "0")
				{
					EmitPendingCancel(value2);
				}
				else if ((double)transaqOrder.Quantity == transaqOrder.Balance || value2.LeavesQty == transaqOrder.Balance)
				{
					EmitCancelled(value2);
				}
				else if (!balanceByOrderNoForCancel.ContainsKey(transaqOrder.OrderNo))
				{
					balanceByOrderNoForCancel.Add(transaqOrder.OrderNo, transaqOrder.Balance);
				}
				else
				{
					balanceByOrderNoForCancel[transaqOrder.OrderNo] = transaqOrder.Balance;
				}
				break;
			case "disabled":
			case "removed":
				if ((double)transaqOrder.Quantity == transaqOrder.Balance || value2.LeavesQty == transaqOrder.Balance)
				{
					EmitRejected(value2, transaqOrder.Result);
				}
				else if (!balanceByOrderNoForCancel.ContainsKey(transaqOrder.OrderNo))
				{
					balanceByOrderNoForCancel.Add(transaqOrder.OrderNo, transaqOrder.Balance);
				}
				else
				{
					balanceByOrderNoForCancel[transaqOrder.OrderNo] = transaqOrder.Balance;
				}
				break;
			case "denied":
			case "failed":
			case "refused":
			case "rejected":
				EmitRejected(value2, transaqOrder.Result);
				break;
			}
			if (value2.IsDone)
			{
				RemoveOrder(transaqOrder.TransactionId, transaqOrder.OrderNo, ordByOrdId: true, ordByOrdNo: true, trOrdByTrId: true, trStopOrdByTrId: false);
			}
		}

		private void UpdateStopOrder(string data)
		{
			TransaqStopOrder transaqStopOrder = new TransaqStopOrder(data);
			TransaqStopOrder value = null;
			Order value2 = null;
			try
			{
				rwTransaqStopOrderByTrId.EnterUpgradeableReadLock();
				if (!transaqStopOrderByTrId.TryGetValue(transaqStopOrder.TransactionId, out value))
				{
					rwTransaqStopOrderByTrId.EnterWriteLock();
					transaqStopOrderByTrId.Add(transaqStopOrder.TransactionId, transaqStopOrder);
				}
				else
				{
					value.Update(transaqStopOrder);
					transaqStopOrder = value;
					rwTransaqStopOrderByTrId.EnterWriteLock();
					transaqStopOrderByTrId[transaqStopOrder.TransactionId] = transaqStopOrder;
				}
			}
			finally
			{
				if (rwTransaqStopOrderByTrId.IsWriteLockHeld)
				{
					rwTransaqStopOrderByTrId.ExitWriteLock();
				}
				rwTransaqStopOrderByTrId.ExitUpgradeableReadLock();
			}
			try
			{
				rwOrderByOrdId.EnterReadLock();
				orderByOrdId.TryGetValue(transaqStopOrder.TransactionId.ToString(), out value2);
			}
			finally
			{
				rwOrderByOrdId.ExitReadLock();
			}
			if (value2 == null)
			{
				if (OutputUnknownOrderError && isConnected)
				{
					EmitError($"Unknown stoporder. TransactionID={transaqStopOrder.TransactionId}");
				}
			}
			else
			{
				if (string.IsNullOrWhiteSpace(transaqStopOrder.Status))
				{
					return;
				}
				switch (transaqStopOrder.Status)
				{
				case "watching":
					if (value2.Status == OrderStatus.PendingNew)
					{
						EmitAccepted(value2);
					}
					break;
				case "cancelled":
					if (transaqStopOrder.WithdrawTime == "0")
					{
						if (value2.Status != OrderStatus.PendingCancel && value2.Status != OrderStatus.Cancelled)
						{
							EmitPendingCancel(value2);
						}
					}
					else if (value2.Status != OrderStatus.Cancelled)
					{
						EmitCancelled(value2);
					}
					break;
				case "disabled":
					if (value2.Status != OrderStatus.Cancelled)
					{
						EmitCancelled(value2);
					}
					break;
				case "denied":
				case "expired":
				case "failed":
				case "rejected":
					if (value2.Status != OrderStatus.Rejected)
					{
						EmitRejected(value2, transaqStopOrder.Result);
					}
					break;
				case "sl_executed":
				case "tp_executed":
					if (transaqStopOrder.ActiveOrderNo == 0)
					{
						break;
					}
					try
					{
						rwOrderByOrdNo.EnterUpgradeableReadLock();
						if (!orderByOrdNo.ContainsKey(transaqStopOrder.ActiveOrderNo))
						{
							rwOrderByOrdNo.EnterWriteLock();
							orderByOrdNo.Add(transaqStopOrder.ActiveOrderNo, value2);
						}
					}
					finally
					{
						if (rwOrderByOrdNo.IsWriteLockHeld)
						{
							rwOrderByOrdNo.ExitWriteLock();
						}
						rwOrderByOrdNo.ExitUpgradeableReadLock();
					}
					RemoveOrder(transaqStopOrder.TransactionId, transaqStopOrder.ActiveOrderNo, ordByOrdId: true, ordByOrdNo: false, trOrdByTrId: false, trStopOrdByTrId: true);
					break;
				}
				if (value2.IsDone)
				{
					RemoveOrder(transaqStopOrder.TransactionId, transaqStopOrder.ActiveOrderNo, ordByOrdId: true, ordByOrdNo: false, trOrdByTrId: false, trStopOrdByTrId: true);
				}
			}
		}

		private void AddClientTrade(string data)
		{
			TransaqClientTrade transaqClientTrade = new TransaqClientTrade(data);
			Order value = null;
			try
			{
				rwOrderByOrdNo.EnterReadLock();
				orderByOrdNo.TryGetValue(transaqClientTrade.OrderNo, out value);
			}
			finally
			{
				rwOrderByOrdNo.ExitReadLock();
			}
			if (clientTradeNo.ContainsKey(transaqClientTrade.TradeNo))
			{
				return;
			}
			if (value != null)
			{
				if (value.Status == OrderStatus.PendingNew && transaqClientTrade.OrderNo != 0)
				{
					EmitAccepted(value);
				}
				EmitFilled(value, transaqClientTrade.Price, transaqClientTrade.Quantity, CommissionType.Absolute, transaqClientTrade.Commission);
				if (balanceByOrderNoForCancel.ContainsKey(transaqClientTrade.OrderNo) && value.LeavesQty == balanceByOrderNoForCancel[transaqClientTrade.OrderNo])
				{
					EmitCancelled(value);
					balanceByOrderNoForCancel.Remove(transaqClientTrade.OrderNo);
				}
				long result = 0L;
				if (value.IsDone && long.TryParse(value.OrderID, out result))
				{
					RemoveOrder(result, transaqClientTrade.OrderNo, ordByOrdId: true, ordByOrdNo: true, trOrdByTrId: true, trStopOrdByTrId: true);
				}
			}
			clientTradeNo.Add(transaqClientTrade.TradeNo, null);
		}

		private void RemoveOrder(long trId, long orderNo, bool ordByOrdId, bool ordByOrdNo, bool trOrdByTrId, bool trStopOrdByTrId)
		{
			if (ordByOrdId)
			{
				try
				{
					rwOrderByOrdId.EnterWriteLock();
					orderByOrdId.Remove(trId.ToString());
				}
				finally
				{
					rwOrderByOrdId.ExitWriteLock();
				}
			}
			if (ordByOrdNo)
			{
				try
				{
					rwOrderByOrdNo.EnterWriteLock();
					orderByOrdNo.Remove(orderNo);
				}
				finally
				{
					rwOrderByOrdNo.ExitWriteLock();
				}
			}
			if (trOrdByTrId)
			{
				try
				{
					rwTransaqOrderByTrId.EnterWriteLock();
					transaqOrderByTrId.Remove(trId);
				}
				finally
				{
					rwTransaqOrderByTrId.ExitWriteLock();
				}
			}
			if (trStopOrdByTrId)
			{
				try
				{
					rwTransaqStopOrderByTrId.EnterWriteLock();
					transaqStopOrderByTrId.Remove(trId);
				}
				finally
				{
					rwTransaqStopOrderByTrId.ExitWriteLock();
				}
			}
		}

		protected override void Connect()
		{
			if (isConnected)
			{
				return;
			}
			if (!Directory.Exists(LogFilesDir))
			{
				EmitError($"LogFilesDir does not exist: {LogFilesDir}");
				return;
			}
			string text = (LogFilesDir.EndsWith("\\") ? LogFilesDir : (LogFilesDir + "\\"));
			if (!isInitialize)
			{
				short logLevel = short.Parse(LogLevelInit);
				_ = string.Empty;
				if (selectedDll == DllSelector.txmlconnector_dll)
				{
					try
					{
						if (connector != null)
						{
							connector.ConnectorUnInitialize();
						}
						connector = new TXmlConnector();
					}
					catch (Exception ex)
					{
						EmitError(ex.Message);
						return;
					}
					connector.NewData += ReceiveMessage;
					connector.ConnectorInitialize(text + '\0', logLevel);
					PasswordChangingTypeEditor.selectedDll = DllSelector.txmlconnector_dll;
				}
				else if (selectedDll == DllSelector.txcn_dll)
				{
					try
					{
						if (connector != null)
						{
							connector.ConnectorUnInitialize();
						}
						connector = new TXcnConnector();
					}
					catch (Exception ex2)
					{
						EmitError(ex2.Message);
						return;
					}
					connector.NewData += ReceiveMessage;
					connector.ConnectorInitialize(text + '\0', logLevel);
					PasswordChangingTypeEditor.selectedDll = DllSelector.txcn_dll;
				}
				isInitialize = true;
			}
			string text2 = "<command id=\"connect\">";
			text2 = text2 + "<login>" + Username + "</login>";
			text2 = text2 + "<password>" + Password + "</password>";
			text2 = text2 + "<host>" + Host + "</host>";
			object obj = text2;
			text2 = string.Concat(obj, "<port>", Port, "</port>");
			text2 = text2 + "<logsdir>" + text + "</logsdir>";
			text2 = text2 + "<loglevel>" + LogLevelConn + "</loglevel>";
			object obj2 = text2;
			text2 = string.Concat(obj2, "<micex_registers>", MicexRegisters, "</micex_registers>");
			object obj3 = text2;
			text2 = string.Concat(obj3, "<rqdelay>", RQDelay, "</rqdelay>");
			object obj4 = text2;
			text2 = string.Concat(obj4, "<session_timeout>", SessionTimeout, "</session_timeout>");
			object obj5 = text2;
			text2 = string.Concat(obj5, "<request_timeout>", RequestTimeout, "</request_timeout>");
			proxyType = ProxyType;
			proxyUsing = ProxyUsing;
			if (proxyUsing == "True" && ProxyType != "" && ProxyHost != "")
			{
				text2 = text2 + "<proxy type=\"" + proxyType + "\"";
				text2 = text2 + " addr=\"" + ProxyHost + "\"";
				object obj6 = text2;
				text2 = string.Concat(obj6, " port=\"", ProxyPort, "\"");
				if (ProxyUsername != "")
				{
					text2 = text2 + " login=\"" + ProxyUsername + "\"";
				}
				if (ProxyPassword != "")
				{
					text2 = text2 + " password=\"" + ProxyPassword + "\"";
				}
				text2 += "/>";
			}
			text2 += "</command>";
			SendCommandLine(text2);
		}

		protected override void Disconnect()
		{
			if (isConnected)
			{
				string cmd = "<command id=\"disconnect\"/>";
				SendCommandLine(cmd);
			}
		}

		protected override void Shutdown()
		{
			Disconnect();
		}

		public Transaq()
		{
			name = "Finam Transaq";
			description = "Finam Transaq Provider";
			id = 117;
			url = "http://www.finam.ru/";
			selectedDll = DllSelector.txmlconnector_dll;
			selectedSecurity = SecuritySelector.seccode;
			host = "127.0.0.1";
			port = 0;
			username = "";
			logLevelConn = "0";
			logLevelInit = "2";
			rqdelay = 100;
			micexRegisters = false;
			sessionTimeout = 120;
			requestTimeout = 20;
			proxyUsing = "";
			proxyType = "";
			proxyHost = "";
			proxyPort = 0;
			proxyUsername = "";
			proxyPassword = "";
			isInitialize = false;
			positions = new TransaqPositions();
			marketById = new Dictionary<int, TransaqMarket>();
			boardById = new Dictionary<string, TransaqBoard>();
			candleKindByPeriod = new Dictionary<long, TransaqCandleKind>();
			instrumentBySymbol = new Dictionary<string, Instrument>();
			securityBySecId = new Dictionary<int, TransaqSecurity>();
			securityBySCB = new Dictionary<string, TransaqSecurity>();
			tickEndTimeBySecId = new Dictionary<int, DateTime>();
			tickEndTimeBySCB = new Dictionary<string, DateTime>();
			hdrBySecId = new Dictionary<int, HistoricalDataRequest>();
			hdrBySCB = new Dictionary<string, HistoricalDataRequest>();
			openBookBySecId = new Dictionary<int, OpenBook>();
			openBookBySCB = new Dictionary<string, OpenBook>();
			bidOfferPairBySecId = new Dictionary<int, BidOfferPair>();
			bidOfferPairBySCB = new Dictionary<string, BidOfferPair>();
			rwClientById = new ReaderWriterLockSlim();
			rwClientLimits = new ReaderWriterLockSlim();
			clientById = new Dictionary<string, TransaqClient>();
			clientLimits = new Dictionary<string, TransaqClientLimit>();
			brokerAccountByClientId = new Dictionary<string, BrokerAccount>();
			rwOrderByOrdId = new ReaderWriterLockSlim();
			rwOrderByOrdNo = new ReaderWriterLockSlim();
			rwTransaqOrderByTrId = new ReaderWriterLockSlim();
			rwTransaqStopOrderByTrId = new ReaderWriterLockSlim();
			orderByOrdId = new Dictionary<string, Order>();
			orderByOrdNo = new Dictionary<long, Order>();
			transaqOrderByTrId = new Dictionary<long, TransaqOrder>();
			transaqStopOrderByTrId = new Dictionary<long, TransaqStopOrder>();
			clientTradeNo = new Dictionary<long, object>();
			balanceByOrderNoForCancel = new Dictionary<long, double>();
			ProxyUsingSelectorEditor.t = this;
		}

		protected override void Send(Order order)
		{
			try
			{
				if (!isConnected)
				{
					EmitRejected(order, "Send order: provider is not connected");
					return;
				}
				if (order.Type == OrderType.Market && (order.Instrument.Type == InstrumentType.Option || order.Instrument.Type == InstrumentType.Futures))
				{
					EmitRejected(order, "Send order: market order for options and futures is unavailable");
					return;
				}
				TransaqSecurity transaqSecurity = null;
				string symbol = order.Instrument.GetSymbol(name);
				if (selectedSecurity == SecuritySelector.seccode)
				{
					foreach (int key in securityBySecId.Keys)
					{
						TransaqSecurity transaqSecurity2 = securityBySecId[key];
						if (transaqSecurity2.SecCode == symbol)
						{
							transaqSecurity = transaqSecurity2;
							break;
						}
					}
				}
				else if (selectedSecurity == SecuritySelector.seccode_board)
				{
					foreach (string key2 in securityBySCB.Keys)
					{
						TransaqSecurity transaqSecurity3 = securityBySCB[key2];
						if (transaqSecurity3.SecCodeBoard == symbol)
						{
							transaqSecurity = transaqSecurity3;
							break;
						}
					}
				}
				if (transaqSecurity == null)
				{
					EmitRejected(order, $"Send order: unknown instrument {symbol}");
					return;
				}
				string text = ((order.Side == OrderSide.Buy) ? "B" : "S");
				string empty = string.Empty;
				if (order.Account != null && !string.IsNullOrWhiteSpace(order.Account))
				{
					empty = order.Account;
					goto IL_01a6;
				}
				if (DefaultAccount != null && !string.IsNullOrWhiteSpace(DefaultAccount))
				{
					empty = DefaultAccount;
					goto IL_01a6;
				}
				EmitRejected(order, "Send order: unknown account");
				goto end_IL_0000;
				IL_01a6:
				try
				{
					rwClientById.EnterReadLock();
					if (!clientById.ContainsKey(empty))
					{
						EmitRejected(order, $"Send order: unknown account {empty}");
					}
				}
				finally
				{
					rwClientById.ExitReadLock();
				}
				string cmd = string.Empty;
				switch (order.Type)
				{
				case OrderType.Market:
				{
					if (!transaqSecurity.ByMarket)
					{
						EmitRejected(order, $"Send order: bymarket isn't available for {symbol}");
						EmitError($"Send order: bymarket isn't available for {symbol}");
						return;
					}
					cmd = "<command id=\"neworder\">";
					if (selectedSecurity == SecuritySelector.seccode)
					{
						object obj5 = cmd;
						cmd = string.Concat(obj5, "<secid>", transaqSecurity.SecId, "</secid>");
					}
					else if (selectedSecurity == SecuritySelector.seccode_board)
					{
						string text5 = cmd;
						cmd = text5 + "<security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security>";
					}
					cmd = cmd + "<client>" + empty + "</client>";
					cmd += "<hidden>0</hidden>";
					object obj6 = cmd;
					cmd = string.Concat(obj6, "<quantity>", order.Qty, "</quantity>");
					cmd = cmd + "<buysell>" + text + "</buysell>";
					cmd += "<bymarket/>";
					cmd += "<unfilled>PutInQueue</unfilled>";
					if (transaqSecurity.UseCredit)
					{
						cmd += "<usecredit/>";
					}
					cmd += "</command>";
					break;
				}
				case OrderType.Limit:
				{
					cmd = "<command id=\"neworder\">";
					if (selectedSecurity == SecuritySelector.seccode)
					{
						object obj3 = cmd;
						cmd = string.Concat(obj3, "<secid>", transaqSecurity.SecId, "</secid>");
					}
					else if (selectedSecurity == SecuritySelector.seccode_board)
					{
						string text4 = cmd;
						cmd = text4 + "<security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security>";
					}
					cmd = cmd + "<client>" + empty + "</client>";
					cmd = cmd + "<price>" + order.Price.ToString().Replace(',', '.') + "</price>";
					cmd += "<hidden>0</hidden>";
					object obj4 = cmd;
					cmd = string.Concat(obj4, "<quantity>", order.Qty, "</quantity>");
					cmd = cmd + "<buysell>" + text + "</buysell>";
					cmd += "<unfilled>PutInQueue</unfilled>";
					if (transaqSecurity.UseCredit)
					{
						cmd += "<usecredit/>";
					}
					if (transaqSecurity.NoSplit)
					{
						cmd += "<nosplit/>";
					}
					cmd += "</command>";
					break;
				}
				case OrderType.Stop:
				{
					cmd = "<command id=\"newstoporder\">";
					if (selectedSecurity == SecuritySelector.seccode)
					{
						object obj7 = cmd;
						cmd = string.Concat(obj7, "<secid>", transaqSecurity.SecId, "</secid>");
					}
					else if (selectedSecurity == SecuritySelector.seccode_board)
					{
						string text6 = cmd;
						cmd = text6 + "<security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security>";
					}
					cmd = cmd + "<client>" + empty + "</client>";
					cmd = cmd + "<buysell>" + text + "</buysell>";
					cmd += "<stoploss>";
					cmd = cmd + "<activationprice>" + order.StopPrice.ToString().Replace(',', '.') + "</activationprice>";
					cmd += "<bymarket/>";
					object obj8 = cmd;
					cmd = string.Concat(obj8, "<quantity>", order.Qty, "</quantity>");
					if (transaqSecurity.UseCredit)
					{
						cmd += "<usecredit/>";
					}
					cmd += "</stoploss>";
					cmd += "</command>";
					break;
				}
				case OrderType.StopLimit:
				{
					string text2 = "";
					text2 = ((!(text == "B")) ? "LastDown" : "LastUp");
					cmd = "<command id=\"newcondorder\">";
					if (selectedSecurity == SecuritySelector.seccode)
					{
						object obj = cmd;
						cmd = string.Concat(obj, "<secid>", transaqSecurity.SecId, "</secid>");
					}
					else if (selectedSecurity == SecuritySelector.seccode_board)
					{
						string text3 = cmd;
						cmd = text3 + "<security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security>";
					}
					cmd = cmd + "<client>" + empty + "</client>";
					cmd = cmd + "<price>" + order.Price.ToString().Replace(',', '.') + "</price>";
					cmd += "<hidden>0</hidden>";
					object obj2 = cmd;
					cmd = string.Concat(obj2, "<quantity>", order.Qty, "</quantity>");
					cmd = cmd + "<buysell>" + text + "</buysell>";
					cmd = cmd + "<cond_type>" + text2 + "</cond_type>";
					cmd = cmd + "<cond_value>" + order.StopPrice.ToString().Replace(',', '.') + "</cond_value>";
					cmd += "<validafter>0</validafter>";
					cmd += "<validbefore>0</validbefore>";
					if (transaqSecurity.UseCredit)
					{
						cmd += "<usecredit/>";
					}
					if (transaqSecurity.NoSplit)
					{
						cmd += "<nosplit/>";
					}
					cmd += "</command>";
					break;
				}
				}
				TransaqResult transaqResult = SendCommandLine(cmd);
				if (transaqResult.Success)
				{
					order.OrderID = transaqResult.TransactionId;
					try
					{
						rwOrderByOrdId.EnterUpgradeableReadLock();
						if (!orderByOrdId.ContainsKey(order.OrderID))
						{
							rwOrderByOrdId.EnterWriteLock();
							orderByOrdId.Add(order.OrderID, order);
						}
						else
						{
							rwOrderByOrdId.EnterWriteLock();
							orderByOrdId[order.OrderID] = order;
						}
					}
					finally
					{
						if (rwOrderByOrdId.IsWriteLockHeld)
						{
							rwOrderByOrdId.ExitWriteLock();
						}
						rwOrderByOrdId.ExitUpgradeableReadLock();
					}
				}
				else
				{
					EmitRejected(order, $"Send order: not success! Error={transaqResult.Message}");
				}
				end_IL_0000:;
			}
			catch (Exception ex)
			{
				EmitRejected(order, $"Send order: Exception={ex.Message} StackTrace={ex.StackTrace}");
			}
		}

		protected override void Cancel(Order order)
		{
			try
			{
				if (!isConnected)
				{
					EmitCancelReject(order, order.Status, "provider is not connected");
					return;
				}
				if (string.IsNullOrWhiteSpace(order.OrderID))
				{
					EmitCancelReject(order, order.Status, "Cancel order: can't cancel order with empty TransactionId");
					return;
				}
				string empty = string.Empty;
				if (order.Type == OrderType.Stop)
				{
					empty += "<command id=\"cancelstoporder\">";
					empty = empty + "<transactionid>" + order.OrderID + "</transactionid>";
					empty += "</command>";
				}
				else
				{
					empty += "<command id=\"cancelorder\">";
					empty = empty + "<transactionid>" + order.OrderID + "</transactionid>";
					empty += "</command>";
				}
				TransaqResult transaqResult = SendCommandLine(empty);
				if (!transaqResult.Success)
				{
					EmitCancelReject(order, order.Status, $"Cancel order: not success! {transaqResult.Message}");
				}
			}
			catch (Exception ex)
			{
				EmitCancelReject(order, order.Status, $"Cancel order: Exception={ex.Message} StackTrace={ex.StackTrace}");
			}
		}

		protected override void Replace(Order order, double newQty, double newPrice, double newStopPrice)
		{
			EmitReplaceReject(order, order.Status, "Order replace is unavailable");
			EmitError("Order replace is unavailable");
		}

		protected override void Subscribe(Instrument instrument, SubscriptionDataType subscriptionDataType)
		{
			if (!isConnected)
			{
				return;
			}
			TransaqSecurity transaqSecurity = null;
			string symbol = instrument.GetSymbol(name);
			if (selectedSecurity == SecuritySelector.seccode)
			{
				foreach (int key in securityBySecId.Keys)
				{
					TransaqSecurity transaqSecurity2 = securityBySecId[key];
					if (transaqSecurity2.SecCode == symbol)
					{
						transaqSecurity = transaqSecurity2;
						break;
					}
				}
			}
			else if (selectedSecurity == SecuritySelector.seccode_board)
			{
				foreach (string key2 in securityBySCB.Keys)
				{
					TransaqSecurity transaqSecurity3 = securityBySCB[key2];
					if (transaqSecurity3.SecCodeBoard == symbol)
					{
						transaqSecurity = transaqSecurity3;
						break;
					}
				}
			}
			if (transaqSecurity == null)
			{
				EmitError($"Unknown instrument {symbol} for subscribe {subscriptionDataType}");
				return;
			}
			if (!instrumentBySymbol.ContainsKey(symbol))
			{
				instrumentBySymbol.Add(symbol, instrument);
			}
			if (selectedSecurity == SecuritySelector.seccode)
			{
				string text = "<command id=\"subscribe\">";
				switch (subscriptionDataType)
				{
				case SubscriptionDataType.Trades:
				{
					object obj3 = text;
					text = string.Concat(obj3, "<alltrades><secid>", transaqSecurity.SecId, "</secid></alltrades>");
					break;
				}
				case SubscriptionDataType.Quotes:
				{
					object obj = text;
					text = string.Concat(obj, "<quotations><secid>", transaqSecurity.SecId, "</secid></quotations>");
					object obj2 = text;
					text = string.Concat(obj2, "<quotes><secid>", transaqSecurity.SecId, "</secid></quotes>");
					break;
				}
				case SubscriptionDataType.OrderBook:
					return;
				}
				text += "</command>";
				if (SendCommandLine(text).Success && !openBookBySecId.ContainsKey(transaqSecurity.SecId))
				{
					openBookBySecId.Add(transaqSecurity.SecId, new OpenBook());
				}
			}
			else if (selectedSecurity == SecuritySelector.seccode_board)
			{
				string text2 = "<command id=\"subscribe\">";
				switch (subscriptionDataType)
				{
				case SubscriptionDataType.Trades:
				{
					string text5 = text2;
					text2 = text5 + "<alltrades><security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security></alltrades>";
					break;
				}
				case SubscriptionDataType.Quotes:
				{
					string text3 = text2;
					text2 = text3 + "<quotations><security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security></quotations>";
					string text4 = text2;
					text2 = text4 + "<quotes><security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security></quotes>";
					break;
				}
				case SubscriptionDataType.OrderBook:
					return;
				}
				text2 += "</command>";
				if (SendCommandLine(text2).Success && !openBookBySCB.ContainsKey(transaqSecurity.SecCodeBoard))
				{
					openBookBySCB.Add(transaqSecurity.SecCodeBoard, new OpenBook());
				}
			}
		}

		protected override void Unsubscribe(Instrument instrument, SubscriptionDataType subscriptionDataType)
		{
			if (!isConnected)
			{
				return;
			}
			TransaqSecurity transaqSecurity = null;
			string symbol = instrument.GetSymbol(name);
			if (selectedSecurity == SecuritySelector.seccode)
			{
				foreach (int key in securityBySecId.Keys)
				{
					TransaqSecurity transaqSecurity2 = securityBySecId[key];
					if (transaqSecurity2.SecCode == symbol)
					{
						transaqSecurity = transaqSecurity2;
						break;
					}
				}
			}
			else if (selectedSecurity == SecuritySelector.seccode_board)
			{
				foreach (string key2 in securityBySCB.Keys)
				{
					TransaqSecurity transaqSecurity3 = securityBySCB[key2];
					if (transaqSecurity3.SecCodeBoard == symbol)
					{
						transaqSecurity = transaqSecurity3;
						break;
					}
				}
			}
			if (transaqSecurity == null)
			{
				EmitError($"Unknown instrument {symbol} for unsubscribe {subscriptionDataType}");
			}
			else if (selectedSecurity == SecuritySelector.seccode)
			{
				string text = "<command id=\"unsubscribe\">";
				switch (subscriptionDataType)
				{
				case SubscriptionDataType.Trades:
				{
					object obj3 = text;
					text = string.Concat(obj3, "<alltrades><secid>", transaqSecurity.SecId, "</secid></alltrades>");
					break;
				}
				case SubscriptionDataType.Quotes:
				{
					object obj = text;
					text = string.Concat(obj, "<quotations><secid>", transaqSecurity.SecId, "</secid></quotations>");
					object obj2 = text;
					text = string.Concat(obj2, "<quotes><secid>", transaqSecurity.SecId, "</secid></quotes>");
					break;
				}
				case SubscriptionDataType.OrderBook:
					return;
				}
				text += "</command>";
				if (SendCommandLine(text).Success && openBookBySecId.ContainsKey(transaqSecurity.SecId))
				{
					openBookBySecId.Remove(transaqSecurity.SecId);
				}
			}
			else if (selectedSecurity == SecuritySelector.seccode_board)
			{
				string text2 = "<command id=\"unsubscribe\">";
				switch (subscriptionDataType)
				{
				case SubscriptionDataType.Trades:
				{
					string text5 = text2;
					text2 = text5 + "<alltrades><security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security></alltrades>";
					break;
				}
				case SubscriptionDataType.Quotes:
				{
					string text3 = text2;
					text2 = text3 + "<quotations><security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security></quotations>";
					string text4 = text2;
					text2 = text4 + "<quotes><security><board>" + transaqSecurity.Board + "</board><seccode>" + transaqSecurity.SecCode + "</seccode></security></quotes>";
					break;
				}
				case SubscriptionDataType.OrderBook:
					return;
				}
				text2 += "</command>";
				if (SendCommandLine(text2).Success && openBookBySCB.ContainsKey(transaqSecurity.SecCodeBoard))
				{
					openBookBySCB.Remove(transaqSecurity.SecCodeBoard);
				}
			}
		}
	}
}
