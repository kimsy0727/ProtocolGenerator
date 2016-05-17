using System;
using System.Collections;
using System.Data;
using System.Xml;

namespace Common
{
    public class XMLManager
    {
        // xml 파일 읽어오는 경로
        private string mPath;
        public XMLManager(string path)
        {
            mPath = path;
        }
        public DataSet Load()
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(mPath);

                return dataSet;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return null;
            }
        }
        public Boolean Save(String url, Hashtable map)
        {
            return true;
        }
    }
}