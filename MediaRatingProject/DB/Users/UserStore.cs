namespace MediaRatingProject.DB.Users
{
    internal class UserStore
    {
        public List<BaseUser> Users;
        public UserStore()
        {
            this.Users = new();
        }
    }
}
