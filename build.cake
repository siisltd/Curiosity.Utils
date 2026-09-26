///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

var artifactsDir = Directory("./artifacts");
var packages = "./artifacts/packages";
var solutionPath = "./Curiosity.Utils.sln";
var nugetSource = "https://api.nuget.org/v3/index.json";
var nugetApiKey = Argument<string>("nugetApiKey", null);

Task("Clean")
    .Does(() => 
    {
        DotNetClean(solutionPath);
        DirectoryPath[] cleanDirectories = new DirectoryPath[] {
            artifactsDir
        };
    
        CleanDirectories(cleanDirectories);
    
        foreach(var path in cleanDirectories) { EnsureDirectoryExists(path); }
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() => 
    {
        var settings = new DotNetBuildSettings
          {
              Configuration = configuration
          };
          
        DotNetBuild(
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
            
            DotNetTest(
                project.FullPath,
                new DotNetTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = false
                });
        }
    });
     
Task("IntegrationTests")
    .Does(() =>
    {        
        Information("IntegrationTests task...");

        Information("Running docker...");
//         StartProcess("docker-compose", "-f tests/IntegrationTests/env-compose.yml up -d");
        Information("Running docker completed");

        var projects = GetFiles("./tests/IntegrationTests/**/*csproj");
        foreach(var project in projects)
        {
            Information(project);
            
            DotNetTest(
                project.FullPath,
                new DotNetTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = false
                });
        }
    })
    .Finally(() =>
    {  
        Information("Stopping docker...");
//         StartProcess("docker-compose", "-f tests/IntegrationTests/env-compose.yml down");
        Information("Stopping docker completed");
    });  
    
Task("Pack")
    .Does(() =>
    {        
         Information("Packing to nupkg...");
         var settings = new DotNetPackSettings
          {
              Configuration = configuration,
              OutputDirectory = packages
          };
         
          DotNetPack(solutionPath, settings);
    });
 
Task("Publish")
    .IsDependentOn("Pack")
    .Does(() =>
    {
         var pushSettings = new DotNetNuGetPushSettings
         {
             Source = nugetSource,
             ApiKey = nugetApiKey,
             SkipDuplicate = true
         };

         var pkgs = GetFiles($"{packages}/*.nupkg");
         foreach(var pkg in pkgs)
         {
             Information($"Publishing \"{pkg}\".");
             DotNetNuGetPush(pkg.FullPath, pushSettings);
         }
 });
 
Task("ForcePublish")
    .IsDependentOn("Pack")
    .Does(() =>
    {
         var pushSettings = new DotNetNuGetPushSettings 
         {
             Source = nugetSource,
             ApiKey = nugetApiKey,
             SkipDuplicate = true
         };
         
         var pkgs = GetFiles($"{packages}/*.nupkg");
         foreach(var pkg in pkgs) 
         {     
             Information($"Publishing \"{pkg}\".");
             DotNetNuGetPush(pkg.FullPath, pushSettings);
         }
 }); 
 
    
Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("UnitTests")
    .IsDependentOn("IntegrationTests");
    
Task("GitHub")
    .IsDependentOn("Build")
    .IsDependentOn("UnitTests")
    .IsDependentOn("IntegrationTests")
    .IsDependentOn("Pack")
    .IsDependentOn("Publish");
  
RunTarget(target);
