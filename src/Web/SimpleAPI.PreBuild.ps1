# Output file
$OutFile = "Setup\BuildInformation.txt"

# Git details
$tags = (git tag --points-at HEAD)
$GitCommit = (git rev-parse HEAD).Substring(0, 10)
$DirtyRepo = 0;

$ChangedFiles = $( git status --porcelain | Measure-Object | Select-Object -expand Count )
if ($ChangedFiles -gt 0) { $DirtyRepo = 1; }

# Output info to file
$tagsStr = $tags | Join-String -Separator ', '
Get-Date -Format 'yyyy/MM/dd HH:mm:ss' -AsUTC | Out-File -FilePath $OutFile
Write-Output ''                               | Out-File -FilePath $OutFile -Append
Write-Output "Commit: $GitCommit"             | Out-File -FilePath $OutFile -Append
Write-Output "Tags: $tagsStr"                 | Out-File -FilePath $OutFile -Append
Write-Output "DirtyRepo: $DirtyRepo"          | Out-File -FilePath $OutFile -Append
