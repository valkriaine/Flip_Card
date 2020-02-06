using FlipBear.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace FlipBear
{
    public sealed partial class CardPage : Page
    {
        public static Card selected;
        public static AppBarToggleButton editToggle;
        public static bool edite = false;
        public static bool showFront = true;
        public static Image image;
        public static StackPanel HeaderP;
        public static CommandBar HeaderB;
        public static ToggleSwitch fronttoggle;
        public static AppBarToggleButton togle;
        Compositor _compositor = Window.Current.Compositor;
        SpringVector3NaturalMotionAnimation _springAnimation;
        private bool playshowfront = true;

        public CardPage()
        {
            this.InitializeComponent();
            editToggle = editeMode;
            image = DeckImage;
            HeaderP = HeaderPanel;
            HeaderB = HeaderBar;
            fronttoggle = fronttoggleswitch;
            togle = togleb;
        }
        //check
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DeckImage.Source = new BitmapImage(new Uri(BaseUri, App.viewModel.selectedDeck.Image));
            DeckName.Text = App.viewModel.selectedDeck.Name;
            MasterToggle.IsChecked = App.viewModel.selectedDeck.Mastered;
            MarkToggle.IsChecked = App.viewModel.selectedDeck.Marked;
            PlayGrid.Visibility = Visibility.Collapsed;
            edite = false;
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("CoverImage");
            //setting default card view to front
            if (animation != null)
            {
                animation.TryStart(DeckImage);
            }
            DetailFrame.Navigate(typeof(CardGridPage));
            CardGridPage.front.ItemsSource = App.viewModel.cards;
            CardGridPage.back.ItemsSource = App.viewModel.cards;
            CardGridPage.front.Visibility = Visibility.Visible;
            CardGridPage.back.Visibility = Visibility.Collapsed;
            CardGridPage.front.IsMultiSelectCheckBoxEnabled = false;
            CardGridPage.back.IsMultiSelectCheckBoxEnabled = false;
            CardGridPage.front.SelectionMode = ListViewSelectionMode.None;
            CardGridPage.back.SelectionMode = ListViewSelectionMode.None;
            CardGridPage.front.IsItemClickEnabled = true;
            CardGridPage.back.IsItemClickEnabled = true;
        }
        //check
        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.viewModel.selectedDeck.Number = App.viewModel.cards.Count;
            App.viewModel.updateSelected();
            fronttoggle.Toggled -= Ts_Toggled;
            selected = null;
            if (edite == true)
                edit();
        }
        //check
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DetailFrame.CanGoBack)
            {
                if (DetailedCardPage.front.Visibility == Visibility.Visible)
                {
                    await DetailedCardPage.SaveCard(true);

                }
                if (DetailedCardPage.back.Visibility == Visibility.Visible)
                {
                    await DetailedCardPage.SaveCard(false);
                }
                DetailFrame.GoBack();
            }
            else
            {
                await exit();
            }
        }
        //check
        private async Task exit()
        {
            App.viewModel.updateCard(selected);
            playshowfront = true;
            this.Frame.GoBack();
        }
        //check
        private async void DeckImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await exit();
        }
        //check
        private void Master_Click(object sender, RoutedEventArgs e)
        {

            App.viewModel.selectedDeck.Mastered = !App.viewModel.selectedDeck.Mastered;
            if (App.viewModel.selectedDeck.Mastered == true)
            {
                App.viewModel.selectedDeck.MasterColor = "#00BFFF";
            }
            else
            {
                App.viewModel.selectedDeck.MasterColor = "Transparent";
            }
            App.viewModel.updateSelected();
        }
        //check
        private void Mark_Click(object sender, RoutedEventArgs e)
        {

            App.viewModel.selectedDeck.Marked = !App.viewModel.selectedDeck.Marked;
            if (App.viewModel.selectedDeck.Marked == true)
            {
                App.viewModel.selectedDeck.MarkColor = "#FFD700";
            }
            else
            {
                App.viewModel.selectedDeck.MarkColor = "Transparent";
            }
            App.viewModel.updateSelected();
        }
        //check
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            App.viewModel.addCard(new Card());
            App.viewModel.updateSelected();
        }
        //check
        private void editeMode_Click(object sender, RoutedEventArgs e)
        {
            edit();
        }

        private void edit()
        {
            edite = !edite;
            if (edite == true)
            {
                CardGridPage.front.SelectionMode = ListViewSelectionMode.Multiple;
                CardGridPage.back.SelectionMode = ListViewSelectionMode.Multiple;
                CardGridPage.front.IsMultiSelectCheckBoxEnabled = true;
                CardGridPage.back.IsMultiSelectCheckBoxEnabled = true;
                CardGridPage.front.IsItemClickEnabled = false;
                CardGridPage.back.IsItemClickEnabled = false;
            }
            else
            {
                CardGridPage.front.SelectionMode = ListViewSelectionMode.None;
                CardGridPage.back.SelectionMode = ListViewSelectionMode.None;
                CardGridPage.front.IsMultiSelectCheckBoxEnabled = false;
                CardGridPage.back.IsMultiSelectCheckBoxEnabled = false;
                CardGridPage.front.IsItemClickEnabled = true;
                CardGridPage.back.IsItemClickEnabled = true;
            }
        }
        //check
        private void fronttoggle_Loaded(object sender, RoutedEventArgs e)
        {
            fronttoggle.IsOn = showFront;
            fronttoggle.Toggled += Ts_Toggled;
        }

        public static async void Ts_Toggled(object sender, RoutedEventArgs e)
        {
            showFront = !showFront;
            if (showFront)
            {
                CardGridPage.front.Visibility = Visibility.Visible;
                CardGridPage.back.Visibility = Visibility.Collapsed;
            }
            else
            {
                CardGridPage.front.Visibility = Visibility.Collapsed;
                CardGridPage.back.Visibility = Visibility.Visible;
            }
        }
        //check
        private void play(object sender, RoutedEventArgs e)
        {
            PlayGrid.Visibility = Visibility.Visible;
            cardpage.Background = new SolidColorBrush(Colors.Black);
            PlayList.ItemsSource = App.viewModel.cards.ToList();
            playbutton.Flyout.Hide();
            playbutton2.Flyout.Hide();
        }
        //check
        private void ExitPlay(object sender, TappedRoutedEventArgs e)
        {
            cardpage.Background = Application.Current.Resources["SystemControlAcrylicWindowBrush"] as AcrylicBrush;
            PlayGrid.Visibility = Visibility.Collapsed;
            PlayList.ItemsSource = null;
            playshowfront = true;
            playtoggle.IsChecked = !playshowfront;
        }
        //check
        private void ExitPlayButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CreateOrUpdateSpringAnimation(1.5f);
            line1.StartAnimation(_springAnimation);
            line2.StartAnimation(_springAnimation);
        }
        //check
        private void ExitPlayButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CreateOrUpdateSpringAnimation(1.0f);
            line1.StartAnimation(_springAnimation);
            line2.StartAnimation(_springAnimation);
        }
        //check
        private void CreateOrUpdateSpringAnimation(float finalValue)
        {
            if (_springAnimation == null)
            {
                _springAnimation = _compositor.CreateSpringVector3Animation();
                _springAnimation.Target = "Scale";
            }
            _springAnimation.FinalValue = new System.Numerics.Vector3(finalValue);
        }
        //check
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid g = sender as Grid;
            Grid fg = FindChild<Grid>(g, "fgrid");
            Grid bg = FindChild<Grid>(g, "bgrid");
            Storyboard fs = g.Resources["flipfront"] as Storyboard;
            Storyboard bs = g.Resources["flipback"] as Storyboard;
            if (fg.Visibility == Visibility.Visible)
            {
                fg.Visibility = Visibility.Collapsed;
                fs.Begin();
                bg.Visibility = Visibility.Visible;

            }
            else
            {
                fg.Visibility = Visibility.Visible;
                bs.Begin();
                bg.Visibility = Visibility.Collapsed;
            }
        }
        //check
        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        //check
        private void bgrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid g = sender as Grid;
            if (playshowfront == true)
            {
                g.Visibility = Visibility.Collapsed;
            }
            else
            {
                g.Visibility = Visibility.Visible;
            }
        }
        //check
        private void fgrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid g = sender as Grid;
            if (playshowfront == true)
            {
                g.Visibility = Visibility.Visible;
            }
            else
            {
                g.Visibility = Visibility.Collapsed;
            }
        }
        //check
        private void showbackfronttoggle_Click(object sender, RoutedEventArgs e)
        {
            playshowfront = !playshowfront;
            playtoggle.IsChecked = !playshowfront;
        }
        //check
        private void shuffleplay(object sender, RoutedEventArgs e)
        {
            PlayGrid.Visibility = Visibility.Visible;
            cardpage.Background = new SolidColorBrush(Colors.Black);
            List<Card> cardlist = new List<Card>(App.viewModel.cards);
            var shuffled = cardlist.OrderBy(item => Guid.NewGuid());
            PlayList.ItemsSource = shuffled;
            playbutton.Flyout.Hide();
            playbutton2.Flyout.Hide();
        }

        private void editeMode2_Click(object sender, RoutedEventArgs e)
        {
            DetailedCardPage.edit();
        }

        private void togglebackfront(object sender, RoutedEventArgs e)
        {
            Ts_Toggled(sender, e);
            if (showFront)
            {
                togle.Content = "Showing front";
                togle.IsChecked = true;
                DetailedCardPage.FrontGrid.Visibility = Visibility.Visible;
                DetailedCardPage.BackGrid.Visibility = Visibility.Collapsed;
                if (DetailedCardPage.front.Visibility == Visibility.Collapsed)
                {
                    DetailedCardPage.Flip();
                }
            }
            else
            {
                togle.Content = "SHowing back";
                togle.IsChecked = false;
                DetailedCardPage.FrontGrid.Visibility = Visibility.Collapsed;
                DetailedCardPage.BackGrid.Visibility = Visibility.Visible;
                if (DetailedCardPage.back.Visibility == Visibility.Collapsed)
                {
                    DetailedCardPage.Flip();
                }
            }
            
        }

        private void AppBarToggleButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (showFront)
            {
                togle.Content = "Showing front";
                togle.IsChecked = true;
            }
            else
            {
                togle.Content = "Showing back";
                togle.IsChecked = false;
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Here I'm calculating the number of columns I want based on
            // the width of the page
           ((ItemsWrapGrid)PlayList.ItemsPanelRoot).ItemWidth = PlayGrid.ActualWidth * 3/4;
            ((ItemsWrapGrid)PlayList.ItemsPanelRoot).ItemHeight = ((ItemsWrapGrid)PlayList.ItemsPanelRoot).ItemWidth * 3 / 4;
        }
    }
}
