using MediaRatingProject.DB.MediaTypes;
using MediaRatingProject.DB.Ratings;
using MediaRatingProject.DB.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingProject.Controllers
{
    using MediaRatingProject.DB.Users;

    internal class MediaDBController
    {

        #region Add/Remove methods
        /// <summary>
        /// Adds an entry to the media database.
        /// </summary>
        /// <param name="entry">The entry to add.</param>
        /// <param name="adder">The user who added the entry.</param>
        /// <returns>True if media was added successfully, false otherwise.</returns>
        public bool AddMediaEntry(MediaEntry entry, BaseUser adder)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes a media entry from the database.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="remover"></param>
        /// <returns>True if media was added successfully, false otherwise.</returns>
        public bool RemoveMediaEntry(MediaEntry entry, BaseUser remover)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a rating to a media entry.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="rating"></param>
        /// <returns>True if rating was added successfully.</returns>      
        public bool AddMediaRating(MediaEntry entry, MediaRating rating)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Update Entry Methods
        public bool UpdateEntryName(MediaEntry entry, BaseUser editor, string newName)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntryDescription(MediaEntry entry, BaseUser editor, string newDescription)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntryGenre(MediaEntry entry, BaseUser editor, string newGenre)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntryYear(MediaEntry entry, BaseUser editor, DateTime newYear)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEntryAgeRestriction(MediaEntry entry, BaseUser editor, Enums.AgeRestriction newAgeRestriction)
        {
            throw new NotImplementedException();
        }
#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool Updateaverage(MediaEntry entry)
        {
            entry.AverageRating = CalculateAverageRating(entry);
            return true;
        }

        private double CalculateAverageRating(MediaEntry entry)
        {
            return entry.Ratings.Average(rating => rating.Score);
        }
    }
}
