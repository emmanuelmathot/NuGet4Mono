using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NuGet4Mono.Extensions {
    
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyProjectUrlAttribute : Attribute {
        
        Uri projecturi;

        public AssemblyProjectUrlAttribute() : this(string.Empty) {
            projecturi = null;
        }

        public AssemblyProjectUrlAttribute(string uri) {
            projecturi = new Uri(uri);
        }

        public Uri ProjectUri {
            get {
                return projecturi;
            }
        }
            
    }
}

