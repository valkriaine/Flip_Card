using System;
using Windows.UI;

namespace FlipBear.Classes
{
    public class Card : ObservableObject
    {
        private string name;
        private string frontText;
        private string backText;
        private string front;
        private string back;
        private int frontfontSize;
        private int backfontSize;
        private Color background;
        private Color textcolor;
        private string textcolorString;
        private string colorString;

        public string ColorString
        {
            get => colorString;
            set => Set(ref colorString, value);
        }

        public Color TextColor
        {
            get => textcolor;
            set => Set(ref textcolor, value);
        }

        public string TextColorString
        {
            get => textcolorString;
            set => Set(ref textcolorString, value);
        }

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public int FrontFontSize
        {
            get => frontfontSize;
            set => Set(ref frontfontSize, value);
        }

        public int BackFontSize
        {
            get => backfontSize;
            set => Set(ref backfontSize, value);
        }

        public Color Background
        {
            get => background;
            set => Set(ref background, value);
        }

        public string Front
        {
            get => front;
            set => Set(ref front, value);
        }

        public string Back
        {
            get => back;
            set => Set(ref back, value);
        }

        public string FrontText
        {
            get => frontText;
            set => Set(ref frontText, value);
        }

        public string BackText
        {
            get => backText;
            set => Set(ref backText, value);
        }

        public Card()
        {
            Front = "Assets/empty.png";
            Back = "Assets/empty.png";
            FrontText = "";
            BackText = "";
            Name = string.Format(@"{0}", DateTime.Now.Ticks);
            FrontFontSize = 30;
            BackFontSize = 30;
            Background = Colors.Wheat;
            TextColor = Colors.Black;
            ColorString = Background.ToString();
            TextColorString = TextColor.ToString();
        }
    }
}
