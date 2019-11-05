#  Xamarin Forms WalletView

WalletView is a control built in completely in Xamarin Forms that enables building a UI similar to the Apple Wallet App.

  ![Screenshot](images/wallet_ios.gif =300x) ![Screenshot](images/wallet_android.gif =300x)

# Getting started

- Clone the project and checkout the sample app
- Use the nuget package to install into an existing project on every platform
	- Nuget: <<LINK>>
    - Install into your PCL/.NET Standard project and Client projects.

**Additional iOS Setup**

- Add this line to your AppDelegate.cs after the Xamarin.Forms initialisation:
```
   Xamarin.Forms.iOS.WalletViewRenderer.Init() 
```

***Platform Support***

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

**Declaring an item template:**

**Declaring a header template:**

**Declaring a details template:**

# Limitations

- Because on Android, the native ScrollView does not support for overscroll, expanding and collapsing behavior of the card is not implemented.
- There is no support for virtualisation, though performance should not be a problem with a decent amount of cards

# License

The MIT License (MIT), see [LICENSE](LICENSE) file.