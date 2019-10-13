using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Trader.Models
{
    public class BuyInfo
    {
        public SymbolPrice SymbolPrice { get; set; }
        public decimal BtcAmount { get; set; }
        public double TakeProfit { get; set; }
        public double StopLoss { get; set; }

        public BuyInfo()
        {
        }
    }
}
