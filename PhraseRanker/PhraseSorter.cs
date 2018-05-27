using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhraseFighter
{
    public class PhraseSorter
    {
        private readonly List<QuickSortGroup> _groups;
        public IEnumerable<QuickSortGroup> Groups { get { return _groups; } }

        public PhraseSorter(IEnumerable<SignitureItem> items)
        {
            _groups = GetGroups(items);
        }

        public void UndergoMitosis(QuickSortGroup completedGroup)
        {
            MitosisResult result = completedGroup.UndergoMitosis();
            if (result.BetterGroup != null)
            {
                _groups.Add(result.BetterGroup);
            }
            if(result.WorseGroup != null)
            {
                _groups.Add(result.WorseGroup);
            }
        }

        private static List<QuickSortGroup> GetGroups(IEnumerable<SignitureItem> items)
        {
            Dictionary<int, List<SignitureItem>> dictionary = new Dictionary<int, List<SignitureItem>>();
            foreach (SignitureItem item in items)
            {
                if (dictionary.ContainsKey(item.Rank))
                {
                    dictionary[item.Rank].Add(item);
                }
                else
                {
                    dictionary.Add(item.Rank, new List<SignitureItem>() { item });
                }
            }
            List<QuickSortGroup> ret = new List<QuickSortGroup>();
            foreach (List<SignitureItem> item in dictionary.Values)
            {
                List<SignitureItem> unsortedSet = item.ToList();
                SignitureItem pivotItem = unsortedSet[0];
                unsortedSet.RemoveAt(0);
                QuickSortGroup group = new QuickSortGroup(pivotItem, unsortedSet, pivotItem.Rank);
                ret.Add(group);
            }
            return ret;
        }
    }
}
