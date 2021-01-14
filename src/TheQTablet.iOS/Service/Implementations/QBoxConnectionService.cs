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

        public override void DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            //base.DisconnectedPeripheral(central, peripheral, error);
        }

        public override void UpdatedState(CBCentralManager central)
        {
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
            _devices.Add(new Peripheral() { Identifier = "1234", Name = "DEVICE" });
            _devices.Add(new Peripheral() { Identifier = "5678", Name = "ANOTHER DEVICE" });
            DevicesChanged?.Invoke(this, EventArgs.Empty);
#else
            _centralManager.ScanForPeripherals(UUIDs.UartService);
#endif
        }

        public async Task Connect(Peripheral peripheral)
        {
#if MOCK_CONNECTION
            Console.WriteLine($"Fake connect: {peripheral}");
            QBoxBTName = peripheral.Name;
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
            var task = new TaskCompletionSource<bool>();

#if MOCK_CONNECTION
            var encodedSSIDA = BitConverter.ToString(Encoding.Default.GetBytes("tim_open_w")).Replace("-", "");
            var encodedSSIDB = BitConverter.ToString(Encoding.Default.GetBytes("NETGEAR35")).Replace("-", "");
            var encodedSSIDC = BitConverter.ToString(Encoding.Default.GetBytes("tim_test_w")).Replace("-", "");
            Console.WriteLine(encodedSSIDA);
            Console.WriteLine(encodedSSIDB);
            ScanDataReceived(null, $"networks:{encodedSSIDA}:none,{encodedSSIDB}:password,{encodedSSIDC}:usernamepassword", task);
            await task.Task;
#else
            if (_theQBox != null)
            {
                var handler = new EventHandler<NSData>((object s, NSData e) => ScanDataReceived(s, e, task));
                
                _theQBox.DataReceived += handler;

                _theQBox.Send("scan");
                await task.Task;

                _theQBox.DataReceived -= handler;
            }
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

        private void ScanDataReceived(object sender, NSData e, TaskCompletionSource<bool> task)
        {
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
            else
            {
                task.TrySetResult(false);
            }
        }

        public async Task<bool> EnsureBluetoothEnabled()
        {
            while (BluetoothState != BluetoothState.PoweredOn)
            {
                if (BluetoothState == BluetoothState.Unsupported)
                {
                    await _userDialogs.AlertAsync(new AlertConfig()
                    {
                        Title = "Bluetooth Unsupported",
                        Message = "Bluetooth cannot be used in the simulator.",
                        OkText = "Ok"
                    });
#if MOCK_CONNECTION
                    return true;
#else
                    return false;
#endif
                }
                else if (BluetoothState == BluetoothState.Unauthorized)
                {
                    var ok = await _userDialogs.ConfirmAsync(new ConfirmConfig()
                    {
                        Title = "Bluetooth Unauthorized",
                        Message = "Please allow The Q app to use Bluetooth.",
                        OkText = "Done",
                        CancelText = "Exit",
                    });
                    if (!ok)
                    {
                        return false;
                    }
                }
                else if (BluetoothState == BluetoothState.PoweredOff)
                {
                    var ok = await _userDialogs.ConfirmAsync(new ConfirmConfig()
                    {
                        Title = "Connection Error",
                        Message = "Please enable Bluetooth",
                        OkText = "Done",
                        CancelText = "Exit",
                    });
                    if(!ok)
                    {
                        return false;
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            return true;
        }

        public async Task GetQBoxDetails()
        {
            var task = new TaskCompletionSource<bool>();

#if MOCK_CONNECTION
            var encodedSSID = BitConverter.ToString(Encoding.Default.GetBytes("DODO-D45A")).Replace("-", "");
            DetailsDataReceived(null, $"ssid:{encodedSSID},ip:192.168.1.9", task);
            await task.Task;
#else

            if (_theQBox != null)
            {
                var handler = new EventHandler<NSData>((object s, NSData e) => DetailsDataReceived(s, e, task));

                _theQBox.DataReceived += handler;

                _theQBox.Send("details");
                await task.Task;

                _theQBox.DataReceived -= handler;
            }
#endif
        }

        private void DetailsDataReceived(object sender, NSData e, TaskCompletionSource<bool> task)
        {
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

#if MOCK_CONNECTION
        private int _mockAttempts = 0;
#endif
        public async Task<bool> CheckConnection()
        {
#if MOCK_CONNECTION
            if (QBoxSSID == "AAA")
            {
                if (_mockAttempts % 2 == 1)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    return true;
                }
                _mockAttempts++;
            }
            return false;
#else
            // Sometimes RPi responds with IP moments before it can actually be
            // communicated with over the network, so try connecting multiple
            // times with a small delay
            var attempts = 5;
            while (attempts > 0)
            {
                var result = await _simulatorService.RunQASMAsync(0, 0);
                var success = result.Error.Equals("no error");
                if(success)
                {
                    return true;
                }
                attempts--;
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            return false;
#endif
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

#if MOCK_CONNECTION
            ConnectToNetworkDataReceived(null, $"success,ssid:414141,ip:192.168.1.9", task);
            return await task.Task;
#else
            if (_theQBox != null)
            {
                var handler = new EventHandler<NSData>((object s, NSData e) => ConnectToNetworkDataReceived(s, e, task));

                _theQBox.DataReceived += handler;

                _theQBox.Send(details);
                var result = await task.Task;

                _theQBox.DataReceived -= handler;

                return result;
            }
            else
            {
                return false;
            }
#endif
        }

        public async Task<bool> ConnectNetwork(string ssid)
        {
            var encodedSSID = BitConverter.ToString(Encoding.Default.GetBytes(ssid)).Replace("-", "");
            var toSend = $"connect:{encodedSSID}";
            Console.WriteLine($"Sent: {toSend}");

            return await ConnectToNetwork(toSend);
        }

        public async Task<bool> ConnectNetwork(string ssid, string password)
        {
            var encodedSSID = BitConverter.ToString(Encoding.Default.GetBytes(ssid)).Replace("-", "");
            var toSend = $"connect:{encodedSSID},{password}";
            Console.WriteLine($"Sent: {toSend}");

            return await ConnectToNetwork(toSend);
        }

        public async Task<bool> ConnectToNetwork(string ssid, string username, string password)
        {
            var encodedSSID = BitConverter.ToString(Encoding.Default.GetBytes(ssid)).Replace("-", "");
            var toSend = $"connect:{encodedSSID},{username},{password}";
            Console.WriteLine($"Sent: {toSend}");

            return await ConnectToNetwork(toSend);
        }

        private void ConnectToNetworkDataReceived(object sender, NSData e, TaskCompletionSource<bool> task)
        {
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
