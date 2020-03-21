using SymbolT80ScannerTest.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SymbolT80ScannerTest
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<App, string>(this, "ScanBarcode", (sender, arg) => {
                txtBarCode.Text = arg;
            });
        }

        private void btnScan_Clicked(object sender, EventArgs e)
        {
            var scanner = DependencyService.Get<IScanner>();
            if (scanner != null)
            {
                scanner.Scan();
            }
        }
    }
}
