using System;
using System.Collections.Generic;
using System.Text;

using System.Data.SQLite;

namespace FilteredCorrelation
{
    public class DBManager
    {
        private String connectionString;
        private String table = "fire";
        private SQLiteConnection conn;


        public DBManager(String dataSource)
        {
            // build connection string
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = dataSource;
            connectionString = builder.ToString();
        }

        public bool open()
        {
            try
            {
                conn = new SQLiteConnection(connectionString);
                conn.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void close()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        public int delete()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM " + table;
            return cmd.ExecuteNonQuery();
        }

        public int insert(object[] data)
        {
            if (data.Length < 17)
                return 0;

            SQLiteCommand cmd = conn.CreateCommand();        
            String sql = String.Format("INSERT INTO " + table + "(火险等级,救援速度,气温范围,树,交通灯,桥,购物中心,工厂,饭店,停车场,变压器,医院,学校,宾馆,浴室,加油站,垃圾场)" +
                   "VALUES('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}', '{14}', '{15}', '{16}')", data);
            cmd.CommandText = sql;
            return cmd.ExecuteNonQuery();
        }
    }
}
