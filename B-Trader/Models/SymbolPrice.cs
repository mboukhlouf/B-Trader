using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Binance;

namespace B_Trader.Models
{
    public class SymbolPrice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SymbolInfo SymbolInfo { get; set; }

        private decimal? price;
        public decimal? Price
        {
            get => price;
            set
            {
                price = value;
                OnPropertyChanged("Price");
            }
        }
            
        public SymbolPrice()
        {
        }

        public SymbolPrice(SymbolInfo symbolInfo, decimal? price)
        {
            SymbolInfo = symbolInfo;
            Price = price;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
