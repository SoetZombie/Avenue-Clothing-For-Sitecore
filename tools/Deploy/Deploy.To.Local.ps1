[CmdletBinding()]
Param(
	[Parameter(Mandatory=$False)]
    [string]$WebsiteRoot = "C:\inetpub\SC8\Website"
)

function Get-ScriptDirectory { 
    Split-Path -parent $PSCommandPath 
}

function Run-It () {
	$scriptPath = Get-ScriptDirectory;
	$src = Resolve-Path "$scriptPath\..\..\src";
	$src = Resolve-Path "$scriptPath\..\..\src";
	$base_dir = Resolve-Path "$scriptPath\..\.."

	Import-Module "$scriptPath\..\psake\4.3.0.0\psake.psm1"
    
	$properties = @{
	"configuration"="Debug"; 
	"base_dir"="$base_dir";
	"src"=$src;
	"working_dir"=$WebsiteRoot;
	"Apis" = "CommerceConnect";
	"projects" = @("AvenueClothing.Project.Website", "AvenueClothing.Project.Catalog", "AvenueClothing.Project.DemoStore", "AvenueClothing.Project.Header", "AvenueClothing.Project.Navigation", "AvenueClothing.Project.Transaction", "AvenueClothing.Project.UserFeedback");
	};
    
	Invoke-PSake "$scriptPath\uCommerce.build.ps1" "DeploySitecoreLocal" -properties $properties
}

Run-It