using Business_logic_Layer;
using Data_Access_Layer;
using Data_Access_Layer.Repository.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly BALMission _balMission;
        private readonly ResponseResult result = new ResponseResult();
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public MissionController(BALMission balMission, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _balMission = balMission;
            _environment = environment;
        }

        [HttpGet]
        [Route("MissionList")]
        public ResponseResult MissionList()
        {
            try
            {
                result.Data = _balMission.MissionList();
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
        [Route("AddMission")]
        public ResponseResult AddMission(Missions mission)
        {
            try
            {
                result.Data = _balMission.AddMission(mission);
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
        [Route("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] List<IFormFile> upload)
        {
            try
            {
                string filePath = "";
                string fullPath = "";
                var files = Request.Form.Files;
                List<string> fileList = new List<string>();
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        filePath = Path.Combine("UploadMissionImage", "Mission");
                        string fileRootPath = Path.Combine(_environment.WebRootPath, filePath);

                        if (!Directory.Exists(fileRootPath))
                        {
                            Directory.CreateDirectory(fileRootPath);
                        }

                        string name = Path.GetFileNameWithoutExtension(fileName);
                        string extension = Path.GetExtension(fileName);
                        string fullFileName = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                        fullPath = Path.Combine(fileRootPath, fullFileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        fileList.Add(fullPath);
                    }
                }
                return Ok(fileList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteMission/{id}")]
        public async Task<ActionResult<ResponseResult>> DeleteMission(int id)
        {
            try
            {
                var result = await _balMission.DeleteMissionAsync(id);
                return Ok(new ResponseResult { Data = result, Result = ResponseStatus.Success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseResult { Result = ResponseStatus.Error, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("UpdateMission")]
        public async Task<ActionResult<ResponseResult>> UpdateMission(Missions mission)
        {
            try
            {
                var result = await _balMission.UpdateMissionAsync(mission);
                return Ok(new ResponseResult { Data = result, Result = ResponseStatus.Success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseResult { Result = ResponseStatus.Error, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("MissionApplicationApprove/{missionApplicationId}")]
        public ResponseResult MissionApplicationApprove(int missionApplicationId)
        {
            try
            {
                result.Data = _balMission.MissionApplicationApprove(missionApplicationId);
                result.Result = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ResponseStatus.Error;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}