using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsBåtLager
{
    internal class BoatStorageInstance
    {
        [Key]
        public Guid id  { get; set; }

        [Required]
        public DateTime ArrivalDate { get; set; }

        [Required]
        public DateTime DepartureDate { get; set;}

        [Required]
        public Guid BoatId { get; set; }

        [Required]
        public int Slot { get; set; }

        public BoatStorageInstance()
        {

        }

        public BoatStorageInstance(DateTime arrivalDate, Guid boatId, int slot)
        {
            id = Guid.NewGuid();
            ArrivalDate = arrivalDate;
            //Database freaks out at a DateTime?, so in lack of a better solution this value is treated as null if earlier than arrivalDate
            DepartureDate = arrivalDate.Subtract(TimeSpan.FromDays(365));
            BoatId = boatId;
            Slot = slot;
        }
    }
}
