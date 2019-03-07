using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Design;
using WebApi.Models;
using WebGame.Domain;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet("{userId}", Name = nameof(GetUserById))]
        [HttpHead("{userId}")]
        public ActionResult<UserDto> GetUserById([FromRoute] Guid userId)
        {
            var userFromRepository = userRepository.FindById(userId);

            if (userFromRepository == null)
            {
                return NotFound();
            }

            var userDto = Mapper.Map<UserDto>(userFromRepository);

            return Ok(userDto);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserDTO user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (user.Login == null || !user.Login.All(l => char.IsDigit(l) || char.IsLetter(l)))
            {
                ModelState.AddModelError(nameof(CreateUserDTO.Login), "Логин должен состоять только из цифр и букв.");
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var createdUserEntity = Mapper.Map<UserEntity>(user);
            createdUserEntity = userRepository.Insert(createdUserEntity);

            return CreatedAtRoute(
                nameof(GetUserById),
                new {userId = createdUserEntity.Id},
                createdUserEntity.Id
            );
        }

        [HttpDelete("{userId}")]
        public IActionResult CreateUser([FromRoute] Guid userId)
        {
            var user = userRepository.FindById(userId);

            if (user == null)
            {
                return NotFound();
            }

            userRepository.Delete(userId);

            return NoContent();
        }

        [HttpPut("{userId}")]
        public IActionResult Upsert([FromRoute] Guid userId, [FromBody] UpsertUserDTO user)
        {
            if (user == null || userId == Guid.Empty)
            {
                return BadRequest();
            }

            if (user.Login != null && !user.Login.All(c => char.IsDigit(c) || char.IsLetter(c)))
            {
                ModelState.AddModelError(nameof(UpsertUserDTO.Login), "Логин должен состоять только из цифр и букв.");
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var isExists = userRepository.FindById(userId) != null;
            var userEntity = new UserEntity(userId);
            Mapper.Map(user, userEntity);
            userRepository.UpdateOrInsert(userEntity);

            if (isExists)
            {
                return NoContent();
            }

            return CreatedAtRoute(
                nameof(GetUserById),
                new {userId = userEntity.Id},
                userEntity.Id
            );
        }

        [HttpPatch("{userId}")]
        public IActionResult PartiallyUpdateUser([FromRoute] Guid userId, [FromBody] JsonPatchDocument<UpsertUserDTO> patchUser)
        {
            if (patchUser == null)
            {
                return BadRequest();
            }
            
            var userEntity = userRepository.FindById(userId);

            if (userEntity == null)
            {
                return NotFound();
            }

            var updatedDto = Mapper.Map<UpsertUserDTO>(userEntity);
            patchUser.ApplyTo(updatedDto, ModelState);
            TryValidateModel(updatedDto);
            
            if (updatedDto.Login == null ||
                updatedDto.Login != null && !updatedDto.Login.All(c => char.IsDigit(c) || char.IsLetter(c)))
            {
                ModelState.AddModelError(nameof(UpsertUserDTO.Login), "Логин должен состоять только из цифр и букв.");
            }
            
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            
            userEntity = new UserEntity(userId);
            Mapper.Map(updatedDto, userEntity);
            userRepository.Update(userEntity);

            return NoContent();
        }
    }
}