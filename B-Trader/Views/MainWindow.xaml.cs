using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Controls;

using System.Threading.Tasks;

using Binance;
using Binance.Helpers;

using B_Trader.Models;
using B_Trader.Views.Controls;
using System.Globalization;

namespace B_Trader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<String, SymbolPrice> btcPairs;
        private ExchangeInfo exchangeInfo;

        public BinanceClient BinanceClient { get; private set; }

        public BuyInfo BuyInfo
        {
            get
            {
                decimal btcAmount = decimal.Parse(totalBtcTextBox.Text);
                double takeProfit = double.Parse(takeProfitTextBox.Text);
                double stopLoss = double.Parse(stopLossTextBox.Text);

                return new BuyInfo()
                {
                    SymbolPrice = (SymbolPrice)pairsListView.SelectedItem,
                    BtcAmount = btcAmount,
                    TakeProfit = takeProfit,
                    StopLoss = stopLoss
                };
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            BinanceClient = new BinanceClient();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            exchangeInfo = await BinanceClient.GetExchangeInfoAsync();
            btcPairs = exchangeInfo.Symbols
                                    .Where(symbol => symbol.QuoteAsset == "BTC")
                                    .ToDictionary(symbol => symbol.Symbol, symbol => new SymbolPrice(symbol, null));

            pairsListView.ItemsSource = btcPairs.Values;
            Task updatePriceTask = new Task(async () =>
            {
                while (true)
                {
                    UpdatePrices();
                    await Task.Delay(1000);
                }
            });

            updatePriceTask.Start();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (btcPairs != null)
            {
                String searchWord = ((TextBox)sender).Text;
                pairsListView.ItemsSource = btcPairs.Values.Where(symbol => symbol.SymbolInfo.BaseAsset.Contains(searchWord.ToUpper()));
            }
        }

        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            bool testing = false;
            if (sender == testOrderbutton)
                testing = true;

            BuyInfo buyInfo;
            try
            {
                buyInfo = BuyInfo;
            }
            catch (Exception)
            {
                alert.ShowAlert("Input not in format", Alert.AlertTypes.Error, 3000);
                return;
            }

            if (buyInfo.SymbolPrice == null)
            {
                alert.ShowAlert("You didn't select any symbol", Alert.AlertTypes.Error, 3000);
                return;
            }

            BinanceClient.ApiKey = apiKeyTextBox.Text.Trim();
            BinanceClient.SecretKey = secretKeyTextBox.Text.Trim();

            OrderResponse marketOrderResponse;

            try
            {
                 marketOrderResponse = await MakeMarketOrder(buyInfo, testing);
                if (marketOrderResponse != null)
                    logBox.Log(marketOrderResponse.ToString());
            }
            catch (Exception ex)
            {
                alert.ShowAlert(ex.Message, Alert.AlertTypes.Error, 3000);
                logBox.Log(ex.Message);
                return;
            }

            try
            {
                var ocoOrderResponse = await MakeOcoOrder(buyInfo, marketOrderResponse.OrigQty);
                if (ocoOrderResponse != null)
                    logBox.Log(ocoOrderResponse.ToString());
            }
            catch (Exception ex)
            {
                alert.ShowAlert(ex.Message, Alert.AlertTypes.Error, 3000);
                logBox.Log(ex.Message);
                return;
            }
        }

        private async Task<OrderResponse> MakeMarketOrder(BuyInfo buyInfo, bool testing)
        {
            decimal stepSize = (decimal)exchangeInfo.Symbols.Where(symbol => symbol.Symbol == buyInfo.SymbolPrice.SymbolInfo.Symbol)
                                .First().LotSizeFilter.StepSize;

            decimal quantity = buyInfo.BtcAmount / (decimal)buyInfo.SymbolPrice.Price;
            quantity = quantity - quantity % stepSize;
            Order order = new Order()
            {
                Symbol = buyInfo.SymbolPrice.SymbolInfo.Symbol,
                Side = OrderSide.BUY,
                Type = OrderType.MARKET,
                Quantity = quantity
            };

            logBox.Log($"BUY Market order: Symbol = {order.Symbol}, Quantity: {order.Quantity}");

            OrderResponse orderResponse = null;
            try
            {
                if (testing)
                    await BinanceClient.TestNewOrderAsync(order);
                else
                    orderResponse = await BinanceClient.NewOrderAsync(order);
            }
            catch (Exception e)
            {
                logBox.Log(e.Message);
                alert.ShowAlert(e.Message, Alert.AlertTypes.Error, 3000);
                return orderResponse;
            }

            String message;
            if (testing)
                message = "Test order went through without a problem.";
            else
                message = "Market order went through without a problem.";

            logBox.Log(message);
            alert.ShowAlert(message, Alert.AlertTypes.Success, 3000);

            return orderResponse;
        }

        private async Task<OcoOrderResponse> MakeOcoOrder(BuyInfo buyInfo, decimal quantity)
        {
            decimal stepSize = (decimal)exchangeInfo.Symbols.Where(symbol => symbol.Symbol == buyInfo.SymbolPrice.SymbolInfo.Symbol)
                                .First().LotSizeFilter.StepSize;

            decimal tpPrice = (decimal)buyInfo.SymbolPrice.Price + ((decimal)buyInfo.SymbolPrice.Price * (decimal)buyInfo.TakeProfit) / 100;
            decimal slPrice = (decimal)buyInfo.SymbolPrice.Price - ((decimal)buyInfo.SymbolPrice.Price * (decimal)buyInfo.StopLoss) / 100;

            OcoOrder order = new OcoOrder()
            {
                Symbol = buyInfo.SymbolPrice.SymbolInfo.Symbol,
                Side = OrderSide.SELL,
                Quantity = quantity,
                Price = tpPrice,
                StopPrice = slPrice,
                StopLimitPrice = slPrice,
                StopLimitTimeInForce = TimeInForce.GTC
            };

            logBox.Log($"Sell OCO: Quantity = {quantity}, TP = {tpPrice}, SL = {slPrice}");

            OcoOrderResponse orderResponse = null;
            try
            {
                orderResponse = await BinanceClient.NewOcoOrderAsync(order);
            }
            catch (Exception e)
            {
                logBox.Log(e.Message);
                alert.ShowAlert(e.Message, Alert.AlertTypes.Error, 3000);
                return orderResponse;
            }

            String message = "Oco order went through without a problem.";

            logBox.Log(message);
            alert.ShowAlert(message, Alert.AlertTypes.Success, 3000);

            return orderResponse;
        }


        private async void UpdatePrices()
        {
            var prices = await BinanceClient.GetSymbolPriceTickerAsync();
            foreach (var price in prices)
            {
                if (btcPairs.ContainsKey(price.Key))
                {
                    btcPairs[price.Key].Price = price.Value;
                }
            }
        }
    }
}
