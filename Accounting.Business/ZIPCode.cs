//CREATE TABLE "ZipCode"
//(
//    "ID" SERIAL PRIMARY KEY NOT NULL,
//    "Zip5" VARCHAR(5) NOT NULL,
//    "Latitude" FLOAT NOT NULL,
//    "Longitude" FLOAT NOT NULL,
//    "City" VARCHAR(50) NOT NULL,
//    "State2" VARCHAR(2) NOT NULL,
//    "Location" GEOGRAPHY(Point, 4326) NULL
//);

using Accounting.Common;

namespace Accounting.Business
{
  public class ZIPCode : IIdentifiable<int>
  {
    public int ID { get; set; }
    public string? Zip5 { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public string? City { get; set; }
    public string? State2 { get; set; }
    public int Identifiable => this.ID;
  }
}