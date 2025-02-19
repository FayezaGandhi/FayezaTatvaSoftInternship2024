﻿using Business_logic_Layer;
using Data_Access_Layer.Repository.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly BALAdminUser _balAdminUser;
        ResponseResult result = new ResponseResult();

        public AdminUserController(BALAdminUser balAdminUser)
        {
            _balAdminUser = balAdminUser;
        }

        [HttpGet("UserDetailList")]
        public async Task<IActionResult> GetUserDetailList()
        {
            try
            {
                var userDetailList = await _balAdminUser.UserDetailsAsync();
                return Ok(new ResponseResult { Data = userDetailList, Result = ResponseStatus.Success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseResult { Result = ResponseStatus.Error, Message = ex.Message });
            }
        }

        [HttpDelete("DeleteUserAndUserDetail/{userId}")]
        public async Task<IActionResult> DeleteUserAndUserDetail(int userId)
        {
            try
            {
                var result = await _balAdminUser.DeleteUserAndUserDetailAsync(userId);
                return Ok(new ResponseResult { Data = result, Result = ResponseStatus.Success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseResult { Result = ResponseStatus.Error, Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("MissionApplicationList")]
        public ResponseResult MissionApplicationList()
        {
            try
            {
                result.Data = _balAdminUser.GetMissionApplicationList();
                result.Result = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Result = ResponseStatus.Error;
            }
            return result;
        }

    }
}