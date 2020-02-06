using FlipBear.Classes;
using System;
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
    public sealed partial class DeletedDeckPage : Page
    {
        private bool edite = false;
        private ViewModel viewModel = App.viewModel;
        public DeletedDeckPage()
        {
            InitializeComponent();
            gridList.IsMultiSelectCheckBoxEnabled = false;
            gridList.SelectionMode = ListViewSelectionMode.None;
            gridList.IsItemClickEnabled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (edite == true)
                edit();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var i = sender as MenuFlyoutItem;
            Deck item = i.DataContext as Deck;
            if (!edite)
            {

                App.viewModel.RemoveDeck(item);
            }
            else
            {
                foreach (var selectedItem in gridList.SelectedItems)
                {
                    App.viewModel.RemoveDeck(selectedItem as Deck);
                }
                App.viewModel.RemoveDeck(gridList.SelectedItem as Deck);
            }
        }

        private void editeMode(object sender, RoutedEventArgs e)
        {
            edit();
        }

        private void restore_Click(object sender, RoutedEventArgs e)
        {
            var i = sender as MenuFlyoutItem;
            Deck item = i.DataContext as Deck;
            if (!edite)
            {

                App.viewModel.restore(item);
            }
            else
            {
                while (gridList.SelectedItems.Count != 0)
                {
                    foreach (var selectedItem in gridList.SelectedItems)
                    {
                        App.viewModel.restore(selectedItem as Deck);
                    }
                }
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

        private void EditFromFlyout(object sender, RoutedEventArgs e)
        {
            edit();
            var i = sender as MenuFlyoutItem;
            Object item = i.DataContext;
            gridList.SelectedItems.Add(item);
        }

        private void deleteAll_Click(object sender, RoutedEventArgs e)
        {
            viewModel.deleteAll();
        }

        private void restoreAll_Click(object sender, RoutedEventArgs e)
        {
            viewModel.restoreAll();
        }

    }
}
