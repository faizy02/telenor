using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Prov_DataDumper
{
    [DelimitedRecord("\t")]
    class ranges
    {
        public int count;
        public string range_start;
        public string range_end;
    }
}
