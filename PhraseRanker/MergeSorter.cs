using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhraseFighter
{
    class MergeSorter : INotifyPropertyChanged
    {
        private MergeGroupFight CurrentFight { get; set; }

        private ItemFight _currentItemFight;
        public ItemFight CurrentItemFight
        {
            get { return _currentItemFight; }
            set
            {
                _currentItemFight = value;
                NotifyPropertyChanged(nameof(CurrentItemFight));
            }
        }

        private readonly List<MergeSortGroup> _groups;
        private List<MergeGroupFight> _incompleteFights;

        public event PropertyChangedEventHandler PropertyChanged;

        public MergeSorter(IEnumerable<SignitureItem> items)
        {
            _groups = items.Select(item => 
                new MergeSortGroup(new List<SignitureItem>() { item }))
                .ToList();
            _incompleteFights = new List<MergeGroupFight>();
        }

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        internal void Advance()
        {
            if(CurrentFight.Complete)
            {
                MergeSortGroup newGroup = CurrentFight.ToGroup();
                _groups.Add(newGroup);
                AdvanceCurrentFight();
            }
            if(CurrentItemFight != null)
            {
                CurrentItemFight = CurrentFight.GetNextFight();
            }
        }

        private void AdvanceCurrentFight()
        {
            if(_incompleteFights.Any())
            {
                CurrentFight = _incompleteFights[0];
                _incompleteFights.RemoveAt(0);
            }
            else if (_groups.Count == 1)
            {
                // DONE
            }
            else
            {
                _incompleteFights = CreateNewFights();
                _groups.Clear();
                CurrentFight = _incompleteFights[0];
                _incompleteFights.RemoveAt(0);
            }
        }

        private List<MergeGroupFight> CreateNewFights()
        {
            List<MergeGroupFight> ret = new List<MergeGroupFight>();
            for (int i = 0; i < _groups.Count; i += 2)
            {
                MergeSortGroup leftGroup = _groups[i];
                MergeSortGroup rightGroup = _groups[i + 1];
                MergeGroupFight fight = new MergeGroupFight(leftGroup, rightGroup, this);
                ret.Add(fight);
            }
            //TODO: Handle odd numbers
            return ret;
        }
    }

    class MergeSortGroup
    {
        private readonly List<SignitureItem> _items;
        public bool Done { get { return !_items.Any(); } }
        public IEnumerable<SignitureItem> RemainingItems { get { return _items; } } 

        public MergeSortGroup(List<SignitureItem> initialItems)
        {
            _items = initialItems;
        }

        public SignitureItem TakeLastItem()
        {
            SignitureItem ret = _items.Last();
            _items.Remove(ret);
            return ret;
        }

        public void ReturnItem(SignitureItem item)
        {
            _items.Add(item);
        }
    }

    class MergeGroupFight
    {
        private readonly MergeSorter _mothership;
        private List<SignitureItem> _sortedItems;

        public MergeSortGroup LeftGroup { get; }
        public MergeSortGroup RightGroup { get; }

        public bool Complete { get; private set; }

        public MergeGroupFight(MergeSortGroup left, MergeSortGroup right, MergeSorter mothership)
        {
            _sortedItems = new List<SignitureItem>();
            LeftGroup = left;
            RightGroup = right;
            _mothership = mothership;
        }

        public ItemFight GetNextFight()
        {
            SignitureItem left = LeftGroup.TakeLastItem();
            SignitureItem right = RightGroup.TakeLastItem();
            return new ItemFight(left, right, this);
        }

        internal void LeftWins(ItemFight mergeSortFight)
        {
            _sortedItems.Add(mergeSortFight.Left);
            RightGroup.ReturnItem(mergeSortFight.Right);
            PotentiallyComplete();
        }

        internal void RightWins(ItemFight mergeSortFight)
        {
            _sortedItems.Add(mergeSortFight.Right);
            LeftGroup.ReturnItem(mergeSortFight.Left);
            PotentiallyComplete();
        }

        private void PotentiallyComplete()
        {
            if(LeftGroup.Done || RightGroup.Done)
            {
                _sortedItems.AddRange(LeftGroup.RemainingItems);
                _sortedItems.AddRange(RightGroup.RemainingItems);
                Complete = true;
            }
            _mothership.Advance();
        }

        internal MergeSortGroup ToGroup()
        {
            return new MergeSortGroup(_sortedItems);
        }
    }

    class ItemFight
    {
        private readonly MergeGroupFight _source;

        public SignitureItem Left { get; }
        public SignitureItem Right { get; }


        public ItemFight(SignitureItem left, SignitureItem right, MergeGroupFight source)
        {
            Left = left;
            Right = right;
            _source = source;
        }

        public void LeftWins()
        {
            _source.LeftWins(this);
        }

        public void RightWins()
        {
            _source.RightWins(this);
        }
    }
}
