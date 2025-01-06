using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript
{
    public class TsGeneratorOutput
    {
        public string FileType { get; set; }
        public string Script { get; set; }
        public bool ExcludeFromResultToString { get; set; }

        public Dictionary<string, string> OtherFileTypes { get; private set; }

        public TsGeneratorOutput(string fileType, string script)
        {
            FileType = fileType;
            Script = script;
            OtherFileTypes = new Dictionary<string, string>();
        }
    }
}
