using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;

namespace Grillber.C2.Controllers
{
    [RoutePrefix("api/v1/Users")]
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
        [Route()]
        [SwaggerResponse(HttpStatusCode.OK, "Get all users", typeof(IEnumerable<UserOut>))]
        public IHttpActionResult Get()
        {
            return Ok(StaticUsers);
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, "Get a single user", typeof(UserOut))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified user.")]
        public IHttpActionResult Get(Guid userId)
        {
            var foundTask = StaticUsers.FirstOrDefault(x => x.Id == userId);
            if (foundTask != null)
                return Ok(StaticUsers.First(x => x.Id == userId));
            else
                return NotFound();
        }

        [HttpPost]
        [Route()]
        [SwaggerResponse(HttpStatusCode.OK, "Create a new user.", typeof(UserOut))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Input is incorrect. See message for details.")]
        public IHttpActionResult Post([FromBody] UserNew newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Username))
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
                Username = newUser.Username
            };
            StaticUsers.Add(createdUser);

            return Ok(createdUser);
        }

        [HttpPut]
        [Route("{userId:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, "Update an existing user.", typeof(UserOut))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified user.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Input is incorrect. See message for details.")]
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

        [HttpDelete]
        [Route("{userId:Guid}")]
        [SwaggerResponse(HttpStatusCode.NoContent, "User Deleted Successfully")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified user.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Input is incorrect. See message for details.")]
        public IHttpActionResult Delete(Guid userId)
        {
            var foundUser = StaticUsers.FirstOrDefault(x => x.Id == userId);
            if (foundUser == null)
            {
                return NotFound();
            }

            if (TasksController.StaticTasks.Any(st => st.UserId == foundUser.Id))
            {
                return BadRequest("User has assigned tasks. Cannot delete.");
            }

            StaticUsers.Remove(foundUser);

            return StatusCode(HttpStatusCode.NoContent);
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
