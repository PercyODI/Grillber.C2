using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Grillber.C2.App_Start;
using Swashbuckle.Swagger.Annotations;

namespace Grillber.C2.Controllers
{
    [RoutePrefix("api/v2/Tasks")]
    public class TasksV2Controller : ApiController
    {
        [HttpGet]
        [Route()]
        [SwaggerResponse(HttpStatusCode.OK, "Get all tasks", typeof(IEnumerable<TaskOutV2>))]
        public IHttpActionResult Get()
        {
            return Ok(MappingRegistrations.mapper.Map<IEnumerable<TaskOutV2>>(TasksController.StaticTasks));
        }

        [HttpGet]
        [Route("{taskId:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, "Get a single task", typeof(TaskOutV2))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified task.")]
        public IHttpActionResult Get(Guid taskId)
        {
            var foundTask = TasksController.StaticTasks.FirstOrDefault(x => x.Id == taskId);
            if (foundTask != null)
                return Ok(MappingRegistrations.mapper.Map<TaskOutV2>(
                    TasksController.StaticTasks.First(x => x.Id == taskId)));
            else
                return NotFound();
        }

        [HttpPost]
        [Route()]
        [SwaggerResponse(HttpStatusCode.OK, "Create a new task.", typeof(TaskOutV2))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Input is incorrect. See message for details.")]
        public IHttpActionResult Post([FromBody] TaskNewV2 newV2Task)
        {
            if (newV2Task.ParentTaskId != null)
            {
                if (TasksController.StaticTasks.All(st => st.Id != newV2Task.ParentTaskId))
                {
                    return BadRequest("No task found with given ParentTaskId");
                }
            }

            if (newV2Task.UserId == null || newV2Task.UserId == Guid.Empty)
            {
                return BadRequest("UserId must be provided.");
            }

            if (UsersController.StaticUsers.All(x => x.Id != newV2Task.UserId))
            {
                return BadRequest("No User found with given UserId.");
            }

            if (string.IsNullOrWhiteSpace(newV2Task.TaskBody))
            {
                return BadRequest("TaskBody must be provided.");
            }

            var newCreatedTask = new FullTask()
            {
                Id = Guid.NewGuid(),
                ParentTaskId = newV2Task.ParentTaskId,
                CreatedDate = DateTime.UtcNow,
                TaskBody = newV2Task.TaskBody,
                UserId = UsersController.StaticUsers.First(x => x.Id == newV2Task.UserId).Id,
                CompletedDated = null,
                IsCompleted = false
            };
            TasksController.StaticTasks.Add(newCreatedTask);

            return Ok(MappingRegistrations.mapper.Map<TaskOutV2>(newCreatedTask));
        }

        [HttpPut]
        [Route("{taskId:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, "Update an existing task.", typeof(TaskOutV2))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Could not find the specified task.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Input is incorrect. See message for details.")]
        public IHttpActionResult Put(Guid taskId, [FromBody] TaskUpdateV2 updatedTask)
        {
            var foundTask = TasksController.StaticTasks.FirstOrDefault(x => x.Id == taskId);
            if (foundTask == null)
            {
                return NotFound();
            }

            if (updatedTask.ParentTaskId != null)
            {
                if (string.IsNullOrWhiteSpace(updatedTask.ParentTaskId))
                {
                    foundTask.ParentTaskId = null;
                }
                else
                {
                    // ReSharper disable once InlineOutVariableDeclaration
                    // Doesn't work in Azure deploy :(
                    Guid updatedParentTaskIdOut;
                    if (Guid.TryParse(updatedTask.ParentTaskId, out updatedParentTaskIdOut))
                    {
                        if (foundTask.Id == updatedParentTaskIdOut)
                        {
                            return BadRequest("Cannot set ParentTaskId to self.");
                        }

                        if (TasksController.StaticTasks.Any(st => st.Id == updatedParentTaskIdOut))
                        {
                            foundTask.ParentTaskId = updatedParentTaskIdOut;
                        }
                        else
                        {
                            return BadRequest("No task found with given ParentTaskId");
                        }
                    }
                    else
                    {
                        return BadRequest("ParentTaskId is not a valid GUID.");
                    }
                }
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


            return Ok(MappingRegistrations.mapper.Map<TaskOutV2>(foundTask));
        }

        [HttpDelete]
        [Route("{taskId:Guid}")]
        public IHttpActionResult Delete(Guid taskId)
        {
            var foundTask = TasksController.StaticTasks.FirstOrDefault(x => x.Id == taskId);
            if (foundTask == null)
            {
                return NotFound();
            }

            TasksController.StaticTasks.Remove(foundTask);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }

    public class TaskNewV2
    {
        public Guid UserId { get; set; }
        public Guid? ParentTaskId { get; set; }
        public string TaskBody { get; set; }
    }

    public class TaskUpdateV2
    {
        public string ParentTaskId { get; set; }
        public string TaskBody { get; set; }
        public bool? IsCompleted { get; set; }
        public Guid? UserId { get; set; }
    }

    public class TaskOutV2
    {
        public Guid Id { get; set; }
        public Guid? ParentTaskId { get; set; }
        public Guid UserId { get; set; }
        public string TaskBody { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDated { get; set; }
        public bool IsCompleted { get; set; }
    }
}