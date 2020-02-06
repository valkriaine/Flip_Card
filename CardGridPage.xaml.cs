using FlipBear.Classes;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FlipBear
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CardGridPage : Page
    {
        private ImplicitAnimationCollection _implicitAnimations;
        public static GridView front;
        public static GridView back;
        public CardGridPage()
        {
            this.InitializeComponent();
            front = FrontGrid;
            back = BackGrid;
        }

        //invoked when clicking on a card
        private async void cardsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            CardPage.selected = e.ClickedItem as Card;
            PrepareAnimationWithItem(e.ClickedItem as Card);
            CardPage.image.Visibility = Visibility.Collapsed;
            CardPage.HeaderP.Visibility = Visibility.Collapsed;
            CardPage.HeaderB.Visibility = Visibility.Visible;
            CardPage.fronttoggle.Toggled -= CardPage.Ts_Toggled;
            Frame.Navigate(typeof(DetailedCardPage));
        }
        //check
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var i = sender as MenuFlyoutItem;
            Card item = i.DataContext as Card;
            if (CardPage.edite == false)
            {
                if (item == front.SelectedItem || item == back.SelectedItem)
                {
                    CardPage.selected = null;
                }
                await App.viewModel.deleteCard(item).ConfigureAwait(false);
            }
            else
            {
                Card k;
                if (front.Visibility == Visibility.Visible)
                {
                    while (front.SelectedItems.Count != 0)
                    {
                        foreach (var selectedItem in front.SelectedItems)
                        {
                            k = selectedItem as Card;
                            await App.viewModel.deleteCard(k).ConfigureAwait(true);
                        }
                    }
                }
                else if (front.Visibility == Visibility.Collapsed)
                {
                    while (back.SelectedItems.Count != 0)
                    {
                        foreach (var selectedItem in back.SelectedItems)
                        {
                            k = selectedItem as Card;
                            await App.viewModel.deleteCard(k).ConfigureAwait(true);
                        }
                    }
                }
            }
        }
        //check
        private void cardGrid_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (ApiInformation.IsTypePresent(typeof(ImplicitAnimationCollection).FullName))
            {
                var elementVisual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
                if (args.InRecycleQueue)
                {
                    elementVisual.ImplicitAnimations = null;
                }
                else
                {
                    EnsureImplicitAnimations();
                    elementVisual.ImplicitAnimations = _implicitAnimations;
                }
            }
        }
        //check
        private void EnsureImplicitAnimations()
        {
            if (_implicitAnimations == null)

            {
                var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
                offsetAnimation.Target = nameof(Visual.Offset);
                offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
                offsetAnimation.Duration = TimeSpan.FromMilliseconds(400);
                var animationGroup = compositor.CreateAnimationGroup();
                animationGroup.Add(offsetAnimation);
                _implicitAnimations = compositor.CreateImplicitAnimationCollection();
                _implicitAnimations[nameof(Visual.Offset)] = animationGroup;
            }
        }

        private void select_Click(object sender, RoutedEventArgs e)
        {
            var i = sender as MenuFlyoutItem;
            Card item = i.DataContext as Card;
            if (CardPage.edite == false)
            {
                CardPage.edite = true;
                CardPage.editToggle.IsChecked = true;
                front.IsMultiSelectCheckBoxEnabled = true;
                back.IsMultiSelectCheckBoxEnabled = true;
                front.SelectionMode = ListViewSelectionMode.Multiple;
                back.SelectionMode = ListViewSelectionMode.Multiple;
                front.IsItemClickEnabled = false;
                back.IsItemClickEnabled = false;
            }
            front.SelectedItems.Add(item);
            back.SelectedItems.Add(item);
        }

        void PrepareAnimationWithItem(Card item)
        {
            if (front.Visibility == Visibility.Visible)
            {
                front.PrepareConnectedAnimation("OpenCard", item, "item");
            }
        }
    }
}

