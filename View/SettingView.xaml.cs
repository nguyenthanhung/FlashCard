using FlashCard.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace FlashCard.View
{
    /// <summary>
    /// Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : Window
    {
        private FlashCardView _flashCardView;
        private NotifyIcon _notifyIcon;
        private List<FlashCardView> _listFlashCardView = null;
        // The path to the key where Windows looks for startup applications
        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public SettingView()
        {
            InitializeComponent();
            this.Loaded += SettingView_Loaded;
        }

        #region Common
        private void InitializeSetting()
        {
            this.ShowActivated = false;

            CbShowFC_Checked(this.cbShowFC, null);
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        private  void InitializeFlashCard()
        {
            _flashCardView = new FlashCardView();
            _flashCardView.Topmost = true;
            _flashCardView.ShowActivated = false;
            //this.Left
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();

            _notifyIcon.Icon = Asset.Resource.icon;
            _notifyIcon.Visible = true;

            ContextMenu cm = new ContextMenu();
            MenuItem menuItem = new MenuItem("Add new word", new EventHandler((s, u) => { BtnAddEdit_Click(null, null); }));
            menuItem.Name = "Add new word";
            cm.MenuItems.Add(menuItem);

            menuItem = new MenuItem("Exit", new EventHandler((s, u) => {

                if (System.Windows.MessageBox.Show("Are you sure?", "Kanji Flashcard",
                                     MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }));
            menuItem.Name = "Exit";
            cm.MenuItems.Add(menuItem);
            this._notifyIcon.ContextMenu = cm;
        }

        private void InitializeDataForFontCombo()
        {
            var fontsDict = FlashCard.Common.Common.GetFontFamilyNameFromResource();
            List<KanjiFontViewModel> kanjiFontList = new List<KanjiFontViewModel>();

            foreach (var fontName in fontsDict.Keys)
            {

                kanjiFontList.Add(new KanjiFontViewModel
                {
                    FontName = Convert.ToString(fontName),
                    FontFamilyName = Convert.ToString(fontsDict[fontName]),
                });
            }
            cmbFonts.ItemsSource = kanjiFontList;
            CmbFonts_SelectionChanged(null, null);
            //this._flashCardView.SetLabelFont(Convert.ToString(cmbFonts.SelectedValue));
            //KanjiFont fonts = new KanjiFont();
            //fontsDict.Keys.CopyTo(fonts.FontFamilyNameList, 0);
            //this.cmbFonts.ItemsSource = fonts.FontFamilyNameList;
        }

        private void InitializeDataForGroupCombo()
        {
            List<GROUP_KANJI> grouplist = new List<GROUP_KANJI>();
            using (mainEntities mainEntities = new mainEntities())
            {
                grouplist = mainEntities.GROUP_KANJI.ToList<GROUP_KANJI>();
            }

            List<GroupWordViewModel> groupListViewModel = new List<GroupWordViewModel>();

            foreach (var group in grouplist)
            {
                groupListViewModel.Add(new GroupWordViewModel
                {
                    GroupName = Convert.ToString(group.groupname),
                    GroupId = (int)group.groupid,
                });
            }
            cmbGroup.ItemsSource = groupListViewModel;
            cmbGroup_SelectionChanged(null, null);
            //this._flashCardView.SetLabelFont(Convert.ToString(cmbFonts.SelectedValue));
            //KanjiFont fonts = new KanjiFont();
            //fontsDict.Keys.CopyTo(fonts.FontFamilyNameList, 0);
            //this.cmbFonts.ItemsSource = fonts.FontFamilyNameList;
        }


        private void InitializeStartWithWindowCheckbox()
        {
            // Check to see the current state (running at startup or not)
            if (rkApp.GetValue("KanjiFlashCardStartWithWindow") == null)
            {
                // The value doesn't exist, the application is not set to run at startup, Check box
                cbStartWithWindow.IsChecked = false;
            }
            else
            {
                // The value exists, the application is set to run at startup
                cbStartWithWindow.IsChecked = true;
            }
        }

        private void CreateEvent()
        {
            this.StateChanged += SettingView_StateChanged;
            this.Closing += SettingView_Closing;
            this.cbShowFC.Checked += CbShowFC_Checked;
            this.cbShowFC.Unchecked += CbShowFC_Checked;
            this.cbShowKanjiOnly.Checked += CbShowKanjiOnly_Checked; ;
            this.cbShowKanjiOnly.Unchecked += CbShowKanjiOnly_Checked;
            this.cbStartWithWindow.Checked += CbStartWithWindow_Checked;
            this.cbStartWithWindow.Unchecked += CbStartWithWindow_Checked;
            this.btnAddEdit.Click += BtnAddEdit_Click;
            this.btnShowAll.Click += BtnShowAll_Click;
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }
        #endregion

        #region Events
        private void SettingView_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeFlashCard();
            InitializeSetting();
            InitializeNotifyIcon();
            InitializeDataForFontCombo();
            InitializeDataForGroupCombo();
            InitializeStartWithWindowCheckbox();
            CreateEvent();
            this.WindowState = WindowState.Minimized;
        }

        private void SettingView_StateChanged(object sender, EventArgs e)
        {
            if(this.WindowState == WindowState.Minimized)
            {
                //this.Visibility = Visibility.Hidden;
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
                this.WindowState = WindowState.Normal;
            this.Visibility = Visibility.Visible;
            this.Show();
            this.Activate();
        }

        private void SettingView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Properties.Settings.Default.Save();
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Minimized;
                this.Visibility = Visibility.Hidden;
            }
        }

        private void CbShowFC_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.CheckBox).IsChecked.Value)
            {
                _flashCardView.Visibility = Visibility.Visible;
            }
            else
            {
                _flashCardView.Visibility = Visibility.Hidden;
            }
        }

        private void KanjiTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(_flashCardView==null)
                return;
            this._flashCardView._timer.Interval = TimeSpan.FromSeconds(this.kanjiTimeSlider.Value);
        }

        private void CbShowKanjiOnly_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == null)
                return;

            _flashCardView.SetLabelSize((sender as System.Windows.Controls.CheckBox).IsChecked.Value);
        }

        private void CmbFonts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbFonts.SelectedItem == null)
                return;

            this._flashCardView.SetKanjiFont(Convert.ToString((cmbFonts.SelectedItem as KanjiFontViewModel).FontFamilyName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbStartWithWindow_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == null)
                return;

            if (cbStartWithWindow.IsChecked.Value)
            {
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("KanjiFlashCardStartWithWindow", System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue("KanjiFlashCardStartWithWindow", false);
            }
        }

        public void BtnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            AddEditView addEditView = new AddEditView(null, this);
            this.btnAddEdit.IsEnabled = false;
            this._notifyIcon.ContextMenu.MenuItems["Add new word"].Enabled = false;
            addEditView.Closed += AddEditView_Closed;
            addEditView.Show();
        }

        private void AddEditView_Closed(object sender, EventArgs e)
        {
            this.btnAddEdit.IsEnabled = true;
            this._notifyIcon.ContextMenu.MenuItems["Add new word"].Enabled = true;
        }

        private void cmbGroup_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            if(_listFlashCardView == null)
            {
                _listFlashCardView = new List<FlashCardView>();
                var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;

                int columnNumber = (int)(desktopWorkingArea.Width / _flashCardView.ActualWidth);
                int rowNumber = (int)(desktopWorkingArea.Height / _flashCardView.ActualHeight);
                double leftStartPostion = (desktopWorkingArea.Width - (columnNumber * _flashCardView.ActualWidth)) / 2;
                double topStartPostion = (desktopWorkingArea.Height - (rowNumber * _flashCardView.ActualHeight)) / 2;

                for (int i = 0;i<rowNumber;i++)
                {
                    for (int j = 0; j < columnNumber; j++)
                    {
                        FlashCardView card = new FlashCardView();
                        card.Owner = this;
                        card.Left = leftStartPostion + _flashCardView.ActualWidth * j;
                        card.Top = topStartPostion + _flashCardView.ActualHeight * i;
                        _listFlashCardView.Add(card);
                    }
                }
            }


            foreach (var card in _listFlashCardView)
            {
                card.ShowDialog();
            }
        }

        private void btnGroupManage_Click(object sender, RoutedEventArgs e)
        {
            GroupManger groupManger = new GroupManger();
            groupManger.Show();
        }
        #endregion
    }
}