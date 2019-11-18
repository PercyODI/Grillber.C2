using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Grillber.C2.Controllers
{
    public class UsersController : ApiController
    {
        public static List<UserOut> StaticUsers { get; } = new List<UserOut>()
        {
            new UserOut()
            {
                Id = Guid.NewGuid(),
                FirstName = "Pearse",
                LastName = "Hutson",
                Username = "GrillMaster5767"
            },
            new UserOut()
            {
                Id = Guid.NewGuid(),
                FirstName = "Laura",
                LastName = "Laurason",
                Username = "Laraborabar"
            },
            new UserOut()
            {
                Id = Guid.NewGuid(),
                FirstName = "Marshall",
                LastName = "Eastfall",
                Username = "Marfall"
            }
        };

    }
    public class UserOut
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
