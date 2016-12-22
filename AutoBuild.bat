rd .\BuildResults /S /Q
md .\BuildResults


cd "C:\Program Files (x86)\MSBuild\14.0\Bin"
REM set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v3.5
call msbuild.exe  "D:\projects\C#\VkMessenger\Messenger.sln" /p:Configuration=Debug /l:FileLogger,Microsoft.Build.Engine;logfile="D:\projects\C#\VkMessenger\Messenger.log" /m
