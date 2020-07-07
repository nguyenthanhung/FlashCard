using FlashCard.ViewModel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace FlashCard.View
{
    /// <summary>
    /// Interaction logic for AddEditView.xaml
    /// </summary>
    public partial class AddEditView : Window
    {
        public MyKanji _myKanji = new MyKanji();
        private Window _parentWindow = null;
        private readonly DisplayKanjiModel _kanjiViewModel = new DisplayKanjiModel(null);
        private DisplayKanjiModel _kanjiViewModel_Old = new DisplayKanjiModel(null);

        public AddEditView(MyKanji kanji, Window parent)
        {
            InitializeComponent();

            this.DataContext = _kanjiViewModel;
            _parentWindow = parent;
            _myKanji = kanji?? _myKanji;
            this.Loaded += AddEditView_Loaded;
        }

        #region Common
        private void InitializeWindow(bool isEdit)
        {
            this.Topmost = true;
            this.ShowActivated = false;
            if (isEdit)
            {
                this.Title = "Edit " + _kanjiViewModel.Chinese;
                this.cbGotta.Visibility = Visibility.Visible;
                this.btnNext.Visibility = Visibility.Visible;
                this.btnPrevious.Visibility = Visibility.Visible;
            }
            else
            {
                this.Title = "Add new word";
                this.cbGotta.Visibility = Visibility.Hidden;
                this.btnNext.Visibility = Visibility.Hidden;
                this.btnPrevious.Visibility = Visibility.Hidden;
            }

            this.lblDone.Visibility = Visibility.Hidden;
            this.lblFail.Visibility = Visibility.Hidden;
            SetDataToView(_myKanji);
        }

        private void SetPosition()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;

            if (_parentWindow is SettingView)
            {
                this.Left = desktopWorkingArea.Width - this.ActualWidth;
                this.Top = _parentWindow.Top - this.ActualHeight + 10;
            }
            else
            {
                if ((_parentWindow.Left + this.ActualWidth) > desktopWorkingArea.Width)
                {
                    this.Left = desktopWorkingArea.Width - this.ActualWidth;
                }
                else if (_parentWindow.Left < 0)
                {
                    this.Left = 0;
                }
                else
                {
                    this.Left = _parentWindow.Left;
                }

                if ((_parentWindow.Top + _parentWindow.ActualHeight + this.ActualHeight) > desktopWorkingArea.Height)
                {
                    this.Top = _parentWindow.Top - this.ActualHeight;
                }
                else if (_parentWindow.Top < 0)
                {
                    this.Top = 0;
                }
                else
                {
                    this.Top = _parentWindow.Top + _parentWindow.ActualHeight;
                }
            }
        }

        private void SetDataToView(MyKanji mykanji)
        {
            if(mykanji == null)
            {
                return;
            }
            _kanjiViewModel.Chinese = mykanji.C_chinese;
            _kanjiViewModel.Pronunciation = mykanji.C_pronunciation;
            _kanjiViewModel.Kanji = mykanji.C_kanji;
            _kanjiViewModel.Example = mykanji.C_example;
            _kanjiViewModel.Gotta = mykanji.C_didRemember == 1;
            _kanjiViewModel_Old = new DisplayKanjiModel( _kanjiViewModel);

            this.lblDone.Visibility = Visibility.Hidden;
            this.lblFail.Visibility = Visibility.Hidden;
        }

        private void SetData(ref MyKanji mykanji, DisplayKanjiModel viewKanji)
        {
            mykanji.C_chinese = _kanjiViewModel.Chinese;
            mykanji.C_pronunciation = _kanjiViewModel.Pronunciation;
            mykanji.C_kanji = _kanjiViewModel.Kanji;
            mykanji.C_example = _kanjiViewModel.Example;
            mykanji.C_didRemember = _kanjiViewModel.Gotta ? 1 : 0;
        }

        private void CreateEvent()
        {
            this.Deactivated += AddEditView_Deactivated;
            this.btnChangeMode.Click += BtnChangeMode_Click;
            this.btnNext.Click += BtnNext_Click;
            this.btnPrevious.Click += BtnNext_Click;
            this.btnSave.Click += BtnSave_Click;
            this.Closing += AddEditView_Closing;
            this.txtKanji.LostFocus += TxtKanji_LostFocus;
        }

        private void SaveToDb(bool isEdit, MyKanji kanji)
        {
            try
            {
                if (isEdit)
                {
                    using (var db = new mainEntities())
                    {
                        var result = db.MyKanjis.SingleOrDefault(b => b.C_key == kanji.C_key);
                        if (result != null)
                        {
                            SetData(ref result, _kanjiViewModel);
                            db.SaveChanges();
                            FlashCardView.KanjiList = db.MyKanjis.ToList<MyKanji>();
                        }
                    }
                }
                else
                {
                    using (var db = new mainEntities())
                    {
                        MyKanji result = new MyKanji();
                        long maxKey = (from ka in FlashCardView.KanjiList select ka.C_key).Max();
                        SetData(ref result, _kanjiViewModel);
                        db.MyKanjis.Add(result);
                        db.SaveChanges();
                        FlashCardView.KanjiList = db.MyKanjis.ToList<MyKanji>();
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
        #endregion

        #region Event
        private void AddEditView_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeWindow(_myKanji.C_key!=0);
            SetPosition();
            CreateEvent();
        }

        private void AddEditView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_kanjiViewModel.Equals(_kanjiViewModel_Old))
            {
                if (MessageBox.Show(this, "You will lose the data you have changed. Continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void AddEditView_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }

        private void BtnChangeMode_Click(object sender, RoutedEventArgs e)
        {
            if (!_kanjiViewModel.Equals(_kanjiViewModel_Old))
            {
                if (MessageBox.Show(this, "You will lose the data you have changed. Continue?", "Kanji Flashcard",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (this.Title.Contains("Edit"))
            {
                _myKanji = new MyKanji();
                InitializeWindow(false);
            }
            else
            {
                MyKanji kanji;
                kanji = FlashCardView.KanjiList.Where(x => string.IsNullOrEmpty(x.C_kanji) ||
                                                                string.IsNullOrEmpty(x.C_chinese) ||
                                                                string.IsNullOrEmpty(x.C_pronunciation) ||
                                                                string.IsNullOrEmpty(x.C_example)).LastOrDefault();
                _myKanji = kanji ?? _myKanji;
                InitializeWindow(true);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (!_kanjiViewModel.Equals(_kanjiViewModel_Old))
            {
                if(MessageBox.Show(this, "You will lose the data you have changed. Continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
            }

            MyKanji kanji;
            if(sender.Equals(this.btnNext))
                kanji = FlashCardView.KanjiList.Where(x => x.C_key > _myKanji.C_key &&(string.IsNullOrEmpty(x.C_kanji) ||
                                                                string.IsNullOrEmpty(x.C_chinese) ||
                                                                string.IsNullOrEmpty(x.C_pronunciation) ||
                                                                string.IsNullOrEmpty(x.C_example))).FirstOrDefault();
            else
                kanji = FlashCardView.KanjiList.Where(x => x.C_key < _myKanji.C_key &&(string.IsNullOrEmpty(x.C_kanji) ||
                                                                string.IsNullOrEmpty(x.C_chinese) ||
                                                                string.IsNullOrEmpty(x.C_pronunciation) ||
                                                                string.IsNullOrEmpty(x.C_example))).LastOrDefault();

            _myKanji = kanji ?? _myKanji;
            SetDataToView(_myKanji);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Title.Contains("Edit"))
                {
                    if (_kanjiViewModel.Equals(_kanjiViewModel_Old))
                    {
                        MessageBox.Show(this, "Nothing changes", "Kanji Flashcard", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else
                {
                    if ((_kanjiViewModel.Kanji == null || _kanjiViewModel.Kanji == "") &&
                        (_kanjiViewModel.Chinese == null || _kanjiViewModel.Chinese == "") &&
                        (_kanjiViewModel.Pronunciation == null || _kanjiViewModel.Pronunciation == "") &&
                        (_kanjiViewModel.Example == null || _kanjiViewModel.Example == ""))
                    {
                        MessageBox.Show(this, "Cannot save empty word", "Kanji Flashcard", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                SaveToDb(this.Title.Contains("Edit"), _myKanji);
                _kanjiViewModel_Old = new DisplayKanjiModel(_kanjiViewModel);

                this.lblDone.Visibility = Visibility.Visible;
            }
            catch(Exception ex)
            {
                this.lblFail.Visibility = Visibility.Visible;
            }
        }

        private void TxtKanji_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.Title.Contains("Add"))
            {
                return;
            }

            if ((sender as TextBox).Text.Trim() == "")
            {
                return;
            }

            MyKanji kj = FlashCardView.KanjiList.Where(x => x.C_kanji == (sender as TextBox).Text.Trim()).FirstOrDefault();
            if(kj != null)
            {
                _myKanji = kj;
                InitializeWindow(true);
            }
        }
        #endregion
    }
}
