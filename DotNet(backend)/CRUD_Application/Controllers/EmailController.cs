using CRUD_Application.Models;
using CRUD_Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailServices _emailServices;

        public EmailController(IEmailServices emailServices)
        {
            _emailServices = emailServices;
        }

        [HttpPost("sendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto emailRequest)
        {
            if(string.IsNullOrWhiteSpace(emailRequest.ToEmail) || string.IsNullOrWhiteSpace(emailRequest.Subject) || string.IsNullOrWhiteSpace(emailRequest.Body))
            {
                return BadRequest("Please Fill all the required fields?");
            }

            await _emailServices.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body);
            return Ok("Email Sent Successfully");    
        }
    }
}
