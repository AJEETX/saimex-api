using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApi.Model
{
    public class Vehicle
    {
        [BsonId]
        public ObjectId ID { get; set; }
        // external Id, easier to reference: 1,2,3 or A, B, C etc.
        public long PId { get; set; }         
        public string Name { get; set; }
        public string Detail { get; set; }
        public bool Watch { get; set; }
        public string Time { get; set; }
        public string Location { get; set; }
        public decimal Amountlost { get; set; }
        public string EventNo { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }         
    }
    public class VehicleDto
    {
        public long PId { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
        public bool Watch { get; set; }
        public string Time { get; set; }
        public string Location { get; set; }
        public decimal Amountlost { get; set; }
        public string EventNo { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }         
    }    
}