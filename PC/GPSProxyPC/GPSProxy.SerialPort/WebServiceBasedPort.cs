using System;
using System.Threading;

using GPSProxy.SerialPort.ServiceWrapper;

namespace GPSProxy.SerialPort
{
    
    public class WebServiceBasedPort : IPort, IDisposable
    {

        #region Data members
        private bool disposed = false;

        GPSManagerClient serviceClient = new GPSManagerClient();
        PathInfo pathInfo = new PathInfo();
        String dataProvider = "N/A";
        Int32 lastDataID = -1;
        bool bIsReplay = false; // true: Replay the target path; false: only get the latest data.

        private bool bIsRead;
        private Timer readTimer;
        #endregion

        #region Ctor
        public WebServiceBasedPort(Int32 pathID, String pathPWD, String provider, bool isRead, bool isReplay)
        {
            pathInfo.ID = pathID;
            pathInfo.Password = pathPWD;
            dataProvider = provider;
            bIsRead = isRead;
            bIsReplay = isReplay;

            readTimer = new Timer(new TimerCallback(ReadTimerCallback), null, Timeout.Infinite, Timeout.Infinite);

        }

        ~WebServiceBasedPort()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (! disposed) 
			{
				if (disposing)
				{
                    readTimer.Dispose();
				}

				disposed = true;
			}
        }

        #endregion

        #region Read data from web service
        private void ReadTimerCallback(object state)
        {
            if(IsOpen())
            {
                try
                {
                    GPSDataRequestParameter reqPara = new GPSDataRequestParameter();
                    reqPara.LastDataID = lastDataID;
                    reqPara.MaxLines = 10;
                    reqPara.LatestDataOnly = !bIsReplay;
                    reqPara.PathID = pathInfo.ID;
                    reqPara.PathPassword = pathInfo.Password;

                    GPSDownloadData[] receivedDatas = serviceClient.GetGPSData(reqPara);
                    if (receivedDatas != null)
                    {
                        foreach (GPSDownloadData dataItem in receivedDatas)
                        {
                            if (dataItem != null)
                            {
                                // Update the id
                                if (lastDataID < dataItem.ID)
                                    lastDataID = dataItem.ID;

                                if (dataItem.NMEASentence.Length == 0)
                                    continue;

                                byte[] data = new byte[System.Text.Encoding.Default.GetByteCount(dataItem.NMEASentence)];
                                data = System.Text.Encoding.Default.GetBytes(dataItem.NMEASentence);

                                // Dispatch the received data.
                                if (data != null)
                                {
                                    if (Read != null)
                                        Read(this, data);
                                }
                            }
                        }
                    }
                }
                catch (System.Exception )
                {
                	// Don't exit if failed.
                }                  

                // ToDO - for the replay model, we should use the time tamp of the gps data.
                try
                {
                    // There is exception when close the port during receiving process.
                    if (!disposed)
                        readTimer.Change(1000, Timeout.Infinite); // Read data every second
                }
                catch (System.Exception)
                {

                }
            }
        }
        #endregion

        #region IPort Members

        public event PortReadEvent Read;

        public event PortErrorEvent Error;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Open()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            try
            {
                lastDataID = -1;
                if (serviceClient.IsServiceAvailable("PCClient"))
                {
                    if(bIsRead)
                    {
                        readTimer.Change(0, Timeout.Infinite);
                    }
                    return true;
                }
            }
            catch (System.Exception)
            {
                throw;
            }

            return false;
        }

        public bool Close()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            readTimer.Change(Timeout.Infinite, Timeout.Infinite);

            return true;
        }

        public bool IsOpen()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            return serviceClient.IsServiceAvailable("PCClient");
        }

        public int Write(byte[] data)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            try
            {
                if (IsOpen() && data.Length > 0)
                {
                    GPSUploadData gpsData = new GPSUploadData();
                    gpsData.PathID = pathInfo.ID;
                    gpsData.PathPassword = pathInfo.Password;
                    gpsData.Provider = dataProvider;
                    gpsData.NMEASentence = System.Text.Encoding.Default.GetString(data, 0, data.Length);

                    bool ret = serviceClient.UploadGPSData(gpsData);

                    if (ret)
                        return data.Length;
                }
            }
            catch (System.Exception e)
            {
                // Don't exit if failed.       	
            }

            return 0;
        }

        #endregion
    }
}
