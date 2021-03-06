﻿[CmdletBinding()]
Param(
    [Parameter(Mandatory=$False)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$False)]
    [string]$Version = "current",

    [Parameter(Mandatory=$False)]
    [ValidateSet("True", "False")]
    [string]$UpdateAssemblyInfo = "True"
)

function Get-ScriptDirectory { 
    Split-Path -parent $PSCommandPath 
}

function Run-It () {
  
        $scriptPath = Get-ScriptDirectory
        . "$scriptPath\Deploy.Common.ps1"

        $working_directories = GetWorkingDirectories;
    
        $src = Resolve-Path "$scriptPath\..\..\src";
        $base_dir = Resolve-Path "$scriptPath\..\.."

	    Import-Module "$scriptPath\..\psake\4.9.0\psake.psm1"
    
        $properties = @{
                "UpdateAssemblyInfo"="$UpdateAssemblyInfo";
                "configuration"="$Configuration"; 
                "version"="$Version";
                "base_dir"="$base_dir";
                "src"=$src;
                "working_dir"=$working_directories["Sitecore"];
                "target"="Sitecore";
				"projects" = @("AvenueClothing.Project.Website", "AvenueClothing.Project.Catalog", "AvenueClothing.Project.Header", "AvenueClothing.Project.Navigation", "AvenueClothing.Project.Transaction", "AvenueClothing.Project.UserFeedback");
            };
    
        Invoke-PSake "$scriptPath\uCommerce.build.ps1" "CreateSitecorePackage" -properties $properties
        

}

Run-It