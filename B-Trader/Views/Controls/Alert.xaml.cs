using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace B_Trader.Views.Controls
{
    /// <summary>
    /// Interaction logic for Alert.xaml
    /// </summary>
    public partial class Alert : UserControl
    {
        public enum AlertTypes
        {
            Information,
            Success,
            Error,
            Warning
        }

        public static readonly DependencyProperty AlertTypeDependency = DependencyProperty.Register("AlertType", typeof(AlertTypes), typeof(Alert), new UIPropertyMetadata(AlertTypes.Information));

        public AlertTypes AlertType
        {
            get
            {
                return (AlertTypes)GetValue(AlertTypeDependency);
            }
            set
            {
                SetValue(AlertTypeDependency, value);
            }
        }

        public Alert()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the alert that will disappear after a certain amount of time 
        /// </summary>
        /// <param name="message">The alert message</param>
        /// <param name="millisecondsTime">The time after which the alert will disappear in milliseconds</param>
        public async void ShowAlert(String message, int millisecondsTime = 3 * 1000)
        {
            alertMessageTextBlock.Text = message;
            this.Visibility = Visibility.Visible;
            await Task.Delay(millisecondsTime);
            DoubleAnimation animation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(1)), FillBehavior.Stop);
            animation.Completed += (sender, e) => this.Visibility = Visibility.Collapsed;
            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        /// <summary>
        /// Shows the alert with the specified alert type that will disappear after a certain amount of time 
        /// </summary>
        /// <param name="message">The alert message</param>
        /// <param name="alertType">The type of the alert</param>
        /// <param name="millisecondsTime">The time after which the alert will disappear in milliseconds</param>
        public async void ShowAlert(String message, AlertTypes alertType, int millisecondsTime = 3 * 1000)
        {
            alertMessageTextBlock.Text = message;
            this.AlertType = alertType;
            this.Visibility = Visibility.Visible;
            await Task.Delay(millisecondsTime);
            DoubleAnimation animation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(1)), FillBehavior.Stop);
            animation.Completed += (sender, e) => this.Visibility = Visibility.Collapsed;
            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }
}
