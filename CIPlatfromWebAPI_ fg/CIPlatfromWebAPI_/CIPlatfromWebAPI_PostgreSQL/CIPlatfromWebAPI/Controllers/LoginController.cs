using Business_logic_Layer;
using Data_Access_Layer;
using Data_Access_Layer.Repository.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly BALLogin _balLogin;
        ResponseResult result = new ResponseResult();
        public LoginController(BALLogin balLogin)
        {
            _balLogin = balLogin;
        }


        [HttpPost]
        [Route("LoginUser")]
        public ResponseResult LoginUser(User user)
        {
            try
            {
                result.Data = _balLogin.LoginUser(user);
                result.Result = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ResponseStatus.Error;
                result.Message = ex.Message;
            }
            return result;
        }

        [HttpPost]
        [Route("Register")]
        public ResponseResult RegisterUser(User user)
        {
            try
            {
                result.Data = _balLogin.Register(user);
                result.Result = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ResponseStatus.Error;
                result.Message = ex.Message;
            }
            return result;
        }


        [HttpPut("UpdateUserAndUserDetail/{userId}")]
        public async Task<IActionResult> UpdateUserAndUserDetail(int userId, User user)
        {
            try
            {
                var result = await _balLogin.UpdateUserAndUserDetailAsync(userId, user);
                return Ok(new ResponseResult { Data = result, Result = ResponseStatus.Success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseResult { Result = ResponseStatus.Error, Message = ex.Message });
            }
        }
    }
}