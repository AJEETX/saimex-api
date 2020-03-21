using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApi.Helpers;
using WebApi.Identity;
using WebApi.Model;

namespace WebApi.Services
{
    public interface IUserService
    {
        User Register(User user, string password);
        User Create(User user);
        User Authenticate(string username, string password);
        List<User> GetUsers(string userid,string q = "");
        User GetUserById(string id);
        bool UpdateUser(User user);
    }

    public class UserService : IUserService
    {
        private DataContext _context;
        private ITokeniser _tokeniser;

        public UserService(IOptions<Settings> settings, ITokeniser tokeniser)
        {
            _context = new DataContext(settings);
            _tokeniser = tokeniser;
        }
        public User Register(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Find(u => u.Username == user.Username.Trim()).FirstOrDefault()!=null)
                throw new AppException("Username '" + user.Username + "' is already taken");
            try
            {    
                byte[] passwordHash, passwordSalt;
                PasswordHasher.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.Roles=new List<Role>{new Role{Name= "User" }};
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.DateCreated=DateTime.Now;
                user.DateModified=DateTime.Now;
                user.Username=user.Username.Trim();
                user.UserId=_tokeniser.CreateToken(user.FirstName,user.LastName);
                _context.Users.InsertOne(user);
            }
            catch (AppException)
            {
                //shout/catch/throw/log
            }
            return user;
        }
        public User Create(User user)
        {
            if (user==null)
                throw new AppException("Password is required");

            if (_context.Users.Find(u => u.Username == user.Username.Trim()).FirstOrDefault()!=null)
                throw new AppException("Username '" + user.Username + "' is already taken");
            try
            {    
                user.Roles=new List<Role>{new Role{Name= "User" }};
                user.DateCreated=DateTime.Now;
                user.DateModified=DateTime.Now;
                user.Username=user.Username.Trim();
                user.UserId=_tokeniser.CreateToken(user.FirstName,user.LastName);
                _context.Users.InsertOne(user);
            }
            catch (AppException)
            {
                //shout/catch/throw/log
            }
            return user;
        }
        
        public User GetUserById(string id)
        {
            var user=default(User);
            if(string.IsNullOrWhiteSpace(id)) return user;
            try{
                user= _context.Users.Find(u=>u.ID==GetInternalId(id) || u.UserId==id )?.FirstOrDefault();
            }
            catch (AppException)
            {
                //shout/catch/throw/log
            }        
            return user;    
        }
        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new AppException("No username/password");
                var user=default(User);
            try
            {    
                 user = _context.Users.Find(x => x.Username == username).FirstOrDefault();

                if (user == null)
                    throw new AppException("No user found");

                if (!PasswordHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    throw new AppException("Password incorrect");
            }
            catch (AppException)
            {
                //shout/catch/throw/log
            }     
            return user;
        }

        public List<User> GetUsers(string userid,string q = "")
        {
            List<User> users=default(List<User>);
            try{
                if(!string.IsNullOrWhiteSpace(userid)){
                    users= _context.Users.Find(u=>u.CreatedBy.ToLowerInvariant()==userid.ToLowerInvariant()).ToList();
                    if(!string.IsNullOrWhiteSpace(q)){
                        q=q.ToLowerInvariant();
                        users= _context.Users.Find(u=>u.FirstName.ToLowerInvariant().Contains(q) ||
                        u.LastName.ToLowerInvariant().Contains(q) || u.Username.ToLowerInvariant().Contains(q) && 
                        u.Username.ToLowerInvariant()==userid.ToLowerInvariant()).ToList();                    
                    }
                        
                }
                else{
                    users= _context.Users.Find(_=>true).ToList();
                }
            }
            catch (AppException)
            {
               //shout/catch/throw/log
            }     
            return users;          
        }

        public bool UpdateUser(User user)
        {
            try{
                var filter = Builders<User>.Filter.Eq(s => s.Username, user.Username);
                var update = Builders<User>.Update.Set(s => s.FirstName, user.FirstName)
                .Set(s => s.LastName, user.LastName)
                .Set(s => s.DateModified, DateTime.Now)
                .Set(s => s.Location, user.Location);            
                var updateResult = _context.Users.UpdateOne(filter,update);
                return updateResult.IsAcknowledged && updateResult.MatchedCount>0;                
            }
            catch (AppException)
            {
               return false; //shout/catch/throw/log
            }               
        }
                // Try to convert the Id to a BSonId value
        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }
    }
}