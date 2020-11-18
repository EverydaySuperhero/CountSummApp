using System.Collections.Generic;

namespace CountSummLib.Models
{
    public struct StructFolder
    {
        public List<string> Files { get; set; }
        public List<string> Directories { get; set; }
        public string BaseDirectory { get; set; }
    }
}
