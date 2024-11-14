//using LundsBåtLager.OtherObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LundsBåtLager
{
    internal class LundsBåtLager
    {
        BoatObj[] boatSpots = new BoatObj[128]; //On paper, there are 64 spots. For the sake of practicality, each spot
                                                //consists of two "subspots".
        List<BoatStorageInstance> BSIs = new List<BoatStorageInstance>();
        DateTime currentDay = DateTime.Today;
        List<(Boat boat, int slot)> todaysArrivals = new();
        List<Boat> rejectedBoats = new();

        public void Run()
        {   
            Initialize();
            ShowBoatInfo();
            while (true)
            {                
                RunDay();
                ShowBoatInfo();
            }
        }

        private void Initialize()
        {
            GetSavedData();
        }

        private void GetSavedData()
        {
            using (PortContext pc = new PortContext())
            {                                                    
                List<BoatStorageInstance> currentInstances = pc.BSIs
                    .Where(x => x.DepartureDate < x.ArrivalDate)
                    .OrderByDescending(d => d.ArrivalDate)
                    .ToList();

                if (currentInstances.Count > 0)
                {
                    currentDay = currentInstances[currentInstances.Count - 1].ArrivalDate;
                    BSIs = currentInstances;

                    for (int i = 0; i < currentInstances.Count; i++)
                    {
                        Guid boatId = currentInstances[i].BoatId;
                        Boat boat = pc.Boats.First(c => c.id == boatId);
                        int slot = currentInstances[i].Slot;
                        DockBoat(boat, slot, false);
                    }
                }

            }
        }

        private void RunDay()
        {
            RunDepartures();

            RunArrivals();

            UpdateDatabase();
            todaysArrivals.Clear();

            currentDay = currentDay.AddDays(1);
        }

        private void RunDepartures()
        {
            List<BoatStorageInstance> todaysDepartures = GetTodaysDepartures();
            RemoveBoats(todaysDepartures);
        }

        private List<BoatStorageInstance> GetTodaysDepartures()
        {
            List<BoatStorageInstance> todaysDepartures = new();

            for (int i = 0; i < BSIs.Count; i++)
            {
                BoatObj boat = boatSpots[BSIs[i].Slot];
                bool departing = false;

                //Most readable way I could think of writing this
                if (boat is Cargoboat cBoat && currentDay == BSIs[i].ArrivalDate.AddDays(6))
                    departing = true;
                else if (boat is Motorboat mBoat && currentDay == BSIs[i].ArrivalDate.AddDays(3))
                    departing = true;
                else if (boat is Rowboat rBoat && currentDay == BSIs[i].ArrivalDate.AddDays(1))
                    departing = true;
                else if (boat is Sailboat sBoat && currentDay == BSIs[i].ArrivalDate.AddDays(4))
                    departing = true;

                if (departing)
                    todaysDepartures.Add(BSIs[i]);
            }

            return todaysDepartures;
        }

        private void RemoveBoats(List<BoatStorageInstance> todaysRemovals)
        {
            using (PortContext pc = new())
            {
                for (int i = 0; i < todaysRemovals.Count; i++)
                {
                    RemoveBoat(todaysRemovals[i].Slot);
                    todaysRemovals[i].DepartureDate = currentDay;

                    //This sucks, but I do not have any more creative solution in mind right now
                    Guid guid = todaysRemovals[i].id;
                    var bsi =  pc.BSIs.FirstOrDefault(bsi => bsi.id == guid);
                    if (bsi != null)
                        bsi.DepartureDate = currentDay;
                }
                pc.SaveChanges();
            }

            //Removes dayStorageInstances connected to departed boats
            BSIs = BSIs.Where(x => x.DepartureDate < x.ArrivalDate).ToList();
        }

        private void RemoveBoat(int boatSpot)
        {
            int boatSize = Utilities.GetBoatSize(boatSpots[boatSpot]);

            for (int i = boatSpot; i < boatSpot + boatSize; i++)
                boatSpots[i] = null;
        }

        private void RunArrivals()
        {
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                int num = random.Next(1, 5);
                Boat newBoat = GenerateBoat(num);
                TryToDockBoat(newBoat);
            }
        }

        private Boat GenerateBoat(int num)
        {
            if (num == 1)
            {
                return new Cargoboat(1);
            }
            else if (num == 2)
            {
                return new Motorboat(1);
            }
            else if (num == 3)
            {
                return new Rowboat(1);
            }
            else // 4
            {
                return new Sailboat(1);
            }
        }

        private bool TryToDockBoat(Boat boat)
        {
            int boatsize = Utilities.GetBoatSize(boat);

            List<(int start, int length)> emptyRegions;
            if (boat is Rowboat)
            {
                emptyRegions = GetEmptySubspotRegions();
            }
            else
            {
                emptyRegions = GetEmptySpotRegions();
            }
            
            emptyRegions = [.. emptyRegions.OrderBy(i => i.length)];

            for (int i = 0; i < emptyRegions.Count; i++)
            {
                if (boatsize <= emptyRegions[i].length)
                {
                    DockBoat(boat, emptyRegions[i].start, true);
                    return true;
                }
            }
            rejectedBoats.Add(boat);
            return false;

        }

        private List<(int, int)> GetEmptySpotRegions()
        {
            List<(int start, int length)> emptyRegions = new List<(int start, int length)>();
            (int start, int length) emptyRegion = (-1, 0);
            for (int i = 0; i < 128; i += 2)
            {
                if (boatSpots[i] == null && boatSpots[i + 1] == null)
                {
                    if (emptyRegion.start == -1)
                    {
                        emptyRegion.start = i;
                    }
                    emptyRegion.length += 2;
                }
                else if ((boatSpots[i] != null || boatSpots[i + 1] != null) && emptyRegion.start != -1)
                {
                    emptyRegions.Add(emptyRegion);
                    emptyRegion = (-1, 0);
                }
            }
            if (emptyRegion.start != -1)
            {
                emptyRegions.Add(emptyRegion);
            }
            return emptyRegions;
        }

        private List<(int, int)> GetEmptySubspotRegions()
        {
            List<(int start, int length)> emptyRegions = new List<(int start, int length)>();
            (int start, int length) emptyRegion = (-1, 0);
            for (int i = 0; i < 128; i++)
            {
                if (boatSpots[i] == null)
                {
                    if (emptyRegion.start == -1)
                    {
                        emptyRegion.start = i;
                    }
                    emptyRegion.length++;
                }
                else if (boatSpots[i] != null && emptyRegion.start != -1)
                {
                    emptyRegions.Add(emptyRegion);
                    emptyRegion = (-1, 0);
                }
            }
            if (emptyRegion.start != -1)
            {
                emptyRegions.Add(emptyRegion);
            }
            return emptyRegions;
        }


        private void DockBoat(Boat boat, int spot, bool newBoat)
        {
            int size = Utilities.GetBoatSize(boat);
            boatSpots[spot] = boat;
            for (int i = spot + 1; i < spot + size; i++)
            {
                boatSpots[i] = new pBoat();
            }

            if (newBoat)
            {
                todaysArrivals.Add((boat, spot));
            }
        }

        private void UpdateDatabase()
        {
            using (PortContext pc = new())
            {
                for (int i = 0; i < todaysArrivals.Count; i++)
                {
                    pc.Boats.Add(todaysArrivals[i].boat);

                    BoatStorageInstance bsi = new(currentDay, todaysArrivals[i].boat.id, todaysArrivals[i].slot);
                    BSIs.Add(bsi);
                    pc.BSIs.Add(bsi);
                }
                pc.SaveChanges();
            }
        }

        private (List<Boat>, List<int>) GetDockedBoats()
        {
            List<Boat> dockedBoats = new();
            List<int> dockSlots = new();

            for (int i = 0; i < 128; i++)
            {
                if (boatSpots[i] is Boat boat)
                {
                    dockedBoats.Add(boat);
                    //Skips ahead to where the boat stops taking up space in the array
                    if (boat is Motorboat)
                        i++;
                    else if (boat is Sailboat)
                        i += 3;
                    else if (boat is Cargoboat)
                        i += 7;

                    dockSlots.Add(i);
                }
            }
            return (dockedBoats, dockSlots);
        }

        private void UpdateDeparturesSchedule()
        {
            //days.Add(days[0].AddDays(7));
            //days.RemoveAt(0);

            //departures.Add([]); //This apparently adds a List<int>.
            //departures.RemoveAt(0);
        }

        private void ShowBoatInfo()
        {
            ShowStorage();
            Console.WriteLine("");
            ShowBoatStats();

            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        private void ShowStorage()
        {
            Console.WriteLine("Place(s) " +
                "Boat type     " +
                "ID     " +
                "weight in kg   " +
                "Max speed in km   " +
                "Boat-specific attribute  "
                );


            for (int i = 0; i < 128; i += 2)
            {
                if (boatSpots[i] is pBoat)
                {
                    continue;
                }
                else if (boatSpots[i] == null)
                {
                    Console.WriteLine(((i / 2) + 1).ToString().PadRight(9) +
                    "---           " +
                    "---    " +
                    "---            " +
                    "---               " +
                    "---                      "
                  );
                }
                else if (boatSpots[i] is Boat boat)
                {
                    PrintBoat(i, boat);

                    if (boatSpots[i + 1] is Rowboat rowboat)
                        PrintBoat(i + 1, rowboat);
                }
            }
        }

        private void PrintBoat(int i, Boat boat)
        {
            string occupiedSlots = Utilities.GetBoatSlotDisplayString(i, boat);
            Console.WriteLine(occupiedSlots.PadRight(9) +
                              boat.GetType().Name.PadRight(14) +
                              boat.displayId.PadRight(7) +
                              boat.weight.ToString().PadRight(15) +
                              Utilities.GetSpeedInKm(boat.maxSpeed).ToString().PadRight(18) +
                              Utilities.GetBoatUniqueString(boat).PadRight(25)
                              );
        }

        //This method is too long, but I don't know any split method alternative I like better
        private void ShowBoatStats()
        {
            List<Boat> dockedBoats = boatSpots.Where(i => i is Boat).ToList().ConvertAll(i => (Boat)i);

            int rBs = dockedBoats.Where(x => x is Rowboat).Count();
            int cBs = dockedBoats.Where(x => x is Cargoboat).Count();
            int mBs = dockedBoats.Where(x => x is Motorboat).Count();
            int sBs = dockedBoats.Where(x => x is Sailboat).Count();
            Console.WriteLine($"Total Cargo, Motor, Row and Sail- boats: " + cBs + ", " + mBs + ", " + rBs + ", " + sBs);

            int weight = dockedBoats.Sum(x => x.weight);
            Console.WriteLine($"Total weight of all boats: " + weight + "kg");

            //You can't perform Average() on a list with no members without crashing. This is the cool but janky secret failsafe.
            if (dockedBoats.Count == 0)
            {
                Cargoboat failsafeDummy = new();
                failsafeDummy.maxSpeed = 0;
                dockedBoats.Add(failsafeDummy);
            }
            double avgSpeed = Utilities.GetSpeedInKm(Convert.ToInt32(dockedBoats.Average(x => x.maxSpeed)));
            Console.WriteLine($"Average speed of all boats: " + avgSpeed + "km/h");

            int emptySpots = boatSpots.Where(i => i is not BoatObj).Count() / 2;
            Console.WriteLine("Empty spots: " + emptySpots);

            Console.WriteLine("Total rejected boats (not saved in database): " + rejectedBoats.Count);
        }
    }
}
