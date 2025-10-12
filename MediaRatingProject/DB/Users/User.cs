using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingProject.DB.Users
{
    public class User: BaseUser
    {
        public User(string Name, string password): base(Name, password) { }
    }
}
