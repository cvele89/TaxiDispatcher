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

        protected Taxi taxi1 = new Taxi { DriverId = 1, DriverName = "Predrag", Company = taxiCompany_Naxi, CurrentLocation = 1 };
        protected Taxi taxi2 = new Taxi { DriverId = 2, DriverName = "Nenad", Company = taxiCompany_Naxi, CurrentLocation = 4 };
        protected Taxi taxi3 = new Taxi { DriverId = 3, DriverName = "Dragan", Company = taxiCompany_Alfa, CurrentLocation = 6 };
        protected Taxi taxi4 = new Taxi { DriverId = 4, DriverName = "Goran", Company = taxiCompany_Gold, CurrentLocation = 7 };

        public Ride OrderRide(int startLocation, int endLocation, RideType rideType, DateTime time)
        {
            #region FindingTheBestVehicle 

            Taxi nearestTaxi = taxi1;
            int min_distance = Math.Abs(taxi1.CurrentLocation - startLocation);

            if (Math.Abs(taxi2.CurrentLocation - startLocation) < min_distance)
            {
                nearestTaxi = taxi2;
                min_distance = Math.Abs(taxi2.CurrentLocation - startLocation);
            }

            if (Math.Abs(taxi3.CurrentLocation - startLocation) < min_distance)
            {
                nearestTaxi = taxi3;
                min_distance = Math.Abs(taxi3.CurrentLocation - startLocation);
            }

            if (Math.Abs(taxi4.CurrentLocation - startLocation) < min_distance)
            {
                nearestTaxi = taxi4;
                min_distance = Math.Abs(taxi4.CurrentLocation - startLocation);
            }

            if (min_distance > 15)
                throw new Exception("There are no available taxi vehicles!");

            #endregion

            #region CreatingRide

            Ride ride = new Ride();
            ride.TaxiInfo = nearestTaxi;
            ride.StartLocation = startLocation;
            ride.EndLocation = endLocation;

            #endregion

            CalculateRidePrice(ride, rideType, startLocation, endLocation, time);

            Console.WriteLine("Ride ordered, price: " + ride.Price.ToString());
            return ride;
        }

        public void AcceptRide(Ride ride)
        {
            InMemoryRideDataBase.SaveRide(ride);

            if (taxi1.DriverId == ride.TaxiInfo.DriverId)
            {
                taxi1.CurrentLocation = ride.EndLocation;
            }

            if (taxi2.DriverId == ride.TaxiInfo.DriverId)
            {
                taxi2.CurrentLocation = ride.EndLocation;
            }

            if (taxi3.DriverId == ride.TaxiInfo.DriverId)
            {
                taxi3.CurrentLocation = ride.EndLocation;
            }

            if (taxi4.DriverId == ride.TaxiInfo.DriverId)
            {
                taxi4.CurrentLocation = ride.EndLocation;
            }

            Console.WriteLine("Ride accepted, waiting for driver: " + ride.TaxiInfo.DriverName);
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

        private void CalculateRidePrice(Ride ride, RideType rideType, int startLocation, int endLocation, DateTime time)
        {
            ride.Price = ride.TaxiInfo.Company.PriceModifier * Math.Abs(startLocation - endLocation);

            if (rideType == RideType.InterCity)
                ride.Price *= 2;
            
            if (time.Hour < 6 || time.Hour > 22)
                ride.Price *= 2;
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
            public int Price { get; set; }
        }

        public class TaxiCompany
        {
            public string CompanyName { get; set; }
            public int PriceModifier { get; set; } = 1;
        }
    }
}
