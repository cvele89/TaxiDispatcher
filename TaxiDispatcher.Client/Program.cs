using System;
using TaxiDispatcher.App;

namespace TaxiDispatcher.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Scheduler scheduler = new Scheduler();

            scheduler.OrderRide(5, 0, RideType.City, new DateTime(2018, 1, 1, 23, 0, 0));
            scheduler.OrderRide(0, 12, RideType.InterCity, new DateTime(2018, 1, 1, 9, 0, 0));
            scheduler.OrderRide(5, 0, RideType.City, new DateTime(2018, 1, 1, 11, 0, 0));
            scheduler.OrderRide(35, 12, RideType.City, new DateTime(2018, 1, 1, 11, 0, 0));

            scheduler.PrintDriverEarnings(2);

            Console.ReadLine();
        }
    }
}
