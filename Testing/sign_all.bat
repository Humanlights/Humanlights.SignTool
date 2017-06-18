cd "Docs"
Humanlights.SignTool.exe sign overwrite /f /da SHA1 -folder "TestingDlls" -certificate "github.pfx" -altcertificate "github.cer" +password "1234"