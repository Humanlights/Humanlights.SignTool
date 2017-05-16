cd "Docs"
Humanlights.SignTool.exe /unsign /full -folder "TestingDlls"
Humanlights.SignTool.exe /sign /full -file "TestingDlls/Newtonsoft.Json.dll" -certificate "github.cer" 