using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace APN_Provisioning_Tool
{
    [DelimitedRecord("\t")]
    class ranges
    {
        public int count;
        public string range_start;
        public string range_end;
    }
}
