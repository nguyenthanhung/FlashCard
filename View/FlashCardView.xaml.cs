using FlashCard.ExtensionMethod;
using FlashCard.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace FlashCard.View
{
    /// <summary>
    /// Interaction logic for FlashCardView.xaml
    /// </summary>
    public partial class FlashCardView : Window
    {
        //private DMMIcrosoft DMM = new DMMIcrosoft();
        public static List<MyKanji> KanjiList;
        public MyKanji _myKanji = new MyKanji();
        private readonly DisplayKanjiModel _kanjiViewModel = new DisplayKanjiModel(null);
        public DispatcherTimer _timer;
        private bool _isEditFormShow = false;
        //private bool _isMouseEntering = false;

        public FlashCardView()
        {
            InitializeComponent();

            this.DataContext = _kanjiViewModel;

            //DMM.Chui = "dm may Microsoft";
            GetRandomKanji();

            this.AllowsTransparency = true;
            this.Loaded += FlashCardView_Loaded;
        }

        #region Common
        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();

            _timer.Interval = TimeSpan.FromMilliseconds(Properties.Settings.Default.KanjiChangeTime * 1000);
            _timer.Tick += Timer_Tick;

            _timer.Start();
        }

        private void InitializeFlashCard()
        {
            this.Background = Brushes.Transparent;
            this.ShowInTaskbar = false;

            this.windowBorder.Background = Brushes.White;
            windowBorder.BorderBrush = Brushes.Black;
            windowBorder.BorderThickness = new Thickness(1);
            windowBorder.CornerRadius = new CornerRadius(10);
        }

        private void CreateEvent()
        {
            this.Loaded += FlashCardView_Loaded;
            this.Deactivated += FlashCardView_Deactivated;
            this.vbKanji2.SizeChanged += VbKanji_SizeChanged;
            this.MouseRightButtonUp += FlashCardView_MouseRightButtonUp;
        }
        #endregion

        #region Connect Data
        private void GetRandomKanji()
        {
            Random rand = new Random();
            if (KanjiList == null || KanjiList.Count() == 0)
            {
                mainEntities main = new mainEntities();
                KanjiList = main.MyKanjis.ToList<MyKanji>();
            }
            int max = KanjiList.Count() - 1;
            int id = rand.Next(0, max);

            //DMM.Chui = "dm may Microsoft" + DateTime.Now.Second;
            this._myKanji = KanjiList.Where(x =>x.C_didRemember != 1).ElementAt(id);

            _kanjiViewModel.Chinese = _myKanji.C_chinese;
            _kanjiViewModel.Pronunciation = _myKanji.C_pronunciation;
            _kanjiViewModel.Kanji = _myKanji.C_kanji;
            _kanjiViewModel.Kanji2 = string.Empty;
            SetLabelSize(Properties.Settings.Default.IsShowKanjiOnlyOnCard);
            _kanjiViewModel.Example = _myKanji.C_example;
        }

        /// <summary>
        /// 
        /// </summary>
        private void FormatLabel()
        {
            SetLabelFont();
            SetLabelSize(Properties.Settings.Default.IsShowKanjiOnlyOnCard);
        }

        //private void SetFamilyNameFromFileToResource()
        //{
        //    using (ResXResourceWriter resx = new ResXResourceWriter(@"./Asset/Font.resx"))
        //    {
        //        var files = Directory.GetFiles(@"C:\Users\12191\Desktop\Font\FontToUse");
        //        foreach (var file in files)
        //        {
        //            FileInfo fileInfo = new FileInfo(file);
        //            foreach (FontFamily fontFamily in Fonts.GetFontFamilies(file))
        //            {
        //                var fontFamilyName = fontFamily.Source.Split('/')[fontFamily.Source.Split('/').Count() - 1];

        //                resx.AddResource(fileInfo.Name, fontFamilyName);
        //            }
        //        }
        //    }
        //}

        public void SetKanjiFont(string fontFamilyName)
        {
            if (fontFamilyName == string.Empty)
                return;

            tbKanji.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), string.Format("./Asset/Font/{0}", fontFamilyName));
            tbKanji2.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), string.Format("./Asset/Font/{0}", fontFamilyName));
            //tbKanji.FontFamily = new FontFamily(new Uri("pack://application:,,,/Asset/Font"), string.Format("./{0}", fontFamilyName));
        }

        public void SetLabelFont()
        {
            tbExample.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), string.Format("./Asset/Font/{0}", "TimeNewRoman.ttf#Times New Roman"));
        }

        public void SetLabelSize(bool isKanjiOnly)
        {
            if (isKanjiOnly)
            {
                if (_kanjiViewModel.Kanji.Length > 4)
                {
                    this.row0.Height = new GridLength(60);
                    this.row1.Height = new GridLength(20);
                    this.row2.Height = new GridLength(40);

                    this.rowKanji2.Visibility = Visibility.Visible;
                    this.rowChinesePronunciation.Visibility = Visibility.Hidden;
                    this.rowExample.Visibility = Visibility.Hidden;

                    string kanji = _kanjiViewModel.Kanji;
                    if(kanji.Length % 2 == 0)
                    {
                        _kanjiViewModel.Kanji = kanji.Substring(0, kanji.Length / 2);
                    }
                    else
                    {
                        _kanjiViewModel.Kanji = kanji.Substring(0, kanji.Length / 2 + 1);
                    }

                    _kanjiViewModel.Kanji2 = kanji.Replace(_kanjiViewModel.Kanji, string.Empty);
                }
                else
                {
                    this.row1.Height = new GridLength(0);
                    this.row2.Height = new GridLength(0);
                    this.row0.Height = new GridLength(123);
                    this.rowKanji2.Visibility = Visibility.Hidden;
                    this.rowChinesePronunciation.Visibility = Visibility.Hidden;
                    this.rowExample.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                _kanjiViewModel.Kanji = _kanjiViewModel.Kanji + _kanjiViewModel.Kanji2;
                this.row1.Height = new GridLength(15);
                this.row2.Height = new GridLength(45);
                this.row0.Height = new GridLength(60);
                this.rowKanji2.Visibility = Visibility.Hidden;
                this.rowChinesePronunciation.Visibility = Visibility.Visible;
                this.rowExample.Visibility = Visibility.Visible;
            }
        }

        private ContextMenu FormatContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = "Show next word";
            item.Click += (s, e) => { GetRandomKanji(); };
            contextMenu.Items.Add(item);

            item = new MenuItem();
            item.Header = "Edit";
            item.Click += (s, e) => {
                AddEditView addEditView = new AddEditView(_myKanji, this);
                addEditView.Closed += (s1, e1)=> { _isEditFormShow = false; };
                _isEditFormShow = true;
                addEditView.Show();
            };
            contextMenu.Items.Add(item);

            return contextMenu;
        }
        #endregion

        #region Event
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!this.IsMouseOver && !_isEditFormShow)
            {
                GetRandomKanji();
            }
        }

        private void FlashCardView_Loaded(object sender, RoutedEventArgs e)
        {
            //_myKanji = GetRandomKanji();
            InitializeFlashCard();
            InitializeTimer();
            FormatLabel();
            CreateEvent();
        }

        private void FlashCardView_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
            //(sender as Window).LeftC
            Properties.Settings.Default.Save();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Properties.Settings.Default.IsShowKanjiOnlyOnCard)
            {
                return;
            }
            //_isMouseEntering = true;
            SetLabelSize(false);
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Properties.Settings.Default.IsShowKanjiOnlyOnCard)
            {
                return;
            }
            //_isMouseEntering = false;
            SetLabelSize(true);
        }

        private void VbKanji_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double scale = this.vbKanji.GetScaleFactor();
            //this.tbKanji2.Height = scale * this.tbKanji2.ActualHeight;
            this.vbKanji2.Height = this.vbKanji.ActualHeight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlashCardView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isEditFormShow)
                return;

            ContextMenu cm = FormatContextMenu();
            cm.PlacementTarget = sender as Window;
            cm.IsOpen = true;
        }
        #endregion
    }
}
