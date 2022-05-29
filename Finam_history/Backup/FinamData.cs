using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace SmartQuant.Finam
{
    /// <summary>
    /// Структура для баров
    /// </summary>
    public struct BarData
    {
        public DateTime BarDateTime;
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public long Volume;

        public BarData(DateTime barDateTime,
            double open, double high, double low, double close, long volume)
        {
            BarDateTime = barDateTime;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }
    }

    /// <summary>
    /// Структура для сделок
    /// </summary>
    public struct TradeData
    {
        public DateTime TradeDateTime;
        public double TradePrice;
        public long Volume;
        public TradeData(DateTime tradeDateTime, double tradePrice, long volume)
        {
            TradeDateTime = tradeDateTime;
            TradePrice = tradePrice;
            Volume = volume;
        }
    }

    /// <summary>
    /// Класс, обслуживающий получение данных с сайта Финам
    /// </summary>
    public class FinamData
    {
        #region Перечисления

        /// <summary>
        /// Таймфрейм
        /// </summary>
        public enum EnumTimeFrame
        {
            Tick = 1,
            Min01 = 2,
            Min05 = 3,
            Min10 = 4,
            Min15 = 5,
            Min30 = 6,
            Hourly = 7,
            Daily = 8,
            Weekly = 9,
            Monthly = 10,
            Hourly1030 = 11
        }

        /// <summary>
        /// Выдавать время свечи
        /// </summary>
        public enum EnumBarTimeMode
        {
            /// <summary>
            /// На начало бара
            /// </summary>
            BarStart = 0,

            /// <summary>
            /// На конец бара
            /// </summary>
            BarEnd = 1
        }

        #endregion

        /// <summary>
        /// Структура базы Финам для формирования запросов к данным
        /// </summary>
        public struct FinamDb
        {
            public int EmitentId;
            public string EmitentName;
            public string EmitentCode;
            public int EmitentMarket;
            public FinamDb(int emitentId, string emitentName, string emitentCode, int emitentMarket)
            {
                EmitentId = emitentId;
                EmitentName = emitentName;
                EmitentCode = emitentCode;
                EmitentMarket = emitentMarket;
            }
        }

        #region Private-переменные

        /*"http://195.128.78.52/<FILE_NAME>.<FILE_EXTENTION>" + 
        "?d=d&m=<MARKET_CODE>&em=<SECURITY_ID>&p=<TIMEFRAME>&df=<DAY_FROM>&mf=<MONTH_FROM>" + 
        "&yf=<YEAR_FROM>&dt=<DAY_TO>&mt=<MONTH_TO>&yt=<YEAR_TO>&f=<FILE_NAME>&e=.<FILE_EXTENTION>" + 
        "&dtf=<DATE_FORMAT>&tmf=<TIME_FORMAT>&MSOR=<BAR_TIME_MODE>&cn=<SECURITY_CODE>" + 
        "&sep=<FIELD_SEPARATOR>&sep2=<DECIMAL_SEPARATOR>&datf=<FILE_FORMAT>" + 
        "[&at=<ADD_TITLE>][&fsp=<FILL_EMPTY_PERIODS>]"*/

        private const string Urlpattern = "http://{0}/TempData.txt" +
            "?d=d&m={1}&em={2}&p={3}&df={4}&mf={5}" +
            "&yf={6}&dt={7}&mt={8}&yt={9}&f=TempData&e=.txt" +
            "&dtf=1&tmf=1&MSOR={10}&cn=SECURITYCODE" +
            "&sep=3&sep2=1&datf={11}";

        #endregion

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public FinamData()
        {
            FinamDataBase = new List<FinamDb>();
            FinamDbFile = Application.StartupPath + @"\TempDb.txt";
            FinamDbUrl = "http://www.finam.ru/scripts/export.js";
            DataTrade = new List<TradeData>();
            DataBar = new List<BarData>();
            TempFile = Application.StartupPath + @"\TempData.txt";
            Header = "http://www.finam.ru/analysis/export/default.asp";
            EndDate = DateTime.MinValue;
            StartDate = DateTime.MinValue;
            SecurityId = -1;
            MarketCode = -1;
            Ip = "195.128.78.52";
        }

        #region Свойства

        /// <summary>
        /// IP-адрес сервера, куда производиться запрос
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Код рынка запрашиваемой бумаги
        /// </summary>
        public int MarketCode { get; set; }

        /// <summary>
        /// Идентификатор запрашиваемого инструмента
        /// </summary>
        public int SecurityId { get; set; }

        /// <summary>
        /// Таймфрейм запрашиваемого инструмента
        /// </summary>
        public EnumTimeFrame TimeFrame { get; set; }

        /// <summary>
        /// Время бара - на начало бара или наконец
        /// </summary>
        public EnumBarTimeMode BarTimeMode { get; set; }

        /// <summary>
        /// Дата начала запроса
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания запроса
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Указывает _referer'a в заголовке при запросе данных
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Указывает полный путь к временному файлу с данными
        /// </summary>
        public string TempFile { get; set; }

        /// <summary>
        /// Возвращает или задает массив полученных данных для баров
        /// </summary>
        public List<BarData> DataBar { get; set; }

        /// <summary>
        /// Возвращает или задает массив полученных данных для сделок
        /// </summary>
        public List<TradeData> DataTrade { get; set; }

        /// <summary>
        /// Возвращает или задает удаленный путь к Бд финам для формирования запросов на получение данных
        /// </summary>
        public string FinamDbUrl { get; set; }

        /// <summary>
        /// Возвращает или задает локальный полный путь для файла базы данных Финам
        /// </summary>
        public string FinamDbFile { get; set; }

        /// <summary>
        /// Возвращает или задает массив данных в базе для формирования запросов на получение данных
        /// </summary>
        public List<FinamDb> FinamDataBase { get; set; }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Формирует строку для запроса на сервер Финам
        /// </summary>
        /// <param name="marketCode">взять из файла MARKETS для выбранного инструмента</param>
        /// <param name="securityId">взять из файла IDs для выбранного инструмента</param>
        /// <param name="timeFrame">Таймфрейм</param>
        /// <param name="startDate">Дата начала</param>
        /// <param name="endDate">Дата окончания</param>
        /// <param name="barTimeMode">Время свечи (на конец или на начало)</param>
        /// <returns></returns>
        private string RequestedUrl(int marketCode, int securityId, EnumTimeFrame timeFrame,
            DateTime startDate, DateTime endDate, EnumBarTimeMode barTimeMode)
        {
            int timemode = 5;
            if (timeFrame == EnumTimeFrame.Tick)
                timemode = 9;

            return string.Format(Urlpattern,
                Ip,
                marketCode, securityId, (int)timeFrame, startDate.Day.ToString("#0"), (startDate.Month - 1).ToString("#0"),
                startDate.Year.ToString("0000"), endDate.Day.ToString("#0"), (endDate.Month - 1).ToString("#0"),
                endDate.Year.ToString("0000"), (int)barTimeMode, timemode);
        }

        /// <summary>
        /// Добавляет данные для формата BarData
        /// </summary>
        private void AddBarData()
        {
            var sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var lines = System.IO.File.ReadAllLines(TempFile);

            for (long i = 0; i < lines.LongLength; i++)
            {
                var values = lines[i].Split(';');
                var dt = DateTime.ParseExact(values[0] + values[1], "yyyyMMddHHmmss", null);
                var open = double.Parse(values[2].Replace(".", sep));
                var high = double.Parse(values[3].Replace(".", sep));
                var low = double.Parse(values[4].Replace(".", sep));
                var close = double.Parse(values[5].Replace(".", sep));
                var volume = long.Parse(values[6]);
                if ((dt < StartDate.Date) || (dt > EndDate.AddSeconds(215999))) continue;
                DataBar.Add(new BarData(dt, open, high, low, close, volume));

                /*System.Diagnostics.Debug.Print(
                    "Дата, добавляемая в банк = {0}, " +
                    "Open = {1}, High = {2}, Low = {3}, Close = {4}, Volume = {5}",
                    dt.ToString("dd.MM.yyyy HH:mm:ss"),
                    open.ToString("#0.0000"),
                    high.ToString("#0.0000"),
                    low.ToString("#0.0000"),
                    close.ToString("#0.0000"),
                    volume);*/
            }
        }

        /// <summary>
        /// Добавляет данные для формата TradeData
        /// </summary>
        private void AddTradeData()
        {
            var sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var lines = System.IO.File.ReadAllLines(TempFile);

            for (long i = 0; i < lines.LongLength; i++)
            {
                var values = lines[i].Split(';');
                try
                {
                    var dt = DateTime.ParseExact(values[0] + values[1], "yyyyMMddHHmmss", null);
                    var last = double.Parse(values[2].Replace(".", sep));
                    var volume = long.Parse(values[3]);
                    if ((dt >= StartDate.Date) && (dt <= EndDate.AddSeconds(215999)))
                    {
                        DataTrade.Add(new TradeData(dt, last, volume));

                        /*System.Diagnostics.Debug.Print(
                            "Дата, добавляемая в банк = {0}, " +
                            "Last = {1}, Volume = {2}",
                            dt.ToString("dd.MM.yyyy HH:mm:ss"),
                            last.ToString("#0.0000"),
                            volume);*/
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "AddTradeData", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Загружаем данные в память в зависимости от таймфрейма
        /// </summary>
        /// <param name="timeFrame"></param>
        private void AddData(EnumTimeFrame timeFrame)
        {
            switch (timeFrame)
            {
                case EnumTimeFrame.Tick:
                    AddTradeData();
                    break;
                default:
                    AddBarData();
                    break;
            }
        }

        #endregion

        #region Public-методы

        /// <summary>
        /// Загружаем дату в память
        /// </summary>
        public void FillData()
        {
            var dt = StartDate;
            var cdt = StartDate;

            DataBar.Clear();
            DataTrade.Clear();
            try
            {
                while (dt <= EndDate)
                {
                    var url = RequestedUrl(MarketCode, SecurityId, TimeFrame,
                        dt, dt, BarTimeMode);
                    //System.Diagnostics.Debug.Print("Загружаем = " + dt.ToString("dd.MM.yyyy HH:mm:ss"));
                    //System.Diagnostics.Debug.Print(url);
                    using (var wc = new WebClient())
                    {
                        wc.Headers.Add(HttpRequestHeader.Referer, Header);
                        wc.DownloadFile(url, TempFile);
                    }

                    AddData(TimeFrame);
                    cdt = cdt.AddDays(1);

                    //Выбираем максимальную дату из this.Data
                    if (DataBar.Count > 0)
                        //dt = DataBar.Max(d => d.BarDateTime).AddDays(1);
                        dt = DataBar[DataBar.Count - 1].BarDateTime.AddDays(1);
                    //Если счетчик больше последней даты, выбираем счетчик
                    if (DateTime.Compare(cdt, dt) > 0)
                        dt = cdt;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "FillData", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Загружаем дату в память
        /// </summary>
        /// <param name="marketCode">Взять из файла MARKETS для выбранного инструмента</param>
        /// <param name="securityId">Взять из файла IDs для выбранного инструмента</param>
        /// <param name="timeFrame">Таймфрейм</param>
        /// <param name="startDate">Дата начала запроса</param>
        /// <param name="endDate">Дата окончания запроса</param>
        /// <param name="barTimeMode">Время бара - на начало бара или наконец</param>
        public void FillData(int marketCode, int securityId, EnumTimeFrame timeFrame,
            DateTime startDate, DateTime endDate, EnumBarTimeMode barTimeMode)
        {
            var dt = startDate;
            var cdt = startDate;

            DataBar.Clear();
            DataTrade.Clear();
            try
            {
                while (dt <= endDate)
                {
                    var url = RequestedUrl(marketCode, securityId, timeFrame,
                        dt, dt, barTimeMode);
                    using (var wc = new WebClient())
                    {
                        wc.Headers.Add(HttpRequestHeader.Referer, Header);
                        wc.DownloadFile(url, TempFile);
                    }
                    AddData(timeFrame);
                    cdt = cdt.AddDays(1);

                    //Выбираем максимальную дату из this.Data                    
                    if (DataBar.Count > 0)
                        //dt = DataBar.Max(d => d.BarDateTime).AddDays(1);                        
                        dt = DataBar[DataBar.Count - 1].BarDateTime.AddDays(1);
                    //Если счетчик больше последней даты, выбираем счетчик
                    if (DateTime.Compare(cdt, dt) > 0)
                        dt = cdt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "FillData", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загружаем Бд Финама о доступных бумагах в память
        /// </summary>
        public void GetFinamDb(bool isUpdateFromWeb)
        {
            try
            {
                if (isUpdateFromWeb)
                {
                    using (var wc = new WebClient())
                    {
                        wc.Headers.Add(HttpRequestHeader.Referer, Header);
                        wc.DownloadFile(FinamDbUrl, FinamDbFile);
                    }
                }
                if (!System.IO.File.Exists(FinamDbFile))
                    throw new Exception("Не найден файл базы инструментов Финам");
                var lns = System.IO.File.ReadAllLines(FinamDbFile, System.Text.Encoding.Default);
                for (int i = 0; i < lns.Length; i++)
                {
                    lns[i] = lns[i].Replace("var aEmitentIds=new Array(", "");
                    lns[i] = lns[i].Replace("var aEmitentNames=new Array(", "");
                    lns[i] = lns[i].Replace("var aEmitentCodes=new Array(", "");
                    lns[i] = lns[i].Replace("var aEmitentMarkets=new Array(", "");
                    lns[i] = lns[i].Replace(");", "");
                    lns[i] = lns[i].Replace("'", "");
                }
                char[] sep = { ',' };
                var aEmitentIds = lns[0].Split(sep);
                var aEmitentNames = lns[1].Split(sep);
                var aEmitentCodes = lns[2].Split(sep);
                var aEmitentMarkets = lns[3].Split(sep);

                FinamDataBase.Clear();
                for (int j = 0; j < aEmitentIds.Length; j++)
                {
                    FinamDataBase.Add(new FinamDb(Convert.ToInt32(aEmitentIds[j]), aEmitentNames[j],
                        aEmitentCodes[j], Convert.ToInt32(aEmitentMarkets[j])));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetFinamDb", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}