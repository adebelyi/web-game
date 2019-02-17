using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        [HttpGet("{userId}")]
        public ActionResult<UserDto> GetUserById([FromRoute] Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] object user)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteUser([FromRoute] Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateUser([FromRoute] Guid userId, [FromBody] object user)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{userId}")]
        public IActionResult PartiallyUpdateUser([FromRoute] Guid userId,
            [FromBody] JsonPatchDocument<object> patchDoc)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public IActionResult GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        [HttpOptions]
        public IActionResult GetUsersOptions()
        {
            throw new NotImplementedException();
        }
    }
}