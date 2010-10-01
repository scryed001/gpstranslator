using System;

using System.IO.Ports;

namespace GPSProxy.SerialPort
{
    public class MSPort : IPort, IDisposable
    {
        #region Data members
        private bool disposed = false;

        private System.IO.Ports.SerialPort serialPort;
        #endregion

        #region Ctor
        public MSPort(string portName, int baudRate)
		{
            serialPort = new System.IO.Ports.SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;

            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;

            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(serialPort_ErrorReceived);

        }

        ~MSPort()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                }
                disposed = true;
            }
        }
        #endregion

        #region Data read and write
        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            // do nothing
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Byte[] data = new Byte[serialPort.BytesToRead];

            serialPort.Read(data, 0, data.Length);

            if (Read != null && data.Length > 0)
                Read(this, data);
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
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }

        public bool Close()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (serialPort.IsOpen)
                serialPort.Close();

            return true;
        }

        public bool IsOpen()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            return serialPort.IsOpen;
        }

        public int Write(byte[] data)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

           if(IsOpen() && data.Length > 0)
           {
               serialPort.Write(data, 0, data.Length);
               return data.Length;
           }
           return 0;
        }

        #endregion
    }
}
