using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Collections;
using FilteredCorrelation.Data;
namespace FilteredCorrelation.Algorithm.FPTree
{
    public class FP
    {
        FPTree fpTree;
        ArrayList allPatterns;
        String[] colNames;

        ArrayList filteredPatterns;

        int[] colCount;
        int[] sortedCols;
        int recordCount;

        public FP(String[] colNames, int recordCount, int[] colCount)
        {
            fpTree = new FPTree();
            allPatterns = new ArrayList();
            filteredPatterns = new ArrayList();

            this.colNames = colNames;
            this.colCount = colCount;

            this.sortedCols = new int[colNames.Length];
            for (int i = 0; i < colNames.Length; ++i)
            {
                this.sortedCols[i] = i;
            }
        }

        public void sortCols()
        {
            for (int i = 0; i < colCount.Length; ++i)
            {
                for (int j = colCount.Length - 1; j > i; --j)
                {
                    if (colCount[j] > colCount[j - 1])
                    {
                        int tmp = colCount[j];
                        colCount[j] = colCount[j - 1];
                        colCount[j - 1] = tmp;

                        tmp = sortedCols[j];
                        sortedCols[j] = sortedCols[j - 1];
                        sortedCols[j - 1] = tmp;
                    }
                }
            }

            //for (int i = 0; i < sortedCols.Length; ++i)
            //{
            //    Console.Write(this.colNames[sortedCols[i]] + " ");
            //}
            //Console.WriteLine();
        }

        public void insertRecord(Boolean[] record)
        {
            recordCount += 1;
            int[] sorted = new int[record.Length];
            int index = 0;
            for (int i = 0; i < sortedCols.Length; ++i)
            {
                if (record[sortedCols[i]])
                {
                    sorted[index++] = sortedCols[i];
                }
            }

            int[] result = new int[index];
            for (int i = 0; i < index; ++i)
            {
                result[i] = sorted[i];
            }

            fpTree.insert(result, 1);
        }

        public ArrayList analysis(double support, int[] contains = null)
        {
            int minCount = (int)Math.Ceiling(recordCount * support);
            fpGrowth(fpTree, null, minCount, contains);

            if (contains != null)
            {
                for (int i = 0; i < contains.Length; ++i)
                {
                    calcConfidence(contains[i]);
                }
            }

            return filteredPatterns;
        }

        public String[] getColNames()
        {
            return this.colNames;
        }

        private void fpGrowth(FPTree tree, int[] columns, int minCount, int[] containsCols = null)
        {
            if (columns != null && tree.root.Count >= minCount)
            {
                allPatterns.Add(new Pattern(columns, tree.root.Count, tree.root.Count / (double)this.recordCount));

                if (containsCols != null)
                {
                    // check if this pattern contains any required column
                    Boolean contains = false;
                    for (int i = 0; i < containsCols.Length && !contains; ++i)
                    {
                        for (int j = 0; j < columns.Length; ++j)
                        {
                            if (columns[j] == containsCols[i])
                            {
                                filteredPatterns.Add(new Pattern(columns, tree.root.Count, tree.root.Count / (double)this.recordCount));
                                contains = true;
                                break;
                            }
                        }
                    }
                }

            }
            else if (columns != null)
            {
                return;
            }

            foreach (int column in tree.header.Keys)
            {
                int[] postfix = insertPrefix(column, columns);
                FPTree newTree = getPatternsAndTree(tree, postfix[0]);

                fpGrowth(newTree, postfix, minCount, containsCols);

            }
        }

        private String printColumns(int[] columns, int count)
        {
            String result = "";
            if (columns != null)
            {
                for (int i = 0; i < columns.Length; ++i)
                {
                    result += colNames[columns[i]] + ", ";
                }
            }
            result += "[" + count + "]";
            return result;
        }

        private FPTree getPatternsAndTree(FPTree tree, int column)
        {
            FPTree result = new FPTree();
            if (tree.header.ContainsKey(column))
            {
                result.root.Count = 0;
                FPTreeNode colNode = tree.header[column];
                while (colNode != null)
                {
                    // get a path
                    int count = colNode.Count;
                    result.root.Count += count;
                    ArrayList reversePath = new ArrayList();
                    FPTreeNode current = colNode.Parent;
                    while (current.Column != -1)
                    {
                        reversePath.Add(current.Column);
                        current = current.Parent;
                    }

                    int[] path = new int[reversePath.Count];
                    for (int i = 0; i < path.Length; ++i)
                    {
                        path[i] = (int)reversePath[path.Length - 1 - i];
                    }
                    result.insert(path, count);
                    colNode = colNode.ColumnBrother;
                }
            }
            return result;
        }

        private int[] insertPrefix(int column, int[] columns)
        {
            if (columns == null)
            {
                return new int[] { column };
            }
            else
            {
                int[] result = new int[columns.Length + 1];
                result[0] = column;
                for (int i = 0; i < columns.Length; ++i)
                {
                    result[i + 1] = columns[i];
                }
                return result;
            }
        }

        private void calcConfidence(int column)
        {
            for (int i = 0; i < filteredPatterns.Count; ++i)
            {
                Pattern p = (Pattern)filteredPatterns[i];
                int[] columns = arrayExcept(p.columns, column);
                if (columns != null)
                {
                    int count = getPatternCount(columns);
                    if (count > 0)
                    {
                        p.Confidence = Math.Round(p.Count / (double)count * 10000) /10000.0;

                    }
                }
            }
        }

        private int[] arrayExcept(int[] array, int exception)
        {
            int[] result = new int[array.Length - 1];
            int index = 0;
            Boolean find = false;

            for (int i = 0; i < array.Length && index < result.Length; ++i)
            {
                if (array[i] != exception)
                {
                    result[index++] = array[i];
                }
                else
                {
                    find = true;
                }
            }
            return (find || (array[result.Length] == exception)) ? result : null;
        }

        private Boolean arrayEquals(int[] arrA, int[] arrB)
        {
            if (arrA.Length != arrB.Length)
            {
                return false;
            }

            for (int i = 0; i < arrA.Length; ++i)
            {
                if (arrA[i] != arrB[i])
                {
                    return false;
                }
            }
            return true;
        }

        private int getPatternCount(int[] columns)
        {
            for (int i = 0; i < allPatterns.Count; ++i)
            {
                Pattern p = (Pattern)allPatterns[i];
                if (arrayEquals(p.columns, columns))
                {
                    return p.Count;
                }
            }
            return 0;
        }

        public ArrayList getTree()
        {
            return nodeToList(fpTree.root);
        }

        private ArrayList nodeToList(FPTreeNode node)
        {
            ArrayList result = new ArrayList();
            String[] array = new String[4]{
                node.GetHashCode() + "",
                node.Column >= 0 ? colNames[node.Column] : "Root",
                (node.Parent == null)? "": node.Parent.GetHashCode()+"",
                "Count: " + node.Count
            };

            result.Add(array);

            FPTreeNode child = node.FirstChild;
            while (child != null)
            {
                result.AddRange(nodeToList(child));
                child = child.Brother;
            }

            return result;
        }
    }
}
