namespace FlashCard.ViewModel
{
    public class KanjiFontViewModel
    {
        private string _fontName;
        private string _fontFamilyName;

        public string FontName
        {
            get { return _fontName; }
            set
            {
                _fontName = value;
            }
        }

        public string FontFamilyName
        {
            get { return _fontFamilyName; }
            set
            {
                _fontFamilyName = value;
            }
        }
    }
}
