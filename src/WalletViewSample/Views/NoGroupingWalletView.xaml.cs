using System;
using System.Collections.Generic;
using WalletViewSample.ViewModels;
using Xamarin.Forms;

namespace WalletViewSample.Views
{
    public partial class NoGroupingWalletView : ContentPage
    {
        public NoGroupingWalletView()
        {
            InitializeComponent();

            BindingContext = new NoGroupingWalletViewModel();
        }
    }
}
