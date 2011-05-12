@For /F "tokens=2,3,4 delims=/ " %%A in ('Date /t') do @(
	Set Month=%%A
    Set Day=%%B
    Set Year=%%C
)

SET date=%Month%%Day%%Year%

@SET softwaredir="\\DXCBURG03F010.conus.ds.dcma.mil\Software"
@SET softwareSN="\\DXCBURG03F010.conus.ds.dcma.mil\SpaceNet\"
@SET destSN="\\blue01vfrig2\dcmadfs$\WESTERN\DCMA Phoenix\MW\MWQ\MWQC\MWOD
Team 1\
@SET destSW="\\blue01vfrig2\dcmadfs$\WESTERN\DCMA Phoenix\MW\MWQ\MWQC\MWOD
Team 1\Honeywell DSES\Software"


@MD %destSN%SpaceNet-Backup\%date%"

REM "*****Backing up Spacenet*****"
xcopy %softwareSN% %destSN%SpaceNet-Backup\%date%\" /M/Z/V

REM "*****Backing up Software directory*****"
xcopy %softwaredir% %destSW% /M/Z/V/E
