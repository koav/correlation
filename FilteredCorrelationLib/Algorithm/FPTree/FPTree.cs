using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Collections;
namespace FilteredCorrelation.Algorithm.FPTree
{
    class FPTree
    {
        public FPTreeNode root;
        public Dictionary<int, FPTreeNode> header;

        public FPTree()
        {
            root = new FPTreeNode(-1, -1, null);
            header = new Dictionary<int, FPTreeNode>();
        }

        public void insert(int[] record, int count)
        {
            FPTreeNode current = root;
            for (int i = 0; i < record.Length; ++i)
            {
                current = insert(current, record[i], count);
            }
        }

        private FPTreeNode insert(FPTreeNode parent, int column, int count)
        {
            FPTreeNode child = parent.insertChild(column, count);
            add2Header(child);
            return child;
        }

        private void add2Header(FPTreeNode node)
        {
            if (header.ContainsKey(node.Column))
            {
                FPTreeNode current = header[node.Column];
                while (current.ColumnBrother != null && current != node)
                {
                    current = current.ColumnBrother;
                }
                if(current != node){
                    current.ColumnBrother = node;
                }
            }
            else
            {
                header.Add(node.Column, node);
            }
        }
    }
}