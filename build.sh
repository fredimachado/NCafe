#!/bin/bash
set -e

projectPaths=("src/Admin/NCafe.Admin.Api" "src/Barista/NCafe.Barista.Api" "src/Cashier/NCafe.Cashier.Api" "src/UI/NCafe.Web")

echo "Restoring packages for all projects..."
dotnet restore

for projectPath in "${projectPaths[@]}"
do
	echo "Building project $projectPath..."
	dotnet build -c Release --no-restore $projectPath
done

for projectPath in "${projectPaths[@]}"
do
	echo "Publishing project $projectPath..."
	dotnet publish -c Release --no-restore -o "$projectPath/output" $projectPath
done
