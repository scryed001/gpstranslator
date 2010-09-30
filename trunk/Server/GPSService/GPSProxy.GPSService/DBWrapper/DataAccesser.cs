﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq.Expressions;
using System.Reflection;

using GPSProxy.GPSService;

// Use the following code to generate the linq wrapper.
// "C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\Sqlmetal.exe" "D:\GPSService\GPSProxy.GPSService\GPS.sdf" /code:"D:\GPSService\GPSProxy.GPSService\DBWrapper\GPSLinqWrapper.cs"
// "C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\Sqlmetal.exe" "G:\sunzhongkui\code\gpstranslator\Server\GPSService\GPSProxy.GPSService\GPS.sdf" /code:"G:\sunzhongkui\code\gpstranslator\Server\GPSService\GPSProxy.GPSService\DBWrapper\GPSLinqWrapper.cs"
// Reference: http://blogs.msdn.com/b/sqlservercompact/archive/2007/08/21/linq-with-sql-server-compact-a-ka-dlinq-over-sql-ce.aspx

namespace GPSProxy.GPSService.DBWrapper
{
    class DataAccesser
    {
        public DataAccesser()
        {
            String assName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            String ApplicationPath = System.IO.Path.GetDirectoryName(assName);
            String connectionString = "Data Source=" + ApplicationPath + "\\GPS.sdf";

            // Just for test
            //connectionString = GPSProxy.GPSService.Properties.Settings.Default.GPSConnectionString;

            mGPSDB = new GPS(connectionString);
        }

        private GPS mGPSDB;
        private static int maxReturnedSentence = 2;         

        public bool AddNewPath(String pathName, String pathPwd, String creator)
        {
            try
            {
                Int32 pathID = GetPathID(pathName);
                if (-1 != pathID)
                    return false;

                Path gpsPath = new Path();
                gpsPath.Name = Encode(pathName);
                gpsPath.Password = Encode(pathPwd);
                gpsPath.Added_By = Encode(creator);
                gpsPath.Added_On = DateTime.Now;
                gpsPath.Visible = true;

                mGPSDB.Path.InsertOnSubmit(gpsPath);

                mGPSDB.SubmitChanges();

                pathID = GetPathID(pathName);
                if (-1 == pathID)
                    return false;
            }
            catch (System.Exception)
            {
                return false;
            }
            
            return true;
        }

        public List<String> GetPathList(String searchString)
        {
            // Get the valid path list
            List<String> pathList = new List<string>();

            try
            {
                searchString = Encode(searchString);

                var paths = from item in mGPSDB.Path.ToList()
                              where item.Visible == true
                              && (item.Name.Contains(searchString))
                              select item.Name;

                pathList.AddRange(paths);
            }
            catch (System.Exception)
            {

            }

            return pathList;
        }

        public Int32 GetPathID(String pathName)
        {
            try
            {
                Path gpsPath = mGPSDB.Path.ToList().SingleOrDefault(x => (x.Name == Encode(pathName) && x.Visible == true));
                if (gpsPath != null)
                    return gpsPath.ID;
            }
            catch (System.Exception)
            {
               
            }

            return -1;
        }

        public Int32 GetPathID(String pathName, String pathPwd)
        {
            try
            {
                Path gpsPath = mGPSDB.Path.ToList().SingleOrDefault(x => (x.Name == Encode(pathName) && x.Password == Encode(pathPwd) && x.Visible == true));
                if (gpsPath != null)
                    return gpsPath.ID;
            }
            catch (System.Exception)
            {

            }

            return -1;
        }

        public bool AddGPSSentence(String sentence, String creator, Int32 pathID)
        {
            try
            {
                PathDetail pathDetail = new PathDetail();
                pathDetail.Gpssentence = Encode(sentence);
                pathDetail.Added_by = Encode(creator);
                pathDetail.Pathid = pathID;
                mGPSDB.PathDetail.InsertOnSubmit(pathDetail);

                mGPSDB.SubmitChanges();
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }

        public List<GPSDownloadData> GetGPSData(GPSDataRequestParameter para)
        {
            
            List<GPSDownloadData> gpsDataList = new List<GPSDownloadData>();

            try
            {
                if(para.LatestDataOnly)
                {
                    var datas = (from item in mGPSDB.PathDetail.ToList()
                                 where (item.Id > para.LastDataID) && (item.Pathid == para.PathID)
                                 orderby item.Id descending
                                 select new GPSDownloadData() { ID = item.Id, NMEASentence = Decode(item.Gpssentence) }).Take(maxReturnedSentence);

                    gpsDataList.AddRange(datas);
                }
                else
                {
                    var datas = (from item in mGPSDB.PathDetail.ToList()
                                 where (item.Id > para.LastDataID) && (item.Pathid == para.PathID)
                                 orderby item.Id ascending
                                 select new GPSDownloadData() { ID = item.Id, NMEASentence = Decode(item.Gpssentence) }).Take(maxReturnedSentence);

                    gpsDataList.AddRange(datas);
                }

            }
            catch (System.Exception)
            {
            	
            }

            return gpsDataList;
        }

        private String GetPathDetailTableName(Int32 pathID )
        {
            String prefix = "_PathDetail_";
            return prefix + pathID.ToString();
        }

        // Reference: http://www.cnblogs.com/ansiboy/archive/2009/02/08/1386088.html
        private bool CreatePathDetailTable(Int32 pathID)
        {            
            String tablleName = GetPathDetailTableName(pathID);
            String templateTableName = "PathDetailTemplate";
            try
            {
                MetaTable metaTable = mGPSDB.Mapping.GetTable(typeof(PathDetail /*PathDetailTemplate*/));
                var typeName = "System.Data.Linq.SqlClient.SqlBuilder";
                var type = typeof(DataContext).Assembly.GetType(typeName);
                var bf = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
                var sql = type.InvokeMember("GetCreateTableCommand", bf, null, null, new[] { metaTable });
                String sqlState = sql.ToString().ToLower();
                sqlState = sqlState.ToString().Replace(templateTableName.ToLower(), tablleName.ToLower());
                mGPSDB.ExecuteCommand(sqlState);
            }
            catch (System.Exception)
            {
                return false;
            }
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
