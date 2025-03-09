using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReCommendedExtension.Deployment.Properties;

namespace ReCommendedExtension.Deployment;

internal static class Program
{
    static int Main()
    {
        try
        {
            var targetPlatform = GeTargetPlatform();

            var executionDirectoryPath = GetExecutionDirectoryPath();

            CopyAssembly(executionDirectoryPath, targetPlatform, out var assemblyPath, out var isReleaseBuild);

            if (isReleaseBuild)
            {
                ResignAssembly(assemblyPath);
            }

            switch (targetPlatform)
            {
                case TargetPlatform.ReSharper:
                    UpdateNuspec(executionDirectoryPath, assemblyPath, out var nuspecPath);

                    SetAssemblyReference(nuspecPath, assemblyPath, out var packageFileName);

                    BuildPackage(nuspecPath);

                    OpenInWindowsExplorer(nuspecPath, packageFileName);
                    break;

                case TargetPlatform.Rider:
                    // todo: package for Rider
                    break;
            }

            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("\n" + e);
            return -1;
        }
        finally
        {
            Console.WriteLine("\nPress ENTER to exit.");
            Console.ReadLine();
        }
    }

    enum TargetPlatform
    {
        ReSharper,
        Rider,
    }

    [Pure]
    static TargetPlatform GeTargetPlatform()
    {
#if RESHARPER
        return TargetPlatform.ReSharper;
#endif

#if RIDER
        return TargetPlatform.Rider;
#endif
    }

    [Pure]
    static string GetExecutionDirectoryPath()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        Debug.Assert(entryAssembly is { });

        var directoryPath = Path.GetDirectoryName(entryAssembly.Location);
        Debug.Assert(directoryPath is { });

        return directoryPath;
    }

    static void CopyAssembly(string executionDirectoryPath, TargetPlatform targetPlatform, out string assemblyPath, out bool isReleaseBuild)
    {
        Console.Write("Copying assembly...");

        const string fileName = "ReCommendedExtension.dll";

        var executionDirectory = Path.GetFileName(Path.GetDirectoryName(executionDirectoryPath));

        isReleaseBuild = string.Equals(executionDirectory, $"Release{targetPlatform:G}", StringComparison.OrdinalIgnoreCase);

        var projectDirectory = Path.Combine(executionDirectoryPath, @"..\..\..\..\ReCommendedExtension");

        var projectFilePath = Path.Combine(projectDirectory, "ReCommendedExtension.csproj");
        var projectFile = XDocument.Load(projectFilePath);
        Debug.Assert(projectFile.Root is { });

        var targetFramework = (string)projectFile.Root.Elements("PropertyGroup").Elements("TargetFramework").First();

        var sourceAssemblyPath = Path.Combine(projectDirectory, "bin", executionDirectory, targetFramework, fileName);
        assemblyPath = Path.Combine(executionDirectoryPath, fileName);
        File.Copy(sourceAssemblyPath, assemblyPath, true);

        Console.WriteLine("done");
    }

    static void ResignAssembly(string assemblyPath)
    {
        Console.WriteLine("Resigning assembly...");

        Console.WriteLine($"Tool path: {Settings.Default.SnPath}");

        // switch to project directory
        // set a secret: > dotnet user-secrets set "SNKey" "..."
        // list secrets: > dotnet user-secrets list

        var userSecretId = Assembly.GetExecutingAssembly().GetCustomAttribute<UserSecretsIdAttribute>()?.UserSecretsId;
        Debug.Assert(userSecretId is { });

        var secretsPath = PathHelper.GetSecretsPathFromSecretsId(userSecretId);
        // must be: %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json

        var json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(secretsPath));
        Debug.Assert(json is { });

        var snkPath = (string?)json["SNKey"];

        Console.WriteLine($"Key path: {snkPath}");

        RunConsoleApplication($"\"{Settings.Default.SnPath}\"", $"-R \"{assemblyPath}\" \"{snkPath}\"");
    }

    static void UpdateNuspec(string executionDirectoryPath, string assemblyPath, out string nuspecPath)
    {
        Console.Write("Updating nuspec...");

        var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);

        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (_, e) =>
        {
            var name = new AssemblyName(e.Name).Name;

            if (name.StartsWith("System.", StringComparison.Ordinal))
            {
                return Assembly.ReflectionOnlyLoad(e.Name);
            }

            var packagesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
            var file = (
                from f in Directory.EnumerateFiles(packagesPath, $"{name}.dll", SearchOption.AllDirectories)
                let version = FileVersionInfo.GetVersionInfo(f)
                orderby version.FileMajorPart descending, version.FileMinorPart descending, version.FileBuildPart descending, version.FilePrivatePart
                    descending
                select f).First();

            return Assembly.ReflectionOnlyLoadFrom(file);
        };

        nuspecPath = Path.Combine(executionDirectoryPath, $"{Path.GetFileNameWithoutExtension(assemblyPath)}.nuspec");
        var nuspec = File.ReadAllText(nuspecPath, Encoding.UTF8);

        ReplacePlaceholders(ref nuspec, assembly);

        File.WriteAllText(nuspecPath, nuspec, Encoding.UTF8);

        Console.WriteLine("done");
    }

    static void ReplacePlaceholders(ref string nuspec, Assembly assembly)
    {
        var customAttributes = assembly.GetCustomAttributesData();

        const string startToken = "{{";
        const string endToken = "}}";

        while (true)
        {
            var start = nuspec.IndexOf(startToken, StringComparison.Ordinal);
            if (start == -1)
            {
                break;
            }

            var end = nuspec.IndexOf(endToken, start + startToken.Length, StringComparison.Ordinal);

            var shortAttributeTypeName = nuspec.Substring(start + startToken.Length, end - start - endToken.Length);

            var fullAttributeTypeName = $"System.Reflection.{shortAttributeTypeName}Attribute";

            var attributeType = Type.GetType(fullAttributeTypeName);

            var customAttribute = customAttributes.First(a => a.AttributeType == attributeType);

            Debug.Assert(customAttribute.ConstructorArguments[0].Value is string);

            var replacementText = (string)customAttribute.ConstructorArguments[0].Value;

            nuspec = nuspec.Remove(start, end - start + endToken.Length);
            nuspec = nuspec.Insert(start, replacementText);
        }
    }

    static void SetAssemblyReference(string nuspecPath, string assemblyPath, out string packageFileName)
    {
        Console.Write("Setting assembly reference...");

        var nuspec = XDocument.Load(nuspecPath);
        Debug.Assert(nuspec.Root is { });

        nuspec
            .Root.Element("files")
            ?.Elements("file")
            .First(e => (string)e.Attribute("src") == "")
            .SetAttributeValue("src", Path.GetFileName(assemblyPath));

        var metadataElement = nuspec.Root.Element("metadata");
        Debug.Assert(metadataElement is { });

        packageFileName = $"{(string)metadataElement.Element("id")}.{(string)metadataElement.Element("version")}.nupkg";

        nuspec.Save(nuspecPath);

        Console.WriteLine("done");
    }

    static void BuildPackage(string nuspecPath)
    {
        Console.WriteLine("Building package...");

        var nuspecDirectoryPath = Path.GetDirectoryName(nuspecPath);
        Debug.Assert(nuspecDirectoryPath is { });

        RunConsoleApplication(
            $"\"{Path.Combine(nuspecDirectoryPath, @"..\..\..\..\.nuget\NuGet.exe")}\"",
            $"pack \"{nuspecPath}\" -OutputDirectory \"{nuspecDirectoryPath}\" -NoPackageAnalysis -Verbosity detailed");
    }

    static void RunConsoleApplication(string executablePath, string arguments)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            },
            EnableRaisingEvents = true,
        };
        process.OutputDataReceived += Process_DataReceived;
        process.ErrorDataReceived += Process_DataReceived;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Console application exited with the code {process.ExitCode}.");
        }
    }

    static void Process_DataReceived(object? sender, DataReceivedEventArgs e) => Console.WriteLine($"    {e.Data}");

    static void OpenInWindowsExplorer(string nuspecPath, string packageFileName)
    {
        var nuspecDirectoryPath = Path.GetDirectoryName(nuspecPath);
        Debug.Assert(nuspecDirectoryPath is { });

        using (Process.Start("explorer", $"/select, \"{Path.Combine(nuspecDirectoryPath, packageFileName)}\"")) { }
    }
}