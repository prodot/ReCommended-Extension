using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration.UserSecrets;
using ReCommendedExtension.Deployment.Properties;

namespace ReCommendedExtension.Deployment;

internal static class Program
{
    static int Main()
    {
        try
        {
            var targetPlatform = GeTargetPlatform();

            var solutionDirectoryPath = GetSolutionDirectoryPath();
            var executionDirectoryPath = GetExecutionDirectoryPath();
            Debug.Assert(executionDirectoryPath.StartsWith(solutionDirectoryPath, StringComparison.OrdinalIgnoreCase));

            CopyAssembly(executionDirectoryPath, targetPlatform, solutionDirectoryPath, out var assemblyPath, out var isReleaseBuild);
            Debug.Assert(assemblyPath.StartsWith(solutionDirectoryPath, StringComparison.OrdinalIgnoreCase));

            if (isReleaseBuild)
            {
                ResignAssembly(assemblyPath);
            }

            var assembly = LoadAssemblyForReflection(assemblyPath);

            switch (targetPlatform)
            {
                case TargetPlatform.ReSharper:
                    UpdateNuspec(executionDirectoryPath, assemblyPath, assembly, out var nuspecPath);
                    Debug.Assert(nuspecPath.StartsWith(solutionDirectoryPath, StringComparison.OrdinalIgnoreCase));

                    SetAssemblyReference(nuspecPath, assemblyPath, out var nugetFileName);

                    BuildNuget(solutionDirectoryPath, nuspecPath, nugetFileName, out var nugetPath);
                    Debug.Assert(nugetPath.StartsWith(solutionDirectoryPath, StringComparison.OrdinalIgnoreCase));

                    OpenInWindowsExplorer(nugetPath);
                    break;

                case TargetPlatform.Rider:
                    var riderBuildPath = GetRiderBuildPath(solutionDirectoryPath);
                    Debug.Assert(riderBuildPath.StartsWith(solutionDirectoryPath, StringComparison.OrdinalIgnoreCase));

                    var encoding = new UTF8Encoding(false, true); // UTF-8 without BOM

                    UpdatePluginXml(riderBuildPath, assembly, encoding);
                    UpdateGradleProperties(riderBuildPath, assembly, encoding);
                    UpdateSettingsGradleKts(riderBuildPath, assembly, encoding);

                    BuildJar(riderBuildPath, assembly, out var jarPath);
                    Debug.Assert(jarPath.StartsWith(riderBuildPath, StringComparison.OrdinalIgnoreCase));

                    BuildZip(executionDirectoryPath, assembly, assemblyPath, jarPath, out var zipPath);

                    OpenInWindowsExplorer(zipPath);
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
    static string GetSolutionDirectoryPath([CallerFilePath] string? currentFilePath = null)
    {
        var solutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(currentFilePath));
        Debug.Assert(solutionDirectory is { });

        return solutionDirectory;
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

    static void CopyAssembly(
        string executionDirectoryPath,
        TargetPlatform targetPlatform,
        string solutionDirectoryPath,
        out string assemblyPath,
        out bool isReleaseBuild)
    {
        Console.Write("Copying assembly...");

        const string fileName = "ReCommendedExtension.dll";

        var executionDirectory = Path.GetFileName(Path.GetDirectoryName(executionDirectoryPath));

        isReleaseBuild = string.Equals(executionDirectory, $"Release{targetPlatform:G}", StringComparison.OrdinalIgnoreCase);

        var extensionProjectDirectory = Path.Combine(solutionDirectoryPath, "ReCommendedExtension");

        var projectFile = XDocument.Load(Path.Combine(extensionProjectDirectory, "ReCommendedExtension.csproj"));
        Debug.Assert(projectFile.Root is { });

        var targetFramework = (string)projectFile.Root.Elements("PropertyGroup").Elements("TargetFramework").First();

        var sourceAssemblyPath = Path.Combine(extensionProjectDirectory, "bin", executionDirectory, targetFramework, fileName);
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

        var json = JsonDocument.Parse(File.ReadAllText(secretsPath));
        var snkPath = json.RootElement.GetProperty("SNKey").GetString();

        Console.WriteLine($"Key path: {snkPath}");

        RunConsoleApplication($"\"{Settings.Default.SnPath}\"", $"-R \"{assemblyPath}\" \"{snkPath}\"");
    }

    [Pure]
    static Assembly LoadAssemblyForReflection(string assemblyPath)
    {
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

        return assembly;
    }

    static void UpdateNuspec(string executionDirectoryPath, string assemblyPath, Assembly assembly, out string nuspecPath)
    {
        Console.Write("Updating nuspec...");

        nuspecPath = Path.Combine(executionDirectoryPath, $"{Path.GetFileNameWithoutExtension(assemblyPath)}.nuspec");
        var nuspec = File.ReadAllText(nuspecPath, Encoding.UTF8);

        ReplacePlaceholders(ref nuspec, assembly);

        File.WriteAllText(nuspecPath, nuspec, Encoding.UTF8);

        Console.WriteLine("done");
    }

    static void ReplacePlaceholders(ref string nuspec, Assembly assembly)
    {
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
            Debug.Assert(attributeType is { });

            var replacementText = assembly.GetAttributeValue(attributeType);

            nuspec = nuspec.Remove(start, end - start + endToken.Length);
            nuspec = nuspec.Insert(start, replacementText);
        }
    }

    static void SetAssemblyReference(string nuspecPath, string assemblyPath, out string nugetFileName)
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

        nugetFileName = $"{(string)metadataElement.Element("id")}.{(string)metadataElement.Element("version")}.nupkg";

        nuspec.Save(nuspecPath);

        Console.WriteLine("done");
    }

    static void BuildNuget(string solutionDirectoryPath, string nuspecPath, string packageFileName, out string nugetPath)
    {
        Console.WriteLine("Building nuget...");

        var nuspecDirectoryPath = Path.GetDirectoryName(nuspecPath);
        Debug.Assert(nuspecDirectoryPath is { });

        RunConsoleApplication(
            $"\"{Path.Combine(solutionDirectoryPath, ".nuget", "NuGet.exe")}\"",
            $"pack \"{nuspecPath}\" -OutputDirectory \"{nuspecDirectoryPath}\" -NoPackageAnalysis -Verbosity detailed");

        nugetPath = Path.Combine(nuspecDirectoryPath, packageFileName);
        Debug.Assert(File.Exists(nugetPath));
    }

    [Pure]
    static string GetRiderBuildPath(string solutionDirectoryPath) => Path.Combine(solutionDirectoryPath, "ReCommendedExtension.Deployment", "Rider");

    static void UpdatePluginXml(string riderBuildPath, Assembly assembly, Encoding encoding)
    {
        Console.Write("Updating plugin.xml...");

        var pluginXmlPath = Path.Combine(riderBuildPath, "src", "rider", "main", "resources", "META-INF", "plugin.xml");

        var pluginXml = XDocument.Load(pluginXmlPath);
        Debug.Assert(pluginXml.Root is { });

        pluginXml.Root.Element("name")?.SetValue(assembly.GetAttributeValue<AssemblyTitleAttribute>());
        pluginXml.Root.Element("vendor")?.SetValue(assembly.GetAttributeValue<AssemblyCompanyAttribute>());

        using (var writer = XmlWriter.Create(pluginXmlPath, new XmlWriterSettings { Encoding = encoding, Indent = true }))
        {
            pluginXml.Save(writer);
        }

        Console.WriteLine("done");
    }

    static void UpdateGradleProperties(string riderBuildPath, Assembly assembly, Encoding encoding)
    {
        Console.Write("Updating gradle.properties...");

        var gradePropertiesPath = Path.Combine(riderBuildPath, "gradle.properties");

        var gradeProperties = File.ReadAllLines(gradePropertiesPath);

        for (var i = 0; i < gradeProperties.Length; i++)
        {
            const string propertyName = "PluginVersion";

            var line = gradeProperties[i];

            if (line.StartsWith($"{propertyName}=", StringComparison.Ordinal))
            {
                gradeProperties[i] = $"{propertyName}={assembly.GetAttributeValue<AssemblyFileVersionAttribute>()}";
                break;
            }
        }

        File.WriteAllLines(gradePropertiesPath, gradeProperties, encoding);

        Console.WriteLine("done");
    }

    static void UpdateSettingsGradleKts(string riderBuildPath, Assembly assembly, Encoding encoding)
    {
        Console.Write("Updating settings.gradle.kts...");

        var settingsPath = Path.Combine(riderBuildPath, "settings.gradle.kts");

        var lines = File.ReadAllLines(settingsPath);

        for (var i = 0; i < lines.Length; i++)
        {
            const string propertyName = "rootProject.name";

            var line = lines[i];

            if (line.StartsWith($"{propertyName} = ", StringComparison.Ordinal))
            {
                lines[i] = $"{propertyName} = \"Prodot.{assembly.GetAttributeValue<AssemblyProductAttribute>()}\"";
                break;
            }
        }

        File.WriteAllLines(settingsPath, lines, encoding);

        Console.WriteLine("done");
    }

    static void BuildJar(string riderBuildPath, Assembly assembly, out string jarPath)
    {
        Console.WriteLine("Building JAR...");

        Directory.SetCurrentDirectory(riderBuildPath);

        RunConsoleApplication("gradlew.bat", "build");

        jarPath = Path.Combine(
            riderBuildPath,
            "build",
            "libs",
            $"Prodot.{assembly.GetAttributeValue<AssemblyProductAttribute>()}-{assembly.GetAttributeValue<AssemblyFileVersionAttribute>()}.jar");
        Debug.Assert(File.Exists(jarPath));
    }

    static void BuildZip(string executionDirectoryPath, Assembly assembly, string assemblyPath, string jarPath, out string zipPath)
    {
        Console.Write("Building zip...");

        zipPath = Path.Combine(executionDirectoryPath, $"{Path.GetFileNameWithoutExtension(jarPath)}.zip");

        using var fileStream = File.Create(zipPath);
        using var zip = new ZipArchive(fileStream, ZipArchiveMode.Create, true);

        var rootEntry = $"Prodot.{assembly.GetAttributeValue<AssemblyProductAttribute>()}";

        zip.CreateEntryFromFile(assemblyPath, $"{rootEntry}/dotnet/{Path.GetFileName(assemblyPath)}");
        zip.CreateEntryFromFile(jarPath, $"{rootEntry}/lib/{Path.GetFileName(jarPath)}");

        Console.WriteLine("done");
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

    static void OpenInWindowsExplorer(string filePath)
    {
        using (Process.Start("explorer", $"/select, \"{filePath}\"")) { }
    }

    [Pure]
    static string GetAttributeValue<A>(this Assembly assembly) where A : Attribute => assembly.GetAttributeValue(typeof(A));

    [Pure]
    static string GetAttributeValue(this Assembly assembly, Type attributeType)
    {
        var customAttribute = assembly.GetCustomAttributesData().First(a => a.AttributeType == attributeType);
        Debug.Assert(customAttribute.ConstructorArguments[0].Value is string);

        return (string)customAttribute.ConstructorArguments[0].Value;
    }
}