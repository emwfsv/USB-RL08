using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using USB_RLY08_Drivers.Lib;

namespace USB_RLY08
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Initate data
            Connected = false;
            EnableRelayButtons(false);
            //rect_Power.Fill = new SolidColorBrush(Colors.Red);

            //Populate data
            PopulateSerialPorts();
            CreateUSBSupervision(this, null);

        }

        private bool Connected { set; get; }
        private BoardHandler BoardHandler { set; get; }
        private int RelaySetStatus { set; get; }
        private bool SetAllRelays { set; get; }


        //==============================================
        //               User Interaction
        //==============================================

        private void cmBox_ComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmBox_ComPorts.SelectedIndex < 0)
            {
                btn_Connect.IsEnabled = false;
            }
            else
            {
                btn_Connect.IsEnabled = true;

                //We might have changed balue, Set it to disconnected
                Connected = false;
                btn_Connect.Content = "Connect";
            }
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!Connected)
            {
                BoardHandler = new BoardHandler(cmBox_ComPorts.SelectedItem.ToString());
                Connected = BoardHandler.InitiateBoard(out var softwareVersion);

                RelaySetStatus = 0;
                SetRelayButtons(!Connected);

                btn_Connect.Content = Connected ? "Disconnect" : "Connect";
                txtBox_SwVersion.Text = Connected ? softwareVersion : "";
            }
            else
            {
                Connected = false;
                btn_Connect.Content = "Connect";
                txtBox_SwVersion.Text = "";
                BoardHandler.DisconnectBoard();
            }

            btn_SetAllRelays.IsEnabled = Connected;
            btn_ResetAllRelays.IsEnabled = Connected;
            EnableRelayButtons(Connected);
        }

        private void chkBox_Relay_ValueSet(object sender, RoutedEventArgs e)
        {
            if (SetAllRelays)
                return;

            var t = (CheckBox)sender;

            try
            {
                var relayData = t.Name.Split('_');
                if (relayData.Length == 3)
                {
                    int.TryParse(relayData[2], out var relayNo);

                    if(!(bool)t.IsChecked)
                    {
                        var bitAndNumber = 0x01 << (relayNo - 1);
                        var b = ~bitAndNumber;
                        RelaySetStatus = RelaySetStatus & ~bitAndNumber;
                    }
                    else
                    {
                        var bitAndNumber = 0x01 << (relayNo - 1);

                        RelaySetStatus = RelaySetStatus | bitAndNumber;
                    }

                    BoardHandler.SetRelay(RelaySetStatus);
                }

            }
            catch { }

        }

        private void btn_SetAllRelays_Click(object sender, RoutedEventArgs e)
        {
            BoardHandler.SetRelay(255);
            SetRelayButtons(true);
        }

        private void btn_ResetAllRelays_Click(object sender, RoutedEventArgs e)
        {
            BoardHandler.SetRelay(0);
            SetRelayButtons(false);
        }
        //==============================================
        //               Populate Data
        //==============================================

        private void PopulateSerialPorts()
        {
            var availableSerials = SerialPort.GetPortNames();

            cmBox_ComPorts.Items.Clear();

            foreach (var ser in availableSerials)
            {
                cmBox_ComPorts.Items.Add(ser);
            }
        }

        private async void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            foreach (var property in instance.Properties)
            {
                Console.WriteLine(property.Name + " = " + property.Value);
            }
            try
            {
                await Task.Run(() => PopulateSerialPorts());
            }
            catch { }

            Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                // Update UI component here
                PopulateSerialPorts();
            });

        }

        private async void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                // Update UI component here
                PopulateSerialPorts();
            });
        }

        private void CreateUSBSupervision(object sender, DoWorkEventArgs e)
        {
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");

            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertWatcher.Start();

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removeWatcher.Start();

        }

        private void SetRelayButtons(bool closed)
        {
            SetAllRelays = true;
            chkBox_Relay_1.IsChecked = closed;
            chkBox_Relay_2.IsChecked = closed;
            chkBox_Relay_3.IsChecked = closed;
            chkBox_Relay_4.IsChecked = closed;
            chkBox_Relay_5.IsChecked = closed;
            chkBox_Relay_6.IsChecked = closed;
            chkBox_Relay_7.IsChecked = closed;
            chkBox_Relay_8.IsChecked = closed;

            Thread.Sleep(50);

            SetAllRelays = false;
        }

        private void EnableRelayButtons(bool enabled)
        {
            chkBox_Relay_1.IsEnabled = enabled;
            chkBox_Relay_2.IsEnabled = enabled;
            chkBox_Relay_3.IsEnabled = enabled;
            chkBox_Relay_4.IsEnabled = enabled;
            chkBox_Relay_5.IsEnabled = enabled;
            chkBox_Relay_6.IsEnabled = enabled;
            chkBox_Relay_7.IsEnabled = enabled;
            chkBox_Relay_8.IsEnabled = enabled;
        }
    }
}
