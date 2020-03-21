using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Symbol.XamarinEMDK;
using Xamarin.Forms;
using Android.Content;
using System.Xml;
using System.IO;

namespace SymbolT80ScannerTest.Droid
{
    [Activity(Label = "SymbolT80ScannerTest", Name = "com.companyname.symbolt80scannertest.MainActivity", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, EMDKManager.IEMDKListener
    {
        private EMDKManager mEmdkManager = null;
        private ProfileManager mProfileManager = null;
        private const String mProfileName = "Test";
        ScanReceiver _broadcastReceiver;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            EMDKResults result = EMDKManager.GetEMDKManager(Android.App.Application.Context, this);
            if (result.StatusCode != EMDKResults.STATUS_CODE.Success)
            {
                Toast.MakeText(this, "Error opening the EMDK Manager", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "EMDK Manager is available", ToastLength.Long).Show();
            }

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

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
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

        protected override void OnDestroy()
        {
            if (mProfileManager != null)
            {
                mProfileManager = null;
            }

            if (mEmdkManager != null)
            {
                mEmdkManager.Release();
                mEmdkManager = null;
            }

            base.OnDestroy();
        }

        public void OnClosed()
        {
            if (mEmdkManager != null)
            {
                mEmdkManager.Release();
                mEmdkManager = null;
            }
        }

        public void OnOpened(EMDKManager emdkManager)
        {
            
            mEmdkManager = emdkManager;
            String strStatus = "";
            String[] modifyData = new String[1];

            mProfileManager = (ProfileManager)mEmdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Profile);

            EMDKResults results = mProfileManager.ProcessProfile(mProfileName, ProfileManager.PROFILE_FLAG.Set, modifyData);

            if (results.StatusCode == EMDKResults.STATUS_CODE.Success)
            {
                strStatus = "Profile processed succesfully";
            }
            else if (results.StatusCode == EMDKResults.STATUS_CODE.CheckXml)
            {
                //Inspect the XML response to see if there are any errors, if not report success
                using (XmlReader reader = XmlReader.Create(new StringReader(results.StatusString)))
                {
                    String checkXmlStatus = "Status:\n\n";
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "parm-error":
                                        checkXmlStatus += "Parm Error:\n";
                                        checkXmlStatus += reader.GetAttribute("name") + " - ";
                                        checkXmlStatus += reader.GetAttribute("desc") + "\n\n";
                                        break;
                                    case "characteristic-error":
                                        checkXmlStatus += "characteristic Error:\n";
                                        checkXmlStatus += reader.GetAttribute("type") + " - ";
                                        checkXmlStatus += reader.GetAttribute("desc") + "\n\n";
                                        break;
                                }
                                break;
                        }
                    }
                    if (checkXmlStatus == "Status:\n\n")
                    {
                        strStatus = "Status: Profile applied successfully ...";
                    }
                    else
                    {
                        strStatus = checkXmlStatus;
                    }

                }
            }
            else
            {
                strStatus = "Something wrong on processing the profile";
            }

            Toast.MakeText(this, strStatus, ToastLength.Long).Show();
        }
    }
}