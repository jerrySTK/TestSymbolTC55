using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SymbolT80ScannerTest.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(SymbolT80ScannerTest.Droid.ScannerService))]
namespace SymbolT80ScannerTest.Droid
{
    public class ScannerService: IScanner
    {
        private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
        private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
        private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";

        public void Scan()
        {
            var intent = new Intent();
            Context current = Android.App.Application.Context;
            intent.SetAction(ACTION_SOFTSCANTRIGGER);
            intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
            
            current.SendBroadcast(intent);
        }
    }
}