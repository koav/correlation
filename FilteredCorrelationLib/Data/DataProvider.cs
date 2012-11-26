using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

//using System.Data.SqlClient;
using System.Collections;
using System.Data.SQLite;

namespace FilteredCorrelation.Data
{
    public class DataProvider
    {
        private String connectionString;
        private String table = "fire";
        private int offset = 0;
        private int slice = 1000;

        // a column of row_number
        // a column of id
        private int columnOffset = 2;
        private int attributesCount = 3;
        private int[][] attributeVals;

        private SQLiteConnection conn;
        private SQLiteDataReader reader;
        private Boolean nextSlice;

        public DataProvider(String dataSource)
        {
            // build connection string
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = dataSource;
            connectionString = builder.ToString();

            // 火险等级 1, 2, 3
            // 救援速度 1, 2, 3, 4, 5
            // 温度范围 1, 2
            attributeVals = new int[3][];
            attributeVals[0] = new int[]{1, 2, 3};
            attributeVals[1] = new int[]{1, 2, 3, 4, 5};
            attributeVals[2] = new int[]{1, 2};
        }

        public Boolean open()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                return true;
            }

            conn = new SQLiteConnection(connectionString);
            if (conn == null)
            {
                return false;
            }
            conn.Open();
            return true;
        }

        public void close()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        public void startTraverse()
        {
            this.offset = 0;
            this.nextSlice = true;
            this.queryNextSlice();
        }

        public void endTraverse()
        {
            if (reader != null)
            {
                reader.Close();
            }
        }

        public int[] getAttributeValues(int i)
        {
            return this.attributeVals[i];
        }

        public int getPropertyCount()
        {
            return reader.FieldCount - columnOffset - attributesCount;
        }

        public String getPropertyName(int i)
        {
            return reader.GetName(i + columnOffset + attributesCount);
        }

        public int getAttributeCount()
        {
            return this.attributesCount;
        }

        public String getAttributeName(int i)
        {
            return reader.GetName(i + columnOffset);
        }

        public Boolean next()
        {
            if (reader.Read())
            {
                return true;
            }
            else if (nextSlice)
            {
                this.queryNextSlice();
                nextSlice = reader.Read();
                return nextSlice;
            }
            return false;
        }

        public Boolean getPropertyVal(int i)
        {
            return this.reader.GetBoolean(i+ columnOffset + attributesCount);
        }

        public int getAttributeVal(int i)
        {
            return this.reader.GetInt32(i + columnOffset);
        }

        private void queryNextSlice()
        {
            if (reader != null)
            {
                reader.Close();
            }

            //String sql = String.Format("WITH result_res AS(" +
            //            "SELECT ROW_NUMBER() OVER (ORDER BY id) as row_number, * FROM " + table + 
            //            ") SELECT * FROM result_res WHERE row_number > {0} AND row_number <= {1}",
            //            offset, offset + slice);

            String sql = String.Format("SELECT ROWID, * FROM fire LIMIT {0} OFFSET {1}",
                                            slice, offset);


            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();

            offset += slice;
        }
    }
}
