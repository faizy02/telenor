using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace TerminateIt
{
    [DelimitedRecord("\t")]
    class region
    {
        public long start;
        public long end;
        public string range_region;
    }
}
