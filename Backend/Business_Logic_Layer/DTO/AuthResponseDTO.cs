﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.DTO
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string AntiForgeryToken { get; set; }


    }
}
