﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangy_Models
{
    public class SuccessModelDTO
    {
        public string Title { get; set; }
        public int StatusCode { get; set; }
        public string Success { get; set; }
        public object Data { get; set; }
    }
}
