rd .\BuildResults /S /Q
md .\BuildResults

REM set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v3.5
call "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"  "D:\projects\C#\VkMessenger\Messenger.sln" /p:Configuration=Debug /l:FileLogger,Microsoft.Build.Engine;logfile="D:\projects\C#\VkMessenger\Messenger.log" /m
rd "D:\projects\C#\VkMessenger\Messenger.Wpf\bin\Debug\Андрій Шевченко\storage\history"
chdir "D:\projects\C#\VkMessenger\Messenger.Wpf\bin\Debug"
call "VkMessenger.exe"