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

        [HttpPost]
        public IHttpActionResult Post([FromBody] UserNew newUser)
        {
            if (!string.IsNullOrWhiteSpace(newUser.Username))
            {
                return BadRequest("Username must be provided.");
            }

            if (StaticUsers.Any(su => su.Username == newUser.Username))
            {
                return BadRequest("Username is already taken.");
            }

            var createdUser = new UserOut()
            {
                Id = Guid.NewGuid(),
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Username = newUser.LastName
            };
            StaticUsers.Add(createdUser);

            return Ok(createdUser);
        }

        [HttpPut]
        [Route("{userId:Guid}")]
        public IHttpActionResult Put(Guid userId, [FromBody] UserUpdate updatedUser)
        {
            var foundUser = StaticUsers.FirstOrDefault(su => su.Id == userId);
            if (foundUser == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(updatedUser.FirstName))
            {
                foundUser.FirstName = updatedUser.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(updatedUser.LastName))
            {
                foundUser.LastName = updatedUser.LastName;
            }

            return Ok(foundUser);
        } 

    }

    public class UserUpdate
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserNew
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }

    public class UserOut
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
