using API.Models;
using API.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task CreateUser(CreateUserModel model) => await _userService.CreateUser(model);

        [HttpPatch]
        public async Task UpdateUser(UpdateUserModel model) => await _userService.UpdateUser(model);

        [HttpGet]
        public async Task<UserModel> GetUser(Guid id) => await _userService.GetUser(id);
    }
}
