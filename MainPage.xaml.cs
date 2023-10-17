using MauiShinyTest.Extension;
using MauiShinyTest.Popup;
using Microsoft.Maui.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.BluetoothLE;
using Shiny.Hosting;
using Shiny.BluetoothLE.Managed;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;

namespace MauiShinyTest;

public partial class MainPage : ContentPage
{

    public MainPage(IBleManager _bleManager)
    {
        InitializeComponent();
        BindingContext = this;
        bleManager = _bleManager;
    }


    [Reactive] public List<BleCharacteristicInfo> Characteristics { get; private set; }


    public string SensorConnectionStatus { get; set; } = "Disconnected";
    public string SensorScanStatus { get; set; } = "Not found";
    public string SensorCharacteristicStatus { get; set; } = "Disconnected";

    readonly IBleManager bleManager;

    public IPeripheral peripheral;
    public IDisposable scanner, chsResult, wscObserver;
    public Guid uuid;
    public ScanResult scanResult;
    public BleCharacteristicInfo sensorCharacteristic;
    public BleCharacteristicInfo readCharacteristic;
    public BleCharacteristicInfo writeCharacteristic;
    public BleCharacteristicInfo notifyCharacteristic;
    public BleServiceInfo blservice = null!;
    public List<byte> messageBytes;

    private bool hasDeviceStatusSubscription = false;


    async void OnScanClicked(object s, EventArgs e)
    {
        var access = await bleManager.RequestAccess();
        Console.WriteLine("Shiny Access: " + access);

        Stopwatch watch = Stopwatch.StartNew();

        scanner = bleManager.Scan().Subscribe(scanResult =>
        {
            SensorScanStatus = "Scanning...";
            OnPropertyChanged(nameof(SensorScanStatus));

            Debug.WriteLine("Found a device with name " + scanResult.Peripheral.Name);

            var adv = scanResult.AdvertisementData;

            if (adv.LocalName != null && adv.LocalName.Contains("AN8150"))
            {
                watch.Stop();
                SensorScanStatus = "Device found: " + scanResult.Peripheral.Name.ToString();
                OnPropertyChanged(nameof(SensorScanStatus));
                var elapsed = watch.ElapsedMilliseconds;
                Debug.WriteLine("Stopping scan after " + elapsed + " mS");
                peripheral = scanResult.Peripheral;
                scanner?.Dispose();

            }
        });

    }
    private byte[] convertbytes(string hexString)
    {
        //string hexString = "4944454e543f0d";
        byte[] byteArray = new byte[hexString.Length / 2];

        for (int i = 0; i < byteArray.Length; i++)
        {
            byteArray[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        }

        string originalStringss = Encoding.UTF8.GetString(byteArray);
        byte[] command = Encoding.UTF8.GetBytes(originalStringss);
        return command;
    }
    async void OnServiceClicked(object s, EventArgs e)
    {
       // var noti = await peripheral.NotifyCharacteristic(notifyCharacteristic, false, false).Timeout(TimeSpan.FromSeconds(60)).ToTask();
        byte[] command;
        //var chsResults = await peripheral.GetServices(true);
        command = convertbytes("4944454e543f0d");
         var wr = await peripheral.WriteCharacteristic(notifyCharacteristic, command,true);
        command = convertbytes("4d4f44454c3f0d");
        var wr1 = await peripheral.WriteCharacteristic(notifyCharacteristic, command,false);

        //var wr = await peripheral.WriteCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-6daa-4d02-abf6-19569aca69fe", command, true);
        ////var managed = peripheral.(RxApp.MainThreadScheduler); // schedule 
        ////managed.StartRssi();
        //if (peripheral.Status == ConnectionState.Disconnected) // .IsDisconnected())
        //    await peripheral.ConnectAsync();

        ////var ss = await peripheral.GetAllCharacteristics();
        //var teste = await peripheral.GetServices(true);
        //foreach (var serv in teste)
        //{
        //    //var rssi = await device.Gatt.ReadRssi();
        //    Debug.WriteLine($" {serv.Uuid} ");

        //    //Debug.Indent();

        //    foreach (var chars in await peripheral.GetCharacteristics(serv.Uuid))
        //    {
        //        Debug.WriteLine($"{chars.Uuid} Properties:{chars.Properties}");

        //        Debug.Indent();

        //        foreach (var descriptors in await peripheral.GetDescriptors(serv.Uuid, chars.Uuid))
        //        {
        //            Debug.WriteLine($"Descriptor:{descriptors.Uuid} Value:{descriptors.GetValue}");

        //            //var val2 = await descriptors.ReadValueAsync();

        //            //if (descriptors.Uuid == GattDescriptorUuids.ClientCharacteristicConfiguration)
        //            //{
        //            //    Debug.WriteLine($"Notifying:{val2[0] > 0}");
        //            //}
        //            //else if (descriptors.Uuid == GattDescriptorUuids.CharacteristicUserDescription)
        //            //{
        //            //    Debug.WriteLine($"UserDescription:{ByteArrayToString(val2)}");
        //            //}
        //            //else
        //            //{
        //            //    Debug.WriteLine(ByteArrayToString(val2));
        //            //}

        //        }

        //        Debug.Unindent();
        //    }

        //    Debug.Unindent();
        //}



        //byte[] command = Encoding.UTF8.GetBytes("MODEL?\r\n"); // Replace "YourBinaryData" with your actual binary data

        //// string base64String = Convert.ToBase64String(command);
        //var rd= await peripheral.ReadCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-6daa-4d02-abf6-19569aca69fe");
        //var wr = await peripheral.WriteCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-6daa-4d02-abf6-19569aca69fe", command,true);
        //var rds = await peripheral.ReadCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-6daa-4d02-abf6-19569aca69fe");
        //var rds1 = await peripheral.ReadCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-6daa-4d02-abf6-19569aca69fe");
        //var rds2 = await peripheral.ReadCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-6daa-4d02-abf6-19569aca69fe");
        //var rds3 = await peripheral.ReadCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-6daa-4d02-abf6-19569aca69fe");
        //string originalString = Encoding.UTF8.GetString(wr.Data);
        //Debug.WriteLine($"Notifying:{originalString}");

        //string originalString1 = Encoding.UTF8.GetString(rds1.Data);
        //Debug.WriteLine($"Notifying:{originalString1}");

        //string originalString2 = Encoding.UTF8.GetString(rds2.Data);
        //Debug.WriteLine($"Notifying:{originalString2}");


        //string originalString3 = Encoding.UTF8.GetString(rds3.Data);
        //Debug.WriteLine($"Notifying:{originalString3}");





        ////GetCharacteristics
        ////var kk = await peripheral.GetService("0000aaa0-0000-1000-8000-aabbccddeef");

    }

    async void OnConnectClicked(object s, EventArgs e)
    {
        if (!hasDeviceStatusSubscription)
        {
            try
            {
                //
                await peripheral.WithConnectIf().Timeout(TimeSpan.FromSeconds(30)).ToTask();
                await peripheral.ConnectAsync();
                //*** Test All Characteristics
                var teste = await peripheral.GetServices(true);
                readCharacteristic=await peripheral.GetCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-1e4d-4bd9-ba61-23c647249616");
                writeCharacteristic = await peripheral.GetCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-1e4d-4bd9-ba61-23c647249616");
                notifyCharacteristic = await peripheral.GetCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-1e4d-4bd9-ba61-23c647249616");




                wscObserver = peripheral.WhenStatusChanged().Subscribe(connectionState =>
                {
                    hasDeviceStatusSubscription = true;
                    Debug.WriteLine("Connected to " + connectionState.ToString());
                    switch (connectionState)
                    {


                        case Shiny.BluetoothLE.ConnectionState.Connected:
                            Debug.WriteLine("Device connected OK");
                            SensorConnectionStatus = "Connected to " + peripheral.Name.ToString();
                            OnPropertyChanged(nameof(SensorConnectionStatus));
                            break;

                        case Shiny.BluetoothLE.ConnectionState.Disconnected:
                            Debug.WriteLine("Device disconnected");
                            SensorConnectionStatus = "Disconnected";
                            OnPropertyChanged(nameof(SensorConnectionStatus));
                            break;

                        case Shiny.BluetoothLE.ConnectionState.Connecting:
                            Debug.WriteLine("Device in connecting state");
                            SensorConnectionStatus = "Connecting...";
                            OnPropertyChanged(nameof(SensorConnectionStatus));
                            break;


                        default:
                            Debug.WriteLine("break...");

                            break;

                    }

                });

                //*** Get Characteristc - ChBattery
                //sensorCharacteristic = await peripheral.GetCharacteristic(AppSettings.GetServiceUuid, AppSettings.ChBattery).Take(1)
                //      .Timeout(TimeSpan.FromSeconds(10));
            }
            catch (Exception ex)
            {
                await DialogServices.SnackBarMsg(ex.Message);
            }
        }

        Debug.WriteLine("Connecting...");
        Debug.WriteLine("Connected to " + peripheral.Status.ToString());
        //peripheral.Connect(new ConnectionConfig
        //{
        //    AutoConnect = true,

        //});
        if (peripheral.Status == ConnectionState.Disconnected) // .IsDisconnected())
        {
            await peripheral.ConnectAsync();
        }
        else
        {

        }




        //sensorCharacteristic = await peripheral.GetAllCharacteristics();
        //                .WithConnectIf()
        //                .Select(x => x.GetServices())
        //                .Switch()
        //                .ToTask();

        var ss = await peripheral.GetAllCharacteristics();
        //var teste = await peripheral.ReadCharacteristicAsync(AppSettings.GetunknownServiceUuid.ToString(), AppSettings.ChunknownNotifyVib.ToString());
    }

    async void OnStartNotify(object s, EventArgs e)
    {
        sensorCharacteristic = null;
        string hexString = "4944454e543f0d";
        byte[] byteArray = new byte[hexString.Length / 2];

        for (int i = 0; i < byteArray.Length; i++)
        {
            byteArray[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        }

        string originalStringss = Encoding.UTF8.GetString(byteArray);
        byte[] command = Encoding.UTF8.GetBytes(originalStringss);


        //string seriveUUID = AppSettings.ServiceUUID;
        //string charUUID = AppSettings.ChNotifyVib;

        string seriveUUID = AppSettings.GetunknownServiceUuid;
        string charUUID = AppSettings.ChunknownNotifyVib;






        //sensorCharacteristic.AssertWrite(true);
        //sensorCharacteristic.AssertNotify();
        notifyCharacteristic.AssertNotify();
       await peripheral
            //.WriteCharacteristic("49535343-fe7d-4ae5-8fa9-9fafd205e455", "49535343-1e4d-4bd9-ba61-23c647249616",command,true)
            .NotifyCharacteristic(notifyCharacteristic,false,false)
            .Timeout(TimeSpan.FromSeconds(60)).ToTask();
            //.SubOnMainThread(
            //    x =>
            //    {
            //        //read
            //        this.SetReadValue(x, false);
            //        chsResult.Dispose();
            //    }
            //    , ex =>
            //    {
            //        _ = DialogServices.SnackBarMsg("Erro DoNotifyAsync!\n" + ex.Message);
            //        //chsResult.Dispose();
            //    }
                //        async x =>
                //        {
                //            // Read the value and set it
                //            this.SetReadValue(x, false);
                //            //await chsResult.DisposeAsync(); // Assuming DisposeAsync is an asynchronous method
                //        },
                //async ex =>
                //{
                //    _ = DialogServices.SnackBarMsg("Erro DoNotifyAsync!\n" + ex.Message);
                //    //await chsResult.DisposeAsync(); // Assuming DisposeAsync is an asynchronous method
                //}
                //);


        if (sensorCharacteristic != null)
        {
            Debug.WriteLine("Characteristic discovered: " + sensorCharacteristic.Uuid.ToString());
            SensorCharacteristicStatus = "Listening...";
            OnPropertyChanged(nameof(SensorCharacteristicStatus));
        }

    }
    void SetReadValue(BleCharacteristicResult result, bool fromUtf8) => Application.Current.Dispatcher.Dispatch(() => //Device.BeginInvokeOnMainThread(() =>
    {
        var testeResult = fromUtf8
                ? Encoding.UTF8.GetString(result.Data, 0, result.Data.Length)
                : BitConverter.ToString(result.Data).Replace("-", " ");
    });


    async void OnDisconnect(object s, EventArgs e)
    {

        if (sensorCharacteristic != null)
        {
            if (sensorCharacteristic.IsNotifying)
            {
                SensorCharacteristicStatus = "Not listening";
                OnPropertyChanged(nameof(SensorCharacteristicStatus));

                chsResult?.Dispose();
                wscObserver?.Dispose();
                sensorCharacteristic = null;
                hasDeviceStatusSubscription = false;
            }
        }

        if (peripheral.Status == ConnectionState.Connected) //.IsConnected())
        {
            peripheral.CancelConnection();
            SensorConnectionStatus = "Disconnected";
            OnPropertyChanged(nameof(SensorConnectionStatus));

        }

    }

}


