using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.DTOs;

namespace api.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase{
        private readonly StatusService _statusService;

        public StatusController(StatusService statusService){
            _statusService = statusService;
        }

        [HttpPost]
        public async Task<ActionResult<StatusResponseDto>> UpdateStatus(StatusRequestDto statusRequest){
            var response = await _statusService.AnswerStatusAsync(statusRequest);
            return Ok(response);
        }
    }
}