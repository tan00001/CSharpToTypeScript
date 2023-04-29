using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;

namespace CSharpToTypeScript.Options
{
    public class General : BaseOptionModel<General>, IRatingConfig
    {
        public int RatingRequests { get; set; }
    }
}
