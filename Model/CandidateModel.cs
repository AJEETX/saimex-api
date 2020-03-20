using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApi.Model
{
    public class Candidate
    {
        [BsonId]
        public ObjectId ID { get; set; }
        // external Id, easier to reference: 1,2,3 or A, B, C etc.
        public string UserId { get; set; }         
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Role> Roles {get;set;}
        public string Username { get; set; }
        public string Location { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string CreatedBy { get; set; }        
    }
    public class CandidateDto
    {
        [BsonId]
        public int ID { get; set; }
        public string UserId { get; set; }         
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Location { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string Password { get; set; }
        public string CreatedBy { get; set; }        
    }
}
