using System.IO.Compression;
using System.Xml.Linq;

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
var githubReleaseDryRun = HasArgument("githubReleaseDryRun");

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
 
Task("GitHubReleases")
    .Does(() =>
    {
        var repository = EnvironmentVariable("GITHUB_REPOSITORY") ?? "siisltd/Curiosity.Utils";
        var commitSha = EnvironmentVariable("GITHUB_SHA");
        if (!githubReleaseDryRun && String.IsNullOrEmpty(commitSha))
            throw new CakeException("GITHUB_SHA is not set. Pass --githubReleaseDryRun to run locally.");

        var releaseNotesDir = $"{artifactsDir}/release-notes";
        EnsureDirectoryExists(releaseNotesDir);

        var projectDirs = GetProjectDirectoriesByPackageId();
        foreach (var pkg in GetFiles($"{packages}/*.nupkg"))
        {
            var (packageId, version) = ReadPackageIdentity(pkg);
            var tag = $"{packageId}.v{version}";

            if (!githubReleaseDryRun && GitHubReleaseExists(tag))
            {
                Verbose($"Release \"{tag}\" already exists.");
                continue;
            }

            if (!projectDirs.TryGetValue(packageId, out var projectDir))
                throw new CakeException($"Project for package \"{packageId}\" is not found in ./src.");

            var notesFile = File($"{releaseNotesDir}/{tag}.md");
            System.IO.File.WriteAllText(notesFile, BuildReleaseNotes(projectDir, packageId, version, repository, tag));

            if (githubReleaseDryRun)
            {
                Information($"[dry run] Release \"{tag}\", notes: {notesFile}");
                continue;
            }

            Information($"Creating release \"{tag}\"...");
            var arguments = new ProcessArgumentBuilder()
                .Append("release").Append("create").AppendQuoted(tag)
                .Append("--target").Append(commitSha)
                .Append("--title").AppendQuoted($"{packageId} v{version}")
                .Append("--notes-file").AppendQuoted(notesFile.Path.FullPath)
                .Append("--latest=false");
            if (version.Contains('-'))
                arguments.Append("--prerelease");

            var exitCode = StartProcess("gh", new ProcessSettings { Arguments = arguments });
            if (exitCode != 0)
                throw new CakeException($"Failed to create release \"{tag}\" (exit code {exitCode}).");
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
    .IsDependentOn("Publish")
    .IsDependentOn("GitHubReleases");

///////////////////////////////////////////////////////////////////////////////
// HELPERS
///////////////////////////////////////////////////////////////////////////////

// PackageId may differ from the project name (e.g. Curiosity.Configuration.YAML -> Curiosity.Configuration.YML).
Dictionary<string, DirectoryPath> GetProjectDirectoriesByPackageId()
{
    var result = new Dictionary<string, DirectoryPath>(StringComparer.OrdinalIgnoreCase);
    foreach (var project in GetFiles("./src/**/*.csproj"))
    {
        var packageId = XDocument.Load(project.FullPath)
            .Descendants("PackageId")
            .Select(x => x.Value.Trim())
            .FirstOrDefault(x => x.Length > 0)
            ?? project.GetFilenameWithoutExtension().ToString();
        result[packageId] = project.GetDirectory();
    }

    return result;
}

(string PackageId, string Version) ReadPackageIdentity(FilePath nupkg)
{
    using var archive = ZipFile.OpenRead(nupkg.FullPath);
    var nuspecEntry = archive.Entries.Single(x => x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));
    using var stream = nuspecEntry.Open();
    var metadata = XDocument.Load(stream).Descendants().First(x => x.Name.LocalName == "metadata");

    string Get(string name) => metadata.Elements().First(x => x.Name.LocalName == name).Value.Trim();
    return (Get("id"), Get("version"));
}

bool GitHubReleaseExists(string tag)
{
    var exitCode = StartProcess("gh", new ProcessSettings
    {
        Arguments = new ProcessArgumentBuilder().Append("release").Append("view").AppendQuoted(tag),
        RedirectStandardOutput = true,
        RedirectStandardError = true
    });

    return exitCode == 0;
}

string BuildReleaseNotes(DirectoryPath projectDir, string packageId, string version, string repository, string tag)
{
    var changelogPath = projectDir.CombineWithFilePath("CHANGELOG.md");
    var changelogUrl = $"https://github.com/{repository}/blob/{tag}/{MakeAbsolute(Directory(".")).GetRelativePath(changelogPath)}";

    var section = new List<string>();
    if (FileExists(changelogPath))
    {
        var inSection = false;
        foreach (var line in System.IO.File.ReadAllLines(changelogPath.FullPath))
        {
            if (line.StartsWith("## ["))
            {
                if (inSection) break;
                inSection = line.StartsWith($"## [{version}]");
                continue;
            }

            if (inSection) section.Add(line);
        }
    }

    var notes = section.Count > 0
        ? String.Join("\n", section).Trim()
        : $"See [CHANGELOG]({changelogUrl}).";

    return $"{notes}\n\n---\n\nNuGet: https://www.nuget.org/packages/{packageId}/{version}\n";
}

RunTarget(target);
