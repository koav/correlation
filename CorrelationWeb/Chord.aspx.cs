using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Configuration;
using System.Collections;
using FilteredCorrelation;

namespace CorrelationWeb
{
    public partial class Chord : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String dataSource = WebConfigurationManager.AppSettings["dataSource"];

            int attr = Convert.ToInt32(Request["attr"]);
            double filter = Convert.ToDouble(Request["filter"]);
            double support = Convert.ToDouble(Request["support"]);
            double confidence = Convert.ToDouble(Request["confidence"]);

            Analyst analyst = new Analyst(dataSource);
            ArrayList result = analyst.analysis(attr, filter, support, confidence);

            String[] names = analyst.getProperties();
            Hashtable criterions = new Hashtable();
            Hashtable properties = new Hashtable();
            for (int i = 0; i < result.Count; ++i)
            {
                CriterionPattern p = (CriterionPattern)result[i];
                if (p.getColumnLength() == 1)
                {
                    String key = p.getCriterion(names);
                    if (!criterions.ContainsKey(key))
                    {
                        criterions.Add(key, new ArrayList());
                    }
                    ((ArrayList)criterions[key]).Add(p);

                    key = p.getColumns(names);
                    if (!properties.ContainsKey(key))
                    {
                        properties.Add(key, new ArrayList());
                    }
                    ((ArrayList)properties[key]).Add(p);
                }
            }

            Hashtable colIndex = new Hashtable();
            String data = "";
            int index = 0;
            IEnumerator iter = criterions.Keys.GetEnumerator();
            while (iter.MoveNext())
            {
                data += iter.Current + ",";
                colIndex.Add(iter.Current, index++);
            }

            iter = properties.Keys.GetEnumerator();
            while (iter.MoveNext())
            {
                data += iter.Current + ",";
                colIndex.Add(iter.Current, index++);
            }
            data = data.Substring(0, data.Length - 1) + "\n";

            iter = criterions.Keys.GetEnumerator();
            while (iter.MoveNext())
            {
                int[] row = new int[criterions.Count + properties.Count];
                ArrayList list = (ArrayList)criterions[iter.Current];
                for (int i = 0; i < list.Count; ++i)
                {
                    CriterionPattern p = (CriterionPattern)list[i];
                    row[(int)colIndex[p.getColumns(names)]] = p.Count;
                }

                data += arrayToString(row);
            }

            iter = properties.Keys.GetEnumerator();
            while (iter.MoveNext())
            {
                int[] row = new int[criterions.Count + properties.Count];
                ArrayList list = (ArrayList)properties[iter.Current];
                for (int i = 0; i < list.Count; ++i)
                {
                    CriterionPattern p = (CriterionPattern)list[i];
                    row[(int)colIndex[p.getCriterion(names)]] = p.Count;
                }
                data += arrayToString(row);
            }

            Response.Write(data);

        }


        private String arrayToString(int[] array)
        {
            String result = "";
            for (int i = 0; i < array.Length - 1; ++i)
                result += array[i] + ",";

            if (array.Length > 0)
                result += array[array.Length - 1];

            result += "\n";

            return result;
        }
    }
}