using System;
using System.Reflection;
using System.IO;
using NuGet4Mono.Extensions;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Linq;

namespace NuGet4Mono {

    public class AssemblyInfo {
        public AssemblyInfo(Assembly assembly) {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            this.assembly = assembly;
        }

        private readonly Assembly assembly;

        /// <summary>
        /// Gets the title property
        /// </summary>
        public string ProductTitle {
            get {
                return GetAttributeValue<AssemblyTitleAttribute>(a => a.Title, 
                                                                 Path.GetFileNameWithoutExtension(assembly.CodeBase));
            }
        }

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public string Version {
            get {
                string version = GetAttributeValue<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion);
                if ( string.IsNullOrEmpty(version))
                    version = GetAttributeValue<AssemblyVersionAttribute>(a => a.Version);
                if ( string.IsNullOrEmpty(version))
                    version = assembly.GetName().Version.ToString();
                if (version != null)
                    return version;
                else
                    return "1.0.0.0";
            }
        }

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public Version SemVersion {
            get {
                return assembly.GetName().Version;
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description {
            get { return GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description); }
        }


        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product {
            get { return GetAttributeValue<AssemblyProductAttribute>(a => a.Product); }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright {
            get { return GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright); }
        }

        /// <summary>
        /// Gets the company information for the product.
        /// </summary>
        public string Company {
            get { return GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company); }
        }

        /// <summary>
        /// Gets the targetframework for the product.
        /// </summary>
        public string TargetFramework {
            get { return GetAttributeValue<TargetFrameworkAttribute>(a => a.FrameworkName); }
        }

        /// <summary>
        /// Gets the Icon Url for the product.
        /// </summary>
        public Uri IconUrl {
            get { return GetAttributeValue<AssemblyIconUrlAttribute, Uri>(a => a.IconUri); }
        }

        /// <summary>
        /// Gets the License Url for the product.
        /// </summary>
        public Uri LicenseUrl {
            get { return GetAttributeValue<AssemblyLicenseUrlAttribute,Uri>(a => a.LicenseUri); }
        }

        /// <summary>
        /// Gets the Icon Url for the product.
        /// </summary>
        public Uri ProjectUrl {
            get { return GetAttributeValue<AssemblyProjectUrlAttribute,Uri>(a => a.ProjectUri); }
        }

        /// <summary>
        /// Gets the Icon Url for the product.
        /// </summary>
        public string Tags {
            get { return GetAttributeValue<AssemblyTagsAttribute>(a => a.Tags); }
        }

        /// <summary>
        /// Gets the authors information for the product.
        /// </summary>
        public Dictionary<string,string> Authors {

            get {
                var attributes = assembly.GetCustomAttributes<AssemblyAuthorsAttribute>();
                if (attributes.Count() > 0)
                    return attributes.First().Authors;
                else
                    return null;
            }
        }

        protected string GetAttributeValue<TAttr>(Func<TAttr, 
                                                  string> resolveFunc, string defaultResult = null) where TAttr : Attribute {
            object[] attributes = assembly.GetCustomAttributes(typeof(TAttr), false);
            if (attributes.Length > 0)
                return resolveFunc((TAttr)attributes[0]);
            else
                return defaultResult;
        }

        protected TResult GetAttributeValue<TAttr, TResult>(Func<TAttr, 
                                                            TResult> resolveFunc) where TAttr : Attribute {
            object[] attributes = assembly.GetCustomAttributes(typeof(TAttr), false);
            if (attributes.Length > 0)
                return resolveFunc((TAttr)attributes[0]);
            else
                return default (TResult);
        }
    }
}