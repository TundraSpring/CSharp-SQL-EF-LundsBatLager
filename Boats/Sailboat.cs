using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsBåtLager
{
    internal class Sailboat : Boat
    {
        [Required]
        public int boatLength { get; set; }

        public override int GetUniqueAttribute()
        {
            return boatLength;
        }

        public Sailboat()
        {

        }

        public Sailboat(byte i)
        {
            Random random = new Random();
            SetId("S");
            weight = random.Next(800, 6001);
            maxSpeed = random.Next(0, 13);
            boatLength = random.Next(10, 61);
        }
    }
}
