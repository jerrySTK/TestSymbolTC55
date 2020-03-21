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

namespace SymbolT80ScannerTest.Droid
{
    [BroadcastReceiver(Enabled = true)]
    public class ScanReceiver: BroadcastReceiver
    {
        
        public static string SOURCE_TAG = "com.symbolt55scannertest.data";
        
    
        // Intent Action for our operation
        public static string IntentAction = "scanner.RECVR";
        public static string IntentCategory = "android.intent.category.DEFAULT";

        public event EventHandler<String> scanDataReceived;

        public override void OnReceive(Context context, Intent i)
        {
            // check the intent action is for us  
            if (i.Action.Equals(IntentAction))
            {
                String data = i.GetStringExtra(SOURCE_TAG);
         
                if (scanDataReceived != null)
                {
                    scanDataReceived(this, data);
                }
            }
        }
    }
}