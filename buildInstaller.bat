@echo off
setlocal

REM Replace the paths below with the absolute paths to your WiX source files and tools
set WIX_SOURCE_PATH=.\ValCompanionSetup
set CANDLE_PATH=.\packages\WiX.Toolset.UnofficialFork.3.11.2\tools\wix\candle.exe
set LIGHT_PATH=.\packages\WiX.Toolset.UnofficialFork.3.11.2\tools\wix\light.exe
set PATH_TO_STANDALONE_RELEASE=..\StandAlone\bin\Release\net6.0-windows\

REM Compile the .wxs files using candle.exe with the full path
"%CANDLE_PATH%" -dSolutionExt=.sln -dSolutionFileName=ValRestServer.sln -dSolutionName=ValRestServer -d"SolutionPath=.\ValRestServer.sln" -dConfiguration=Release -dOutDir=bin\Release\ -dPlatform=x86 -d"ProjectDir=.\ValCompanionSetup\\" -dProjectExt=.wixproj -dProjectFileName=ValCompanionSetup.wixproj -dProjectName=ValCompanionSetup -d"ProjectPath=.\ValCompanionSetup.wixproj" -d"TargetDir=.\ValCompanionSetup\bin\Release\\" -dTargetExt=.msi -dTargetFileName=ValCompanionSetup.msi -dTargetName=ValCompanionSetup -d"TargetPath=.\ValCompanionSetup\bin\Release\ValCompanionSetup.msi" -dValRestServer.Configuration=Release -d"ValRestServer.FullConfiguration=Release|AnyCPU" -dValRestServer.Platform=AnyCPU -d"ValRestServer.ProjectDir=.\StandAlone\\" -dValRestServer.ProjectExt=.csproj -dValRestServer.ProjectFileName=ValRestServer.csproj -dValRestServer.ProjectName=ValRestServer -d"ValRestServer.ProjectPath=.\StandAlone\ValRestServer.csproj" -d"ValRestServer.TargetDir=.\StandAlone\bin\Release\net6.0-windows\\" -dValRestServer.TargetExt=.dll -dValRestServer.TargetFileName=ValRestServer.dll -dValRestServer.TargetName=ValRestServer -d"ValRestServer.TargetPath=.\StandAlone\bin\Release\net6.0-windows\ValRestServer.dll" -out obj\Release\ -arch x86 -ext ".\StandAlone\packages\WiX.Toolset.UnofficialFork.3.11.2\tools\wix\\WixUIExtension.dll" %WIX_SOURCE_PATH%\Product.wxs

REM Link the compiled objects into an .msi file using light.exe with the full path
"%LIGHT_PATH%" -out ".\ValCompanionSetup\bin\Release\ValCompanionSetup.msi" -pdbout ".\ValCompanionSetup\bin\Release\ValCompanionSetup.wixpdb" -cultures:null -ext ".\StandAlone\packages\WiX.Toolset.UnofficialFork.3.11.2\tools\wix\\WixUIExtension.dll" -contentsfile obj\Release\ValCompanionSetup.wixproj.BindContentsFileListnull.txt -outputsfile obj\Release\ValCompanionSetup.wixproj.BindOutputsFileListnull.txt -builtoutputsfile obj\Release\ValCompanionSetup.wixproj.BindBuiltOutputsFileListnull.txt -wixprojectfile ".\ValCompanionSetup\ValCompanionSetup.wixproj" obj\Release\Product.wixobj

echo Build completed successfully.
exit /b 0
