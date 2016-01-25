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
            try {
                licenseuri = new Uri(uri);
            } catch {
                licenseuri = null;       
            }
        }

        public Uri LicenseUri {
            get {
                return licenseuri;
            }
        }
            
    }
}

