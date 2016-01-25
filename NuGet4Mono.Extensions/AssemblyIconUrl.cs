using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NuGet4Mono.Extensions {
    
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyIconUrlAttribute : Attribute {
        
        Uri iconuri;

        public AssemblyIconUrlAttribute() : this(string.Empty) {
            iconuri = null;
        }

        public AssemblyIconUrlAttribute(string uri) {
            try {
                iconuri = new Uri(uri);
            } catch {
                iconuri = null;       
            }
        }

        public Uri IconUri {
            get {
                return iconuri;
            }
        }
            
    }
}

