using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GridViewOnSteroids.Model
{
    public class Car
    {
        public string Name{ get; set; }
        public string Type { get; set; }
        public decimal Price{ get; set; }
        public string Country{ get; set; }
        public Boolean Availability { get; set; }
    }
}