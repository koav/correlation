using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Configuration;
using FilteredCorrelation;

namespace CorrelationWeb.backend
{
    public partial class insert : System.Web.UI.Page
    {
        private String dataSource;
        protected void Page_Load(object sender, EventArgs e)
        {
            dataSource = WebConfigurationManager.AppSettings["dataSource"];
            int properties = 14;
            int records = getValue("records");

            if (records == 0)
            {
                Response.Write("数据生成失败，总条数必须大于0");
                return;
            }

            int[] fire = new int[3];
            fire[0] = getValue("fire_1");
            fire[1] = getValue("fire_2");
            fire[2] = getValue("fire_3");

            int[] rescue = new int[5];
            rescue[0] = getValue("rescue_1");
            rescue[1] = getValue("rescue_2");
            rescue[2] = getValue("rescue_3");
            rescue[3] = getValue("rescue_4");
            rescue[4] = getValue("rescue_5");

            int[] temp = new int[2];
            temp[0] = getValue("temp_1");
            temp[1] = getValue("temp_2");

            int[] cond = new int[properties];
            //int tree 
            cond[0] = getValue("tree");
            //int traffic 
            cond[1] = getValue("traffic");
            //int bridge
            cond[2] = getValue("bridge");
            //int mall 
            cond[3] = getValue("mall");
            //int factory 
            cond[4] = getValue("factory");
            //int restaurant 
            cond[5] = getValue("restaurant");
            //int parking
            cond[6] = getValue("parking");
            //int transformer 
            cond[7] = getValue("transformer");
            //int hospital 
            cond[8] = getValue("hospital");
            //int school
            cond[9] = getValue("school");
            //int hotel
            cond[10] = getValue("hotel");
            //int bath
            cond[11] = getValue("bath");
            //int gas_station
            cond[12] = getValue("gas_station");
            //int waste
            cond[13] = getValue("waste");

            insert_db(records, fire, rescue, temp, cond);
        }

        private void insert_db(int total, int[] fire, int[] rescue, int[] temp, int[] prob)
        {
            DBManager manager = new DBManager(dataSource);
            manager.open();
            manager.delete();

            Random rand = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
            int attributes = 3;
            object[] result = new object[attributes + prob.Length];
            int[] stat = new int[result.Length];

            int inserted = 0;
            for (int i = 0; i < total; ++i)
            {
                // random data
                int r_fire = rand.Next(100);
                if (r_fire < fire[0])
                    result[0] = 1;
                else if (r_fire < fire[0] + fire[1])
                    result[0] = 2;
                else
                    result[0] = 3;

                int r_rescue = rand.Next(100);
                if (r_rescue < rescue[0])
                    result[1] = 1;
                else if (r_rescue < rescue[0] + rescue[1])
                    result[1] = 2;
                else if (r_rescue < rescue[0] + rescue[1] + rescue[2])
                    result[1] = 3;
                else if (r_rescue < rescue[0] + rescue[1] + rescue[2] + rescue[3])
                    result[1] = 4;
                else
                    result[1] = 5;

                int r_temp = rand.Next(100);
                if (r_temp < temp[0])
                    result[2] = 1;
                else
                    result[2] = 2;
                for (int j = 0; j < prob.Length; ++j)
                {
                    int t = rand.Next(100);
                    result[attributes + j] = (t < prob[j]) ? 1 : 0;
                    stat[attributes + j] += (t < prob[j]) ? 1 : 0;
                }


                inserted += manager.insert(result);
            }

            manager.close();
            Response.Write("插入成功 " + inserted + " 条数据");
        }

        private int getValue(String name)
        {
            try
            {
                int result = Convert.ToInt32(Request[name]);
                if (result <= 0)
                    return 0;
                return result;
            }
            catch (Exception)
            {
                return 50;
            }
        }
    }
}