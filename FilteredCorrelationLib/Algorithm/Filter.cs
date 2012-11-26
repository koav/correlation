using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Collections;

using FilteredCorrelation.Data;
using FilteredCorrelation.Algorithm.FPTree;

namespace FilteredCorrelation.Algorithm
{

    class Filter
    {
        private DataProvider provider;

        private int totalRecords;
        private int[] criterion;
//        private int[] criterionSupport;

        private int[] filtered;
        private String[] propertyNames;
        private String[] attributeNames;
        private int[] columnsCount;
        private String[] colNames;

        private int qualifiedPropertyCount;
        private Hashtable attrs;

        private ArrayList tree;

        public Filter(String dataSource)
        {
            this.provider = new DataProvider(dataSource);
            this.attrs = new Hashtable();
        }

        public String[] getAttributeNames()
        {
            String[] result = new String[provider.getAttributeCount()];
            for (int i = 0; i < result.Length; ++i)
                result[i] = provider.getAttributeName(i);

            return result;
        }

        public String[] getColNames()
        {
            return colNames;
        }

        public ArrayList getTree()
        {
            return tree;
        }

        public ArrayList analysis(int criterionAttr, double diffRatio, double support)
        {
            if (criterionAttr < 0 || criterionAttr >= provider.getAttributeCount())
            {
                return new ArrayList();
            }

            this.filter(diffRatio, criterionAttr);
            return this.applayCorrelation(support);
        }

        public int[] getCriterions()
        {
            return this.criterion;
        }

        private int[] filter(double diffRatio, int criterionAttr)
        {
            provider.open();
            // calc count of each proerty
            CountObj[] values = this.countEachProperty(provider, criterionAttr);
            provider.close();


            // filter attribute
            int attributeCount = provider.getAttributeCount();
            bool[][] attributeQualified = new bool[attributeCount][];
            for (int i = 0; i < attributeCount; ++i)
            {
                int[] attrValues = provider.getAttributeValues(i);
                double[][] attrRatio = new double[values.Length][];
                for (int j = 0; j < values.Length; ++j)
                {
                    int[] attrValCount = values[j].attributeToPropety(i, attrValues);
                    attrRatio[j] = this.calcRatio(attrValCount, values[j].getCount());             
                }
                attributeQualified[i] = filterAttrValues(attrRatio, diffRatio);
            }

            // filter property
            int propertyCount = values[0].getNumOfProperties();
            double[][] propertyRatio = new double[values.Length][];
            for (int i = 0; i < propertyRatio.Length; ++i)
                propertyRatio[i] = calcRatio(values[i].getPropertiesCount(), values[i].getCount());

            bool[] propertyQualified = filterProperties(propertyRatio, diffRatio);

            // merge qualified properties and attribute values
            ArrayList list = new ArrayList();
            for (int i = 0; i < propertyQualified.Length; ++i)
            {
                if (propertyQualified[i])
                {
                    list.Add(i);
                }
            }
            this.qualifiedPropertyCount = list.Count;

            for (int i = 0; i < attributeQualified.Length; ++i)
            {
                if (i == criterionAttr)
                {
                    this.criterion = new int[values.Length];
                    for (int j = 0; j < criterion.Length; ++j)
                        criterion[j] = list.Count + j;
                }

                for (int j = 0; j < attributeQualified[i].Length; ++j)
                {
                    if (attributeQualified[i][j])
                    {
                        this.attrs.Add(list.Count, new int[] { i, j });
                        list.Add(list.Count);
                    }
                }
            }

            this.filtered = new int[list.Count];
            for (int i = 0; i < this.filtered.Length; ++i)
            {
                this.filtered[i] = (int)list[i];
            }


            this.columnsCount = new int[this.filtered.Length];
            for (int i = 0; i < this.qualifiedPropertyCount; ++i)
            {
                int index = this.filtered[i];
                for (int cr = 0; cr < values.Length; ++cr)
                {
                    columnsCount[i] += values[cr].getPropertyCount(index);
                }
            }

            for (int i = this.qualifiedPropertyCount; i < this.filtered.Length; ++i)
            {
                int[] index = (int[])attrs[this.filtered[i]];
                for (int cr = 0; cr < values.Length; ++cr)
                {
                    columnsCount[i] += values[cr].getAttrValCount(index[0], provider.getAttributeValues(index[0])[index[1]]);
                }
            }
            return this.filtered;
        }

        private ArrayList applayCorrelation(double support)
        {
            colNames = new String[filtered.Length];
            for (int i = 0; i < this.qualifiedPropertyCount; ++i)
                colNames[i] = this.propertyNames[this.filtered[i]];
            for (int i = this.qualifiedPropertyCount; i < this.filtered.Length; ++i)
            {
                int[] index = (int[])this.attrs[i];
                colNames[i] = String.Format("{0}_{1}", this.attributeNames[index[0]], provider.getAttributeValues(index[0])[index[1]]);
            }



            FP fp = new FP(colNames, totalRecords, columnsCount);

            fp.sortCols();


            provider.open();
            provider.startTraverse();

            while (provider.next())
            {
                Boolean[] row = new Boolean[filtered.Length];
                for (int i = 0; i < this.qualifiedPropertyCount; ++i)
                {
                    row[i] = provider.getPropertyVal(i);
                }

                for (int i = this.qualifiedPropertyCount; i < this.filtered.Length; ++i)
                {
                    int[] index = (int[])this.attrs[i];
                    row[i] = (provider.getAttributeVal(index[0]) == provider.getAttributeValues(index[0])[index[1]]);
                }
                
                fp.insertRecord(row);
            }

            provider.endTraverse();
            provider.close();

            
            ArrayList result = fp.analysis(support, this.criterion);
            tree = fp.getTree();
            return result;
        }

        private CountObj[] countEachProperty(DataProvider provider, int criterionAttr)
        {
            provider.startTraverse();

            // get name of each attribute
            int attributeCount = provider.getAttributeCount();
            this.attributeNames = new String[attributeCount];
            for (int i = 0; i < attributeCount; ++i)
            {
                attributeNames[i] = provider.getAttributeName(i);
            }

            // get name of each property
            int propertyCount = provider.getPropertyCount();
            this.propertyNames = new String[propertyCount];
            for (int i = 0; i < propertyCount; ++i)
            {
                propertyNames[i] = provider.getPropertyName(i);
            }

            Hashtable valCounter = new Hashtable();
            this.totalRecords = 0;
            while (provider.next())
            {
                totalRecords += 1;
                int key = provider.getAttributeVal(criterionAttr);
                if (!valCounter.ContainsKey(key))
                {
                    valCounter.Add(key, new CountObj(attributeCount, propertyCount));
                }
                CountObj counter = (CountObj)valCounter[key];
                counter.addCount();

                // count each attribute value
                for (int i = 0; i < attributeCount; ++i)
                {
                    int val = provider.getAttributeVal(i);
                    counter.addAttributeCount(i, val);
                }

                // count each property value
                for (int i = 0; i < propertyCount; ++i)
                {
                    if (provider.getPropertyVal(i))
                    {
                        counter.addPropertyCount(i);
                    }
                }
            }
            provider.endTraverse();

            // get each attribute's val count
//            this.criterion = new int[valCounter.Count];
//            this.criterionSupport = new int[valCounter.Count];
            CountObj[] values = new CountObj[valCounter.Count];

            IEnumerator e = valCounter.Keys.GetEnumerator();
            int index = 0;
            while (e.MoveNext())
            {
//                criterion[index] = (int)e.Current;
                values[index] = (CountObj)valCounter[e.Current];
//                criterionSupport[index] = values[index].getCount();
                index += 1;
            }

            return values;
        }


        private double[] calcRatio(int[] count, int total)
        {
            double[] result = new double[count.Length];
            for (int i = 0; i < count.Length; ++i)
            {
                result[i] = count[i] / (double)total;
            }

            return result;
        }

        private bool[] filterAttrValues(double[][] attrValCountRatio, double minDiff)
        {
            bool[] result = new bool[attrValCountRatio[0].Length];

            for (int i = 0; i < result.Length; ++i)
            {
                for (int j = 0; j < attrValCountRatio.Length; ++j)
                {
                    for (int z = j + 1; z < attrValCountRatio.Length; ++z)
                    {
                        if (Math.Abs(attrValCountRatio[j][i] - attrValCountRatio[z][i]) >= minDiff)
                        {
                            result[i] = true;
                            break;
                        }
                    }
                    if (result[i])
                        break;
                }

            }
            return result;
        }

        private bool[] filterProperties(double[][] propertyCountRatio, double minDiff)
        {
            bool[] result = new bool[propertyCountRatio[0].Length];

            for (int i = 0; i < result.Length; ++i)
            {
                for (int j = 0; j < propertyCountRatio.Length; ++j)
                {
                    for (int z = j + 1; z < propertyCountRatio.Length; ++z)
                    {
                        if (Math.Abs(propertyCountRatio[j][i] - propertyCountRatio[z][i]) >= minDiff)
                        {
                            result[i] = true;
                            break;
                        }
                    }
                    if (result[i])
                        break;
                }
            }
            return result;
        }

        private void printArray(int[] array)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                Console.Write(array[i] + " ");
            }
            Console.WriteLine();
        }

        public void insert()
        {
            String connectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=D:\\projects\\vs2010\\DBFiles\\correlation\\Correlation.db.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            Random random = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < 1000; ++i)
            {
                object[] values = new object[14];
                values[0] = random.Next(3) + 1;
                for (int j = 1; j < 14; ++j)
                {
                    values[j] = random.Next(2);
                }


                String sql = String.Format("INSERT INTO fire(degree, tree, traffic_lights, bridge, shopping_mall,factory," +
                                  "restaurant, parking, transformer, hospital, school, hotel, bathroom, gas_station)" +
                                  "VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')", values);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }
    }

    class CountObj
    {
        private int count;
        private int[] propertiesCount;
        private Hashtable[] attributesCount;

        public CountObj(int attributes, int properties)
        {
            this.propertiesCount = new int[properties];
            this.attributesCount = new Hashtable[attributes];
            for (int i = 0; i < attributes; ++i)
            {
                attributesCount[i] = new Hashtable();
            }
        }

        public int getCount()
        {
            return this.count;
        }

        public void addCount()
        {
            count += 1;
        }

        public void addAttributeCount(int index, int attribute)
        {
            if (attributesCount[index].ContainsKey(attribute))
            {
                attributesCount[index][attribute] = (int)attributesCount[index][attribute] + 1;
            }
            else
            {
                attributesCount[index].Add(attribute, 1);
            }
        }

        public void addPropertyCount(int index)
        {
            this.propertiesCount[index] += 1;
        }

        public int[] getPropertiesCount()
        {
            return this.propertiesCount;
        }

        public int getNumOfProperties()
        {
            return this.propertiesCount.Length;
        }

        public int getPropertyCount(int i)
        {
            return this.propertiesCount[i];
        }

        public int getAttrValCount(int attrIndex, int val)
        {
            if (attributesCount[attrIndex].ContainsKey(val))
                return (int)attributesCount[attrIndex][val];
            else
                return 0;
        }

        public int[] attributeToPropety(int attributeIndex, int[] values)
        {
            int[] result = new int[values.Length];
            for (int i = 0; i < values.Length; ++i)
            {
                if (attributesCount[attributeIndex].ContainsKey(values[i]))
                {
                    result[i] = (int)attributesCount[attributeIndex][values[i]];
                }
            }
            return result;
        }
    }
}
