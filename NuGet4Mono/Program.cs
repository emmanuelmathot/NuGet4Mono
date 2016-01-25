using System;
using Mono.Options;
using System.Collections.Generic;
using NuGet;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Runtime.Versioning;

namespace NuGet4Mono {
    class MainClass {

        static private log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static int verbosity;
        static string spec_version;
        static string packages_config_path;

        public static void Main(string[] args) {

            bool show_help = false;

            var p = new OptionSet() { { "version=", "the version to override with.",
                    v => spec_version = v
                }, { "p|packages=", "packages file path.",
                    v => packages_config_path = v
                }, { "v", "increase debug message verbosity",
                    v => {
                        if (v != null)
                            ++verbosity;
                    }
                }, { "h|help",  "show this message and exit", 
                    v => show_help = v != null
                },
            };

            List<string> assemblies;
            try {
                assemblies = p.Parse(args);
            } catch (OptionException e) {
                Console.Write("nuget4mono: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `nuget4mono --help' for more information.");
                return;
            }

            if (show_help) {
                ShowHelp(p);
                return;
            }

            WriteSpec(assemblies);

        }

        static void ShowHelp(OptionSet p) {
            Console.WriteLine("Usage: greet [OPTIONS]+ message");
            Console.WriteLine("Greet a list of individuals with an optional message.");
            Console.WriteLine("If no message is specified, a generic greeting is used.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        static void WriteSpec(List<string> assemblies) {

            if (assemblies == null || assemblies.Count == 0)
                throw new ArgumentNullException("Provide with at least 1 assembly");

            IEnumerable<PackageReference> deps = GetDependencies();

            Manifest m = ManifestFromAssemblies(assemblies, deps);
               
            FileStream stream = new FileStream(m.Metadata.Id + ".nuspec", FileMode.Create);

            m.Save(stream);

            stream.Close();


        }

        static IEnumerable<PackageReference> GetDependencies() {

            if (string.IsNullOrEmpty(packages_config_path)) {
                var currentDir = Directory.GetCurrentDirectory();
                string[] files = Directory.GetFiles(currentDir, "packages.config");
                if (files.Length == 1) {
                    packages_config_path = files[0];
                    log.DebugFormat("packages.config found: {0}", packages_config_path);
                }
            }

            if (packages_config_path == null)
                return null;

            var packageReferenceFile = new PackageReferenceFile(packages_config_path);
            return packageReferenceFile.GetPackageReferences();
        }

        static Manifest ManifestFromAssemblies(List<string> assemblies, IEnumerable<PackageReference> deps) {

            var assembly = Assembly.LoadFrom(assemblies.First());

            AssemblyInfo ainfo = new AssemblyInfo(assembly);

            Manifest manifest = new Manifest();

            // Version
            manifest.Metadata.Version = ainfo.Version;

            // Authors
            if (ainfo.Authors != null) {
                manifest.Metadata.Authors = ainfo.Authors.Keys.Aggregate((key, next) =>
                                                                        key + "," + next);
                manifest.Metadata.Owners = ainfo.Authors.Keys.Aggregate((key, next) =>
                                                                        key + "," + next);
            }

            // Files Assembly
            manifest.Files = new List<ManifestFile>();

            foreach (string assemblyPath in assemblies) {

                ManifestFile mf = new ManifestFile();

                FileInfo file = new FileInfo(assemblyPath);

                if (file.Exists) {

                    assembly = Assembly.LoadFile(assemblyPath);

                    mf.Source = assemblyPath;

                    string version = "";

                    if (string.IsNullOrEmpty(ainfo.TargetFramework)) { 
                        
                        if (assembly.ImageRuntimeVersion.StartsWith("v2.0"))
                            version = "/net20";
                        if (assembly.ImageRuntimeVersion.StartsWith("v3.5"))
                            version = "/net35";
                        if (assembly.ImageRuntimeVersion.StartsWith("v4.0"))
                            version = "/net40";
                        if (assembly.ImageRuntimeVersion.StartsWith("v4.5"))
                            version = "/net45";
                    }
                    else{

                        NetPortableProfile npp = new NetPortableProfile("test", new FrameworkName[1]{new FrameworkName(ainfo.TargetFramework)});
                        version = "/" + npp.CustomProfileString;
                    }
                       
                    mf.Target = String.Format("lib{0}", version);

                    manifest.Files.Add(mf);
                }

            }

            // Copyright
            manifest.Metadata.Copyright = ainfo.Copyright;

            // Dependencies
            if (deps != null) {
                manifest.Metadata.DependencySets = new List<ManifestDependencySet>();

                foreach (var frameworkVersion in deps.Select<PackageReference, FrameworkName>(pr => pr.TargetFramework).Distinct().ToArray()) {

                    NetPortableProfile npp = new NetPortableProfile("test", new FrameworkName[1]{ frameworkVersion });

                    ManifestDependencySet mds = new ManifestDependencySet();
                    mds.Dependencies = new List<ManifestDependency>();
                    mds.TargetFramework = npp.CustomProfileString;

                    manifest.Metadata.DependencySets.Add(mds);
                    foreach (var dep in deps.Where(d => d.TargetFramework == frameworkVersion).ToArray()) {
                    
                        ManifestDependency md = new ManifestDependency();
                        md.Id = dep.Id;
                        md.Version = dep.Version.ToNormalizedString();

                        mds.Dependencies.Add(md);
                   

                    }
                }
            }

            // Description
            manifest.Metadata.Description = ainfo.Description;

            // Icon Url
            if ( ainfo.IconUrl != null )
                manifest.Metadata.IconUrl = ainfo.IconUrl.ToString();
            
            // Id
            manifest.Metadata.Id = ainfo.ProductTitle;

            // License Url
            if (ainfo.LicenseUrl != null)
                manifest.Metadata.LicenseUrl = ainfo.LicenseUrl.ToString();
                
            // Project Url
            if (ainfo.ProjectUrl != null)
                manifest.Metadata.ProjectUrl = ainfo.ProjectUrl.ToString();

            // Tags
            manifest.Metadata.Tags = ainfo.Tags;

            // Title
            manifest.Metadata.Title = ainfo.ProductTitle;


            return manifest;

        }

    }
}
