using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsBåtLager
{
    abstract internal class Boat : BoatObj
    {
        //Assignment-mandated id wasn't good enough, so this GUID was added to make the program not crash from duplicate keys
        [Key]
        [Required]
        public Guid id { get; set; } = Guid.NewGuid();

        [Required]
        public string displayId { get; set; }

        [Required]
        public int weight { get; set; } //In kg

        [Required]
        public int maxSpeed { get; set; } //in "knop"

        //Method that returns the subclass-specific value
        abstract public int GetUniqueAttribute();


        protected void SetId(string displayIdLetter)
        {
            Random random = new Random();
            displayId = displayIdLetter + "-" + (random.Next(0, 1000)).ToString().PadLeft(3, '0');
        }

    }
}
