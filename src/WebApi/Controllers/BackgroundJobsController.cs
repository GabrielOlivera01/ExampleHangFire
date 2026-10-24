using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class BackgroundJobsController : ControllerBase
    {
        readonly IBackgroundJobsService _backgroundJobs;
        
        public BackgroundJobsController(
            IBackgroundJobsService backgroundJobs)
        {
            _backgroundJobs = backgroundJobs;
        }


        [HttpGet("Enqueed")]
        public IActionResult Enqueed()
        {
            var jobId = _backgroundJobs.EnqueedExample();

            return Ok(new
            {
                Message = "Job enqueued successfully.",
                JobId = jobId
            });
        }


        [HttpGet("Scheduled")]
        public IActionResult Scheduled()
        {
            var jobId = _backgroundJobs.ScheduledExample();

            return Ok(new
            {
                Message = "Job scheduled successfully.",
                JobId = jobId
            });
        }


        [HttpGet("RecurringJob")]
        public IActionResult RecurringJob()
        {
            _backgroundJobs.RecurringJobExample();

            return Ok(new
            {
                Message = "Recurring job registered successfully."
            });
        }
    }
}