When the web service address is changed, the end point address in the app.config should be updated.

<endpoint address="http://10.148.135.117/GPS/GPSManager.svc" binding="basicHttpBinding"
                bindingConfiguration="BasicHttpBinding_IGPSManager" contract="ServiceWrapper.IGPSManager"
                name="BasicHttpBinding_IGPSManager" />
