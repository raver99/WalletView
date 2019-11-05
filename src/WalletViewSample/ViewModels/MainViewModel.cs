using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using WalletView.Forms.Model;
using Xamarin.Forms;

namespace WalletViewSample.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<Grouping<string, CardModel>> cardsGroups;
        public ObservableCollection<Grouping<string, CardModel>> CardsGroups
        {
            get { return cardsGroups; }
            set { Set(nameof(CardsGroups), ref cardsGroups, value); }
        }

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

                    if (IsGroupingEnabled)
                    {
                        deleteCardFromGroup(card);
                    }
                    else
                    {
                        deleteCard(card);
                    }
                });
            }
        }

        private void deleteCard(CardModel card)
        {
            if(Cards.Contains(card))
            {
                Cards.Remove(card);
            }
        }

        private void deleteCardFromGroup(CardModel card)
        {
            var list = CardsGroups.ToList();
            foreach (var group in CardsGroups)
            {
                if (group.Contains(card))
                {
                    group.Remove(card);

                    break;
                }
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

        public MainViewModel()
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

            IsGroupingEnabled = true;

            CardsGroups = new ObservableCollection<Grouping<string, CardModel>>()
            {
                new Grouping<string, CardModel>("Cards", theCards),
                new Grouping<string, CardModel>("Discounts", discounts),
                new Grouping<string, CardModel>("Coupons", coupons),
            };

            Cards = discounts;
        }
    }

    public class CardModel
    {
        public Color CardColor { get; set; }
    }
}
