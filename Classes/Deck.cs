using System;

namespace FlipBear.Classes
{
    public class Deck : ObservableObject
    {
        private string name;
        private string image;
        private int number;
        private bool marked;
        private bool deleted;
        private bool mastered;
        private bool isButton;
        private string markColor;
        private string masterColor;
        private string id;
        private DateTime deletionDate;

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
        public string Id
        {
            get => id;
            set => Set(ref id, value);
        }
        public string Image
        {
            get => image;
            set => Set(ref image, value);
        }
        public int Number
        {
            get => number;
            set => Set(ref number, value);
        }
        public bool Marked
        {
            get => marked;
            set => Set(ref marked, value);
        }
        public bool Deleted
        {
            get => deleted;
            set => Set(ref deleted, value);
        }
        public bool Mastered
        {
            get => mastered;
            set => Set(ref mastered, value);
        }
        public bool IsButton
        {
            get => isButton;
            set => Set(ref isButton, value);
        }
        public DateTime DeletionDate
        {
            get => deletionDate;
            set => Set(ref deletionDate, value);
        }
        public string MarkColor
        {
            get => markColor;
            set => Set(ref markColor, value);
        }
        public string MasterColor
        {
            get => masterColor;
            set => Set(ref masterColor, value);
        }

        public Deck()
        {
            Name = "";
            Id = "Deck" + string.Format(@"{0}", DateTime.Now.Ticks);
            Image = "Assets/Defaultcover.png";
            Number = 0;
            Marked = false;
            Deleted = false;
            Mastered = false;
            isButton = false;
            MarkColor = "Transparent";
            MasterColor = "Transparent";
        }


        public Deck(string name)
        {
            Name = name;
            Id = "Deck" + string.Format(@"{0}", DateTime.Now.Ticks); ;
            Image = "Assets/Defaultcover.png";
            Number = 0;
            Marked = false;
            Deleted = false;
            Mastered = false;
            isButton = false;
            MarkColor = "Transparent";
            MasterColor = "Transparent";
        }


        public Deck toButton()
        {
            isButton = true;
            Id = "";
            return this;
        }


    }
}

