using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsBåtLager
{
    internal class Motorboat : Boat
    {
        [Required]
        public int horsepower { get; set; }

        public override int GetUniqueAttribute()
        {
            return horsepower;
        }

        public Motorboat()
        {

        }

        public Motorboat(byte i)
        {
            Random random = new Random();
            SetId("M");
            weight = random.Next(200, 3001);
            maxSpeed = random.Next(0, 61);
            horsepower = random.Next(10, 1001);
        }
    }
}
