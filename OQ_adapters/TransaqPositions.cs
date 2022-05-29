using System.Collections.Generic;

namespace OpenQuant.Finam
{
	public sealed class TransaqPositions
	{
		public object lockerMoneyPosition;

		public object lockerSecPosition;

		public object lockerFortsPosition;

		public object lockerFortsMoney;

		public object lockerFortsCollaterals;

		public object lockerSpotLimit;

		public Dictionary<string, Dictionary<string, TransaqMoneyPosition>> MoneyPosition { get; private set; }

		public Dictionary<string, Dictionary<string, TransaqSecPosition>> SecPosition { get; private set; }

		public Dictionary<string, Dictionary<string, TransaqFortsPosition>> FortsPosition { get; private set; }

		public Dictionary<string, TransaqFortsMoney> FortsMoney { get; private set; }

		public Dictionary<string, TransaqFortsCollaterals> FortsCollaterals { get; private set; }

		public Dictionary<string, TransaqSpotLimit> SpotLimit { get; private set; }

		public TransaqPositions()
		{
			lockerMoneyPosition = new object();
			lockerSecPosition = new object();
			lockerFortsPosition = new object();
			lockerFortsMoney = new object();
			lockerFortsCollaterals = new object();
			lockerSpotLimit = new object();
			MoneyPosition = new Dictionary<string, Dictionary<string, TransaqMoneyPosition>>();
			SecPosition = new Dictionary<string, Dictionary<string, TransaqSecPosition>>();
			FortsPosition = new Dictionary<string, Dictionary<string, TransaqFortsPosition>>();
			FortsMoney = new Dictionary<string, TransaqFortsMoney>();
			FortsCollaterals = new Dictionary<string, TransaqFortsCollaterals>();
			SpotLimit = new Dictionary<string, TransaqSpotLimit>();
		}

		public void AddMoneyPositionInfo(string data)
		{
			TransaqMoneyPosition transaqMoneyPosition = new TransaqMoneyPosition(data);
			lock (lockerMoneyPosition)
			{
				if (!MoneyPosition.ContainsKey(transaqMoneyPosition.Client))
				{
					MoneyPosition.Add(transaqMoneyPosition.Client, new Dictionary<string, TransaqMoneyPosition>());
					MoneyPosition[transaqMoneyPosition.Client].Add(transaqMoneyPosition.Register, transaqMoneyPosition);
				}
				else if (!MoneyPosition[transaqMoneyPosition.Client].ContainsKey(transaqMoneyPosition.Register))
				{
					MoneyPosition[transaqMoneyPosition.Client].Add(transaqMoneyPosition.Register, transaqMoneyPosition);
				}
				else
				{
					MoneyPosition[transaqMoneyPosition.Client][transaqMoneyPosition.Register].Update(transaqMoneyPosition);
				}
			}
		}

		public void AddSecPositionInfo(string data)
		{
			TransaqSecPosition transaqSecPosition = new TransaqSecPosition(data);
			lock (lockerSecPosition)
			{
				if (!SecPosition.ContainsKey(transaqSecPosition.Client))
				{
					SecPosition.Add(transaqSecPosition.Client, new Dictionary<string, TransaqSecPosition>());
					SecPosition[transaqSecPosition.Client].Add(transaqSecPosition.SecCode + transaqSecPosition.Register, transaqSecPosition);
				}
				else if (!SecPosition[transaqSecPosition.Client].ContainsKey(transaqSecPosition.SecCode + transaqSecPosition.Register))
				{
					SecPosition[transaqSecPosition.Client].Add(transaqSecPosition.SecCode + transaqSecPosition.Register, transaqSecPosition);
				}
				else
				{
					SecPosition[transaqSecPosition.Client][transaqSecPosition.SecCode + transaqSecPosition.Register].Update(transaqSecPosition);
				}
			}
		}

		public void AddFortsPositionInfo(string data)
		{
			TransaqFortsPosition transaqFortsPosition = new TransaqFortsPosition(data);
			lock (lockerFortsPosition)
			{
				if (!FortsPosition.ContainsKey(transaqFortsPosition.Client))
				{
					FortsPosition.Add(transaqFortsPosition.Client, new Dictionary<string, TransaqFortsPosition>());
					FortsPosition[transaqFortsPosition.Client].Add(transaqFortsPosition.SecCode, transaqFortsPosition);
				}
				else if (!FortsPosition[transaqFortsPosition.Client].ContainsKey(transaqFortsPosition.SecCode))
				{
					FortsPosition[transaqFortsPosition.Client].Add(transaqFortsPosition.SecCode, transaqFortsPosition);
				}
				else
				{
					FortsPosition[transaqFortsPosition.Client][transaqFortsPosition.SecCode].Update(transaqFortsPosition);
				}
			}
		}

		public void AddFortsMoneyInfo(string data)
		{
			TransaqFortsMoney transaqFortsMoney = new TransaqFortsMoney(data);
			lock (lockerFortsMoney)
			{
				if (!FortsMoney.ContainsKey(transaqFortsMoney.Client))
				{
					FortsMoney.Add(transaqFortsMoney.Client, transaqFortsMoney);
				}
				else
				{
					FortsMoney[transaqFortsMoney.Client].Update(transaqFortsMoney);
				}
			}
		}

		public void AddFortsCollateralsInfo(string data)
		{
			TransaqFortsCollaterals transaqFortsCollaterals = new TransaqFortsCollaterals(data);
			lock (lockerFortsCollaterals)
			{
				if (!FortsCollaterals.ContainsKey(transaqFortsCollaterals.Client))
				{
					FortsCollaterals.Add(transaqFortsCollaterals.Client, transaqFortsCollaterals);
				}
				else
				{
					FortsCollaterals[transaqFortsCollaterals.Client].Update(transaqFortsCollaterals);
				}
			}
		}

		public void AddSpotLimitInfo(string data)
		{
			TransaqSpotLimit transaqSpotLimit = new TransaqSpotLimit(data);
			lock (lockerSpotLimit)
			{
				if (!SpotLimit.ContainsKey(transaqSpotLimit.Client))
				{
					SpotLimit.Add(transaqSpotLimit.Client, transaqSpotLimit);
				}
				else
				{
					SpotLimit[transaqSpotLimit.Client].Update(transaqSpotLimit);
				}
			}
		}
	}
}
