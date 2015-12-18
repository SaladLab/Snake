SET SRC=..\..\..\..\..\..\core\EntityNetwork.Net35\bin\Debug
SET SRC_UNITY3D=..\..\..\..\..\..\plugins\EntityNetwork.Unity3D\bin\Debug
SET DST=.
SET PDB2MDB=..\..\..\..\..\..\tools\unity3d\pdb2mdb.exe

%PDB2MDB% "%SRC%\EntityNetwork.dll"
%PDB2MDB% "%SRC_UNITY3D%\EntityNetwork.Unity3D.dll"

COPY /Y "%SRC%\EntityNetwork.dll*" %DST%
COPY /Y "%SRC_UNITY3D%\EntityNetwork.Unity3D.dll*" %DST%

pause.
