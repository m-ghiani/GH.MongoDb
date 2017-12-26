namespace GH.MongoDb.Interfaces
{
    public class Location : ILocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}