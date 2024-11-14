using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsBåtLager
{
    /// <summary>
    /// Assistant class that provides values that are per-boat class and therefore don't need to be stored in fields.
    /// </summary>
    abstract internal class Utilities
    {
        public static int GetBoatSize(BoatObj boatObj)
        {
            if (boatObj is Cargoboat)
                return 8;
            else if (boatObj is Motorboat)
                return 2;
            else if (boatObj is Rowboat)
                return 1;
            else if (boatObj is Sailboat)
                return 4;
            else // (boatObj is PointerBoat)
                return -1;
        }

        public static int GetBoatStayDuration(BoatObj boatObj) //In days
        {
            if (boatObj is Cargoboat)
                return 6;
            else if (boatObj is Motorboat)
                return 3;
            else if (boatObj is Rowboat)
                return 1;
            else if (boatObj is Sailboat)
                return 4;
            else // (boatObj is PointerBoat)
                return -1;
        }

        public static int GetSpeedInKm(int speedinKnop)
        {
            return Convert.ToInt32(speedinKnop * 1.852);
        }

        public static string GetBoatUniqueString(Boat boat)
        {
            if (boat is Cargoboat)
            {
                return "Containers: " + boat.GetUniqueAttribute();
            }
            else if (boat is Motorboat)
            {
                return "Horsepower: " + boat.GetUniqueAttribute();
            }
            else if (boat is Rowboat)
            {
                return "Max passenger capacity: " + boat.GetUniqueAttribute();
            }
            else // (boat is Sailboat)
            {
                return "Boat Length: " + boat.GetUniqueAttribute();
            }
        }

        public static string GetBoatSlotDisplayString(int i, Boat boat)
        {
            int boatSize = Utilities.GetBoatSize(boat);
            string occupiedSlots = "" + ((i / 2) + 1);
            if (boat is Cargoboat || boat is Sailboat)
            {
                occupiedSlots += "-" + ((i + boatSize + 1) / 2);
            }
            return occupiedSlots;
        }
    }
}
