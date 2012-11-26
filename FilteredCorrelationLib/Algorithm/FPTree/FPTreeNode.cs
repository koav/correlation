using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace FilteredCorrelation.Algorithm.FPTree
{
    class FPTreeNode
    {
        public int Column;
        public int Count;

        public FPTreeNode Parent;
        public FPTreeNode FirstChild;

        public FPTreeNode Brother;
        public FPTreeNode ColumnBrother;

        public FPTreeNode(int column, int count, FPTreeNode parent)
        {
            this.Column = column;
            this.Count = count;
            this.Parent = parent;
        }

        public FPTreeNode insertChild(int column, int count)
        {
            if (FirstChild == null)
            {
                FirstChild = new FPTreeNode(column, count, this);
                return FirstChild;
            }
            else
            {
                FPTreeNode child = FirstChild;
                while ((column != child.Column) && child.Brother != null)
                {
                    child = child.Brother;
                }

                if (child.Column == column)
                {
                    child.Count += count;
                }
                else
                {
                    child.Brother = new FPTreeNode(column, count, this);
                    child = child.Brother;
                }
                return child;
            }
        }
    }
}
