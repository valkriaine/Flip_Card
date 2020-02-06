using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace FlipBear.Classes
{
    public class ViewModel : ObservableObject
    {
        public ObservableCollection<Deck> userDecks { get; private set; }
        public ObservableCollection<Deck> allDecks { get; private set; }
        public ObservableCollection<Deck> deletedDecks { get; private set; }
        public ObservableCollection<Deck> MasteredDecks { get; private set; }
        public ObservableCollection<Deck> MarkedDecks { get; private set; }
        public Deck selectedDeck { get; private set; }
        private Deck button = new Deck().toButton();
        private string mastercolor = "#00BFFF";
        private string markcolor = "#FFD700";
        private string transparent = "Transparent";

        public ObservableCollection<Card> cards { get; private set; }

        public bool checkName(string s)
        {
            foreach (Deck deck in userDecks)
            {
                if (deck.Name.Equals(s))
                {
                    return false;
                }
            }
            return true;
        }
        public void selectDeck(Deck d)
        {
            selectedDeck = d;
            getCards(d);

        }
        public async void AddDeck(string s)
        {
            if (checkName(s))
            {
                Deck d = new Deck(s);
                userDecks.Add(d);
                allDecks.Add(d);
                await changeSave(d).ConfigureAwait(false);
                createCardFolder(d);
            }
        }
        public async void addMastered(string s)
        {
            if (checkName(s))
            {
                Deck d = new Deck(s);
                d.Mastered = true;
                d.MasterColor = mastercolor;
                userDecks.Add(d);
                allDecks.Add(d);
                MasteredDecks.Add(d);
                await changeSave(d).ConfigureAwait(false);
                createCardFolder(d);
            }
        }
        public async void addMarked(string s)
        {
            if (checkName(s))
            {
                Deck d = new Deck(s);
                d.Marked = true;
                d.MarkColor = markcolor;
                userDecks.Add(d);
                allDecks.Add(d);
                MarkedDecks.Add(d);
                await changeSave(d).ConfigureAwait(false);
                createCardFolder(d);
            }
        }

        public ViewModel()
        {
            userDecks = new ObservableCollection<Deck>
            {
                button
            };
            allDecks = new ObservableCollection<Deck>() { button };
            deletedDecks = new ObservableCollection<Deck>() { };
            MasteredDecks = new ObservableCollection<Deck>() { button };
            MarkedDecks = new ObservableCollection<Deck>() { button };
            cards = new ObservableCollection<Card>();
            retrieveLibrary();

        }

        public async void RemoveDeck(Deck d)
        {
            await removeSave(d).ConfigureAwait(true);
            userDecks.Remove(d);
            allDecks.Remove(d);
            deletedDecks.Remove(d);
            MasteredDecks.Remove(d);
            MarkedDecks.Remove(d);
        }

        public async void RenameDeck(Deck d, string n)
        {
            d.Name = n;
            userDecks[userDecks.IndexOf(d)] = d;
            allDecks[allDecks.IndexOf(d)] = d;
            if (deletedDecks.Contains(d))
                deletedDecks[deletedDecks.IndexOf(d)] = d;
            if (MasteredDecks.Contains(d))
                MasteredDecks[MasteredDecks.IndexOf(d)] = d;
            if (MarkedDecks.Contains(d))
                MarkedDecks[MarkedDecks.IndexOf(d)] = d;
            await changeSave(d);
        }

        public void HideButton()
        {
            userDecks.Remove(button);
            allDecks.Remove(button);
            MasteredDecks.Remove(button);
            MarkedDecks.Remove(button);
        }

        public void ShowButton()
        {
            userDecks.Insert(0, button);
            allDecks.Insert(0, button);
            MasteredDecks.Insert(0, button);
            MarkedDecks.Insert(0, button);
        }

        public async Task MasterDeck(Deck d)
        {
            d.Mastered = true;
            d.MasterColor = mastercolor;
            userDecks[userDecks.IndexOf(d)] = d;
            allDecks[allDecks.IndexOf(d)] = d;
            if (!MasteredDecks.Contains(d))
                MasteredDecks.Add(d);
            await changeSave(d).ConfigureAwait(true);
        }

        public async Task unMasterDeck(Deck d)
        {
            d.Mastered = false;
            d.MasterColor = transparent;
            userDecks[userDecks.IndexOf(d)] = d;
            allDecks[allDecks.IndexOf(d)] = d;
            if (MasteredDecks.Contains(d))
                MasteredDecks.Remove(d);
            await changeSave(d).ConfigureAwait(true);
        }

        public async Task MarkDeck(Deck d)
        {
            d.Marked = true;
            d.MarkColor = markcolor;
            userDecks[userDecks.IndexOf(d)] = d;
            allDecks[allDecks.IndexOf(d)] = d;
            if (!MarkedDecks.Contains(d))
                MarkedDecks.Add(d);
            await changeSave(d).ConfigureAwait(true);
        }

        public async Task unMarkDeck(Deck d)
        {
            d.Marked = false;
            d.MarkColor = transparent;
            userDecks[userDecks.IndexOf(d)] = d;
            allDecks[allDecks.IndexOf(d)] = d;
            if (MarkedDecks.Contains(d))
                MarkedDecks.Remove(d);
            await changeSave(d).ConfigureAwait(true);
        }

        public async void SaveLibrary()
        {

            deleteDecks();
            string FileName = "";
            foreach (Deck d in userDecks)
            {
                if (!d.IsButton)
                {
                    string name = d.Id;
                    FileName = name + ".xml";
                    StorageFile localFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
                    XmlSerializer serializer = new XmlSerializer(typeof(Deck));
                    using (Stream stream = await localFile.OpenStreamForWriteAsync().ConfigureAwait(true))
                    {
                        serializer.Serialize(stream, d);
                        stream.Dispose();
                        stream.Close();
                    }
                }
            }
        }

        private async void retrieveLibrary()
        {
            Deck d;
            IReadOnlyList<StorageFile> files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            if (files.Count < 1)
            {
                return;
            }
            foreach (StorageFile f in files)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Deck));
                    using (Stream stream = await f.OpenStreamForReadAsync().ConfigureAwait(true))
                    {
                        d = serializer.Deserialize(stream) as Deck;
                        userDecks.Add(d);
                        if (!d.Deleted)
                            allDecks.Add(d);
                        if (d.Deleted)
                            deletedDecks.Add(d);
                        if (!d.Deleted && d.Mastered)
                            MasteredDecks.Add(d);
                        if (!d.Deleted && d.Marked)
                            MarkedDecks.Add(d);
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private async Task removeSave(Deck d)
        {
            string name = d.Id;
            string file = name + ".xml";
            try
            {
                StorageFile localFile = await ApplicationData.Current.LocalFolder.GetFileAsync(file);
                await localFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                StorageFolder localFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(name);
                await localFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (FileNotFoundException e)
            {
                return;
            }
        }

        private async Task changeSave(Deck d)
        {
            string name = d.Id;
            string file = name + ".xml";
            StorageFile localFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(file, CreationCollisionOption.ReplaceExisting);
            XmlSerializer serializer = new XmlSerializer(typeof(Deck));
            using (Stream stream = await localFile.OpenStreamForWriteAsync().ConfigureAwait(true))
            {
                serializer.Serialize(stream, d);
            }
        }

        public async Task deleteDeck(Deck d, DateTime time)
        {
            d.DeletionDate = time;
            d.Deleted = true;
            userDecks[userDecks.IndexOf(d)] = d;
            if (!deletedDecks.Contains(d))
                deletedDecks.Add(d);
            allDecks.Remove(d);
            MasteredDecks.Remove(d);
            MarkedDecks.Remove(d);
            await changeSave(d).ConfigureAwait(false);
        }

        private void deleteDecks()
        {
            foreach (Deck d in userDecks)
            {
                if (d.Deleted && (DateTime.Now - d.DeletionDate).TotalDays >= 30)
                {
                    RemoveDeck(d);
                }
            }
        }

        public void restore(Deck d)
        {
            d.Deleted = false;
            userDecks[userDecks.IndexOf(d)] = d;
            changeSave(d);
            allDecks.Add(d);
            if (deletedDecks.Contains(d))
                deletedDecks.Remove(d);
            if (d.Marked)
                MarkedDecks.Add(d);
            if (d.Mastered)
                MasteredDecks.Add(d);
        }

        public void restoreAll()
        {
            foreach (Deck d in deletedDecks)
            {
                d.Deleted = false;
                userDecks[userDecks.IndexOf(d)] = d;
                changeSave(d);
                allDecks.Add(d);
                if (d.Marked)
                    MarkedDecks.Add(d);
                if (d.Mastered)
                    MasteredDecks.Add(d);
            }
            deletedDecks.Clear();
        }

        public void deleteAll()
        {
            foreach (Deck d in deletedDecks)
            {
                userDecks.Remove(d);
                removeSave(d);
            }
            deletedDecks.Clear();
        }

        public async void createCardFolder(Deck d)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string name = d.Id;
            try
            {
                StorageFolder newFolder = await localFolder.CreateFolderAsync(name, CreationCollisionOption.FailIfExists);
            }
            catch (Exception e)
            {
                return;
            }
        }

        public async void updateSelected()
        {
            userDecks[userDecks.IndexOf(selectedDeck)] = selectedDeck;
            allDecks[allDecks.IndexOf(selectedDeck)] = selectedDeck;

            if (deletedDecks.Contains(selectedDeck))
                deletedDecks[deletedDecks.IndexOf(selectedDeck)] = selectedDeck;

            if (!MarkedDecks.Contains(selectedDeck) && selectedDeck.Marked)
                MarkedDecks.Add(selectedDeck);
            if (!MasteredDecks.Contains(selectedDeck) && selectedDeck.Mastered)
                MasteredDecks.Add(selectedDeck);

            if (MarkedDecks.Contains(selectedDeck) && !selectedDeck.Marked)
                MarkedDecks.Remove(selectedDeck);
            if (MasteredDecks.Contains(selectedDeck) && !selectedDeck.Mastered)
                MasteredDecks.Remove(selectedDeck);

            await changeSave(selectedDeck).ConfigureAwait(true);
        }

        public async void addCard(Card c)
        {
            StorageFolder deckFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(selectedDeck.Id);
            string cardName = c.Name + ".xml";
            StorageFile cardData = await deckFolder.CreateFileAsync(cardName, CreationCollisionOption.ReplaceExisting);
            XmlSerializer serializer = new XmlSerializer(typeof(Card));
            using (Stream stream = await cardData.OpenStreamForWriteAsync().ConfigureAwait(true))
            {
                serializer.Serialize(stream, c);
            }

            string name = c.Name;
            try
            {      
                StorageFolder cardFolder = await deckFolder.CreateFolderAsync(name, CreationCollisionOption.FailIfExists);
            }
            catch (Exception e)
            {
                return;
            }
            cards.Add(c);
            selectedDeck.Number = selectedDeck.Number + 1;
            userDecks[userDecks.IndexOf(selectedDeck)] = selectedDeck;
            allDecks[allDecks.IndexOf(selectedDeck)] = selectedDeck;
            if (deletedDecks.Contains(selectedDeck))
                deletedDecks[deletedDecks.IndexOf(selectedDeck)] = selectedDeck;
            if (!MarkedDecks.Contains(selectedDeck) && selectedDeck.Marked)
                MarkedDecks.Add(selectedDeck);
            if (!MasteredDecks.Contains(selectedDeck) && selectedDeck.Mastered)
                MasteredDecks.Add(selectedDeck);
            if (MarkedDecks.Contains(selectedDeck) && !selectedDeck.Marked)
                MarkedDecks.Remove(selectedDeck);
            if (MasteredDecks.Contains(selectedDeck) && !selectedDeck.Mastered)
                MasteredDecks.Remove(selectedDeck);
            await changeSave(selectedDeck).ConfigureAwait(true);
        }

        public async Task deleteCard(Card c)
        {
            StorageFolder deckFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(selectedDeck.Id);
            string cardName = c.Name + ".xml";
            StorageFile cardData = await deckFolder.GetFileAsync(cardName);
            StorageFolder cardFolder = await deckFolder.GetFolderAsync(c.Name);
            await cardData.DeleteAsync();
            await cardFolder.DeleteAsync();
            cards.Remove(c);
            selectedDeck.Number = selectedDeck.Number - 1;
            userDecks[userDecks.IndexOf(selectedDeck)] = selectedDeck;
            allDecks[allDecks.IndexOf(selectedDeck)] = selectedDeck;
            if (deletedDecks.Contains(selectedDeck))
                deletedDecks[deletedDecks.IndexOf(selectedDeck)] = selectedDeck;
            if (!MarkedDecks.Contains(selectedDeck) && selectedDeck.Marked)
                MarkedDecks.Add(selectedDeck);
            if (!MasteredDecks.Contains(selectedDeck) && selectedDeck.Mastered)
                MasteredDecks.Add(selectedDeck);
            if (MarkedDecks.Contains(selectedDeck) && !selectedDeck.Marked)
                MarkedDecks.Remove(selectedDeck);
            if (MasteredDecks.Contains(selectedDeck) && !selectedDeck.Mastered)
                MasteredDecks.Remove(selectedDeck);
            await changeSave(selectedDeck).ConfigureAwait(true);
        }

        public void updateCard(Card c)
        {
            if (c != null && cards.Contains(c))
            {
                cards[cards.IndexOf(c)] = c;
            }
        }

        public async Task saveCardChange(Card c)
        {
            if (c != null && cards.Contains(c))
            {
                try
                {
                    StorageFolder deckFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(selectedDeck.Id);
                    string cardName = c.Name + ".xml";
                    StorageFile cardData = await deckFolder.CreateFileAsync(cardName, CreationCollisionOption.ReplaceExisting);
                    XmlSerializer serializer = new XmlSerializer(typeof(Card));
                    using (Stream stream = await cardData.OpenStreamForWriteAsync().ConfigureAwait(true))
                    {
                        serializer.Serialize(stream, c);
                        stream.Dispose();
                    }
                }
                catch (FileLoadException fex)
                {
                    await saveCardChange(c);
                }
            }
        }

        public async Task getCards(Deck d)
        {
            cards.Clear();
            Card c;
            StorageFolder deckFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(d.Id);
            IReadOnlyList<StorageFile> files = await deckFolder.GetFilesAsync();
            if (files.Count < 1)
            {
                return;
            }
            foreach (StorageFile f in files)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Card));
                using (Stream stream = await f.OpenStreamForReadAsync().ConfigureAwait(true))
                {
                    c = serializer.Deserialize(stream) as Card;
                    cards.Add(c);
                }
            }
        }
    }
}
