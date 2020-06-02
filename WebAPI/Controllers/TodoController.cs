using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Controllers
{
    [Authorize]
    public class TodoController : ApiController
    {
        // GET api/todo
        public IEnumerable<string> Get()
        {
            return new List<string> { "Water plants", "Pickup groceries" };
        }

        // GET api/todo/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/todo
        public void Post([FromBody]string value)
        {
        }

        // PUT api/todo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/todo/5
        public void Delete(int id)
        {
        }
    }
}
