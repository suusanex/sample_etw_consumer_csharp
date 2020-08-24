using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using Environment = System.Environment;

namespace sample_etw_consumer_csharp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void WriteLine(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                Output.AppendText(msg + Environment.NewLine);
                Output.ScrollToEnd();
            });
        }

        private TraceEventSession m_TraceEventSession;
        private const string m_TraceProviderGuid = "{my-provider-guid}";
        private ETWTraceEventSource m_Consumer;

        private void OnSessionStart(object sender, RoutedEventArgs e)
        {
            m_TraceEventSession = new TraceEventSession("sample-trace-session");
            m_TraceEventSession.EnableProvider(m_TraceProviderGuid);
        }

        private void OnConsumerStart(object sender, RoutedEventArgs e)
        {
            m_Consumer = new ETWTraceEventSource("sample-consumer", TraceEventSourceType.Session);
            m_Consumer.Dynamic.All += eventData =>
            {
                WriteLine($"{eventData}");
            };
            m_Consumer.Process();
        }

        private void OnTextBoxClear(object sender, RoutedEventArgs e)
        {
            Output.Clear();
        }


        private void OnClosing(object sender, CancelEventArgs e)
        {
            m_TraceEventSession?.Dispose();
        }
    }
}
