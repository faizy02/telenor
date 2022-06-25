using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Prov_DataDumper
{
    [DelimitedRecord(" ")]
    class tech_file
    {
        public string iccid;
        public string msisdn;
        public string imsi;
        public string temp1,temp2,temp3,temp4,temp5;
        public string ki_value;
        public string temp6, temp7, temp8, temp9;

        public void getVar()
        {
            Console.WriteLine(iccid);
        }
    }

}
//8941006070435840271 03454914403 410060435840271 242012435840271 0000 15214800 1884 06830482 3EE5CDD946C8C6806F37D29AFE068E46 79520986 3A9318A5543269AF347A474736832E53 7A5C1A2C36E94DDC5C7425855E35612E 00000