using System;
using System.Collections;
using System.ComponentModel;
using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant.Providers;

namespace SmartQuant.Finam
{
    public class FinamHistoryDownloader: IHistoryProvider, IProvider
    {
        private bool isConnected = false;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event ProviderErrorEventHandler Error;
        public event EventHandler StatusChanged;
        
        #region Члены IHistoryProvider

        #region Свойства

        [Category("Data")]
        public bool BarSupported
        {
            get { return true; }
        }

        [Category("Data")]
        public bool DailySupported
        {
            get { return true; }
        }

        [Category("Data")]
        public bool QuoteSupported
        {
            get { return false; }
        }

        [Category("Data")]
        public bool TradeSupported
        {
            get { return true; }
        }

        #endregion

        public Bar[] GetBarHistory(IFIXInstrument instrument, DateTime datetime1, DateTime datetime2, int barSize)
        {
            Bar[] barArray;
            ArrayList list = new ArrayList();

            FinamData fm = new FinamData();
            fm.MarketCode = Convert.ToInt32(instrument.SecurityExchange.Trim());
            fm.SecurityId = Convert.ToInt32(instrument.SecurityID.Trim());
            fm.BarTimeMode = FinamData.EnumBarTimeMode.BarEnd;
            switch (barSize)
            {
                case 60:    //1 минута
                    fm.TimeFrame = FinamData.EnumTimeFrame.Min01;
                    break;
                case 300:    //5 минут
                    fm.TimeFrame = FinamData.EnumTimeFrame.Min05;
                    break;
                case 600:    //10 минут
                    fm.TimeFrame = FinamData.EnumTimeFrame.Min10;
                    break;
                case 900:    //15 минута
                    fm.TimeFrame = FinamData.EnumTimeFrame.Min15;
                    break;
                case 1800:    //30 минут
                    fm.TimeFrame = FinamData.EnumTimeFrame.Min30;
                    break;
                case 3600:    //60 минут
                    fm.TimeFrame = FinamData.EnumTimeFrame.Hourly;
                    break;
                default:
                    throw new Exception("Не поддерживается данный таймфрейм:" + 
                        "\nЗадано в секундах = " + barSize.ToString() + " (" + 
                        Math.Round((double)(barSize/60)).ToString() + " мин)");

            }
            fm.StartDate = datetime1;
            fm.EndDate = datetime2;

            fm.FillData();
            //Вот тут наверно можно пограмотней сделать:). И без всяких ArrayList list = new ArrayList();
            for (int i = 0; i < fm.DataBar.Count; i++)
            {
                list.Add(new Bar(
                    fm.DataBar[i].BarDateTime,
                    fm.DataBar[i].Open,
                    fm.DataBar[i].High,
                    fm.DataBar[i].Low,
                    fm.DataBar[i].Close,
                    fm.DataBar[i].Volume,
                    barSize));
            }
            barArray = list.ToArray(typeof(Bar)) as Bar[];
            return barArray;
        }

        public Daily[] GetDailyHistory(IFIXInstrument instrument, DateTime datetime1, DateTime datetime2, bool dividendAndSplitAdjusted)
        {
            Daily[] dailyArray;
            ArrayList list = new ArrayList();

            FinamData fm = new FinamData();
            fm.MarketCode = Convert.ToInt32(instrument.SecurityExchange.Trim());
            fm.SecurityId = Convert.ToInt32(instrument.SecurityID.Trim());
            fm.TimeFrame = FinamData.EnumTimeFrame.Daily;
            fm.BarTimeMode = FinamData.EnumBarTimeMode.BarEnd;
            fm.StartDate = datetime1;
            fm.EndDate = datetime2;

            fm.FillData();
            //И тут тоже наверняка:)
            for (int i = 0; i < fm.DataBar.Count; i++)
            {
                list.Add(new Daily(
                    fm.DataBar[i].BarDateTime, 
                    fm.DataBar[i].Open, 
                    fm.DataBar[i].High, 
                    fm.DataBar[i].Low,
                    fm.DataBar[i].Close,
                    fm.DataBar[i].Volume));
            }
            dailyArray = list.ToArray(typeof(Daily)) as Daily[];
            return dailyArray;
        }

        public Quote[] GetQuoteHistory(IFIXInstrument instrument, DateTime datetime1, DateTime datetime2)
        {
            throw new NotSupportedException("This operation is not supported");
        }

        public Trade[] GetTradeHistory(IFIXInstrument instrument, DateTime datetime1, DateTime datetime2)
        {
            Trade[] tradeArray;
            ArrayList list = new ArrayList();
            
            FinamData fm = new FinamData();
            fm.MarketCode = Convert.ToInt32(instrument.SecurityExchange.Trim());
            fm.SecurityId = Convert.ToInt32(instrument.SecurityID.Trim());
            fm.TimeFrame = FinamData.EnumTimeFrame.Tick;
            fm.StartDate = datetime1;
            fm.EndDate = datetime2;

            fm.FillData();
            //А тут уж я совсем намудрил. В общем, не нашел, в List свойство LongLength
            TradeData[] tradedata = fm.DataTrade.ToArray();
            for (long i = 0; i < tradedata.LongLength; i++)
            {
                list.Add(new Trade(
                    tradedata[i].TradeDateTime,
                    tradedata[i].TradePrice,
                    (int)tradedata[i].Volume));
            }
            tradeArray = list.ToArray(typeof(Trade)) as Trade[];
            return tradeArray;
        }

        #endregion

        #region Члены IProvider

        public FinamHistoryDownloader()
        {
            ProviderManager.Add(this);
        }
        
        public void Connect()
        {
            if (!this.isConnected)
            {
                this.isConnected = true;
                this.EmitConnected();
                this.EmitStatusChanged();
            }
        }

        public void Connect(int timeout)
        {
            this.Connect();
            ProviderManager.WaitConnected(this, timeout);
        }

        public void Disconnect()
        {
            if (this.isConnected)
            {
                this.isConnected = false;
                this.EmitDisconnected();
                this.EmitStatusChanged();
            }
        }

        private void EmitConnected()
        {
            if (this.Connected != null)
            {
                this.Connected(this, EventArgs.Empty);
            }
        }

        private void EmitDisconnected()
        {
            if (this.Disconnected != null)
            {
                this.Disconnected(this, EventArgs.Empty);
            }
        }

        private void EmitError(int id, int code, string message)
        {
            if (this.Error != null)
            {
                this.Error(new ProviderErrorEventArgs(new ProviderError(Clock.Now, this, id, code, message)));
            }
        }

        private void EmitStatusChanged()
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, EventArgs.Empty);
            }
        }

        [Category("Information")]
        public byte Id
        {
            get { return 66; }
        }
        
        [Category("Status")]
        public bool IsConnected
        {
            get { return this.isConnected; }
        }

        [Category("Information")]
        public string Name
        {
            get { return "Finam"; }
        }

        public void Shutdown()
        {
            this.Disconnect();
        }

        [Category("Status")]
        public ProviderStatus Status
        {
            get
            {
                if (!this.isConnected)
                {
                    return ProviderStatus.Disconnected;
                }
                return ProviderStatus.Connected;
            }
        }

        [Category("Information")]
        public string Title
        {
            get { return "Finam history data downloader"; }
        }

        [Category("Information")]
        public string URL
        {
            get { return "https://www.finam.ru/analysis/export/default.asp"; }
        }

        #endregion
    }
}
