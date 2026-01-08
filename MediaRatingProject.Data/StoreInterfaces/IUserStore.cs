namespace MediaRatingProject.Data.StoreInterfaces
{
    using MediaRatingProject.Data.Users;

    public interface IUserStore
    {
        public bool CreateUser(BaseUser user);
        public bool RemoveUser(int userId);
        public bool UpdateUser(BaseUser updatedUser, int id);
        public BaseUser GetUserById(int id);
        public BaseUser GetUserByUsername(string username);
    }
}
