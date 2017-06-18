cd "Docs"
Humanlights.SignTool.exe sign /f /da SHA1 -folder "TestingDlls" -certificate "github.pfx" -altcertificate "github.cer" +password "1234"