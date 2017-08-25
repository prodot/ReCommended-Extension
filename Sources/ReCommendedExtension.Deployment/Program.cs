using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using ReCommendedExtension.Deployment.Properties;

namespace ReCommendedExtension.Deployment
{
    internal static class Program
    {
        static int Main([NotNull] [ItemNotNull] string[] args)
        {
            try
            {
                var executionDirectoryPath = GetExecutionDirectoryPath();

                CopyAssembly(executionDirectoryPath, out var assemblyPath, out var isReleaseBuild);

                if (isReleaseBuild)
                {
                    if (args.Length == 0)
                    {
                        throw new ArgumentException("SNK file not specified.");
                    }

                    var snkPath = args[0];
                    ResignAssembly(assemblyPath, snkPath);
                }

                UpdateNuspec(executionDirectoryPath, assemblyPath, out var nuspecPath);

                SetAssemblyReference(nuspecPath, assemblyPath, out var packageFileName);

                BuildPackage(nuspecPath);

                OpenInWindowsExplorer(nuspecPath, packageFileName);

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

        [Pure]
        [NotNull]
        static string GetExecutionDirectoryPath()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            Debug.Assert(entryAssembly != null);

            var directoryPath = Path.GetDirectoryName(entryAssembly.Location);
            Debug.Assert(directoryPath != null);

            return directoryPath;
        }

        static void CopyAssembly([NotNull] string executionDirectoryPath, [NotNull] out string assemblyPath, out bool isReleaseBuild)
        {
            Console.Write("Copying assembly...");

            const string fileName = "ReCommendedExtension.dll";

            var executionDirectory = Path.GetFileName(executionDirectoryPath);
            isReleaseBuild = string.Equals(executionDirectory, "release", StringComparison.OrdinalIgnoreCase);

            var sourceAssemblyPath = Path.Combine(executionDirectoryPath, @"..\..\..\ReCommendedExtension\bin", executionDirectory, fileName);
            assemblyPath = Path.Combine(executionDirectoryPath, fileName);
            File.Copy(sourceAssemblyPath, assemblyPath, true);

            Console.WriteLine("done");
        }

        static void ResignAssembly([NotNull] string assemblyPath, [NotNull] string snkPath)
        {
            Console.WriteLine("Resigning assembly...");

            Console.WriteLine($"Tool path: {Settings.Default.SnPath}");
            Console.WriteLine($"Key path: {snkPath}");

            RunConsoleApplication($"\"{Settings.Default.SnPath}\"", $"-R \"{assemblyPath}\" \"{snkPath}\"");
        }

        static void UpdateNuspec([NotNull] string executionDirectoryPath, [NotNull] string assemblyPath, [NotNull] out string nuspecPath)
        {
            Console.Write(@"Updating nuspec...");

            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            Debug.Assert(assembly != null);
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (sender, e) =>
            {
                Debug.Assert(e != null);
                Debug.Assert(e.Name != null);

                var name = new AssemblyName(e.Name).Name;
                Debug.Assert(name != null);

                if (name.StartsWith("System.", StringComparison.Ordinal))
                {
                    return Assembly.ReflectionOnlyLoad(e.Name);
                }

                var assemblyDirectoryPath = Path.GetDirectoryName(assemblyPath);
                Debug.Assert(assemblyDirectoryPath != null);

                var packagesPath = Path.Combine(assemblyDirectoryPath, @"..\..\..\packages");
                var file = Directory.EnumerateFiles(packagesPath, name + ".dll", SearchOption.AllDirectories).Single();
                Debug.Assert(file != null);

                return Assembly.ReflectionOnlyLoadFrom(file);
            };

            nuspecPath = Path.Combine(executionDirectoryPath, Path.GetFileNameWithoutExtension(assemblyPath) + ".nuspec");
            var nuspec = File.ReadAllText(nuspecPath, Encoding.UTF8);

            ReplacePlaceholders(ref nuspec, assembly);

            File.WriteAllText(nuspecPath, nuspec, Encoding.UTF8);

            Console.WriteLine("done");
        }

        static void ReplacePlaceholders([NotNull] ref string nuspec, [NotNull] Assembly assembly)
        {
            var customAttributes = assembly.GetCustomAttributesData();
            Debug.Assert(customAttributes != null);

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

                var fullAttributeTypeName = "System.Reflection." + shortAttributeTypeName + "Attribute";

                var attributeType = Type.GetType(fullAttributeTypeName);

                var customAttribute = customAttributes.First(a => a.AttributeType == attributeType);

                Debug.Assert(customAttribute != null);
                Debug.Assert(customAttribute.ConstructorArguments[0].Value is string);

                var replacementText = (string)customAttribute.ConstructorArguments[0].Value;

                nuspec = nuspec.Remove(start, end - start + endToken.Length);
                nuspec = nuspec.Insert(start, replacementText);
            }
        }

        static void SetAssemblyReference([NotNull] string nuspecPath, [NotNull] string assemblyPath, [NotNull] out string packageFileName)
        {
            Console.Write("Setting assembly reference...");

            var nuspec = XDocument.Load(nuspecPath);

            Debug.Assert(nuspec.Root != null);

            nuspec.Root.Element("files")?.Element("file")?.SetAttributeValue("src", Path.GetFileName(assemblyPath));

            var metadataElement = nuspec.Root.Element("metadata");
            Debug.Assert(metadataElement != null);

            packageFileName = $"{(string)metadataElement.Element("id")}.{(string)metadataElement.Element("version")}.nupkg";

            nuspec.Save(nuspecPath);

            Console.WriteLine("done");
        }

        static void BuildPackage([NotNull] string nuspecPath)
        {
            Console.WriteLine("Building package...");

            var nuspecDirectoryPath = Path.GetDirectoryName(nuspecPath);
            Debug.Assert(nuspecDirectoryPath != null);

            RunConsoleApplication(
                $"\"{Path.Combine(nuspecDirectoryPath, @"..\..\..\.nuget\NuGet.exe")}\"",
                $"pack \"{nuspecPath}\" -OutputDirectory \"{nuspecDirectoryPath}\" -NoPackageAnalysis -Verbosity detailed");
        }

        static void RunConsoleApplication([NotNull] string executablePath, [NotNull] string arguments)
        {
            using (
                var process = new Process
                {
                    StartInfo =
                        new ProcessStartInfo
                        {
                            FileName = executablePath,
                            Arguments = arguments,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                        },
                    EnableRaisingEvents = true,
                })
            {
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
        }

        static void Process_DataReceived(object sender, [NotNull] DataReceivedEventArgs e) => Console.WriteLine("    " + e.Data);

        static void OpenInWindowsExplorer([NotNull] string nuspecPath, [NotNull] string packageFileName)
        {
            var nuspecDirectoryPath = Path.GetDirectoryName(nuspecPath);
            Debug.Assert(nuspecDirectoryPath != null);

            using (Process.Start("explorer", "/select, \"" + Path.Combine(nuspecDirectoryPath, packageFileName) + "\"")) { }
        }
    }
}
