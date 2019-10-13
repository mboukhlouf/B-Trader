using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Controls;

using System.Threading.Tasks;

using Binance;
using Binance.Helpers;

using B_Trader.Models;

namespace B_Trader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<String, SymbolPrice> btcPairs;

        public BinanceClient BinanceClient { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            BinanceClient = new BinanceClient();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExchangeInfo exchangeInfo = await BinanceClient.GetExchangeInfoAsync();
            btcPairs = exchangeInfo.Symbols
                                    .Where(symbol => symbol.QuoteAsset == "BTC")
                                    .ToDictionary(symbol => symbol.Symbol, symbol => new SymbolPrice(symbol.BaseAsset, symbol.QuoteAsset, null));

            pairsListBox.ItemsSource = btcPairs.Values;
            Task updatePriceTask = new Task(async () =>
            {
                while(true)
                {
                    UpdatePrices();
                    await Task.Delay(1000);
                }
            });

            updatePriceTask.Start();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String searchWord = ((TextBox)sender).Text;
            pairsListBox.ItemsSource = btcPairs.Values.Where(symbol => symbol.BaseAsset.Contains(searchWord.ToUpper()));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ExchangeInfo exchangeInfo = await BinanceClient.GetExchangeInfoAsync();
            
            /*BinanceClient.ApiKey = apiKeyTextBox.Text.Trim();
            BinanceClient.SecretKey = secretKeyTextBox.Text.Trim();

            Order order = new Order()
            {
                Symbol = "ETHBTC",
                Side = OrderSide.BUY,
                Type = OrderType.LIMIT,
                Quantity = 1.0m,
                TimeInForce = TimeInForce.GTC,
                Price = 0.021077m
            };

            await BinanceClient.TestNewOrderAsync(order);*/
        }

        private async void UpdatePrices()
        {
            var prices = await BinanceClient.GetSymbolPriceTickerAsync();
            foreach(var price in prices)
            {
                if(btcPairs.ContainsKey(price.Key))
                {
                    btcPairs[price.Key].Price = price.Value;
                }
            }
        }
    }
}
