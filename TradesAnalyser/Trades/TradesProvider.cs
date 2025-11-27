using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using TradesAnalyser.Models;

namespace TradesAnalyser.Trades
{
    internal class TradesProvider
    {
        private readonly IFormatProvider numberFormatProvider = new NumberFormatInfo { 
            NumberGroupSeparator = " ",
            NumberDecimalSeparator = "." 
        };
        private readonly CultureInfo cultureInfo = new CultureInfo("ru-RU");

        public List<Trade> Trades { get; private set;  }
        public TradesProvider() {
        }

        public void Load(string fileName)
        {
            var result = new List<Trade>();
            foreach (string line in File.ReadLines(fileName))
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue; 
                }
                if (Parse(line, out Trade trade))
                {
                    result.Add(trade);
                }
            }
            Trades = result;
        }
        
        private bool Parse(string input, out Trade result)
        {
            result = null;
            var parts = input.Split(';');

            if (parts.Length < 23)
            {
                return false;
            }

            decimal pnl;
            try
            {
                pnl = decimal.Parse(parts[23], numberFormatProvider);
            }
            catch(Exception)
            {
                //MessageBox.Show($"{e.Message}: '{parts[23]}'");
                return false;
            }

            if (!int.TryParse(parts[3], out int lot))
            {
                return false;
            }

            var side = Base.Side.Buy;
            if (parts[1].ToLower() == "\"короткая\"")
            {
                side = Base.Side.Sell;
            }

            DateTime parsedDateTime;
            string dateTimeString = $"{parts[8]} {parts[9]}";
            string dateTimeFormat = "dd.MM.yyyy HH:mm:ss";
            try
            {
                parsedDateTime = DateTime.ParseExact(dateTimeString, dateTimeFormat, cultureInfo);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n{dateTimeString}\n{dateTimeFormat}");
                return false;
            }

            result = new Trade
            {
                Time = parsedDateTime,
                Side = side,
                Pnl = pnl,
                Lot = lot
            };

            return true;
        }
    }
}
