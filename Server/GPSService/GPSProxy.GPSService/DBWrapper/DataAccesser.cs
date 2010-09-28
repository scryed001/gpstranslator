using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPSProxy.GPSService.DBWrapper.GPSDBTableAdapters;

namespace GPSProxy.GPSService.DBWrapper
{
    class DataAccesser
    {
        public DataAccesser()
        {
            mPathTableAdapter = new PathTableAdapter();
            String assName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            String ApplicationPath = System.IO.Path.GetDirectoryName(assName);
            mPathTableAdapter.Connection.ConnectionString = "Data Source=" + ApplicationPath + "\\GPS.sdf";
        }

        PathTableAdapter mPathTableAdapter;

        public bool AddNewPath(String pathName, String pathPwd, String creator)
        {

            // ToDo - update the table definition.
            mPathTableAdapter.Insert(0, pathName, pathPwd, creator, null ,"", null, true);
            
            return true;
        }

        #region Encode/Decode - Avoid SQL injection
        // % -- &37;
        public String Encode(String str)
        {
            if (str == null)
                return "";

            String EncodedStr = str.Replace("%", "&37;"); // Replace with the ASCII code.

            return EncodedStr;
        }

        public String Decode(String str)
        {
            if (str == null)
                return "";

            String DecodedStr = str.Replace("&37;", "%"); // Replace with the ASCII code.

            return DecodedStr;
        }
        #endregion
    }
}
