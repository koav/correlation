using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using FilteredCorrelation.Algorithm.FPTree;
namespace FilteredCorrelation
{
    public class CriterionPattern
    {
        
        public int Count;
        public double Support;
        public double Confidence;

        private int[] columns;
        private int criterion;

        public CriterionPattern(Pattern p, int[] criterions)
        {
            this.Support = p.Support;
            this.Confidence = p.Confidence;
            this.Count = p.Count;
            
            this.columns = new int[p.columns.Length-1];
            
            int index = 0;
            for (int i = 0; i < p.columns.Length; ++i)
            {
                if (isInArray(p.columns[i], criterions))
                    this.criterion = p.columns[i];
                else
                    this.columns[index++] = p.columns[i];
            }
        }

        public String getCriterion(String[] properties)
        {
            return properties[this.criterion];
        }

        public String getColumns(String[] properties)
        {
            if(columns == null || columns.Length == 0)
            {
                return "";
            }

            String str = "";
            for (int i = 0; i < columns.Length - 1; ++i)
            {
                str += properties[columns[i]] + "<br>";
            }

            //Console.WriteLine(properties.Length);

            str += properties[columns[columns.Length - 1]];

            return str;
        }

        public int getColumnLength()
        {
            return this.columns.Length;
        }

        private bool isInArray(int val, int[] array)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i] == val)
                    return true;
            }

            return false;
        }
    }
}
