using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NuGet4Mono.Extensions {
    
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyLicenseUrlAttribute : Attribute {
        
        Uri licenseuri;

        public AssemblyLicenseUrlAttribute() : this(string.Empty) {
            licenseuri = null;
        }

        public AssemblyLicenseUrlAttribute(string uri) {
            licenseuri = new Uri(uri);
        }

        public Uri LicenseUri {
            get {
                return licenseuri;
            }
        }
            
    }
}

