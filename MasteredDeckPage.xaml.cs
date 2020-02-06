using FlipBear.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FlipBear
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MasteredDeckPage : Page
    {
        private bool edite;
        private ViewModel viewModel;
        public MasteredDeckPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (edite == true)
                edit();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            viewModel = App.viewModel;
            gridList.IsMultiSelectCheckBoxEnabled = false;
            gridList.SelectionMode = ListViewSelectionMode.None;
            gridList.IsItemClickEnabled = true;
            edite = false;
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            ADD("New Deck");
        }

        private async void ADD(string message)
        {
            string name = await InputTextDialogAsync(message).ConfigureAwait(true);
            if (String.IsNullOrEmpty(name))
            {
                //do nothing
            }
            else
            {
                if (!App.viewModel.checkName(name))
                {
                    ADD("Name already exists");
                }
                else
                    App.viewModel.addMastered(name);
            }
        }

        private static async Task<string> InputTextDialogAsync(string title)
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Confirm";
            dialog.SecondaryButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
            {
                return "";
            }

        }

        private async void delete_Click(object sender, RoutedEventArgs e)
        {
            DateTime delete = DateTime.Now;
            var i = sender as MenuFlyoutItem;
            Deck item = i.DataContext as Deck;
            if (!edite)
            {
                await App.viewModel.deleteDeck(item, delete).ConfigureAwait(false);
            }
            else
            {
                Deck k;
                while (gridList.SelectedItems.Count != 0)
                {
                    foreach (var selectedItem in gridList.SelectedItems)
                    {
                        k = selectedItem as Deck;
                        await App.viewModel.deleteDeck(k, delete).ConfigureAwait(true);
                    }
                }
            }
        }

        private void editeMode(object sender, RoutedEventArgs e)
        {
            edit();
        }

        private async void rename_Click(object sender, RoutedEventArgs e)
        {
            if (edite == true)
                edit();

            var i = sender as MenuFlyoutItem;
            Deck item = i.DataContext as Deck;
            RENAME(item, "Rename");
        }

        private async void RENAME(Deck deck, string message)
        {
            string name = await InputTextDialogAsync(message).ConfigureAwait(true);
            if (String.IsNullOrEmpty(name))
            {
                //do nothing
            }
            else
            {
                if (!App.viewModel.checkName(name))
                {
                    RENAME(deck, "Name already exists in library");
                }
                else
                    App.viewModel.RenameDeck(deck, name);
            }
        }

        private void edit()
        {
            edite = !edite;
            gridList.IsMultiSelectCheckBoxEnabled = !gridList.IsMultiSelectCheckBoxEnabled;
            if (edite == true)
            {
                App.viewModel.HideButton();
                EditeBar.Background = new SolidColorBrush(Color.FromArgb(169, 169, 169, 169));
                gridList.SelectionMode = ListViewSelectionMode.Multiple;
                gridList.IsItemClickEnabled = false;
            }
            else
            {
                App.viewModel.ShowButton();
                EditeBar.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                gridList.SelectionMode = ListViewSelectionMode.None;
                gridList.IsItemClickEnabled = true;
            }
        }

        private async void Mastered_Click(object sender, RoutedEventArgs e)
        {
            if (edite == false)
            {
                var i = sender as ToggleMenuFlyoutItem;
                Deck item = i.DataContext as Deck;
                if (item.Mastered == true)
                    await App.viewModel.unMasterDeck(item).ConfigureAwait(false);
                else
                    await App.viewModel.MasterDeck(item).ConfigureAwait(false);
            }
            else
            {
                IEnumerable<Deck> l = gridList.SelectedItems.Cast<Deck>();
                Deck s = gridList.SelectedItem as Deck;
                Deck k;
                if (s.Mastered == true)
                {

                    while (l.Any(x => x.Mastered == true))
                    {
                        foreach (var selectedItem in l)
                        {
                            k = selectedItem as Deck;
                            if (k.Mastered == true)
                                await App.viewModel.unMasterDeck(k).ConfigureAwait(true);
                        }
                    }
                }
                else
                {
                    while (l.Any(x => x.Mastered == false))
                    {
                        foreach (var selectedItem in l)
                        {
                            k = selectedItem as Deck;
                            if (k.Mastered == false)
                                await App.viewModel.MasterDeck(k).ConfigureAwait(true);
                        }
                    }
                }
            }
        }

        private async void BookMarked_Click(object sender, RoutedEventArgs e)
        {
            if (edite == false)
            {
                var i = sender as ToggleMenuFlyoutItem;
                Deck item = i.DataContext as Deck;
                if (item.Marked == true)
                    await App.viewModel.unMarkDeck(item).ConfigureAwait(false);
                else
                    await App.viewModel.MarkDeck(item).ConfigureAwait(false);
            }
            else
            {
                IEnumerable<Deck> l = gridList.SelectedItems.Cast<Deck>();
                Deck s = gridList.SelectedItem as Deck;
                Deck k;
                if (s.Marked == true)
                {

                    while (l.Any(x => x.Marked == true))
                    {
                        foreach (var selectedItem in l)
                        {
                            k = selectedItem as Deck;
                            if (k.Marked == true)
                                await App.viewModel.unMarkDeck(k).ConfigureAwait(true);
                        }
                    }
                }
                else
                {
                    while (l.Any(x => x.Marked == false))
                    {
                        foreach (var selectedItem in l)
                        {
                            k = selectedItem as Deck;
                            if (k.Marked == false)
                                await App.viewModel.MarkDeck(k).ConfigureAwait(true);
                        }
                    }
                }
            }
        }

        private void EditFromFlyout(object sender, RoutedEventArgs e)
        {
            edit();
            var i = sender as MenuFlyoutItem;
            Object item = i.DataContext;
            gridList.SelectedItems.Add(item);
        }

        private void gridList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //open deck
            Deck clicked = e.ClickedItem as Deck;
            App.viewModel.selectDeck(clicked);
            gridList.PrepareConnectedAnimation("CoverImage", clicked, "Cover");
            Frame mainFrame = Window.Current.Content as Frame;
            mainFrame.Navigate(typeof(CardPage));
        }

    }
}
