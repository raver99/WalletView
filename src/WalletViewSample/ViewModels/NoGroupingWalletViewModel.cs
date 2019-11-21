using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace WalletViewSample.ViewModels
{
    public class NoGroupingWalletViewModel : ViewModelBase
    {
        private ObservableCollection<CardModel> cards;
        public ObservableCollection<CardModel> Cards
        {
            get { return cards; }
            set { Set(nameof(Cards), ref cards, value); }
        }

        private bool isGroupingEnabled;
        public bool IsGroupingEnabled
        {
            get { return isGroupingEnabled; }
            set { Set(nameof(IsGroupingEnabled), ref isGroupingEnabled, value); }
        }

        public string Key
        {
            get
            {
                return "TestHeader";
            }
        }

        public Command EditTappedCommand
        {
            get
            {
                return new Command(() =>
                {

                });
            }
        }

        public Command DeleteTappedCommand
        {
            get
            {
                return new Command((parameter) =>
                {
                    CardModel card = (CardModel)parameter;
                    deleteCard(card);
                });
            }
        }

        private void deleteCard(CardModel card)
        {
            if (Cards.Contains(card))
            {
                Cards.Remove(card);
            }
        }

        public Command ShowCouponsCommand
        {
            get
            {
                return new Command((parameter) =>
                {

                });
            }
        }

        public NoGroupingWalletViewModel()
        {
            var theCards = new ObservableCollection<CardModel>()
            {
                new CardModel()
                {
                    CardColor = Color.Fuchsia
                },
                new CardModel()
                {
                    CardColor = Color.Violet
                },
                new CardModel()
                {
                    CardColor = Color.Turquoise
                },
            };

            var coupons = new ObservableCollection<CardModel>()
            {
                new CardModel()
                {
                    CardColor = Color.Fuchsia
                },
                new CardModel()
                {
                    CardColor = Color.Violet
                },
                new CardModel()
                {
                    CardColor = Color.Turquoise
                },
            };

            var discounts = new ObservableCollection<CardModel>()
            {
                new CardModel()
                {
                    CardColor = Color.IndianRed
                },
                new CardModel()
                {
                    CardColor = Color.GreenYellow
                },
                new CardModel()
                {
                    CardColor = Color.RoyalBlue
                },
            };

            IsGroupingEnabled = false;

            //Cards = new ObservableCollection<CardModel>();

            Cards = discounts;
        }
    }
}
    