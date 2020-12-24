#define MOCK_CONNECTION

using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Acr.UserDialogs;

using CoreBluetooth;

using Foundation;

using MvvmCross.ViewModels;

using TheQTablet.Core.Rest.Interfaces;
using TheQTablet.Core.Service.Interfaces;

namespace TheQTablet.iOS.Service.Implementations
{
    public static class UUIDs
    {
        public static readonly CBUUID UartService =          CBUUID.FromString("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        public static readonly CBUUID UartRxCharacteristic = CBUUID.FromString("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
        public static readonly CBUUID UartTxCharacteristic = CBUUID.FromString("6e400003-b5a3-f393-e0a9-e50e24dcca9e");
    }

    public class QBoxConnectionService : CBCentralManagerDelegate, IQBoxConnectionService
    {
        private readonly IUserDialogs _userDialogs;
        private readonly ISimulatorService _simulatorService;
        private readonly IQSimClient _qsimClient;

        private CBCentralManager _centralManager;
        private QBox _theQBox;

        public BluetoothState BluetoothState => CBStateToBTState(_centralManager.State);
        public event EventHandler BluetoothStateChanged;

        private string _qBoxSSID;
        public string QBoxSSID
        {
            get => _qBoxSSID;
            private set
            {
                _qBoxSSID = value;
                QBoxSSIDChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler QBoxSSIDChanged;

        private string _qBoxIP;
        public string QBoxIP
        {
            get => _qBoxIP;
            private set
            {
                _qBoxIP = value;
                _qsimClient.Host = QBoxIP;
                QBoxIPChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler QBoxIPChanged;

        private ObservableCollection<string> _networks;
        public ObservableCollection<string> Networks
        {
            get => _networks;
            private set
            {
                _networks = value;
                NetworksChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler NetworksChanged;

        private ObservableCollection<string> _devices;
        public ObservableCollection<string> Devices
        {
            get => _devices;
            private set
            {
                _devices = value;
                DevicesChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler DevicesChanged;

        public QBoxConnectionService(IUserDialogs userDialogs, ISimulatorService simulatorService, IQSimClient qsimClient)
        {
            _userDialogs = userDialogs;
            _simulatorService = simulatorService;
            _qsimClient = qsimClient;

            _devices = new ObservableCollection<string>();

            Console.WriteLine("BT INIT?");
            _centralManager = new CBCentralManager(this, null);
        }

        public override void UpdatedState(CBCentralManager central)
        {
            switch(central.State)
            {
                case CBCentralManagerState.Unknown:
                    break;
                case CBCentralManagerState.Resetting:
                    break;
                case CBCentralManagerState.Unsupported:
                    break;
                case CBCentralManagerState.PoweredOff:
                    break;
                case CBCentralManagerState.PoweredOn:
                    break;
                case CBCentralManagerState.Unauthorized:
                    break;
            }

            Console.WriteLine(central.State);
            BluetoothStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            Console.WriteLine("Discovered: " + peripheral);

            _devices.Add(peripheral.Name);
            DevicesChanged?.Invoke(this, EventArgs.Empty);
        }

        //public override void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        //{
        //    Console.WriteLine("Connected: " + peripheral);

        //    peripheral.DiscoverServices();
        //}

        public void ScanDevices()
        {
#if MOCK_CONNECTION
            _devices.Add("A DEVICE");
            _devices.Add("ANOTHER DEVICE");
            DevicesChanged?.Invoke(this, EventArgs.Empty);
#else
            _centralManager.ScanForPeripherals(UUIDs.UartService);
#endif
        }

        public void Connect(string peripheral)
        {
#if MOCK_CONNECTION
            Console.WriteLine($"Fake connect: {peripheral}");
#else
            _centralManager.StopScan();

            _theQBox = new QBox(cBPeripheral);
            _theQBox.Peripheral.DiscoverServices();
#endif
        }

        public void ScanNetworks()
        {
#if MOCK_CONNECTION
            ScanDataReceived(null, "Networks:DODO-D45A,NETGEAR35");
#else
            _theQBox.Send("scan");
            _theQBox.DataReceived += ScanDataReceived;
#endif
        }

        private void ScanDataReceived(object sender, NSData e)
        {
            Regex rx = new Regex(@"^Networks:(?<networks>.*)$", RegexOptions.Compiled);
            var match = rx.Match(e.ToString(NSStringEncoding.UTF8));
            if (match.Success)
            {
                var networks = match.Groups["networks"].Value.Split(",");
                Networks = new ObservableCollection<string>(networks);
            }

#if !MOCK_CONNECTION
            _theQBox.DataReceived -= ScanDataReceived;
#endif
        }

        public void EnsureBluetoothEnabled()
        {
            if (BluetoothState == BluetoothState.Unsupported)
            {
                _userDialogs.Alert(new AlertConfig()
                {
                    Title = "Bluetooth Unsupported",
                    Message = "Bluetooth cannot be used in the simulator.",
                    OkText = "Ok"
                });
                return;
            }

            while (BluetoothState != BluetoothState.PoweredOn)
            {
                if (BluetoothState == BluetoothState.Unauthorized)
                {
                    _userDialogs.Alert(new AlertConfig()
                    {
                        Title = "Bluetooth Unauthorized",
                        Message = "Please allow The Q app to use Bluetooth.",
                        OkText = "Ok"
                    });
                }
                else if (BluetoothState == BluetoothState.PoweredOff)
                {
                    _userDialogs.Alert(new AlertConfig()
                    {
                        Title = "Connection Error",
                        Message = "Please enable Bluetooth",
                        OkText = "Done"
                    });
                }
            }
        }

        public void GetQBoxDetails()
        {
#if MOCK_CONNECTION
            DetailsDataReceived(null, "ssid:DODO-D45A,ip:192.168.1.17");
#else
            _theQBox.Send("details");
            _theQBox.DataReceived += DetailsDataReceived;
#endif
        }

        private void DetailsDataReceived(object sender, NSData e)
        {
            Regex rx = new Regex(@"^ssid:(?<ssid>.*),ip:(?<ip>.*)$", RegexOptions.Compiled);
            var match = rx.Match(e.ToString(NSStringEncoding.UTF8));
            if (match.Success)
            {
                QBoxSSID = match.Groups["ssid"].Value;
                QBoxIP = match.Groups["ip"].Value;
            }
#if !MOCK_CONNECTION
            _theQBox.DataReceived -= DetailsDataReceived;
#endif
        }

        public async Task<bool> CheckConnection()
        {
            var result = await _simulatorService.RunQASMAsync(0, 0);
            return result.Error.Equals("no error");
        }

        private static BluetoothState CBStateToBTState(CBCentralManagerState state)
        {
            switch (state)
            {
                case CBCentralManagerState.Resetting:
                    return BluetoothState.Resetting;
                case CBCentralManagerState.Unsupported:
                    return BluetoothState.Unsupported;
                case CBCentralManagerState.PoweredOff:
                    return BluetoothState.PoweredOff;
                case CBCentralManagerState.PoweredOn:
                    return BluetoothState.PoweredOn;
                case CBCentralManagerState.Unauthorized:
                    return BluetoothState.Unauthorized;
                case CBCentralManagerState.Unknown:
                default:
                    return BluetoothState.Unknown;
            }
        }

        public void SendNetworkCredentials(string ssid, string password)
        {
            var toSend = $"connect:{ssid},{password}";
            Console.WriteLine($"Sent: {toSend}");

#if MOCK_CONNECTION
            SendNetworkCredentialsDataReceived(null, $"success,ssid:{ssid},ip:192.168.1.17");
#else
            _theQBox.Send($"connect:{ssid},{password}");
            _theQBox.DataReceived += SendNetworkCredentialsDataReceived;
#endif
        }

        private void SendNetworkCredentialsDataReceived(object sender, NSData e)
        {
            Regex rx = new Regex(@"^(?<success>success,)ssid:(?<ssid>.*),ip:(?<ip>.*)$", RegexOptions.Compiled);
            var match = rx.Match(e.ToString(NSStringEncoding.UTF8));
            if (match.Success)
            {
                if(!match.Groups["success"].Success)
                {
                    Console.WriteLine("Failed");
                }
                QBoxSSID = match.Groups["ssid"].Value;
                QBoxIP = match.Groups["ip"].Value;
            }
#if !MOCK_CONNECTION
            _theQBox.DataReceived -= DetailsDataReceived;
#endif
        }
    }

    public class QBox : CBPeripheralDelegate
    {
        public CBPeripheral Peripheral;

        private CBCharacteristic _rxCharacteristic;
        private CBCharacteristic _txCharacteristic;

        public event EventHandler<NSData> DataReceived;
        public event EventHandler CharacteristicsFound;

        public QBox(CBPeripheral peripheral)
        {
            Peripheral = peripheral;

            Peripheral.Delegate = this;
        }

        public override void DiscoveredService(CBPeripheral peripheral, NSError error)
        {
            foreach(var service in peripheral.Services)
            {
                Console.WriteLine("Discovered service: " + service);
                if(service.UUID == UUIDs.UartService)
                {
                    Console.WriteLine("Found UART service");
                    peripheral.DiscoverCharacteristics(service);
                }
            }
        }

        public override void DiscoveredCharacteristic(CBPeripheral peripheral, CBService service, NSError error)
        {
            Console.WriteLine("Discover??");
            foreach (var characteristic in service.Characteristics)
            {
                Console.WriteLine("Discovered characteristic: " + characteristic);

                if (service.UUID == UUIDs.UartService)
                {
                    if(characteristic.UUID == UUIDs.UartTxCharacteristic)
                    {
                        _txCharacteristic = characteristic;
                        peripheral.SetNotifyValue(true, characteristic);
                    }
                    else if(characteristic.UUID == UUIDs.UartRxCharacteristic)
                    {
                        _rxCharacteristic = characteristic;
                    }
                }
            }

            if(_rxCharacteristic != null && _txCharacteristic != null)
            {
                CharacteristicsFound?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            Console.WriteLine("Char value updated: " + characteristic + " = " + characteristic.Value);

            DataReceived?.Invoke(null, characteristic.Value);
        }

        public bool Send(string message)
        {
            if(_rxCharacteristic != null)
            {
                Peripheral.WriteValue(message, _rxCharacteristic, CBCharacteristicWriteType.WithResponse);
                return true;
            }
            return false;
        }
    }
}
