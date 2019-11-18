#  Xamarin Forms WalletView

WalletView is a control built in completely in Xamarin Forms that enables building a UI similar to the Apple Wallet App.

<table>
  <tr>
    <td><img src="https://github.com/raver99/WalletView/blob/master/images/wallet_ios.gif"></td>
    <td><img src="https://github.com/raver99/WalletView/blob/master/images/wallet_android.gif" alt="ios.gif"></td>
  </tr>
</table>

# Getting started

- Clone the project and checkout the sample app
- Use the nuget package to install into an existing project on every platform
	- [Nuget package](https://www.nuget.org/packages/Xamarin.Forms.Controls.WalletView/1.0.0)
    - Install into your PCL/.NET Standard project and Client projects.

**Platform Support**

| Plugin | README |
| ------ | ------ |
| Xamarin.iOS | 10.0 |
| Xamarin.Android | 5.0 |

# Documentation

The iOS Wallet App has been given a lot of thought in order to display and use cards efficiently.
The WalletView was created to help build a simmilar view. It supports the following functionality:
- Display a list of cards that are stacked on top of each other, revealing only the most important information.
- The list of cards supports grouping
- When selected the other cards are animated out of view and the selected card is highlighted, revealing more information and actions
- There are some subtle parallax animations when over scrolling either on top or bottom
	-   Overscroll top - cards expand
	-   Overscroll bottom - cards collapse (fold)
- The layout is flexible enough to support CRUD actions on cards
- The Control supports templates for:
	-   Card Layout
	- Header Layout
	- Details View for when the item is selected

# Usage

**Use it like this in your XAML:**

```xml
<local:WalletView x:Name="walletControl"
		  ItemsSource="{Binding CardsGroups}"
		  ItemHeight="200"
		  IsGroupingEnabled="true"
		  ContentMargin="30"
		  ItemSpacing="-170">
```

**Declaring an card item template:**

```xml
<local:WalletView.CardTemplate>
	<DataTemplate>
	    <BoxView CornerRadius="15"
		 Color="{Binding CardColor}"
		 HeightRequest="200"/>
	</DataTemplate>
</local:WalletView.CardTemplate>
```

**Declaring a header template:**

```xml
<local:WalletView.HeaderTemplate>
	<DataTemplate>
	    <Label Text="{Binding Key}" FontSize="24" FontAttributes="Bold" Margin="5,20,0,0"/>
	</DataTemplate>
</local:WalletView.HeaderTemplate>
```

**Declaring a details template:**

```xml
    <local:WalletView.DetailViewTemplate>
	<DataTemplate>
	    <StackLayout>
		<StackLayout Orientation="Horizontal">
		<Button Text="Edit" HorizontalOptions="EndAndExpand"
			Command="{Binding EditTappedCommand}"/>
		<Button Text="Delete" HorizontalOptions="StartAndExpand"
			Command="{Binding BindingContext.DeleteTappedCommand, Source={x:Reference page}}" CommandParameter="{Binding .}"/>
		</StackLayout>
	    </StackLayout>
	</DataTemplate>
    </local:WalletView.DetailViewTemplate>
```

# Limitations

- Because on Android, the native ScrollView does not support for overscroll, expanding and collapsing behavior of the card is not implemented.
- There is no support for virtualisation, though performance should not be a problem with a decent amount of cards

# License

The MIT License (MIT), see [LICENSE](LICENSE) file.
