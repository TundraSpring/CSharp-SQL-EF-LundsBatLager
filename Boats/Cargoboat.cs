using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsBåtLager
{
    internal class Cargoboat : Boat
    {
        [Required]
        public int containersOnBoard { get; set; }

        //Normally contructors should come before other methods, but here I think it made more sense to put the Get method next to the related field.
        public override int GetUniqueAttribute()
        {
            return containersOnBoard;
        }

        public Cargoboat()
        {

        }

        //This random byte needs to be here, because I need two different constructors and I want neither to actually
        //use any parameters
        public Cargoboat(byte i)
        {
            Random random = new Random();
            SetId("L");
            weight = random.Next(3000, 20001);
            maxSpeed = random.Next(0, 21);
            containersOnBoard = random.Next(0, 501);
        }
    }
}
