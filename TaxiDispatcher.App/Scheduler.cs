using System;
using System.Collections.Generic;

namespace TaxiDispatcher.App
{
    public enum RideType
    {
        City,
        InterCity
    }

    public class Scheduler
    {
        protected static TaxiCompany taxiCompany_Naxi = new TaxiCompany { CompanyName = "Naxi", PriceModifier = 10 };
        protected static TaxiCompany taxiCompany_Alfa = new TaxiCompany { CompanyName = "Alfa", PriceModifier = 15 };
        protected static TaxiCompany taxiCompany_Gold = new TaxiCompany { CompanyName = "Gold", PriceModifier = 13 };

        protected List<Taxi> taxiDrivers = new List<Taxi>
        {
            new Taxi { DriverId = 1, DriverName = "Predrag", Company = taxiCompany_Naxi, CurrentLocation = 1 },
            new Taxi { DriverId = 2, DriverName = "Nenad", Company = taxiCompany_Naxi, CurrentLocation = 4 },
            new Taxi { DriverId = 3, DriverName = "Dragan", Company = taxiCompany_Alfa, CurrentLocation = 6 },
            new Taxi { DriverId = 4, DriverName = "Goran", Company = taxiCompany_Gold, CurrentLocation = 7 }
        };

        public void OrderRide(int startLocation, int endLocation, RideType rideType, DateTime time)
        {
            Console.WriteLine($"Ordering ride from {startLocation} to {endLocation}...");
            try
            {
                Ride ride = new Ride
                {
                    TaxiInfo = FindNearestTaxi(startLocation),
                    StartLocation = startLocation,
                    EndLocation = endLocation,
                    RideType = rideType,
                    Time = time
                };
                Console.WriteLine("Ride ordered, price: " + ride.Price.ToString());
                AcceptRide(ride);
            }
            catch (Exception e)
            {
                if (e.Message == "There are no available taxi vehicles!")
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("");
                }
                else
                    throw;
            }
        }

        public void AcceptRide(Ride ride)
        {
            InMemoryRideDataBase.SaveRide(ride);

            ride.TaxiInfo.CurrentLocation = ride.EndLocation;

            Console.WriteLine("Ride accepted, waiting for driver: " + ride.TaxiInfo.DriverName);
            Console.WriteLine("");
        }

        public List<Ride> GetRideList(int driverId)
        {
            List<Ride> rides = new List<Ride>();
            List<int> ids = InMemoryRideDataBase.GetRideIds();
            foreach (int id in ids)
            {
                Ride ride = InMemoryRideDataBase.GetRide(id);
                if (ride.TaxiInfo.DriverId == driverId)
                    rides.Add(ride);
            }

            return rides;
        }

        private Taxi FindNearestTaxi(int startLocation)
        {
            Taxi nearestTaxi = null;
            int min_distance = int.MaxValue;

            foreach (var taxi in taxiDrivers)
            {
                if (Math.Abs(taxi.CurrentLocation - startLocation) < min_distance)
                {
                    nearestTaxi = taxi;
                    min_distance = Math.Abs(taxi.CurrentLocation - startLocation);
                }
            }

            if (min_distance > 15)
                throw new Exception("There are no available taxi vehicles!");

            return nearestTaxi;
        }

        public class Taxi
        {
            public int DriverId { get; set; }
            public string DriverName { get; set; }
            public int CurrentLocation { get; set; }
            public TaxiCompany Company { get; set; }
        }

        public class Ride
        {
            public int Id { get; set; }
            public Taxi TaxiInfo { get; set; }
            public int StartLocation { get; set; }
            public int EndLocation { get; set; }
            public RideType RideType { get; set; }
            public DateTime Time { get; set; }

            private int _price = -1;
            public int Price
            {
                get
                {
                    if (_price < 0)
                    {
                        _price = TaxiInfo.Company.PriceModifier * Math.Abs(StartLocation - EndLocation);

                        if (RideType == RideType.InterCity)
                            _price *= 2;

                        if (Time.Hour < 6 || Time.Hour > 22)
                            _price *= 2;
                    }

                    return _price;                    
                }
            }
        }

        public class TaxiCompany
        {
            public string CompanyName { get; set; }
            public int PriceModifier { get; set; } = 1;
        }
    }
}
