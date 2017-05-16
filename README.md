# Humanlights.SignTool
A custom signtool.exe that is able to digitally sign EXE, DLL, OCX, and CAB files and also unsign them.

## Usage
Using a batch file. The following are some real examples of how you could use the application and also command line arguments documentation.

### Command Line Arguments
<code>/sign</code>: This activates the method of signing mode (if /sign and /unsign are both set, runs just /sign).

<code>/unsign</code>: This activates the method of unsigning mode.

<code>-folder</code>: The root folder path of it's files and it's subdirectories' files are going to be signed.

<code>-file</code>: The file in particular that is going to be signed (if valid. If not it'll show a warning).

<code>-certificate</code>: The certificate. Little trick I made: if the certificate is in the same folder as the SignTool, you can type just the file name and not the full path, otherwise the full path (both will work).

<code>-timestamp</code>: The default timestamp is "http://timestamp.verisign.com/scripts/timstamp.dll" but you can override it anytime with this argument.

<code>/full</code>: This is more for the design. If set, it'll print the full path of each file rather just the file name as default.

Note: If both folder and file are set, both are going to run, but first the folder one.

## Examples
The example is placed in the same folder where Humanlights.SignTool.exe is which is accompanied by signtool.exe and the certificate.
The SignTool is customizable and made user-friendly for quick usage, so you can sign all the files that a folder and it's subfolders' contains or just a file manually.

### Signing
This signs a full folder and all it's subdirectories' files that have the extensions: *.exe, *.dll, *.ocx and *.cab.
<p><code>Humanlights.SignTool.exe /sign -folder "your/path" -certificate "certificate.cer"</code></p>

This signs a file only that has one of these extensions: *.exe, *.dll, *.ocx and *.cab.
<p><code>Humanlights.SignTool.exe /sign -file "your/path/app.exe" -certificate "certificate.cer"</code></p>

#### Other tests
<p><code>Humanlights.SignTool.exe /sign /full -file "your/path/app.exe" -certificate "certificate.cer" -timestamp "http://timestamp.verisign.com/scripts/timstamp.dll"</code></p>

<p><code>Humanlights.SignTool.exe /sign /full -folder "your/path/" -certificate "certificate.cer" -timestamp "http://timestamp.verisign.com/scripts/timstamp.dll"</code></p>

### Unsigning
This unsigns a full folder and all it's subdirectories' files that have the extensions: *.exe, *.dll, *.ocx and *.cab.
<p><code>Humanlights.SignTool.exe /unsign -folder "your/path"</code></p>

This removes the signing of a file only that has one of these extensions: *.exe, *.dll, *.ocx and *.cab.
<p><code>Humanlights.SignTool.exe /unsign /full -file "your/path/app.exe"</code></p>

## Notes
- Currently I've only tested with *.cer certificate files. If any trouble with using it, let me know.
- "signtool.exe" MUST BE in the same folder as Humanlights.SignTool.exe

## License
See [license][License] (hint: MIT).

[License]: https://github.com/Humanlights/Humanlights.SignTool/blob/master/LICENSE