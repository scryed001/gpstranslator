When the web service address is changed, the end point address in the app.config should be updated.

The content of app.config will be copied to GPSProxyPC.exe.config after building, which will be used by GPSProxy.SerialPort to configure the web service client. So just configure the service or update the end point in the app.config of the GPSProxy.SerialPort isn't enough.

<endpoint address="http://10.148.135.117/GPS/GPSManager.svc" binding="basicHttpBinding"
                bindingConfiguration="BasicHttpBinding_IGPSManager" contract="ServiceWrapper.IGPSManager"
                name="BasicHttpBinding_IGPSManager" />
