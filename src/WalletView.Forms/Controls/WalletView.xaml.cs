using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WalletView.Forms.Controls
{
    public partial class WalletView : ContentView
    {
        #region Private

        private const double DefaultItemHeight = 200;
        private const double DefaultItemSpacing = -150;
        private const double DefaultScrollContentOffset = 0;
        private const double ExpandItemsOnScrollCoefficient = 0.06;
        private const double CollapseItemsOnScrollCoefficient = 0.7;
        private const double MaxTranslationOffset = 30;

        private const int DetailsTopDelta = 150;
        private const int DefaultAnimationLength = 250;
        private const int DetailsViewHideAnimationLength = 150;

        private const int OutOfViewTranslation = 3000;
        private const int OutOfViewAnimationDuration = 1000;
        private const int BackToUnselectedAnimationDuration = 700;
        private const int BackToUnselectedForCurrentItemAnimationDuration = 500;
        private const int SelectedItemAnimationDuration = 500;
        private const int RevealDetailsAnimationDelay = 300;

        private bool cardIsFocused;

        private bool transitioningToUnselected;

        private bool transitioningToSelected;

        private int focusedIndex;

        #endregion

        #region Lifecycle

        public WalletView()
        {
            InitializeComponent();

            ScrollView.Scrolled += ScrollView_Scrolled;

            ItemsContainerStack.Spacing = DefaultItemSpacing;
        }

        #endregion

        #region Public Properties

        public static readonly BindableProperty ItemHeightProperty = BindableProperty.Create(nameof(ItemHeight),
                          typeof(double),
                          typeof(WalletView),
                          DefaultItemHeight);

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly BindableProperty ItemSpacingProperty = BindableProperty.Create(nameof(ItemSpacing),
                            typeof(int),
                            typeof(WalletView),
                            default(int), propertyChanged: itemSpacingChanged);

        public int ItemSpacing
        {
            get { return (int)GetValue(ItemSpacingProperty); }
            set { SetValue(ItemSpacingProperty, value); }
        }

        private static void itemSpacingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            WalletView wallet = (WalletView)bindable;
            wallet.ItemsContainerStack.Spacing = (int)newValue;
        }

        public static readonly BindableProperty IsGroupingEnabledProperty = BindableProperty.Create(nameof(IsGroupingEnabled),
                          typeof(bool),
                          typeof(WalletView),
                          default(bool));

        public bool IsGroupingEnabled
        {
            get { return (bool)GetValue(IsGroupingEnabledProperty); }
            set { SetValue(IsGroupingEnabledProperty, value); }
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(ICollection), typeof(WalletView), new List<object>(), BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) => { ItemsChanged(bindable, (ICollection)oldValue, (ICollection)newValue); });

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty CardTemplateProperty = BindableProperty.Create(nameof(CardTemplate), typeof(DataTemplate), typeof(WalletView), default(DataTemplate));
        public DataTemplate CardTemplate
        {
            get => (DataTemplate)GetValue(CardTemplateProperty);
            set => SetValue(CardTemplateProperty, value);
        }

        public static readonly BindableProperty DetailViewTemplateProperty = BindableProperty.Create(nameof(DetailViewTemplate), typeof(DataTemplate), typeof(WalletView), default(DataTemplate));
        public DataTemplate DetailViewTemplate
        {
            get => (DataTemplate)GetValue(DetailViewTemplateProperty);
            set => SetValue(DetailViewTemplateProperty, value);
        }
        public static readonly BindableProperty HeaderTemplateProperty = BindableProperty.Create(nameof(HeaderTemplate), typeof(DataTemplate), typeof(WalletView), default(DataTemplate));
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly BindableProperty ItemTappedCommandProperty = BindableProperty.Create(nameof(ItemTappedCommand),
                          typeof(Command),
                          typeof(WalletView),
                          default(Command));

        public Command ItemTappedCommand
        {
            get { return (Command)GetValue(ItemTappedCommandProperty); }
            set { SetValue(ItemTappedCommandProperty, value); }
        }

        public static readonly BindableProperty ItemUnselectedCommandProperty = BindableProperty.Create(nameof(ItemUnselectedCommand),
                          typeof(Command),
                          typeof(WalletView),
                          default(Command));

        public Command ItemUnselectedCommand
        {
            get { return (Command)GetValue(ItemUnselectedCommandProperty); }
            set { SetValue(ItemUnselectedCommandProperty, value); }
        }

        public static readonly BindableProperty DoneButtonTextProperty = BindableProperty.Create(nameof(DoneButtonText),
                          typeof(string),
                          typeof(WalletView),
                          default(string));

        public string DoneButtonText
        {
            get { return (string)GetValue(DoneButtonTextProperty); }
            set { SetValue(DoneButtonTextProperty, value); }
        }

        public static readonly BindableProperty DoneButtonTextColorProperty = BindableProperty.Create(nameof(DoneButtonTextColor),
                          typeof(Color),
                          typeof(WalletView),
                          default(Color));

        public Color DoneButtonTextColor
        {
            get { return (Color)GetValue(DoneButtonTextColorProperty); }
            set { SetValue(DoneButtonTextColorProperty, value); }
        }

        public static readonly BindableProperty ContentMarginProperty = BindableProperty.Create(nameof(ContentMargin),
                          typeof(Thickness),
                          typeof(WalletView),
                          default(Thickness));

        public Thickness ContentMargin
        {
            get { return (Thickness)GetValue(ContentMarginProperty); }
            set { SetValue(ContentMarginProperty, value); }
        }

        #endregion

        #region Public Methods

        public async Task ScrollToItem(object item)
        {
            if (cardIsFocused)
            {
                await BackToUnselectedState();
            }

            var childView = findViewForItem(item);
            await ScrollView.ScrollToAsync(childView, ScrollToPosition.MakeVisible, true);
        }

        #endregion

        #region Event Handlers

        private static async Task ItemsChanged(BindableObject bindable, ICollection oldValue, ICollection newValue)
        {
            var walletControl = (WalletView)bindable;

            await walletControl.registerCollectionChangedHandlers(oldValue, newValue);
        }

        private async void GroupsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (cardIsFocused)
            {
                await BackToUnselectedState();
            }

            BuildItems();
        }

        private async void GroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    BuildItems();
                    break;

                case NotifyCollectionChangedAction.Move:

                    BuildItems();
                    break;

                case NotifyCollectionChangedAction.Remove:

                    if (cardIsFocused)
                    {
                        await BackToUnselectedState();
                    }

                    BuildItems();

                    break;

                case NotifyCollectionChangedAction.Replace:

                    BuildItems();
                    break;

                case NotifyCollectionChangedAction.Reset:

                    BuildItems();
                    break;
            }
        }

        private async void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var items = ItemsSource.Cast<object>().ToList();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    var index = e.NewStartingIndex;

                    foreach (var newItem in e.NewItems)
                    {
                        ItemsContainerStack.Children.Insert(index++, GetItemView(newItem));
                    }
                    break;

                case NotifyCollectionChangedAction.Move:

                    var moveItem = items[e.OldStartingIndex];

                    ItemsContainerStack.Children.RemoveAt(e.OldStartingIndex);
                    ItemsContainerStack.Children.Insert(e.NewStartingIndex, GetItemView(moveItem));
                    break;

                case NotifyCollectionChangedAction.Remove:

                    if (cardIsFocused)
                    {
                        await BackToUnselectedState();
                    }

                    ItemsContainerStack.Children.RemoveAt(e.OldStartingIndex + 1);
                    break;

                case NotifyCollectionChangedAction.Replace:

                    ItemsContainerStack.Children.RemoveAt(e.OldStartingIndex);
                    ItemsContainerStack.Children.Insert(e.NewStartingIndex, GetItemView(items[e.NewStartingIndex]));
                    break;

                case NotifyCollectionChangedAction.Reset:

                    BuildItems();
                    break;
            }
        }

        public async void ItemTapped(object sender, EventArgs args)
        {
            View itemView = (View)sender;

            if (ItemTappedCommand != null)
            {
                ItemTappedCommand.Execute(itemView);
            }

            focusedIndex = ItemsContainerStack.Children.IndexOf(itemView);

            if (focusedIndex < 0)
            {
                focusedIndex = ItemsContainerStack.Children.IndexOf((View)itemView.Parent);
            }

            for (int i = 0; i < ItemsContainerStack.Children.Count; i++)
            {
                var view = ItemsContainerStack.Children[i];

                if (i != focusedIndex)
                {
                    AnimateOtherChildrenOutOfView(view, i < focusedIndex);
                }
            }

            var translateToTopDistance = itemView.Y;

            itemView.TranslateTo(0, -translateToTopDistance, SelectedItemAnimationDuration, Easing.SpringOut);

            cardIsFocused = true;

            transitioningToSelected = true;

            await ScrollView.ScrollToAsync(0, 0, false);

            transitioningToSelected = false;

            await Task.Delay(RevealDetailsAnimationDelay);

            RevealDetailView(itemView.BindingContext);

            RevealDoneButton();
        }

        public async void DoneButton_Clicked(object sender, EventArgs args)
        {
            if (cardIsFocused)
            {
                await BackToUnselectedState();
            }
        }

        private async void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            Debug.WriteLine("Scrolled: " + e.ScrollY);

            if (transitioningToUnselected || transitioningToSelected)
            {
                return;
            }

            if (cardIsFocused)
            {
                await BackToUnselectedState();

                return;
            }

            if (e.ScrollY <= DefaultScrollContentOffset)
            {
                ExpandItemsOnScroll(e.ScrollY);
            }
            else
            {
                var bottomOverscrollLimit = ScrollView.ContentSize.Height - ScrollView.Height;
                if (e.ScrollY > bottomOverscrollLimit)
                {
                    var scrolled = bottomOverscrollLimit - e.ScrollY;
                    CollapseItemsOnScroll(scrolled);
                }
            }
        }

        #endregion

        #region Helper Methods

        private void BuildItems()
        {
            ContentGrid.Margin = ContentMargin;

            ItemsContainerStack.Children.Clear();

            if (DetailViewTemplate != null)
            {
                DetailsView.Content = GetDetailsView();
            }

            if (IsGroupingEnabled == true)
            {
                foreach (var grouping in ItemsSource)
                {
                    var group = grouping as IList;

                    if(group != null)
                    {
                        var headerView = GetHeaderView(group);

                        ItemsContainerStack.Children.Add(headerView);

                        foreach (object item in group)
                        {
                            ItemsContainerStack.Children.Add(GetItemView(item));
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No group found in bound collection, when group expected");
                    }
                }
            }
            else
            {
                ItemsContainerStack.Children.Add(GetHeaderView());

                foreach (object item in ItemsSource)
                {
                    var view = GetItemView(item);
                    ItemsContainerStack.Children.Add(view);
                }
            }
        }

        private View GetDetailsView()
        {
            var detailsView = DetailViewTemplate.CreateContent();
            if (!(detailsView is View view))
            {
                throw new Exception($"Templated control must be a View ({nameof(DetailViewTemplate)})");
            }

            view.BindingContext = BindingContext;

            return view;
        }

        private View GetHeaderView(object group = null)
        {
            var headerView = HeaderTemplate.CreateContent();
            if (!(headerView is View view))
            {
                throw new Exception($"Templated control must be a View ({nameof(HeaderTemplate)})");
            }

            if (IsGroupingEnabled)
            {
                view.BindingContext = group;
            }
            else
            {
                view.BindingContext = BindingContext;
            }

            var topMargin = ItemsContainerStack.Children.Count == 0 ? 0 : Math.Abs(ItemSpacing);

            var margin = new Thickness(view.Margin.Left, topMargin, view.Margin.Right, Math.Abs(ItemSpacing));
            view.Margin = margin;

            return view;
        }

        private View GetItemView(object item)
        {
            var contentView = CardTemplate.CreateContent();
            if (!(contentView is View view))
            {
                throw new Exception($"Templated control must be a View or a ViewCell ({nameof(CardTemplate)})");
            }

            view.BindingContext = item;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += ItemTapped;
            view.GestureRecognizers.Add(tapGestureRecognizer);

            return view;
        }

        private void RevealDoneButton()
        {
            DoneButton.Text = !string.IsNullOrEmpty(DoneButtonText) ? DoneButtonText : "Done";
            DoneButton.TextColor = DoneButtonTextColor;
            DoneButton.FadeTo(1, DefaultAnimationLength);
            DoneButton.IsEnabled = true;
        }

        private void HideDoneButton()
        {
            DoneButton.IsEnabled = false;
            DoneButton.FadeTo(0, DefaultAnimationLength);
        }

        private void RevealDetailView(object bindingContext)
        {
            DetailsView.TranslationY = ItemHeight + DetailsTopDelta;
            DetailsView.FadeTo(1, DefaultAnimationLength);
            DetailsView.IsEnabled = true;
            DetailsView.Content.BindingContext = bindingContext;
        }

        private void HideDetailsView()
        {
            DetailsView.IsEnabled = false;
            DetailsView.FadeTo(0, DetailsViewHideAnimationLength);
        }

        private void AnimateOtherChildrenOutOfView(View view, bool animateToTop)
        {
            var translation = animateToTop ? -OutOfViewTranslation : OutOfViewTranslation;

            view.TranslateTo(0, translation, OutOfViewAnimationDuration, Easing.CubicIn);
        }

        private void AnimateOtherChildrenBackToView(View view)
        {
            view.TranslateTo(0, 0, BackToUnselectedAnimationDuration, Easing.CubicOut);
        }

        private async Task BackToUnselectedState()
        {
            List<Task> transationTasks = new List<Task>();

            transitioningToUnselected = true;

            HideDetailsView();

            HideDoneButton();

            for (int i = 0; i < ItemsContainerStack.Children.Count; i++)
            {
                var view = ItemsContainerStack.Children[i];

                if (i != focusedIndex)
                {
                    AnimateOtherChildrenBackToView(view);
                }
                else
                {
                    if (ItemUnselectedCommand != null)
                    {
                        ItemUnselectedCommand.Execute(view);
                    }

                    Task<bool> translationTask = view.TranslateTo(0, 0, BackToUnselectedForCurrentItemAnimationDuration, Easing.CubicOut);
                    transationTasks.Add(translationTask);
                }
            }

            await Task.WhenAll(transationTasks);

            cardIsFocused = false;

            transitioningToUnselected = false;
        }

        private void ExpandItemsOnScroll(double scrollY)
        {
            for (int i = 0; i < ItemsContainerStack.Children.Count; i++)
            {
                var view = ItemsContainerStack.Children[i];

                view.TranslationY = -i * scrollY * ExpandItemsOnScrollCoefficient;
            }
        }

        private void CollapseItemsOnScroll(double scrolled)
        {
            for (int i = 0; i < ItemsContainerStack.Children.Count; i++)
            {
                var currentView = ItemsContainerStack.Children[i];

                var computedScrollTranslation = GetLockScrollPositionForViewAtIndex(i, scrolled);

                currentView.TranslationY = -computedScrollTranslation;
            }
        }

        private double GetLockScrollPositionForViewAtIndex(int index, double scrolled)
        {
            if (index == 0)
            {
                return scrolled;
            }

            var expectedMaxOffset = ((index - 1) * MaxTranslationOffset + ItemsContainerStack.Children.FirstOrDefault().Height) / (1 - CollapseItemsOnScrollCoefficient);

            if (Math.Abs(scrolled) >= Math.Abs(expectedMaxOffset))
            {
                var test = (expectedMaxOffset * CollapseItemsOnScrollCoefficient + (Math.Abs(scrolled) - expectedMaxOffset));

                return -test;
            }
            else
            {
                return CollapseItemsOnScrollCoefficient * scrolled;
            }
        }

        private View findViewForItem(object item)
        {
            foreach (var childView in ItemsContainerStack.Children)
            {
                if (childView.BindingContext == item)
                {
                    return childView;
                }
            }

            return null;
        }

        private async Task registerCollectionChangedHandlers(ICollection oldValue, ICollection newValue)
        {
            if (oldValue != null)
            {
                if (oldValue is INotifyCollectionChanged observable)
                {
                    if (IsGroupingEnabled)
                    {
                        observable.CollectionChanged -= GroupsChanged;

                        if (observable is ICollection groups)
                        {
                            foreach (var group in groups)
                            {
                                if (group is INotifyCollectionChanged observableGroup)
                                {
                                    observableGroup.CollectionChanged -= GroupCollectionChanged;
                                }
                            }
                        }
                    }
                    else
                    {
                        observable.CollectionChanged -= ItemsCollectionChanged;
                    }
                }

                ItemsContainerStack.Children.Clear();
            }

            if (newValue != null)
            {
                if (cardIsFocused)
                {
                    await BackToUnselectedState();
                }

                BuildItems();

                if (ItemsSource is INotifyCollectionChanged observable)
                {
                    if (IsGroupingEnabled)
                    {
                        observable.CollectionChanged += GroupsChanged;
                        if (observable is ICollection groups)
                        {
                            foreach (var group in groups)
                            {
                                if (group is INotifyCollectionChanged observableGroup)
                                {
                                    observableGroup.CollectionChanged += GroupCollectionChanged;
                                }
                            }
                        }
                    }
                    else
                    {
                        observable.CollectionChanged += ItemsCollectionChanged;
                    }
                }
            }
        }

        #endregion
    }
}
