using System;


namespace GPSProxy.SerialPort
{
    public class PortRedirector : IDisposable
    {
        #region Data members
        private IPort in_port;
        private IPort out_port;
        private bool disposed = false;
        #endregion

        #region Ctor
        public PortRedirector(IPort inPort, IPort outPort)
        {
            in_port = inPort;
            out_port = outPort;            
		}

		~PortRedirector()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (! disposed) 
			{
				if (disposing)
				{
                    // do nothing
				}

				disposed = true;
			}
        }

        #endregion

        #region start and stop
        public void Start()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            try
            {
                if(in_port != null)
                {
                    in_port.Read += new PortReadEvent(in_port_Read);
                    in_port.Error += new PortErrorEvent(in_port_Error);
                    in_port.Open();
                }

                if (out_port != null)
                {
					out_port.Read += new PortReadEvent(out_port_Read);
					out_port.Error += new PortErrorEvent(out_port_Error);
					out_port.Open();
                }
            }
            catch
            {

                if (in_port != null)
                {
                    in_port.Close();
                 
                }
                if (out_port != null)
                {
                    out_port.Close();
                }
                throw;
            }
        }

        public void Stop()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (in_port != null)
            {
                in_port.Close();                
            }


            if (out_port != null)
            {
                out_port.Close();                
            }
        }

        private void in_port_Read(IPort sender, byte[] data)
        {
            if (out_port != null && out_port.IsOpen())
                out_port.Write(data);           
        }

        private void in_port_Error(IPort sender, string error)
        {
            // do nothing
        }

        private void out_port_Read(IPort sender, byte[] data)
        {
            if (in_port != null && in_port.IsOpen())
                in_port.Write(data);    
        }

        private void out_port_Error(IPort sender, string error)
        {
            // do nothing
        }
        #endregion
    }
}
