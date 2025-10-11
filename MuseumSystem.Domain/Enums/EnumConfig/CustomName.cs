﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Enums.EnumConfig
{
    public class CustomName : Attribute
    {
        public string Names { get; set; }

        public CustomName(string name)
        {
            Names = name;
        }
    }
}
