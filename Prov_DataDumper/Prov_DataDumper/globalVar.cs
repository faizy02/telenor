﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prov_DataDumper
{
    public static class globalVar
    {
        public const string GlobalString = "Important Text";

        /// <summary>
        /// Static value protected by access routine.
        /// </summary>
        static int _globalValue;
        
        /// <summary>
        /// Access routine for global variable.
        /// </summary>
        public static int GlobalValue
        {
            get
            {
                return _globalValue;
            }
            set
            {
                _globalValue = value;
            }
        }

        /// <summary>
        /// Global static field.
        /// </summary>
        public static bool GlobalBoolean;

        
    }
}
