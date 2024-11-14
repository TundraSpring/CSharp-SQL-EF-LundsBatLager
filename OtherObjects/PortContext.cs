using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using System.Data.Entity;
//using LundsBåtLager.OtherObjects;

namespace LundsBåtLager
{
    //Simple DbContext class. There is no configuration, so unless I'm missing something it should always automatically
    //create the necessary database and tables when those are missing. Might not work if you close the program while it's
    //creating the database or it's tables.
    internal class PortContext : DbContext
    {
        public DbSet<Boat> Boats { get; set; } //table in database

        public DbSet<BoatStorageInstance> BSIs { get; set; } //table in database
    }
}
