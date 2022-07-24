using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palette
{
    internal class ProcessIdentifier
    {
        [JsonProperty("name")]
        public string name;

        public ProcessIdentifier(string _name)
        {
            name = _name;
        }

        public ProcessIdentifier(Process process)
        {
            name = process.ProcessName;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var item = (ProcessIdentifier)obj;
            return item.name == name;
        }

        public static bool operator !=(ProcessIdentifier a, ProcessIdentifier b) => !(a == b);
        public static bool operator ==(ProcessIdentifier a, ProcessIdentifier b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }
    }
}
