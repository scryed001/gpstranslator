using System;

namespace GPSProxy.SerialPort
{
    public delegate void PortReadEvent(IPort sender, byte[] data);
    public delegate void PortErrorEvent(IPort sender, string error);

    /// <summary>
    /// Summary description for IPort.
    /// </summary>
    public interface IPort
    {
        event PortReadEvent Read;
        event PortErrorEvent Error;

        void Dispose();

        bool Open();
        bool Close();
        bool IsOpen();

        int Write(byte[] data);
    }
}
