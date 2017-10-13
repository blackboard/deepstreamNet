using DeepStreamNet;
using DeepStreamNet.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWPTest.DeepStream;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Connection.Instance.OnEventRecieved += OnDSEventRecieved;
        }

        private async void btnConnect_ClickAsync(object sender, RoutedEventArgs e)
        {
            // TODO: This should be configurable
            //String host = "deepstream-dev.bbpd.io";
            String host = "10.103.66.54"; // VPN for phone doesn't resolve host name
            int port = 6020;

            AppendToOutput("Connecting..");
            bool result = await Connection.Instance.ConnectAsync(host, port);
            if (result)
            {
                AppendToOutput(String.Format("Connected to: {0}:{1}", host, port));
            }
            else
            {
                AppendToOutput("Failed to connect: " + Connection.Instance.LastError);
            }
        }

        private void AppendToOutput(String str)
        {
            txtOutput.Text += "\n" + str;
        }

        private void dumpRecord(IDeepStreamRecord record)
        {
            AppendToOutput(String.Format("Record: {0} {1}", record.RecordName, record.ToString()));            
        }

        private async void btnRecordRead_ClickAsync(object sender, RoutedEventArgs e)
        {
            IDeepStreamRecord record = await Connection.Instance.GetRecordAsync(txtRecordName.Text);

            if (record != null)
            {
                dumpRecord(record);
            }
            else
            {
                AppendToOutput(String.Format("Failed to get record: {0}", Connection.Instance.LastError));
            }
        }

        private async void btnRecordWrite_ClickAsync(object sender, RoutedEventArgs e)
        {
            IDeepStreamRecord record = await Connection.Instance.GetRecordAsync(txtRecordName.Text);

            if (record != null)
            {
                record["detail"] = int.Parse(txtRecordData.Text);
                AppendToOutput("Record updated.");
            }
            else
            {
                AppendToOutput(String.Format("Failed to get record: {0}", Connection.Instance.LastError));
            }
        }

        private void btnEventPublish_Click(object sender, RoutedEventArgs e)
        {
            if (Connection.Instance.Publish(txtEventName.Text, txtEventValue.Text))
            {
                AppendToOutput("Event published.");
            }
            else
            {
                AppendToOutput("Event publishing failed: " + Connection.Instance.LastError);
            }
        }

        private void btnEventSubscribe_Click(object sender, RoutedEventArgs e)
        {
            if (Connection.Instance.SubscribeEvent(txtSubscribeEventName.Text))
            {
                AppendToOutput("Subscribed event: " + txtSubscribeEventName.Text);
            }
            else
            {
                AppendToOutput("Failed to Subscribed event: " + txtSubscribeEventName.Text + " error: " + 
                    Connection.Instance.LastError);
            }
        }

        private void OnDSEventRecieved(object sender, EventArg data)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AppendToOutput(String.Format("Event occured: " + data.Name + ": " + data.Data.ToString()));
            });
        }

        //        private updateUI
    }
}
