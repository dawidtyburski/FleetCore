namespace FleetCore.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        public string Plate { get; set; }

        public long Mileage { get; set; }

        public string VIN { get; set; }

        public string? Note { get; set; }

        public IEnumerable<AppUser>? Users { get; set; }

        public IEnumerable<Event>? Events { get; set; }

        public IEnumerable<Repair>? Repairs { get; set; }

        public IEnumerable<Refueling>? Refuelings { get; set; }

        public Organization Organization { get; set; }
    }
}
