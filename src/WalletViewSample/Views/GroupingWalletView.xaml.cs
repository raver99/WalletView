using System;
using System.Collections.Generic;
using WalletViewSample.ViewModels;
using Xamarin.Forms;

namespace WalletViewSample.Views
{
    public partial class GroupingWalletView : ContentPage
    {
        public GroupingWalletView()
        {
            InitializeComponent();

            BindingContext = new GroupingWalletViewModel();
        }
    }
}
