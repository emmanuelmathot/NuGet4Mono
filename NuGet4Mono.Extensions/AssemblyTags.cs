using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NuGet4Mono.Extensions {
    
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyTagsAttribute : Attribute {
        
        string tags;

        public AssemblyTagsAttribute() : this(string.Empty) {
            tags = null;
        }

        public AssemblyTagsAttribute(string txt) {
            tags = txt;
        }

        public string Tags {
            get {
                return tags;
            }
        }
            
    }
}

