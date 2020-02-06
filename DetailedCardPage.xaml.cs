using FlipBear.Classes;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace FlipBear
{
    public sealed partial class DetailedCardPage : Page
    {
        private ImplicitAnimationCollection _implicitAnimations;
        public static GridView FrontGrid;
        public static GridView BackGrid;
        public static ScrollViewer front;
        public static ScrollViewer back;
        public static TextBlock fb;
        public static InkToolbar frontbar;
        public static InkToolbar backbar;
        public static TextBox FrontText;
        public static TextBox BackText;
        public static InkCanvas FrontInk;
        public static InkCanvas BackInk;
        public static Storyboard showfront;
        public static Storyboard showback;
        public static DropDownButton fontsize;
        public static Grid FrontRESIZE;
        public static Grid BackRESIZE;


        public DetailedCardPage()
        {
            this.InitializeComponent();
            FrontGrid = frontGrid;
            BackGrid = backGrid;
            front = frontgrid;
            back = backgrid;
            fb = fbindicator;
            frontbar = FrontInkBar;
            backbar = BackInkBar;
            FrontText = FrontTextBox;
            BackText = BackTextBox;
            FrontInk = FrontInkCanvas;
            BackInk = BackInkCanvas;
            showfront = showfrontstory;
            showback = showbackstory;
            fontsize = fontsizedrop;
            FrontRESIZE = frontresize;
            BackRESIZE = backresize;
        }
        //check
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //PenInputToggle.IsChecked = false;
            //FrontInk.IsHitTestVisible = false;
            //FrontText.IsHitTestVisible = true;
            //BackText.IsHitTestVisible = true;
            //BackInk.IsHitTestVisible = false;
            //InputMethod.Visibility = Visibility.Collapsed;
            Inputindicator.Symbol = Symbol.Edit;
            FrontInk.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen;
            BackInk.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen;
            if (CardPage.showFront)
            {
                CardPage.togle.Content = "Showing front";
                CardPage.togle.IsChecked = true;
            }
            else
            {
                CardPage.togle.Content = "Showing back";
                CardPage.togle.IsChecked = false;
            }
            FrontGrid.ItemsSource = App.viewModel.cards;
            BackGrid.ItemsSource = App.viewModel.cards;
            resetVisibility();
            FrontInk.InkPresenter.StrokeContainer.Clear();
            BackInk.InkPresenter.StrokeContainer.Clear();
            await getCard(true);
            await getCard(false);
            await getInk(true);
            await getInk(false);
            FrontRESIZE.Background = new SolidColorBrush(CardPage.selected.Background);
            BackRESIZE.Background = new SolidColorBrush(CardPage.selected.Background);
            FrontText.FontSize = CardPage.selected.FrontFontSize;
            BackText.FontSize = CardPage.selected.BackFontSize;
            colorindicator.Fill = new SolidColorBrush(CardPage.selected.Background);
            FrontText.Foreground = new SolidColorBrush(CardPage.selected.TextColor);
            BackText.Foreground = new SolidColorBrush(CardPage.selected.TextColor);

            FrontGrid.IsMultiSelectCheckBoxEnabled = false;
            BackGrid.IsMultiSelectCheckBoxEnabled = false;
            FrontGrid.SelectionMode = ListViewSelectionMode.Single;
            BackGrid.SelectionMode = ListViewSelectionMode.Single;
            FrontGrid.IsItemClickEnabled = true;
            BackGrid.IsItemClickEnabled = true;
            FrontGrid.SelectedItem = CardPage.selected;
            BackGrid.SelectedItem = CardPage.selected;
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("OpenCard");
            if (CardPage.showFront)
            {
                front.Visibility = Visibility.Visible;
                back.Visibility = Visibility.Collapsed;
            }
            else
            {
                front.Visibility = Visibility.Collapsed;
                back.Visibility = Visibility.Visible;
            }

            if (animation != null)
            {
                animation.TryStart(RootGrid);
            }
            if (front.Visibility == Visibility.Visible)
            {
                fb.Text = "Front";
                FrontInkBar.Visibility = Visibility.Visible;
                BackInkBar.Visibility = Visibility.Collapsed;
                fontsize.Content = "" + CardPage.selected.FrontFontSize;
            }
            else if (back.Visibility == Visibility.Visible)
            {
                fb.Text = "Back";
                BackInkBar.Visibility = Visibility.Visible;
                FrontInkBar.Visibility = Visibility.Collapsed;
                fontsize.Content = "" + CardPage.selected.BackFontSize;
            }

            var i = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                FrontGrid.ScrollIntoView(FrontGrid.SelectedItem, ScrollIntoViewAlignment.Leading);
                BackGrid.ScrollIntoView(BackGrid.SelectedItem, ScrollIntoViewAlignment.Leading);
            });


        }
        //check
        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            CardPage.image.Visibility = Visibility.Visible;
            CardPage.HeaderP.Visibility = Visibility.Visible;
            CardPage.HeaderB.Visibility = Visibility.Collapsed;
            CardPage.fronttoggle.IsOn = CardPage.showFront;
            CardPage.fronttoggle.Toggled += CardPage.Ts_Toggled;

        }
        //check
        private async Task exit()
        {
            App.viewModel.updateCard(CardPage.selected);
            if (CardPage.selected != null)
            {
                try
                {
                    if (FrontInk.Visibility == Visibility.Visible)
                    {
                        await SaveCard(true);
                        await saveInk(true);
                    }
                    else
                    {
                        await SaveCard(false);
                        await saveInk(false);
                    }
                }
                catch (FileNotFoundException ex)
                {
                }
                App.viewModel.selectedDeck.Number = App.viewModel.cards.Count;
                App.viewModel.updateSelected();
                CardPage.selected = null;
            }
            if (CardPage.edite == true)
                edit();
            resetVisibility();
            CardPage.image.Visibility = Visibility.Visible;
            CardPage.HeaderP.Visibility = Visibility.Visible;
            CardPage.HeaderB.Visibility = Visibility.Collapsed;
            this.Frame.GoBack();
        }
        //invoked when clicking on a card
        private async void cardsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (CardPage.selected != e.ClickedItem as Card)
            {
                if (front.Visibility == Visibility.Visible)
                {
                    await SaveCard(true);
                    await saveInk(true);

                }
                if (back.Visibility == Visibility.Visible)
                {
                    await SaveCard(false);
                    await saveInk(false);
                }
                FrontGrid.UpdateLayout();
                BackGrid.UpdateLayout();
                App.viewModel.updateCard(CardPage.selected);
                await App.viewModel.saveCardChange(CardPage.selected);
                FrontInk.InkPresenter.StrokeContainer.Clear();
                BackInk.InkPresenter.StrokeContainer.Clear();
                CardPage.selected = e.ClickedItem as Card;
                await getCard(true);
                await getCard(false);
                await getInk(true);
                await getInk(false);
                FrontRESIZE.Background = new SolidColorBrush(CardPage.selected.Background);
                BackRESIZE.Background = new SolidColorBrush(CardPage.selected.Background);
                FrontText.FontSize = CardPage.selected.FrontFontSize;
                BackText.FontSize = CardPage.selected.BackFontSize;
                colorindicator.Fill = new SolidColorBrush(CardPage.selected.Background);
                FrontText.Foreground = new SolidColorBrush(CardPage.selected.TextColor);
                BackText.Foreground = new SolidColorBrush(CardPage.selected.TextColor);
                FrontGrid.SelectedItem = CardPage.selected;
                BackGrid.SelectedItem = CardPage.selected;
                resetVisibility();
                if (front.Visibility == Visibility.Visible)
                    fontsize.Content = "" + CardPage.selected.FrontFontSize;
                else
                    fontsize.Content = "" + CardPage.selected.BackFontSize;

                FrontGrid.UpdateLayout();
                BackGrid.UpdateLayout();
                if (front.Visibility == Visibility.Visible)
                {
                    fb.Text = "Front";
                    FrontInkBar.Visibility = Visibility.Visible;
                    BackInkBar.Visibility = Visibility.Collapsed;
                }
                else if (back.Visibility == Visibility.Visible)
                {
                    fb.Text = "Back";
                    BackInkBar.Visibility = Visibility.Visible;
                    FrontInkBar.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                await exit();
            }


        }
        //check
        public static async Task getCard(bool isfront)
        {
            if (isfront)
            {
                FrontText.Text = CardPage.selected.FrontText;
            }
            else if (!isfront)
            {
                BackText.Text = CardPage.selected.BackText;
            }

        }
        //check
        private async void FlipButton_Click(object sender, RoutedEventArgs e)
        {
            Flip();
        }
        //check
        public static async void Flip()
        {
            FrontRESIZE.Background = new SolidColorBrush(CardPage.selected.Background);
            BackRESIZE.Background = new SolidColorBrush(CardPage.selected.Background);
            if (front.Visibility == Visibility.Visible)
            {
                await SaveCard(true);
                await saveInk(true);
                front.Visibility = Visibility.Collapsed;
                back.Visibility = Visibility.Visible;
                showback.Begin();
            }
            else if (front.Visibility == Visibility.Collapsed)
            {
                await SaveCard(false);
                await saveInk(false);
                back.Visibility = Visibility.Collapsed;
                front.Visibility = Visibility.Visible;
                showfront.Begin();
            }
            //inkbar
            if (front.Visibility == Visibility.Visible)
            {
                fb.Text = "Front";
                frontbar.Visibility = Visibility.Visible;
                backbar.Visibility = Visibility.Collapsed;
                fontsize.Content = "" + CardPage.selected.FrontFontSize;
            }
            else if (back.Visibility == Visibility.Visible)
            {
                fb.Text = "Back";
                backbar.Visibility = Visibility.Visible;
                frontbar.Visibility = Visibility.Collapsed;
                fontsize.Content = "" + CardPage.selected.BackFontSize;
            }
            await App.viewModel.saveCardChange(CardPage.selected);
            FrontGrid.UpdateLayout();
            BackGrid.UpdateLayout();
            FrontGrid.SelectedItem = CardPage.selected;
            BackGrid.SelectedItem = CardPage.selected;
        }
        //check
        public static async Task SaveCard(bool isfront)
        {
            if (CardPage.selected != null)
            {

                if (isfront)
                {
                    String front = FrontText.Text;
                    CardPage.selected.FrontText = front;
                }
                else if (!isfront)
                {
                    String back = BackText.Text;
                    CardPage.selected.BackText = back;
                }
                App.viewModel.updateCard(CardPage.selected);
                await App.viewModel.saveCardChange(CardPage.selected);
            }
        }
        //check
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var i = sender as MenuFlyoutItem;
            Card item = i.DataContext as Card;
            if (!CardPage.edite)
            {
                if (item != FrontGrid.SelectedItem && item != BackGrid.SelectedItem)
                {
                    await App.viewModel.deleteCard(item).ConfigureAwait(false);
                }
                else
                    return;

            }
            else
            {
                Card k;
                if (CardPage.showFront)
                {
                    while (FrontGrid.SelectedItems.Count != 0)
                    {
                        foreach (var selectedItem in FrontGrid.SelectedItems)
                        {
                            k = selectedItem as Card;
                            if (k != CardPage.selected)
                            {
                                await App.viewModel.deleteCard(k).ConfigureAwait(true);
                            }
                        }
                    }
                }
                else if (!CardPage.showFront)
                {
                    while (BackGrid.SelectedItems.Count != 0)
                    {
                        foreach (var selectedItem in BackGrid.SelectedItems)
                        {
                            k = selectedItem as Card;
                            if (k != CardPage.selected)
                            {
                                await App.viewModel.deleteCard(k).ConfigureAwait(true);
                            }
                        }
                    }
                }
            }
        }
        //check
        public static async Task saveInk(bool isfront)
        {
            Card selected = CardPage.selected;
            if (selected != null)
            {
                StorageFolder deckfolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(App.viewModel.selectedDeck.Id);
                StorageFolder cardFolder = await deckfolder.GetFolderAsync(selected.Name);
                if (isfront)
                {
                    IReadOnlyList<InkStroke> currentStrokes = FrontInk.InkPresenter.StrokeContainer.GetStrokes();
                    if (currentStrokes.Count > 0)
                    {
                        try
                        {
                            StorageFile data = await cardFolder.CreateFileAsync("Front" + selected.Name + ".gif", CreationCollisionOption.ReplaceExisting);
                            StorageFile imageNew = await cardFolder.CreateFileAsync("Front" + selected.Name + ".jpeg", CreationCollisionOption.ReplaceExisting);
                            CanvasDevice device = CanvasDevice.GetSharedDevice();
                            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, (int)FrontInk.ActualWidth, (int)FrontInk.ActualHeight, 96);
                            using (var ds = renderTarget.CreateDrawingSession())
                            {
                                ds.Clear(selected.Background);
                                ds.DrawInk(currentStrokes);
                            }

                            using (var fileStream = await imageNew.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                await renderTarget.SaveAsync(fileStream, CanvasBitmapFileFormat.Jpeg, 1f);
                                await fileStream.FlushAsync();
                                fileStream.Dispose();
                            }
                            selected.Front = imageNew.Path;
                            App.viewModel.updateCard(selected);
                            IRandomAccessStream stream = await data.OpenAsync(FileAccessMode.ReadWrite);
                            using (IOutputStream outputStream = stream.GetOutputStreamAt(0))
                            {
                                await FrontInk.InkPresenter.StrokeContainer.SaveAsync(outputStream);
                                await outputStream.FlushAsync();
                            }
                            stream.Dispose();
                        }
                        catch (FileLoadException fex)
                        {
                            await saveInk(true);
                        }
                    }
                    else if (currentStrokes.Count < 1)
                    {
                        selected.Front = "Assets/empty.png";
                        try
                        {
                            StorageFile data = await cardFolder.GetFileAsync("Front" + selected.Name + ".gif");
                            if (data != null)
                            {
                                await data.DeleteAsync();
                            }
                            else if (data == null)
                            {
                                return;
                            }
                        }
                        catch (FileNotFoundException e)
                        {
                            return;
                        }
                    }
                }
                else if (!isfront)
                {
                    IReadOnlyList<InkStroke> currentStrokes = BackInk.InkPresenter.StrokeContainer.GetStrokes();
                    if (currentStrokes.Count > 0)
                    {
                        try
                        {
                            StorageFile data = await cardFolder.CreateFileAsync("Back" + selected.Name + ".gif", CreationCollisionOption.ReplaceExisting);
                            StorageFile imageNew = await cardFolder.CreateFileAsync("Back" + selected.Name + ".jpeg", CreationCollisionOption.ReplaceExisting);
                            CanvasDevice device = CanvasDevice.GetSharedDevice();
                            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, (int)BackInk.ActualWidth, (int)BackInk.ActualHeight, 96);
                            using (var ds = renderTarget.CreateDrawingSession())
                            {
                                ds.Clear(selected.Background);
                                ds.DrawInk(currentStrokes);
                            }

                            using (var fileStream = await imageNew.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                await renderTarget.SaveAsync(fileStream, CanvasBitmapFileFormat.Jpeg, 1f);
                                await fileStream.FlushAsync();
                                fileStream.Dispose();
                            }
                            selected.Back = imageNew.Path;
                            App.viewModel.updateCard(selected);
                            IRandomAccessStream stream = await data.OpenAsync(FileAccessMode.ReadWrite);
                            using (IOutputStream outputStream = stream.GetOutputStreamAt(0))
                            {
                                await BackInk.InkPresenter.StrokeContainer.SaveAsync(outputStream);
                                await outputStream.FlushAsync();
                            }
                            stream.Dispose();
                        }
                        catch (FileLoadException fex)
                        {
                            await saveInk(false);
                        }
                    }
                    else if (currentStrokes.Count < 1)
                    {
                        selected.Back = "Assets/empty.png";
                        try
                        {
                            StorageFile data = await cardFolder.GetFileAsync("Back" + selected.Name + ".gif");
                            if (data != null)
                            {
                                await data.DeleteAsync();
                            }
                            else if (data == null)
                            {
                                return;
                            }
                        }
                        catch (FileNotFoundException e)
                        {
                            return;
                        }
                    }
                }
            }
        }
        //check
        public static async Task getInk(bool isfront)
        {
            StorageFolder deckfolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(App.viewModel.selectedDeck.Id);
            StorageFolder cardFolder = await deckfolder.GetFolderAsync(CardPage.selected.Name);
            if (isfront)
            {
                try
                {
                    StorageFile data = await cardFolder.GetFileAsync("Front" + CardPage.selected.Name + ".gif");
                    if (data != null)
                    {
                        IRandomAccessStream stream = await data.OpenAsync(FileAccessMode.Read);
                        using (var inputStream = stream.GetInputStreamAt(0))
                        {
                            await FrontInk.InkPresenter.StrokeContainer.LoadAsync(inputStream);
                        }
                        stream.Dispose();
                    }
                }
                catch (Exception e)
                {
                    return;
                }


            }
            else if (!isfront)
            {
                try
                {
                    StorageFile data = await cardFolder.GetFileAsync("Back" + CardPage.selected.Name + ".gif");
                    if (data != null)
                    {
                        IRandomAccessStream stream = await data.OpenAsync(FileAccessMode.Read);
                        using (var inputStream = stream.GetInputStreamAt(0))
                        {
                            await BackInk.InkPresenter.StrokeContainer.LoadAsync(inputStream);
                        }
                        stream.Dispose();
                    }
                }
                catch (Exception e)
                {
                    return;
                }

            }
        }
        //check
        public static void edit()
        {
            CardPage.edite = !CardPage.edite;
            if (CardPage.showFront)
            {
                FrontGrid.IsMultiSelectCheckBoxEnabled = !FrontGrid.IsMultiSelectCheckBoxEnabled;
                if (CardPage.edite == true)
                {
                    CardPage.editToggle.IsChecked = true;
                    FrontGrid.SelectionMode = ListViewSelectionMode.Multiple;
                    FrontGrid.IsItemClickEnabled = false;
                }
                else
                {
                    CardPage.editToggle.IsChecked = false;
                    FrontGrid.SelectionMode = ListViewSelectionMode.Single;
                    FrontGrid.IsItemClickEnabled = true;
                }
            }
            else
            {
                BackGrid.IsMultiSelectCheckBoxEnabled = !BackGrid.IsMultiSelectCheckBoxEnabled;
                if (CardPage.edite == true)
                {
                    CardPage.editToggle.IsChecked = true;
                    BackGrid.SelectionMode = ListViewSelectionMode.Multiple;
                    BackGrid.IsItemClickEnabled = false;
                }
                else
                {
                    CardPage.editToggle.IsChecked = false;
                    BackGrid.SelectionMode = ListViewSelectionMode.Single;
                    BackGrid.IsItemClickEnabled = true;
                }
            }
        }
        //check
        public static void resetVisibility()
        {
            if (CardPage.showFront)
            {

                front.Visibility = Visibility.Visible;
                back.Visibility = Visibility.Collapsed;
                fb.Text = "Front";

            }
            else
            {
                back.Visibility = Visibility.Visible;
                front.Visibility = Visibility.Collapsed;
                fb.Text = "Back";
            }
        }
        //check
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (CardPage.edite == false)
            {
                edit();
            }
            var i = sender as MenuFlyoutItem;
            Object item = i.DataContext;
            FrontGrid.SelectedItems.Add(item);
            BackGrid.SelectedItems.Add(item);
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
        //check
        private async void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedColor = (Button)sender;
            var rectangle = (Windows.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
            var color = ((SolidColorBrush)rectangle.Fill).Color;

            FrontRESIZE.Background = new SolidColorBrush(color);
            BackRESIZE.Background = new SolidColorBrush(color);
            colorindicator.Fill = new SolidColorBrush(color);
            CardPage.selected.Background = color;
            CardPage.selected.ColorString = color.ToString();
            if (front.Visibility == Visibility.Visible)
            {
                await saveInk(true);
            }
            else
            {
                await saveInk(false);
            }
            App.viewModel.updateCard(CardPage.selected);
            await App.viewModel.saveCardChange(CardPage.selected);
            backgroundColorButton.Flyout.Hide();
        }
        //check
        private async void TextSize_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            int size = int.Parse(item.Text);
            if (front.Visibility == Visibility.Visible)
            {
                FrontText.FontSize = size;
                CardPage.selected.FrontFontSize = size;
            }
            else if (back.Visibility == Visibility.Visible)
            {
                BackText.FontSize = size;
                CardPage.selected.BackFontSize = size;
            }

            fontsize.Content = "" + size;

            App.viewModel.updateCard(CardPage.selected);
            await App.viewModel.saveCardChange(CardPage.selected);
        }
        //check
        private void resizegrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (frontgrid.Visibility == Visibility.Visible)
            {
                frontresize.Width = frontgrid.ActualWidth;
                frontresize.Height = frontgrid.ActualWidth * 2 / 3;
                backresize.Width = frontresize.Width;
                backresize.Height = frontresize.Height;
            }
            else if (backgrid.Visibility == Visibility.Visible)
            {
                backresize.Width = backgrid.ActualWidth;
                backresize.Height = backgrid.ActualWidth * 2 / 3;
                frontresize.Width = backresize.Width;
                frontresize.Height = backresize.Height;
            }
        }
        
        private void PenInputToggle_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (FrontInk.IsHitTestVisible == false)
            {
                FrontInk.IsHitTestVisible = true;
                FrontText.IsHitTestVisible = false;
                BackInk.IsHitTestVisible = true;
                BackText.IsHitTestVisible = false;
                PenInputToggle.IsChecked = true;
            }
            else
            {
                FrontInk.IsHitTestVisible = false;
                FrontText.IsHitTestVisible = true;
                BackInk.IsHitTestVisible = false;
                BackText.IsHitTestVisible = true;
                PenInputToggle.IsChecked = false;
            }
            if (PenInputToggle.IsChecked == true)
            {
                InputMethod.Visibility = Visibility.Visible;
            }
            else
            {
                InputMethod.Visibility = Visibility.Collapsed;
            }
            */
        }
        
        private void Peninput(object sender, RoutedEventArgs e)
        {
            FrontInk.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen;
            BackInk.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen;
            InputMethod.Flyout.Hide();
            Inputindicator.Symbol = Symbol.Edit;
        }

        private void Touchinput(object sender, RoutedEventArgs e)
        {
            FrontInk.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Touch | CoreInputDeviceTypes.Mouse;
            BackInk.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Touch | CoreInputDeviceTypes.Mouse;
            InputMethod.Flyout.Hide();
            Inputindicator.Symbol = Symbol.TouchPointer;
        }

    }
}
