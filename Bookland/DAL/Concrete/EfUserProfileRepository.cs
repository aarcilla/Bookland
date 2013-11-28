using Bookland.DAL.Abstract;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Bookland.DAL.Concrete
{
    public class EfUserProfileRepository : IUserProfileRepository, IDisposable
    {
        private BookshopContext context;

        public EfUserProfileRepository(BookshopContext context)
        {
            this.context = context;
        }

        public IEnumerable<UserProfile> GetUserProfiles(Expression<Func<UserProfile, string>> order, bool descending = false)
        {
            return !descending ? context.UserProfiles.OrderBy(order) : context.UserProfiles.OrderByDescending(order);
        }

        public IEnumerable<UserProfile> GetUserProfiles(Expression<Func<UserProfile, int>> order, bool descending = false)
        {
            return !descending ? context.UserProfiles.OrderBy(order) : context.UserProfiles.OrderByDescending(order);
        }
        public IEnumerable<UserProfile> GetUserProfiles(Expression<Func<UserProfile, DateTime>> order, bool descending = false)
        {
            return !descending ? context.UserProfiles.OrderBy(order) : context.UserProfiles.OrderByDescending(order);
        }

        public UserProfile GetUserProfile(int userID)
        {
            return context.UserProfiles.Include("Address").FirstOrDefault(u => u.UserID == userID);
        }

        public UserProfile GetUserProfile(string userName)
        {
            return context.UserProfiles.Include("Address").FirstOrDefault(u => u.UserName == userName);
        }

        public void UpdateUserProfile(UserProfile userProfile)
        {
            UserProfile dbUserProfile = GetUserProfile(userProfile.UserName);

            dbUserProfile.FirstName = userProfile.FirstName;
            dbUserProfile.LastName = userProfile.LastName;
            dbUserProfile.Email = userProfile.Email;
        }

        public Address GetAddress(int addressID)
        {
            return context.Addresses.FirstOrDefault(a => a.AddressID == addressID);
        }

        public Address GetAddress(string userName)
        {
            return context.Addresses.FirstOrDefault(a => a.UserProfile.UserName == userName);
        }

        public void CreateAddress(Address address, string userName)
        {
            address.UserProfile = GetUserProfile(userName);

            context.Addresses.Add(address);
        }

        public void UpdateAddress(Address address, string userName)
        {
            Address dbAddress = GetAddress(userName);

            dbAddress.StreetLine1 = address.StreetLine1;
            dbAddress.StreetLine2 = address.StreetLine2;
            dbAddress.City = address.City;
            dbAddress.State = address.State;
            dbAddress.Country = address.Country;
            dbAddress.Postcode = address.Postcode;
        }

        public void DeleteAddress(int addressID)
        {
            Address dbAddress = GetAddress(addressID);

            context.Addresses.Remove(dbAddress);
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Dispose of the database context to ensure DB connection is properly closed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}