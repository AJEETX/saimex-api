using System;
using WebApi.Helpers;
using WebApi.Model;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace WebApi.Services
{
    public interface IVehicleService
    {
        List<Vehicle> Get(string userid ,string q);
        Vehicle GetById(long id);
        Vehicle Add(Vehicle product);
        bool Update(Vehicle product);
        bool Delete(long id);
    }
    public class VehicleService : IVehicleService
    {
        readonly DataContext _context;
        Random random=new Random();
        public VehicleService(IOptions<Settings> settings)
        {
            _context = new DataContext(settings);
        }
        public Vehicle Add(Vehicle vehicle)
        {
            try{
                vehicle.PId=random.Next();
                _context.Vehicles.InsertOne(vehicle);
                return vehicle;
            }
            catch(AppException){
                return null;
            }
        }
        public List<Vehicle> Get(string userid,string q = "")
        {
            var vehicles=default(List<Vehicle>);
            try
            {
                if(!string.IsNullOrWhiteSpace(q)){
                    q=q.ToLowerInvariant();
                    vehicles= _context.Vehicles.Find(p => p.Name.ToLowerInvariant().Contains(q) || 
                    p.Location.ToLowerInvariant().Contains(q) || 
                    p.EventNo.ToLowerInvariant().Contains(q)  || 
                    p.Detail.ToLowerInvariant().Contains(q) || p.Time.Contains(q) )?.ToList();
                }
                else{
                    vehicles=_context.Vehicles.Find(_context=>true).ToList();
                }

            }
            catch (AppException)
            {
                // log or manage the exception
            }
            return vehicles;
        }
        public Vehicle GetById(long id)
        {
            var vehicle=default(Vehicle);
            try
            {
                vehicle= _context.Vehicles.Find(p => p.PId == id)?.FirstOrDefault();
            }
            catch (AppException)
            {
                // log or manage the exception
            }
            return vehicle;
        }
        // Try to convert the Id to a BSonId value
        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }
        public bool Update(Vehicle vehicleInfo)
        {
            try
            {
                var filter = Builders<Vehicle>.Filter.Eq(p => p.PId, vehicleInfo.PId);
                var update = Builders<Vehicle>.Update
                .Set(p => p.Name,vehicleInfo.Name)
                .Set(p => p.EventNo, vehicleInfo.EventNo)
                .Set(p => p.Location, vehicleInfo.Location)
                .Set(p => p.Watch, vehicleInfo.Watch)
                .Set(p => p.Detail, vehicleInfo.Detail)
                .Set(p => p.Date, vehicleInfo.Date)
                .Set(p => p.Time, vehicleInfo.Time)
                .Set(p => p.Amountlost, vehicleInfo.Amountlost);      
                var updateResult = _context.Vehicles.UpdateOne(filter,update);

                return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
            }
            catch (AppException)
            {
                // log or manage the exception
                return false;
            }
        }
        public bool Delete(long id)
        {
            try
            {
                DeleteResult actionResult = _context.Vehicles.DeleteOne(Builders<Vehicle>.Filter.Eq("PId", id));

                return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
            }
            catch (AppException)
            {
                // log or manage the exception
                return false;
            }
        }
    }
}