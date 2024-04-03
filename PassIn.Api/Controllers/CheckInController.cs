using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PassIn.Application.UseCases.Chekins.DoCheckin;
using PassIn.Communication.Responses;

namespace PassIn.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        [HttpPost]
        [Route("{attendeeId}")]
        [ProducesResponseType(typeof(ResponseRegisterJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public IActionResult Checkin([FromRoute] Guid attendeeId)
        {
            var useCase = new DoAttendeeCheckinUseCase();

            var response = useCase.Execute(attendeeId);

            return Created(string.Empty, response);
        }
    }
}
