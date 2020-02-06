using FlipBear.Classes;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Services.Store;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FlipBear
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Deck[] searchList;
        public MainPage()
        {
            InitializeComponent();

        }

        private void nav_Loaded(object sender, RoutedEventArgs e)
        {
            nav.SelectedItem = All;
            LibraryFrame.Navigate(typeof(AllDeckPage));
        }

        private void nav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (!String.IsNullOrEmpty(args.InvokedItem.ToString()))
            {
                switch (args.InvokedItem.ToString())
                {
                    case "All Decks":
                        if (LibraryFrame.Content.GetType() != typeof(AllDeckPage))
                            LibraryFrame.Navigate(typeof(AllDeckPage));
                        break;

                    case "Mastered":
                        if (LibraryFrame.Content.GetType() != typeof(MasteredDeckPage))
                            LibraryFrame.Navigate(typeof(MasteredDeckPage));
                        break;

                    case "Book Marked":
                        if (LibraryFrame.Content.GetType() != typeof(MarkedDeckPage))
                            LibraryFrame.Navigate(typeof(MarkedDeckPage));
                        break;

                    case "Recycle Bin":
                        if (LibraryFrame.Content.GetType() != typeof(DeletedDeckPage))
                            LibraryFrame.Navigate(typeof(DeletedDeckPage));
                        break;
                }
            }
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                searchList = new Deck[App.viewModel.allDecks.Count];
                App.viewModel.allDecks.CopyTo(searchList, 0);
                searchList = searchList.Skip(1).ToArray();
                var q = searchList.Where(X => X.Name.Contains(sender.Text));
                sender.ItemsSource = q;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
            }
            else
            {
                searchList = new Deck[App.viewModel.allDecks.Count];
                App.viewModel.allDecks.CopyTo(searchList, 0);
                searchList = searchList.Skip(1).ToArray();
                var q = searchList.Where(X => X.Name.Contains(args.QueryText));
                sender.ItemsSource = q;
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Deck selected = args.SelectedItem as Deck;
            string name = selected.Name;
            App.viewModel.selectDeck(selected);
            sender.Text = name;
            Frame mainFrame = Window.Current.Content as Frame;
            mainFrame.Navigate(typeof(CardPage));
        }

        private void InAppPurchase_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }
    }
}
