using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Controls;

using Binance;
using Binance.Helpers;
namespace B_Trader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<SymbolInfo> btcPairs;
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
                                    .Where(symbol => symbol.QuoteAsset == "BTC");

            pairsListBox.ItemsSource = btcPairs;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String searchWord = ((TextBox)sender).Text;
            pairsListBox.ItemsSource = btcPairs.Where(symbol => symbol.BaseAsset.Contains(searchWord.ToUpper()));
        }
    }
}
