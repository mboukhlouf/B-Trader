using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace B_Trader.Views.Controls
{
    public class LogBox : TextBox
    {
        public static readonly DependencyProperty MaxLogsProperty = DependencyProperty.Register("MaxLogs", typeof(int), typeof(LogBox), new UIPropertyMetadata(500));

        public int MaxLogs
        {
            get
            {
                return (int)GetValue(MaxLogsProperty);
            }
            set
            {
                SetValue(MaxLogsProperty, value);
            }
        }

        public LogBox() : base()
        {
            IsReadOnly = true;
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            TextWrapping = TextWrapping.Wrap;
            AcceptsReturn = true;
        }

        public void Log(String message, bool logTime = true)
        {
            message = message.Replace("\n", " ");
            String output = $"{(logTime ? $"[{LogTime()}]: " : "")}{message}";
            if (Text.Length != 0)
                AppendText("\n");
            AppendText(output);

            int lineCount = Text.Sum((c) => c == '\n' ? 1 : 0) + 1;
            if (lineCount > MaxLogs)
            {
                Text = Text.Substring(Text.IndexOf('\n') + 1);
            }
        }

        public static string LogTime()
        {
            DateTime time = DateTime.Now;
            return time.ToString("MM/dd/yy h:mm:ss tt");
        }
    }
}
