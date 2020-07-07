using FlashCard.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FlashCard.View
{
    /// <summary>
    /// Interaction logic for GroupManger.xaml
    /// </summary>
    public partial class GroupManger : Window
    {
        public GroupManger()
        {
            InitializeComponent();
            FormatGroupListView();
        }

        #region Common
        private void FormatGroupListView()
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

            int total = 0;
            int complete = 0;

            foreach (var groupViewModel in groupListViewModel)
            {
                total = FlashCardView.KanjiList.Count(x => x.C_groupid == groupViewModel.GroupId);
                complete = FlashCardView.KanjiList.Count(x => x.C_groupid == groupViewModel.GroupId &&
                                                            x.C_didRemember == 1);

                if(total==0)
                {
                    groupViewModel.PercentComplete = 100;
                }
                else
                {
                    groupViewModel.PercentComplete = complete * 100 / total;
                }
            }  
            icGroup.ItemsSource = groupListViewModel;
        }
        #endregion
    }
}
