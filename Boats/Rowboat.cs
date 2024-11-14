using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsBåtLager
{
    internal class Rowboat : Boat
    {
        [Required]
        public int maxPassengers { get; set; }

        public override int GetUniqueAttribute()
        {
            return maxPassengers;
        }

        public Rowboat()
        {

        }

        public Rowboat(byte i)
        {
            Random random = new Random();
            SetId("R");
            weight = random.Next(100, 301);
            maxSpeed = random.Next(0, 3);
            maxPassengers = random.Next(1, 7);
        }
    }
}
