using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletViewSample.ViewModels;
using WalletViewSample.Views;
using Xamarin.Forms;

namespace WalletViewSample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void WalletViewGrouping_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new GroupingWalletView());
        }

        public void WalletViewNoGrouping_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NoGroupingWalletView());
        }
    }
}
