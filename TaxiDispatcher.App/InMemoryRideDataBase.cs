using System.Collections.Generic;
using System.Linq;
using static TaxiDispatcher.App.Scheduler;

namespace TaxiDispatcher.App
{
    public static class InMemoryRideDataBase
    {
        public static List<Ride> Rides = new List<Ride>();

        public static void SaveRide(Ride ride)
        {
            var idList = GetRideIds();
            if (idList.Count > 0)
            {
                idList.Sort((x, y) => x > y ? -1 : 1);
                ride.Id = idList[0] + 1;
            }
            else
                ride.Id = 1;

            Rides.Add(ride);            
        }

        public static Ride GetRide(int id)
        {
            return Rides.FirstOrDefault(x => x.Id == id);
        }

        public static List<int> GetRideIds()
        {
            List<int> ids = new List<int>();
            foreach (Ride ride in Rides)
            {
                ids.Add(ride.Id);
            }

            return ids;
        }
    }
}
