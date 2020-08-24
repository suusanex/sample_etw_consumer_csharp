using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        private const string m_TraceProviderGuid = "{3bcf1aae-4f19-49a0-9521-533e43fce039}";
        private ETWTraceEventSource m_Consumer;
        private static readonly string m_TraceSessionName = "sample-trace-session";
        private CancellationTokenSource m_ConsumerCancel;

        private void OnSessionStart(object sender, RoutedEventArgs e)
        {
            m_TraceEventSession = new TraceEventSession(m_TraceSessionName);
            var bRet = m_TraceEventSession.EnableProvider(new Guid(m_TraceProviderGuid));
            if (bRet)
            {
                WriteLine("Session Exist");
            }

            WriteLine("Session Start");
        }

        private void OnConsumerStart(object sender, RoutedEventArgs e)
        {
            m_Consumer = new ETWTraceEventSource(m_TraceSessionName, TraceEventSourceType.Session);
            m_Consumer.Dynamic.All += eventData =>
            {
                Trace.WriteLine($"{eventData}");
            };
            m_Consumer.Dynamic.AddCallbackForProviderEvent("EventClass", "Test1", eventData =>
            {
                WriteLine($"Test1:{eventData.PayloadByName("message")}");
            });

            m_ConsumerCancel = new CancellationTokenSource();
            Task.Run(() =>
            {

                m_Consumer.Process();

            }, m_ConsumerCancel.Token);

            WriteLine("Consumer Start");
        }

        private void OnTextBoxClear(object sender, RoutedEventArgs e)
        {
            Output.Clear();
        }


        private void OnClosing(object sender, CancelEventArgs e)
        {
            m_Consumer?.StopProcessing();
            m_Consumer?.Dispose();
            m_ConsumerCancel?.Cancel();
            m_TraceEventSession?.Dispose();
        }
    }
}
