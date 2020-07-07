using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
//using System.Threading.Tasks;

namespace FlashCard.ViewModel
{
    public class DisplayKanjiModel : INotifyPropertyChanged
    {
        private string _kanji;
        private string _kanji2; //khi kanji qua dai se tach lam 2
        private string _pronunciation;
        private string _chinese;
        private string _example;
        private bool _gotta;

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        public DisplayKanjiModel(DisplayKanjiModel kanji)
        {
            if (kanji == null)
                return;
            Kanji = kanji.Kanji;
            Pronunciation = kanji.Pronunciation;
            Chinese = kanji.Chinese;
            Example = kanji.Example;
            Gotta = kanji.Gotta;
        }

        public bool Gotta
        {
            get { return _gotta; }
            set
            {
                _gotta = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public string Kanji
        {
            get { return _kanji; }
            set
            {
                _kanji = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public string Kanji2
        {
            get { return _kanji2; }
            set
            {
                _kanji2 = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public string Pronunciation
        {
            get { return _pronunciation; }
            set
            {
                _pronunciation = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public string Chinese
        {
            get { return _chinese; }
            set
            {
                _chinese = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public string Example
        {
            get { return _example; }
            set
            {
                _example = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool Equals(DisplayKanjiModel kanjiView)
        {
            if (Kanji != kanjiView.Kanji)
                return false;

            if (Chinese != kanjiView.Chinese)
                return false;

            if (Pronunciation != kanjiView.Pronunciation)
                return false;

            if (Example != kanjiView.Example)
                return false;

            if (Gotta != kanjiView.Gotta)
                return false;
            return true;
        }
    }
}
