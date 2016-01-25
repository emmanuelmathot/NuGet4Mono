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
            try {
                projecturi = new Uri(uri);
            } catch {
                projecturi = null;       
            }
        }

        public Uri ProjectUri {
            get {
                return projecturi;
            }
        }
            
    }
}

