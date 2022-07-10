echo "Restoring DotNet Tools for code generators"
dotnet tool restore

echo "Generating MessagePack Code"
dotnet mpc -i %~dp0HQDotNet.Presence.Model\HQDotNet.Presence.Model.csproj -o %~dp0HQDotNet.Presence.Client\Assets\HQDotNet.Presence\Generated -m
echo "Generating Magic Onion Code"
dotnet moc -i %~dp0HQDotNet.Presence.Model\HQDotNet.Presence.Model.csproj -o %~dp0HQDotNet.Presence.Client\Assets\HQDotNet.Presence\Generated -m

echo "Copy Shared Model files to Unity Client"
copy %~dp0HQDotNet.Presence.Model\*.cs %~dp0HQDotNet.Presence.Client\Assets\HQDotNet.Presence\Model\

if "%~1"=="-x" goto done
PAUSE

:done
