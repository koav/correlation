using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Collections;

namespace FilteredCorrelation.Algorithm.FPTree
{
    public class Pattern
    {
        public double Support;
        public int Count;
        public int[] columns;
        public double Confidence;

        public Pattern(int[] columns, int count, double support)
        {
            this.columns = columns;
            this.Count = count;
            this.Support = (double)Math.Round(support * 10000) / 10000.0;
        }

        public String getColumns(String[] colNames)
        {
            if (columns == null)
            {
                return "";
            }

            String result = "";
            for (int i = 0; i < columns.Length; ++i)
            {
                result += colNames[columns[i]] + "# ";
            }

            return result;
        }

        public String ToString(String[] colNames)
        {
            if (columns == null)
            {
                return "";
            }

            String result = "";
            for (int i = 0; i < columns.Length; ++i)
            {
                result += colNames[columns[i]] + ", ";
            }

            result = String.Format(result + "[{0}] [{1}%] [{2}%]", Count, Support*100, Confidence*100);
            return result;
        }
    }
}
