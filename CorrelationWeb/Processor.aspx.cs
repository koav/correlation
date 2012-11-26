using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Configuration;
using System.Collections;
using FilteredCorrelation;

namespace CorrelationWeb
{
    public partial class _Default : System.Web.UI.Page
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
            String[] properties = analyst.getProperties();


            Response.Write("{\"aaData\":[");

            for (int i = 0; i < result.Count; ++i)
            {
                CriterionPattern p = (CriterionPattern)result[i];

                String json = "[" +
                              "\"" + p.getCriterion(properties) + "\"," +
                              "\"" + p.getColumns(properties) + "\"," +
                              "\"" + p.Count + "\"," +
                              "\"" + String.Format("{0:P}", p.Support) + "\"," +
                              "\"" + String.Format("{0:P}", p.Confidence) + "\"" +
                              "]";
                if (i < result.Count - 1)
                {
                    json += ",";
                }
                Response.Write(json);

            }
            Response.Write("]}");
        }
    }
}