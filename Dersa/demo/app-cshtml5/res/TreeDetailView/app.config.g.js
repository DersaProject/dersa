window.AppConfig = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\\
<configuration>\\
    <system.serviceModel>\\
        <bindings>\\
            <basicHttpBinding>\\
                <binding name=\"BasicHttpBinding_ISqlService\" />\\
            </basicHttpBinding>\\
        </bindings>\\
        <client>\\
            <endpoint address=\"http://localhost:11433/\" binding=\"basicHttpBinding\"\\
                bindingConfiguration=\"BasicHttpBinding_ISqlService\" contract=\"SqlClientReference.ISqlService\"\\
                name=\"BasicHttpBinding_ISqlService\" />\\
        </client>\\
    </system.serviceModel>\\
</configuration>";