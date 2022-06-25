using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace APN_Provisioning_Tool
{
    [DelimitedRecord(",")]
    public class msisdnINFO
    {
        public string msisdn;
        public string ip;
    }
}
