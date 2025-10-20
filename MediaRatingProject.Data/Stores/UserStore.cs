namespace MediaRatingProject.Data.Stores
{
    using MediaRatingProject.Data.Users;

    public class UserStore
    {
        /// <summary>
        /// ID counter to assign unique IDs to each media added. It never goes down, even if media is removed.
        /// </summary>
        private int _idCount;
        private Dictionary<int, BaseUser> _userStore;

        public UserStore()
        {
            _idCount = 1;
            _userStore = new Dictionary<int, BaseUser>();        
        }

        public bool AddUser(BaseUser user)
        {
            if (_userStore.Values.Any(u => u.Username == user.Username))
            {
                Console.WriteLine("User with the same username already exists.");
                return false;
            }

            user.Id = _idCount;
            _userStore.Add(_idCount, user);
            _idCount++;

            return true;
        }


        public bool RemoveUser(int userId)
        {
            return _userStore.Remove(userId);
        }

        public bool UpdateUser(BaseUser updatedUser, int id)
        {
            if (_userStore.ContainsKey(id))
            {
                _userStore[id] = updatedUser;
                return true;
            }
            return false;
        }

        public BaseUser GetUserById(int id)
        {
            _userStore.TryGetValue(id, out var user);
            return user;
        }

        public BaseUser GetUserByUsername(string username)
        {
            return _userStore.Values.FirstOrDefault(u => u.Username == username);
        }
    }
}
