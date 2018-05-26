using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhraseFighter
{
    public class PhraseFight
    {
        public SignitureItem LeftItem { get; }
        public SignitureItem RightItem { get; }

        public PhraseFight(SignitureItem leftItem, SignitureItem rightItem)
        {
            LeftItem = leftItem;
            RightItem = rightItem;
        }
    }
}
