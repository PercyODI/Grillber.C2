using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Results;
using Grillber.C2.App_Start;
using Swashbuckle.Swagger.Annotations;

namespace Grillber.C2.Controllers
{
    [RoutePrefix("api/v1/Tasks")]
    public class TasksController : ApiController
    {
        public static List<FullTask> StaticTasks { get; } = new List<FullTask>()
        {
            new FullTask()
            {
                Id = Guid.NewGuid(),
                ParentTaskId = null,
                CreatedDate = DateTime.Parse("11/18/2019 4:45 PM").ToUniversalTime(),
                UserId = UsersController.StaticUsers.First(x => x.FirstName == "Pearse").Id,
                TaskBody = "Talk with Marketing about new R&D outreach program.",
                CompletedDated = DateTime.Parse("11/19/2019 3:55 PM").ToUniversalTime(),
                IsCompleted = true
            },
            new FullTask()
            {
                Id = Guid.NewGuid(),
                ParentTaskId = null,
                CreatedDate = DateTime.Parse("11/17/2019 8:15 AM").ToUniversalTime(),
                UserId = UsersController.StaticUsers.First(x => x.FirstName == "Laura").Id,
                TaskBody = "Get list of possible Grill Models and Brands.",
                CompletedDated = null,
                IsCompleted = false
            },
            new FullTask()
            {
                Id = Guid.NewGuid(),
                ParentTaskId = null,
                CreatedDate = DateTime.Parse("11/19/2019 1:01 PM").ToUniversalTime(),
                UserId = UsersController.StaticUsers.First(x => x.FirstName == "Marshall").Id,
                TaskBody = "Let Accounting know if smokers count as a grills. ",
                CompletedDated = null,
                IsCompleted = false
            }
        };

        [HttpGet]
        [Route()]
        [SwaggerResponse(HttpStatusCode.OK, "Get all tasks", typeof(IEnumerable<TaskOutV1>))]
        public IHttpActionResult Get()
        {
            return Ok(MappingRegistrations.mapper.Map<IEnumerable<TaskOutV1>>(StaticTasks));
        }

        [HttpGet]
        [Route("{taskId:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, "Get a single task", typeof(TaskOutV1))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified task.")]
        public IHttpActionResult Get(Guid taskId)
        {
            var foundTask = StaticTasks.FirstOrDefault(x => x.Id == taskId);
            if(foundTask != null)
                return Ok(MappingRegistrations.mapper.Map<TaskOutV1>(foundTask));
            else
                return NotFound();
        }

        [HttpPost]
        [Route()]
        [SwaggerResponse(HttpStatusCode.OK, "Create a new task.", typeof(TaskOutV1))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Input is incorrect. See message for details.")]
        public IHttpActionResult Post([FromBody] TaskNewV1 newV1Task)
        {
            if (newV1Task.UserId == null || newV1Task.UserId == Guid.Empty)
            {
                return BadRequest("UserId must be provided.");
            }

            if (UsersController.StaticUsers.All(x => x.Id != newV1Task.UserId))
            {
                return BadRequest("No User found with given UserId.");
            }

            if (string.IsNullOrWhiteSpace(newV1Task.TaskBody))
            {
                return BadRequest("TaskBody must be provided.");
            }

            var newCreatedTask = new FullTask()
            {
                Id = Guid.NewGuid(),
                ParentTaskId = null,
                CreatedDate = DateTime.UtcNow,
                TaskBody = newV1Task.TaskBody,
                UserId = UsersController.StaticUsers.First(x => x.Id == newV1Task.UserId).Id,
                CompletedDated = null,
                IsCompleted = false
            };
            StaticTasks.Add(newCreatedTask);

            return Ok(MappingRegistrations.mapper.Map<TaskOutV1>(newCreatedTask));
        }

        [HttpPut]
        [Route("{taskId:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, "Update an existing task.", typeof(TaskOutV1))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified task.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Input is incorrect. See message for details.")]
        public IHttpActionResult Put(Guid taskId, [FromBody] TaskUpdateV1 updatedTask)
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

            return Ok(MappingRegistrations.mapper.Map<TaskOutV1>(foundTask));
        }

        [HttpDelete]
        [Route("{taskId:Guid}")]
        [SwaggerResponse(HttpStatusCode.NoContent, "Task Deleted Successfully")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified task.")]
        public IHttpActionResult Delete(Guid taskId)
        {
            var foundTask = StaticTasks.FirstOrDefault(x => x.Id == taskId);
            if (foundTask == null)
            {
                return NotFound();
            }

            StaticTasks.Remove(foundTask);

            return StatusCode(HttpStatusCode.NoContent);
        }

    }

    public class TaskNewV1
    {
        public Guid UserId { get; set; }
        public string TaskBody { get; set; }
    }

    public class TaskUpdateV1
    {
        public string TaskBody { get; set; }
        public bool? IsCompleted { get; set; }
        public Guid? UserId { get; set; }
    }

    public class FullTask
    {
        public Guid Id { get; set; }
        public Guid? ParentTaskId { get; set; }
        public Guid UserId { get; set; }
        public string TaskBody { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDated { get; set; }
        public bool IsCompleted { get; set; }

    }

    public class TaskOutV1
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TaskBody { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDated { get; set; }
        public bool IsCompleted { get; set; }
    }

}
