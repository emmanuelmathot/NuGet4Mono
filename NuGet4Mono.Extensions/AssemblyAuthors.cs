using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NuGet4Mono.Extensions {
    
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public class AssemblyAuthorsAttribute : Attribute {
        
        Dictionary<string,string> authors;

        public AssemblyAuthorsAttribute() : this(string.Empty) {
            authors = new Dictionary<string, string>();
        }

        public AssemblyAuthorsAttribute(string txt) {
            authors = new Dictionary<string, string>();
            var entries = txt.Split(',');
            foreach (var entry in entries) {
                Match match = Regex.Match(txt.Trim(), "(?<name>.*)( <(?<email>.*)>)?");
                if (!match.Success)
                    continue;
                authors.Add(match.Groups["name"].Value, match.Groups["email"].Value);
            }
        }

        public Dictionary<string,string> Authors {
            get {
                return authors;
            }
        }
            
    }
}

