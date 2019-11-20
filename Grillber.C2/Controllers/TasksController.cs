using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Swashbuckle.Swagger.Annotations;

namespace Grillber.C2.Controllers
{
    [RoutePrefix("api/Tasks")]
    public class TasksController : ApiController
    {
        public static List<TaskOut> StaticTasks { get; } = new List<TaskOut>()
        {
            new TaskOut()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Parse("11/18/2019 4:45 PM").ToUniversalTime(),
                UserId = UsersController.StaticUsers.First(x => x.FirstName == "Pearse").Id,
                TaskBody = "Talk with Marketing about new R&D outreach program.",
                CompletedDated = DateTime.Parse("11/19/2019 3:55 PM").ToUniversalTime(),
                IsCompleted = true
            },
            new TaskOut()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Parse("11/17/2019 8:15 AM").ToUniversalTime(),
                UserId = UsersController.StaticUsers.First(x => x.FirstName == "Laura").Id,
                TaskBody = "Get list of possible Grill Models and Brands.",
                CompletedDated = null,
                IsCompleted = false
            },
            new TaskOut()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Parse("11/19/2019 1:01 PM").ToUniversalTime(),
                UserId = UsersController.StaticUsers.First(x => x.FirstName == "Marshall").Id,
                TaskBody = "Let Accounting know if smokers count as a grills. ",
                CompletedDated = null,
                IsCompleted = false
            }
        };
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Get all tasks", typeof(IEnumerable<TaskOut>))]
        public IHttpActionResult Get()
        {
            return Ok(StaticTasks);
        }

        [HttpGet]
        [Route("{taskId:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, "Get a single task", typeof(TaskOut))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified task.")]
        public IHttpActionResult Get(Guid taskId)
        {
            var foundTask = StaticTasks.FirstOrDefault(x => x.Id == taskId);
            if(foundTask != null)
                return Ok(StaticTasks.First(x => x.Id == taskId));
            else
                return NotFound();
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, "Create a new task.", typeof(TaskOut))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Input is incorrect. See message for details.")]
        public IHttpActionResult Post([FromBody] TaskNew newTask)
        {
            if (newTask.UserId == null || newTask.UserId == Guid.Empty)
            {
                return BadRequest("UserId must be provided.");
            }

            if (UsersController.StaticUsers.All(x => x.Id != newTask.UserId))
            {
                return BadRequest("No User found with given UserId.");
            }

            if (string.IsNullOrWhiteSpace(newTask.TaskBody))
            {
                return BadRequest("TaskBody must be provided.");
            }

            var newCreatedTask = new TaskOut()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                TaskBody = newTask.TaskBody,
                UserId = UsersController.StaticUsers.First(x => x.Id == newTask.UserId).Id,
                CompletedDated = null,
                IsCompleted = false
            };
            StaticTasks.Add(newCreatedTask);

            return Ok(newCreatedTask);
        }

        [HttpPut]
        [Route("{taskId:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, "Update an existing task.", typeof(TaskOut))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified task.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Input is incorrect. See message for details.")]
        public IHttpActionResult Put(Guid taskId, [FromBody] TaskUpdate updatedTask)
        {
            var foundTask = StaticTasks.FirstOrDefault(x => x.Id == taskId);
            if (foundTask == null)
            {
                return NotFound();
            }

            if (updatedTask.UserId.HasValue)
            {
                var foundUser = UsersController.StaticUsers.FirstOrDefault(x => x.Id == updatedTask.UserId);
                if (foundUser == null)
                {
                    return BadRequest("No User found with given UserId.");
                }

                foundTask.UserId = foundUser.Id;
            }

            if (updatedTask.IsCompleted.HasValue && updatedTask.IsCompleted != foundTask.IsCompleted)
            {
                foundTask.IsCompleted = updatedTask.IsCompleted.Value;
                if (updatedTask.IsCompleted.Value == true)
                {
                    foundTask.CompletedDated = DateTime.UtcNow;
                }
                else
                {
                    foundTask.CompletedDated = null;
                }
            }

            if (!string.IsNullOrWhiteSpace(updatedTask.TaskBody))
            {
                foundTask.TaskBody = updatedTask.TaskBody;
            }

            return Ok(foundTask);
        }

    }

    public class TaskNew
    {
        public Guid UserId { get; set; }
        public string TaskBody { get; set; }
    }

    public class TaskUpdate
    {

        public string TaskBody { get; set; }
        public bool? IsCompleted { get; set; }
        public Guid? UserId { get; set; }
    }

    public class TaskOut
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TaskBody { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDated { get; set; }
        public bool IsCompleted { get; set; }
    }

}
