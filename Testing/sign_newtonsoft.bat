cd "Docs"
Humanlights.SignTool.exe unsign /f -folder "TestingDlls"
Humanlights.SignTool.exe sign overwrite /f /da SHA1 -file "TestingDlls/Newtonsoft.Json.dll" -certificate "github.pfx" -altcertificate "github.cer" +password "1234"