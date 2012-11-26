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
    public partial class Tree : System.Web.UI.Page
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
            ArrayList tree = analyst.getTree();

            String data = "ID,name,size,parentID,value\n";
            for (int i = 0; i < tree.Count; ++i)
            {
                String[] node = (String[])tree[i];
                data += String.Format("{0},{1},{2},{3},{4}\n", node[0], node[1], 1, node[2], node[3]);
            }

            Response.Write(data);
        }
    }
}