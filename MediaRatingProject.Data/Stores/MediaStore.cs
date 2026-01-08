using MediaRatingProject.Data.Media;
using MediaRatingProject.Data.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingProject.Data.Stores
{
    public class MediaStore
    {
        /// <summary>
        /// ID counter to assign unique IDs to each media added. It never goes down, even if media is removed.
        /// </summary>
        private int _idCount;
        private Dictionary<int, BaseMedia> _mediaStore;

        public MediaStore()
        {
            _idCount = 1;
            _mediaStore = new Dictionary<int, BaseMedia>();
        }

        public bool CreateMedia(BaseMedia media)
        {
            if (_mediaStore.Values.Any(m => m.Title == media.Title))
            {
                Console.WriteLine("Media of the same title already exists.");
                return false;
            }

            media.Id = _idCount;            
            _mediaStore.Add(_idCount, media);
            _idCount++;

            return true;
        }

        public BaseMedia GetMediaById(int id)
        {
            _mediaStore.TryGetValue(id, out var media);
            return media;
        }


        public bool RemoveMedia(int mediaId)
        {           
            return _mediaStore.Remove(mediaId);
        }

        public bool UpdateMedia(BaseMedia updatedMedia, int id)
        {
            if (_mediaStore.ContainsKey(id))
            {
                _mediaStore[id] = updatedMedia;
                return true;
            }

            return false;
        }

        public IReadOnlyCollection<BaseMedia> GetAllMedia()
        {
            return _mediaStore.Values.ToList().AsReadOnly();
        }
    }
}
