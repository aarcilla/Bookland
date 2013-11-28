using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bookland.DAL.Abstract
{
    public interface IUserProfileRepository : IDisposable
    {
        /// <summary>
        /// Retrieve all stored user profiles, based on specified order.
        /// </summary>
        /// <param name="order">A function specifying the property to order the user profiles by. For string-type properties.</param>
        /// <param name="descending">A Boolean specifying whether the order is descending (e.g. Z-A) or ascending (e.g. A-Z).</param>
        /// <returns>An enumeration of stored user profiles, in its desired order.</returns>
        IEnumerable<UserProfile> GetUserProfiles(Expression<Func<UserProfile, string>> order, bool descending = false);

        /// <summary>
        /// Retrieve all stored user profiles, based on specified order.
        /// </summary>
        /// <param name="order">A function specifying the property to order the user profiles by. For integer-type properties.</param>
        /// <param name="descending">A Boolean specifying whether the order is descending (e.g. Z-A) or ascending (e.g. A-Z).</param>
        /// <returns>An enumeration of stored user profiles, in its desired order.</returns>
        IEnumerable<UserProfile> GetUserProfiles(Expression<Func<UserProfile, int>> order, bool descending = false);

        /// <summary>
        /// Retrieve all stored user profiles, based on specified order.
        /// </summary>
        /// <param name="order">A function specifying the property to order the user profiles by. For DateTime-type properties.</param>
        /// <param name="descending">A Boolean specifying whether the order is descending (e.g. Z-A) or ascending (e.g. A-Z).</param>
        /// <returns>An enumeration of stored user profiles, in its desired order.</returns>
        IEnumerable<UserProfile> GetUserProfiles(Expression<Func<UserProfile, DateTime>> order, bool descending = false);

        /// <summary>
        /// Retrieve a specific user profile, based on ID.
        /// </summary>
        /// <param name="userID">The ID specified in the requested user profile.</param>
        /// <returns>A UserProfile object of the requested user profile.</returns>
        UserProfile GetUserProfile(int userID);

        /// <summary>
        /// Retrieve a specific user profile, based on user name.
        /// </summary>
        /// <param name="userName">The name specified in the requested user profile.</param>
        /// <returns>A UserProfile object of the requested user profile.</returns>
        UserProfile GetUserProfile(string userName);

        /// <summary>
        /// Update an existing user profile's values to the DB.
        /// </summary>
        /// <param name="userProfile">The user profile to update, with updated values.</param>
        void UpdateUserProfile(UserProfile userProfile);

        /// <summary>
        /// Retrieve a specific address, based on ID.
        /// </summary>
        /// <param name="addressID">The ID of the requested address.</param>
        /// <returns>An Address object of the requested address.</returns>
        Address GetAddress(int addressID);

        /// <summary>
        /// Retrieve a specific address, based on the associated user's name.
        /// </summary>
        /// <param name="userName">The address's associated user's name.</param>
        /// <returns>An Address object of the requested address.</returns>
        Address GetAddress(string userName);

        /// <summary>
        /// Add an address to the DB.
        /// </summary>
        /// <param name="address">The address to add.</param>
        /// <param name="userName">The address's associated user's name.</param>
        void CreateAddress(Address address, string userName);

        /// <summary>
        /// Update an existing address's values to the DB.
        /// </summary>
        /// <param name="address">The address to update, with updated values.</param>
        /// <param name="userName">The address's associated user's name.</param>
        void UpdateAddress(Address address, string userName);

        /// <summary>
        /// Delete the specified category from the DB.
        /// </summary>
        /// <param name="addressID">The ID of the address to be deleted.</param>
        void DeleteAddress(int addressID);

        /// <summary>
        /// Commit any changes to the DB.
        /// </summary>
        void Commit();
    }
}