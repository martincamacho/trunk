using CastleClub.BusinessLogic.Data;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Managers
{
    public static class UsersManager
    {
        public static bool EmailExists(string email)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Users.Any(u => u.Email == email);
            }
        }

        public static UserDT GetUserByAspNetId(string aspNetId)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                User user = entities.Users.Where(u => u.AspNetId == aspNetId).FirstOrDefault();
                if (user == null)
                {
                    throw new InvalidUserException();
                }                
                return user.GetDT();
            }
        }

        public static UserDT GetUser(int id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                User user = entities.Users.Where(u => u.Id == id).FirstOrDefault();
                if (user == null)
                {
                    throw new InvalidUserException();
                }
                return user.GetDT();
            }
        }

        public static List<UserDT> GetUsers()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Users.Where(u => u.Active).ToList().Select(u => u.GetDT()).ToList();
            }
        }

        public static UserDT CreateUser(UserDT user)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                User newUser = new User();
                newUser.Email = user.Email;
                newUser.FirstName = user.FirstName;
                newUser.LastName = user.LastName;
                newUser.Role = user.Role;
                newUser.AspNetId = user.AspNetId;
                newUser.Active = true;

                entities.Users.Add(newUser);
                entities.SaveChanges();

                return newUser.GetDT();
            }
        }

        public static void DeleteUser(int id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                User user = entities.Users.Where(u => u.Id == id).FirstOrDefault();
                if (user == null)
                {
                    throw new InvalidUserException();
                }
                user.Active = false;
                entities.SaveChanges();
            }
        }
    }
}
