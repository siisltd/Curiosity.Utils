using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// EXTERNAL NUGET TOOLS
//////////////////////////////////////////////////////////////////////

var artifactsDir = Directory("./artifacts");
var binsDir = artifactsDir + Directory("./bins");
var packages = artifactsDir + Directory("./packages");
var testResultsDir = artifactsDir + Directory("test-results");
var publishDir = artifactsDir + Directory("publish");
var solutionPath = "./Marvin.NET.sln";
var framework = "netcoreapp3.1";
var tempDir = "./temp";

var nugetSource = "https://api.nuget.org/v3/index.json";
var nugetApiKey = Argument<string>("nugetApiKey", null);

Task("Clean")
    .Does(() => 
    {            
        DotNetCoreClean(solutionPath);        
        DirectoryPath[] cleanDirectories = new DirectoryPath[] {
            binsDir,
            testResultsDir,
            artifactsDir,
            tempDir
        };
    
        CleanDirectories(cleanDirectories);
    
        foreach(var path in cleanDirectories)
        {
            EnsureDirectoryExists(path);
        }
    });
 
Task("Build")
    .IsDependentOn("Clean")
    .Does(() => 
    {
        var settings = new DotNetCoreBuildSettings
          {
              Configuration = configuration,
              OutputDirectory = binsDir,
              MSBuildSettings = new DotNetCoreMSBuildSettings
              {
                 MaxCpuCount = 4               
              }
          };
          
        DotNetCoreBuild(
            solutionPath,
            settings);
    });

Task("UnitTests")
    .Does(() =>
    {        
        Information("UnitTests task...");
        var projects = GetFiles("./tests/UnitTests/**/*csproj");
        foreach(var project in projects)
        {
            Information(project);
            
            DotNetCoreTest(
                project.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = false
                });
        }
    });
 
Task("IntegrationTests")
    .Does(() =>
    {        
        Information("Core integration tests task...");
        
//         Information("Running docker task...");
//         StartProcess("docker-compose", "-f ./tests/IntegrationTests/env-compose.yml up -d");
        
        var projects = GetFiles("./tests/IntegrationTests/**/*csproj");
        foreach(var project in projects)
        {
            Information(project);
            
            DotNetCoreTest(
                project.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = false
                });
        }
    })
    .Finally(() =>
    {  
//         Information("Stopping docker task...");
//         StartProcess("docker-compose", "-f ./tests/IntegrationTests/Core/env-compose.yml down");
    }); 
    
Task("Pack")
    .Does(() =>
    {        
         Information("Packing to nupkg...");
         var settings = new DotNetCorePackSettings
          {
              Configuration = configuration,
              OutputDirectory = packages
          };
         
          DotNetCorePack(solutionPath, settings);
    });
    
Task("Publish")
.IsDependentOn("Pack")
.Does(() => {
     var pushSettings = new DotNetCoreNuGetPushSettings 
     {
         Source = nugetSource,
         ApiKey = nugetApiKey,
         SkipDuplicate = true
     };
     
     var pkgs = GetFiles($"{packages}/*.nupkg");
     foreach(var pkg in pkgs) 
     {     
         Information($"Publishing \"{pkg}\".");
         DotNetCoreNuGetPush(pkg.FullPath, pushSettings);
     }
 });

Task("Default")
    .IsDependentOn("Build");
    
RunTarget(target);
