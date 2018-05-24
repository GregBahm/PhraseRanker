using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PhraseFighter
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        protected void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class RankFight : ViewModel
    {
        public SignitureItem LeftItem { get; }
        public SignitureItem RightItem { get; }

        public RankFight(SignitureItem leftItem, SignitureItem rightItem)
        {
            LeftItem = leftItem;
            RightItem = rightItem;
        }
    }

    public class MainViewModel : ViewModel
    {
        private readonly string _savePath;
        private int _currentIndex;
        private int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value % _items.Count;
            }
        }
        private readonly Random _rand;
        private List<SignitureItem> _items;
        public IEnumerable<SignitureItem> Items
        {
            get { return _items.OrderByDescending(item => item.Rank); }
        }

        private RankFight _currentFight;
        public RankFight CurrentFight { get { return _currentFight; } }

        public MainViewModel(IEnumerable<SignitureItem> items, string savePath)
        {
            _rand = new Random();
            _items = items.ToList();
            _currentFight = TryGetFight();
            _savePath = savePath;
        }

        public void GrantLeftWin()
        {
            ResolveFight(CurrentFight.LeftItem, CurrentFight.RightItem);
        }

        public void GrantRightWin()
        {
            ResolveFight(CurrentFight.RightItem, CurrentFight.LeftItem);
        }

        private void ResolveFight(SignitureItem winner, SignitureItem loser)
        {
            winner.Rank++;
            NotifyPropertyChanged(nameof(Items));
            _currentFight = TryGetFight();
            NotifyPropertyChanged(nameof(CurrentFight));
            Save();
        }

        private RankFight TryGetFight()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                CurrentIndex++;
                RankFight ret = GetNewFight();
                if (ret != null)
                {
                    int leftIndex = _items.IndexOf(ret.LeftItem);
                    int rightIndex = _items.IndexOf(ret.RightItem);
                    CurrentIndex = Math.Max(leftIndex, rightIndex);
                    return ret;
                }
            }
            return null;
        }

        private RankFight GetNewFight()
        {
            SignitureItem fighterA = _items[CurrentIndex];

            SignitureItem fighterB = GetFighterB(); 
            if(fighterB == null)
            {
                return null;
            }
            return new RankFight(fighterA, fighterB);
        }

        private SignitureItem GetFighterB()
        {
            for (int i = 1; i < _items.Count; i++)
            {
                int index = (CurrentIndex + i) % _items.Count;
                if(_items[index].Rank == _items[CurrentIndex].Rank)
                {
                    return _items[index];
                }
            }
            return null;
        }

        void Save()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement rootElement = doc.CreateElement("Root");
            foreach (SignitureItem item in _items)
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
        
        public static  MainViewModel LoadFromRaw(string rawPath, string xmlPath)
        {
            List<SignitureItem> initialItems = SignitureItem.LoadFromRawText(rawPath);
            return new MainViewModel(initialItems, xmlPath);
        }
    }
}
