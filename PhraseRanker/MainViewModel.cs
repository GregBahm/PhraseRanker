using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PhraseFighter
{

    public class MainViewModel : ViewModel
    {
        private readonly string _savePath;

        private readonly PhraseSorter _sorter;
        
        public IEnumerable<QuickSortGroup> Items
        {
            get
            {
                return _sorter.Groups.OrderBy(item => item.PivotItem.Rank);
            }
        }

        private PhraseFight _currentFight;
        public PhraseFight CurrentFight { get { return _currentFight; } }

        private QuickSortGroup _currentGroup;
        public QuickSortGroup CurrentGroup { get { return _currentGroup; } }

        public MainViewModel(IEnumerable<SignitureItem> items, string savePath)
        {
            _savePath = savePath;
            _sorter = new PhraseSorter(items);
            UpdateCurrentFight();
        }

        private void UpdateCurrentFight()
        {
            _currentGroup = _sorter.Groups.FirstOrDefault(item => !item.Complete);
            NotifyPropertyChanged(nameof(CurrentGroup));
            NotifyPropertyChanged(nameof(Items));
            if (_currentGroup != null)
            {
                _currentFight = _currentGroup.GetNextPhraseFight();
                NotifyPropertyChanged(nameof(CurrentFight));
            }
        }

        public void GrantLeftWin()
        {
            PhraseFight nextFight = CurrentGroup.AdvancePhraseFight(true);
            AdvanceToNextItem(nextFight);
        }

        public void GrantRightWin()
        {
            PhraseFight nextFight = CurrentGroup.AdvancePhraseFight(false);
            AdvanceToNextItem(nextFight);
        }

        private void AdvanceToNextItem(PhraseFight nextFight)
        {
            if(CurrentGroup.Complete)
            {
                _sorter.UndergoMitosis(CurrentGroup);
            }

            UpdateCurrentFight();
            Save();
        }
        
        void Save()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement rootElement = doc.CreateElement("Root");
            foreach (SignitureItem item in _sorter.Groups.SelectMany(item => item.AllItems))
            {
                rootElement.AppendChild(item.ToXml(doc));
            }
            doc.AppendChild(rootElement);
            doc.Save(_savePath);
        }

        public static MainViewModel LoadFromXml(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            List<SignitureItem> items = new List<SignitureItem>();
            foreach (XmlElement item in doc.DocumentElement.ChildNodes)
            {
                SignitureItem loadedItem = SignitureItem.FromXml(item);
                items.Add(loadedItem);
            }
            return new MainViewModel(items, path);
        }

        public static MainViewModel LoadFromRaw(string rawPath, string xmlPath)
        {
            List<SignitureItem> initialItems = SignitureItem.LoadFromRawText(rawPath);
            return new MainViewModel(initialItems, xmlPath);
        }
    }
}
