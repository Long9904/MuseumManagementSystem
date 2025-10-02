using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomId : Attribute
    {
        public String Type { get; set; }
        public CustomId(String type)
        {
            Type = type;
        }
    }
}
