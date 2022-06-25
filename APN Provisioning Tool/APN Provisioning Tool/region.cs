using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace APN_Provisioning_Tool
{
    [DelimitedRecord("\t")]
    class region
    {
        public long start;
        public long end;
        public string range_region;
    }
}
