using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Trader.Models
{
    public class SymbolPrice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public String BaseAsset { get; set; }
        public String QuoteAsset { get; set; }
        public String Symbol { get => $"{BaseAsset}{QuoteAsset}";  }

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

        public SymbolPrice(String baseAsset, String quoteAsset, decimal? price)
        {
            BaseAsset = baseAsset;
            QuoteAsset = quoteAsset;
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
