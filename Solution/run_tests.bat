@ECHO OFF
@RD /S /Q ".\TestResults"
@RD /S /Q ".\TestCoverages"
dotnet test .\BrainBoxUI.Tests --results-directory:.\TestResults --collect:"XPlat Code Coverage" --settings:.\TestSettings.runsettings
dotnet test .\BrainBoxAPI.Tests --results-directory:.\TestResults --collect:"XPlat Code Coverage" --settings:.\TestSettings.runsettings

setlocal enabledelayedexpansion
set "cnt=0"
pushd ".\TestResults"
for /R %%f in (coverage.cobertura.xml) do (
    if exist "%%f" (
		@ECHO "------------------"
        set /a "cnt+=1"
        reportgenerator -reports:"%%f" -targetdir:"..\TestCoverages\!cnt!" -reporttypes:Html
    )
)
popd
endlocal

@PAUSE