using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhraseFighter
{

    public class QuickSortGroup : ViewModel
    {
        private readonly PhraseSorter _mothership;

        public SignitureItem PivotItem { get; }
        private readonly int _rankOffset;

        private readonly List<SignitureItem> _unsortedLines;
        public IEnumerable<SignitureItem> UnsortedLines { get { return _unsortedLines.ToArray(); } }

        private readonly List<SignitureItem> _betterLines;
        public IEnumerable<SignitureItem> BetterLines { get { return _betterLines.ToArray(); } }

        private readonly List<SignitureItem> _worseLines;
        public IEnumerable<SignitureItem> WorseLines { get { return _worseLines.ToArray(); } }

        public IEnumerable<SignitureItem> AllItems
        {
            get
            {
                yield return PivotItem;
                foreach (SignitureItem item in UnsortedLines)
                {
                    yield return item;
                }
                foreach (SignitureItem item in BetterLines)
                {
                    yield return item;
                }
                foreach (SignitureItem item in WorseLines)
                {
                    yield return item;
                }
            }
        }

        public SignitureItem NextContender { get { return _unsortedLines[0]; } }

        public bool Complete { get { return _unsortedLines.Count == 0; } }

        public QuickSortGroup(SignitureItem pivotItem, IEnumerable<SignitureItem> unsortedLines, int rankOffset, PhraseSorter mothership)
        {
            _mothership = mothership;
            PivotItem = pivotItem;
            _rankOffset = rankOffset;
            _unsortedLines = unsortedLines.ToList();
            _betterLines = new List<SignitureItem>();
            _worseLines = new List<SignitureItem>();

            foreach (SignitureItem item in AllItems)
            {
                item.Rank = rankOffset;
            }
        }

        public PhraseFight GetNextPhraseFight()
        {
            return new PhraseFight(PivotItem, NextContender);
        }

        public PhraseFight AdvancePhraseFight(bool pivotItemWon)
        {
            if (pivotItemWon)
            {
                _worseLines.Add(NextContender);
                NotifyPropertyChanged(nameof(WorseLines));
            }
            else
            {
                _betterLines.Add(NextContender);
                NotifyPropertyChanged(nameof(BetterLines));
            }
            _unsortedLines.RemoveAt(0);
            NotifyPropertyChanged(nameof(UnsortedLines));
            NotifyPropertyChanged(nameof(AllItems));
            if (Complete)
            {
                return null;
            }
            return _mothership.GetNextPhraseFight();
        }

        public MitosisResult UndergoMitosis()
        {
            int baseRank = _rankOffset + _worseLines.Count;
            PivotItem.Rank = baseRank;
            QuickSortGroup betterGroup = GetGroupFrom(_betterLines, baseRank + 1);
            QuickSortGroup worseGroup = GetGroupFrom(_worseLines, _rankOffset);
            _betterLines.Clear();
            _worseLines.Clear();
            return new MitosisResult(betterGroup, worseGroup);
        }

        private QuickSortGroup GetGroupFrom(List<SignitureItem> items, int rankOffset)
        {
            if (!items.Any())
            {
                return null;
            }
            SignitureItem pivotItem = GetPivotItem(items);
            List<SignitureItem> unsortedItems = items.ToList();
            unsortedItems.Remove(pivotItem);
            return new QuickSortGroup(pivotItem, unsortedItems, rankOffset, _mothership);
        }

        private static SignitureItem GetPivotItem(List<SignitureItem> lineSet)
        {
            float mid = (float)lineSet.Count / 2;
            int index = (int)Math.Floor(mid);
            return lineSet[index];
        }
    }

    public class MitosisResult
    {
        public QuickSortGroup BetterGroup { get; }
        public QuickSortGroup WorseGroup { get; }

        public MitosisResult(QuickSortGroup betterGroup, QuickSortGroup worseGroup)
        {
            BetterGroup = betterGroup;
            WorseGroup = worseGroup;
        }
    }
}
