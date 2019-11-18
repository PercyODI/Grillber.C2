using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Grillber.C2.Controllers
{
    public class TasksController : ApiController
    {
        public static List<TaskOut> StaticTasks { get; } = new List<TaskOut>()
        {
            new TaskOut()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Parse("11/18/2019 4:45 PM").ToUniversalTime(),
                User = new UserOut()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Pearse",
                    LastName = "Hutson",
                    Username = "GrillMaster5767"
                },
                TaskBody = "Talk with Marketing about new R&D outreach program.",
                CompletedDated = DateTime.Parse("11/19/2019 3:55 PM").ToUniversalTime(),
                IsCompleted = true
            },
            new TaskOut()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Parse("11/17/2019 8:15 AM").ToUniversalTime(),
                User = new UserOut()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Laura",
                    LastName = "Laurason",
                    Username = "Laraborabar"
                },
                TaskBody = "Get list of possible Grill Models and Brands.",
                CompletedDated = null,
                IsCompleted = false
            },
            new TaskOut()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Parse("11/19/2019 1:01 PM").ToUniversalTime(),
                User = new UserOut()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Marshall",
                    LastName = "Eastfall",
                    Username = "Marfall"
                },
                TaskBody = "Let Accounting know if smokers count as a grills. ",
                CompletedDated = null,
                IsCompleted = false
            }
        };
        public IHttpActionResult Get()
        {
            return Ok(StaticTasks);
        }

        public IHttpActionResult Get(Guid taskId)
        {
            var foundTask = StaticTasks.FirstOrDefault(x => x.Id == taskId);
            if(foundTask != null)
                return Ok(StaticTasks.First(x => x.Id == taskId));
            else
                return NotFound();
        }


    }

    public class TaskOut
    {
        public Guid Id { get; set; }
        public UserOut User { get; set; }
        public string TaskBody { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDated { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class UserOut
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
