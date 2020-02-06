using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlipBear.Classes
{
    class GridTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DeckTemplate { get; set; }
        public DataTemplate ButtonTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var selectedTemplate = DeckTemplate;
            var dataItem = item as Deck;
            if (dataItem.IsButton)
            {
                selectedTemplate = ButtonTemplate;
            }
            else
                selectedTemplate = DeckTemplate;

            return selectedTemplate;
        }
    }
}
