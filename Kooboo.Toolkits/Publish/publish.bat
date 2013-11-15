rd Released /Q /S
md Release
md Release\Kooboo.CMS.Toolkit\lib\
md Release\Kooboo.CMS.Toolkit.Controls\lib\

del *.log /Q /S

call update_version.vbs

cd ..

"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild" Kooboo.CMS.Toolkit.sln /t:rebuild /l:FileLogger,Microsoft.Build.Engine;logfile=Publish\Publish.log;

"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild" Kooboo.CMS.Toolkit\Kooboo.CMS.Toolkit.csproj /t:ResolveReferences;Compile /p:Configuration=Release /p:WebProjectOutputDir="Kooboo.CMS.Toolkit" /p:OutputPath="..\Publish\Release\Kooboo.CMS.Toolkit\lib" /l:FileLogger,Microsoft.Build.Engine;logfile=Publish\Publish.log;
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild" Kooboo.CMS.Toolkit.Controls\Kooboo.CMS.Toolkit.Controls.csproj /t:ResolveReferences;Compile /p:Configuration=Release /p:WebProjectOutputDir="Kooboo.CMS.Toolkit.Controls" /p:OutputPath="..\Publish\Release\Kooboo.CMS.Toolkit.Controls\lib" /l:FileLogger,Microsoft.Build.Engine;logfile=Publish\Publish.log;

cd Kooboo.CMS.Toolkit
copy "obj\Release\Kooboo.CMS.Toolkit.dll" "..\Publish\Release\Kooboo.CMS.Toolkit\lib\Kooboo.CMS.Toolkit.dll"
copy "Kooboo.CMS.Toolkit.nuspec" "..\Publish\Release\Kooboo.CMS.Toolkit\Kooboo.CMS.Toolkit.nuspec"
cd ..
cd Publish

nuget pack Release\Kooboo.CMS.Toolkit\Kooboo.CMS.Toolkit.nuspec
nuget setApiKey 12487df2-6ae4-449f-a648-4237aba653b6

pause