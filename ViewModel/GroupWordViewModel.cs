namespace FlashCard.ViewModel
{
    public class GroupWordViewModel
    {
        private string _groupName;
        private int _groupId;
        private int _percentComplete;

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                _groupName = value;
            }
        }

        public int GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
            }
        }
        public int PercentComplete
        {
            get { return _percentComplete; }
            set
            {
                _percentComplete = value;
            }
        }
    }
}
