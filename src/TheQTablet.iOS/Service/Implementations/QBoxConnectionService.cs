//#define MOCK_CONNECTION

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Acr.UserDialogs;

using CoreBluetooth;

using Foundation;

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

        private string _qBoxBTName;
        public string QBoxBTName
        {
            get => _qBoxBTName;
            private set
            {
                _qBoxBTName = value;
                QBoxBTNameChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler QBoxBTNameChanged;

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

        private ObservableCollection<WiFiNetwork> _networks;
        public ObservableCollection<WiFiNetwork> Networks
        {
            get => _networks;
            private set
            {
                _networks = value;
                NetworksChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler NetworksChanged;

        private ObservableCollection<CBPeripheral> _cbDevices;
        private ObservableCollection<Peripheral> _devices;
        public ObservableCollection<Peripheral> Devices
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

            _devices = new ObservableCollection<Peripheral>();
            _cbDevices = new ObservableCollection<CBPeripheral>();

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

            var identifier = peripheral.Identifier.AsString();

            bool found = false;
            foreach(var device in _devices)
            {
                if(device.Identifier == identifier)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                _devices.Add(new Peripheral
                {
                    Identifier = identifier,
                    Name = peripheral.Name,
                });
                _cbDevices.Add(peripheral);
                DevicesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private TaskCompletionSource<bool> connectedTask;
        public override void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            Console.WriteLine("Connected: " + peripheral);

            peripheral.DiscoverServices();

            void handler(object sender, EventArgs e)
            {
                _theQBox.CharacteristicsFound -= handler;
                connectedTask.TrySetResult(true);
            }
            _theQBox.CharacteristicsFound += handler;
        }

        public override void FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            base.FailedToConnectPeripheral(central, peripheral, error);

            connectedTask.TrySetResult(false);
        }

        public void ScanDevices()
        {
            Devices = new ObservableCollection<Peripheral>();

#if MOCK_CONNECTION
            _devices.Add("A DEVICE");
            _devices.Add("ANOTHER DEVICE");
            DevicesChanged?.Invoke(this, EventArgs.Empty);
#else
            _centralManager.ScanForPeripherals(UUIDs.UartService);
#endif
        }

        public async Task Connect(Peripheral peripheral)
        {
#if MOCK_CONNECTION
            Console.WriteLine($"Fake connect: {peripheral}");
            QBoxBTName = peripheral as string;
#else
            connectedTask = new TaskCompletionSource<bool>();

            _centralManager.StopScan();

            CBPeripheral cbPeripheral = null;
            foreach(var device in _cbDevices)
            {
                if(device.Identifier.AsString() == peripheral.Identifier)
                {
                    cbPeripheral = device;
                    break;
                }
            }

            if (cbPeripheral != null)
            {
                _theQBox = new QBox(cbPeripheral);
                _centralManager.ConnectPeripheral(cbPeripheral);
                //_theQBox.Peripheral.DiscoverServices();
                QBoxBTName = _theQBox.Peripheral.Name;
            }
            else
            {
                Console.WriteLine("matching cbperipheral not found");
            }

            await connectedTask.Task;
#endif
        }

        public async Task ScanNetworks()
        {
#if MOCK_CONNECTION
            var encodedSSIDA = BitConverter.ToString(Encoding.Default.GetBytes("tim_open_w")).Replace("-", "");
            var encodedSSIDB = BitConverter.ToString(Encoding.Default.GetBytes("NETGEAR35")).Replace("-", "");
            var encodedSSIDC = BitConverter.ToString(Encoding.Default.GetBytes("tim_test_w")).Replace("-", "");
            Console.WriteLine(encodedSSIDA);
            Console.WriteLine(encodedSSIDB);
            ScanDataReceived(null, $"networks:{encodedSSIDA}:none,{encodedSSIDB}:password,{encodedSSIDC}:usernamepassword");
#else
            var task = new TaskCompletionSource<bool>();
            if (_theQBox != null)
            {
                _theQBox.Send("scan");

                void handler(object sender, NSData e)
                {
                    _theQBox.DataReceived -= handler;

                    Regex rx = new Regex(@"^networks:(?<networks>.+)$", RegexOptions.Compiled);
                    var match = rx.Match(e.ToString(NSStringEncoding.UTF8));
                    if (match.Success)
                    {
                        var networkStrings = match.Groups["networks"].Value.Split(",");
                        var networks = new Collection<WiFiNetwork>();
                        foreach (var networkString in networkStrings)
                        {
                            var split = networkString.Split(":");
                            var network = new WiFiNetwork(FromHex(split[0]), StringToAuth(split[1]));
                            networks.Add(network);
                        }
                        Networks = new ObservableCollection<WiFiNetwork>(networks);
                        task.TrySetResult(true);
                    }
                }
                //_theQBox.DataReceived += DetailsDataReceived;
                _theQBox.DataReceived += handler;
            }

            await task.Task;
#endif
        }

        private string FromHex(string hexString)
        {
            int length = hexString.Length;
            byte[] bytes = new byte[length / 2];
            for(int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return Encoding.Default.GetString(bytes);
        }

        private AuthType StringToAuth(string authString)
        {
            switch(authString)
            {
                case "usernamepassword":
                    return AuthType.UsernamePassword;
                case "none":
                    return AuthType.None;
                case "password":
                default:
                    return AuthType.Password;
            }
        }

        private void ScanDataReceived(object sender, NSData e)
        {
            Regex rx = new Regex(@"^networks:(?<networks>.+)$", RegexOptions.Compiled);
            var match = rx.Match(e.ToString(NSStringEncoding.UTF8));
            if (match.Success)
            {
                var networkStrings = match.Groups["networks"].Value.Split(",");
                var networks = new Collection<WiFiNetwork>();
                foreach(var networkString in networkStrings)
                {
                    var split = networkString.Split(":");
                    var network = new WiFiNetwork(FromHex(split[0]), StringToAuth(split[1]));
                    networks.Add(network);
                }
                Networks = new ObservableCollection<WiFiNetwork>(networks);
            }

#if !MOCK_CONNECTION
            _theQBox.DataReceived -= ScanDataReceived;
#endif
        }

        public async Task EnsureBluetoothEnabled()
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

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public async Task GetQBoxDetails()
        {
#if MOCK_CONNECTION
            var encodedSSID = BitConverter.ToString(Encoding.Default.GetBytes("DODO-D45A")).Replace("-", "");
            DetailsDataReceived(null, $"ssid:{encodedSSID},ip:192.168.1.9");
#else
            var task = new TaskCompletionSource<bool>();

            if (_theQBox != null)
            {
                _theQBox.Send("details");

                void handler(object sender, NSData e)
                {
                    _theQBox.DataReceived -= handler;

                    Regex rx = new Regex(@"^ssid:(?<ssid>.*),ip:(?<ip>.*)$", RegexOptions.Compiled);
                    var match = rx.Match(e.ToString(NSStringEncoding.UTF8));
                    if (match.Success)
                    {
                        QBoxSSID = FromHex(match.Groups["ssid"].Value);
                        QBoxIP = match.Groups["ip"].Value;
                        task.TrySetResult(true);
                    }
                    else
                    {
                        QBoxSSID = null;
                        QBoxIP = null;
                        task.TrySetResult(false);
                    }
                }
                //_theQBox.DataReceived += DetailsDataReceived;
                _theQBox.DataReceived += handler;
            }

            await task.Task;
#endif
        }

//        private void DetailsDataReceived(object sender, NSData e)
//        {
//            Regex rx = new Regex(@"^ssid:(?<ssid>.*),ip:(?<ip>.*)$", RegexOptions.Compiled);
//            var match = rx.Match(e.ToString(NSStringEncoding.UTF8));
//            if (match.Success)
//            {
//                QBoxSSID = FromHex(match.Groups["ssid"].Value);
//                QBoxIP = match.Groups["ip"].Value;
//            }
//#if !MOCK_CONNECTION
//            _theQBox.DataReceived -= DetailsDataReceived;
//#endif
//        }

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

        private async Task<bool> ConnectToNetwork(string details)
        {
            var task = new TaskCompletionSource<bool>();

            _theQBox.Send(details);

            void handler(object sender, NSData e)
            {
                _theQBox.DataReceived -= handler;

                Regex rx = new Regex(@"^(?<success>success,)ssid:(?<ssid>.*),ip:(?<ip>.*)$", RegexOptions.Compiled);
                var match = rx.Match(e.ToString(NSStringEncoding.UTF8));
                if (match.Success)
                {
                    QBoxSSID = FromHex(match.Groups["ssid"].Value);
                    QBoxIP = match.Groups["ip"].Value;
                    task.TrySetResult(match.Groups["success"].Success);
                }
                else
                {
                   task.TrySetResult(false);
                }
            }
            //_theQBox.DataReceived += DetailsDataReceived;
            _theQBox.DataReceived += handler;

            return await task.Task;
        }

        public async Task<bool> ConnectNetwork(string ssid)
        {
            var encodedSSID = BitConverter.ToString(Encoding.Default.GetBytes(ssid)).Replace("-", "");
            var toSend = $"connect:{encodedSSID}";
            Console.WriteLine($"Sent: {toSend}");

#if MOCK_CONNECTION
            ConnectToNetworkDataReceived(null, $"success,ssid:{encodedSSID},ip:192.168.1.9");
#else
            return await ConnectToNetwork(toSend);
#endif
        }

        public async Task<bool> ConnectNetwork(string ssid, string password)
        {
            var encodedSSID = BitConverter.ToString(Encoding.Default.GetBytes(ssid)).Replace("-", "");
            var toSend = $"connect:{encodedSSID},{password}";
            Console.WriteLine($"Sent: {toSend}");

#if MOCK_CONNECTION
            ConnectToNetworkDataReceived(null, $"success,ssid:{encodedSSID},ip:192.168.1.20");
#else
            return await ConnectToNetwork(toSend);
#endif
        }

        public async Task<bool> ConnectToNetwork(string ssid, string username, string password)
        {
            var encodedSSID = BitConverter.ToString(Encoding.Default.GetBytes(ssid)).Replace("-", "");
            var toSend = $"connect:{encodedSSID},{username},{password}";
            Console.WriteLine($"Sent: {toSend}");

#if MOCK_CONNECTION
            ConnectToNetworkDataReceived(null, $"success,ssid:{encodedSSID},ip:192.168.1.199");
#else
            return await ConnectToNetwork(toSend);
#endif
        }

        private void ConnectToNetworkDataReceived(object sender, NSData e)
        {
            Regex rx = new Regex(@"^(?<success>success,)ssid:(?<ssid>.*),ip:(?<ip>.*)$", RegexOptions.Compiled);
            var match = rx.Match(e.ToString(NSStringEncoding.UTF8));
            if (match.Success)
            {
                if(!match.Groups["success"].Success)
                {
                    Console.WriteLine("Failed");
                }
                QBoxSSID = FromHex(match.Groups["ssid"].Value);
                QBoxIP = match.Groups["ip"].Value;
            }
#if !MOCK_CONNECTION
            _theQBox.DataReceived -= ConnectToNetworkDataReceived;
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
