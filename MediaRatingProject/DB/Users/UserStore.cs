namespace MediaRatingProject.DB.Users
{
    public class UserStore
    {
        public List<BaseUser> Users;
        public UserStore()
        {
            this.Users = new();
        }
    }
}
