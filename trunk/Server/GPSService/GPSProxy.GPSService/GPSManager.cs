using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GPSProxy.GPSService.DBWrapper;

namespace GPSProxy.GPSService
{
    // NOTE: If you change the class name "GPSManager" here, you must also update the reference to "GPSManager" in App.config.
    public class GPSManager : IGPSManager
    {
        private DataAccesser mDataAccesser = new DataAccesser();

        public bool CreateNewPath(PathInfo path, UserInfo user)
        {
            if (null == path || null == user)
                return false;
            if (null == path.Name
                || null == user.Name) // user password can be null
                return false;

            String pathName = path.Name.Trim();
            if (pathName.Length == 0)
                return false;

            String pathPassword = "";
            if(null != path.Password)
                pathPassword = path.Password;

            // Verify the user.

            // Create path

            return mDataAccesser.AddNewPath(pathName, pathPassword, user.Name);
        }

        public List<String> GetPathList(String searchString, UserInfo user)
        {
            // Verify the user.
            if (null == searchString)
                searchString = "";

            // ToDo - we need check the user validation.

            // Get the valid path list

            return mDataAccesser.GetPathList(searchString);
        }

        public Int32 GetPathID(PathInfo path)
        {
            if (null == path)
                return -1;
            if (null == path.Name) // user password can be null
                return -1;

            String pathName = path.Name.Trim();
            if (pathName.Length == 0)
                return -1;

            String pathPwd = "";
            if (null != path.Password)
                pathPwd = path.Password;

            return mDataAccesser.GetPathID(pathName, pathPwd);
        }

        public bool UploadGPSData(GPSUploadData data)
        {
            // Verify the path info. 
            if (null == data)
                return false;

            if (-1 == data.PathID)
                return false;
            // ToDo we need check the id and password.

            if (null == data.NMEASentence || null == data.Provider)
                return false;

            if (data.NMEASentence.Length == 0)
                return false;

            // Save the gps data.
            return mDataAccesser.AddGPSSentence(data.NMEASentence, data.Provider, data.PathID);
        }

        /// <summary>
        /// Return the last 10 sentences at most whose IDs are later than the lastID.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lastID">if it is -1,qurey from the first sentence</param>
        /// <returns></returns>
        public List<GPSDownloadData> GetGPSData(PathInfo path, Int32 lastID)
        {
            // Verify the path info. 
            if (null == path)
                return null;

            if (-1 == path.ID)
                return null;

            // ToDo we need check the id and password.

            // Get the gps data.

            return mDataAccesser.GetGPSData(path, lastID);

        }
    }
}
