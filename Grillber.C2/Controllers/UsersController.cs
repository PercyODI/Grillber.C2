using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Grillber.C2.Controllers
{
    [RoutePrefix("api/Users")]
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

        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(StaticUsers);
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        public IHttpActionResult Get(Guid userId)
        {
            var foundTask = StaticUsers.FirstOrDefault(x => x.Id == userId);
            if (foundTask != null)
                return Ok(StaticUsers.First(x => x.Id == userId));
            else
                return NotFound();
        }

    }
    public class UserOut
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
