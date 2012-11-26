using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Collections;
using FilteredCorrelation.Algorithm;
using FilteredCorrelation.Algorithm.FPTree;

namespace FilteredCorrelation
{
    public class Analyst
    {
        private Filter filter;

        public Analyst(String dataSource)
        {
            filter = new Filter(dataSource);
        }

        public ArrayList getTree()
        {
            return filter.getTree();
        }

        public ArrayList analysis(int criterionAttr, double diffRaio, double support, double confidence)
        {
            ArrayList list = filter.analysis(criterionAttr, diffRaio, support);
            int[] criterions = filter.getCriterions();
            ArrayList result = new ArrayList();
            for (int i = 0; i < list.Count; ++i)
            {
                Pattern p = (Pattern)list[i];
                if (p.Confidence >= confidence && p.Confidence > 0)
                {
                    result.Add(new CriterionPattern(p, criterions));
                    //result.Add(p);
                }
            }
            return result;
        }

        public String[] getAttributes()
        {
            return filter.getAttributeNames();
        }

        public String[] getProperties()
        {
            return filter.getColNames();
        }
    }
}
