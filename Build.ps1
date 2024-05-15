$projectPaths = @(
    '.\src\Admin\NCafe.Admin.Api'
    '.\src\Barista\NCafe.Barista.Api'
    '.\src\Cashier\NCafe.Cashier.Api'
    '.\src\UI\NCafe.Web'
)

Write-Host "Restoring packages for all projects..."
dotnet restore

foreach ($projectPath in $projectPaths) {
    Write-Host "Building project $($projectPath)..."
    dotnet build -c Release --no-restore $projectPath
}

foreach ($projectPath in $projectPaths) {
    Write-Host "Publishing project $($projectPath)..."
    dotnet publish -c Release --no-restore -o "$($projectPath)\output" $projectPath
}
