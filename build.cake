var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

////////////////////////////////////////////////////////////////
// Tasks

Task("Build")
    .Does(context => 
{
    DotNetCoreBuild("./src/Spectre.Verify.Extensions.sln", new DotNetCoreBuildSettings {
        Configuration = configuration,
        NoIncremental = context.HasArgument("rebuild"),
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Package")
    .IsDependentOn("Build")
    .Does(context => 
{
    context.CleanDirectory("./.artifacts");

    context.DotNetCorePack($"./src/Spectre.Verify.Extensions.sln", new DotNetCorePackSettings {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
        OutputDirectory = "./.artifacts",
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Publish-GitHub")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Package")
    .Does(context => 
{
    var apiKey = Argument<string>("github-key", null);
    if(string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No GitHub API key was provided.");
    }

    // Publish to GitHub Packages
    var exitCode = 0;
    foreach(var file in context.GetFiles("./.artifacts/*.nupkg")) 
    {
        try
        {
            context.Information("Publishing {0}...", file.GetFilename().FullPath);
            exitCode += StartProcess("dotnet", 
                new ProcessSettings {
                    Arguments = new ProcessArgumentBuilder()
                        .Append("gpr")
                        .Append("push")
                        .AppendQuoted(file.FullPath)
                        .AppendSwitchSecret("-k", " ", apiKey)
                }
            );
        }
        catch
        {
            // Ignore
        }
    }

    if (exitCode != 0) 
    {
        Information("Could not push one or more GitHub packages.");
    }
});

Task("Publish-NuGet")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Package")
    .Does(context => 
{
    var apiKey = Argument<string>("nuget-key", null);
    if(string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No NuGet API key was provided.");
    }

    // Publish to GitHub Packages
    foreach(var file in context.GetFiles("./.artifacts/*.nupkg")) 
    {
        context.Information("Publishing {0}...", file.GetFilename().FullPath);
        DotNetCoreNuGetPush(file.FullPath, new DotNetCoreNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = apiKey,
        });
    }
});

////////////////////////////////////////////////////////////////
// Targets

Task("Publish")
    .IsDependentOn("Publish-GitHub")
    .IsDependentOn("Publish-NuGet");

Task("Default")
    .IsDependentOn("Package");

////////////////////////////////////////////////////////////////
// Execution

RunTarget(target)