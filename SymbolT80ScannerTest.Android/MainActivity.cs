using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using Xamarin.Forms;
using Android.Content;
using System.Xml;
using System.IO;

namespace SymbolT80ScannerTest.Droid
{
    [Activity(Label = "SymbolT80ScannerTest", Name = "com.companyname.symbolt80scannertest.MainActivity", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {


        BarcodeScannerManager _scannerManager;
        ScanReceiver _broadcastReceiver;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            _scannerManager = new BarcodeScannerManager(Android.App.Application.Context);
            _scannerManager.ScanReceived += OnScanReceived;
            

            _broadcastReceiver = new ScanReceiver();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var my_application = new App();

            _broadcastReceiver.scanDataReceived += (s, scanData) =>
            {
                MessagingCenter.Send<App, string>(my_application, "ScanBarcode", scanData);

            };

            
            
            LoadApplication(my_application);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void OnScanReceived(object snder, Scanner.DataEventArgs args)
        {
            var data = args.P0;
            if(data?.Result == ScannerResults.Success)
            {
                Intent intent = new Intent(ScanReceiver.IntentAction);
                intent.PutExtra(ScanReceiver.SOURCE_TAG,data.GetScanData()[0].Data);
                SendBroadcast(intent);
            }
            
        }


        protected override void OnPause()
        {
            base.OnPause();
            if (_broadcastReceiver != null)
                Android.App.Application.Context.UnregisterReceiver(this._broadcastReceiver);
        }

        protected override void OnResume()
        {
            base.OnResume();
            IntentFilter filter = new IntentFilter(ScanReceiver.IntentAction);
            filter.AddCategory(ScanReceiver.IntentCategory);
            Android.App.Application.Context.RegisterReceiver(_broadcastReceiver, filter);
        }

       
     

     
    }
}