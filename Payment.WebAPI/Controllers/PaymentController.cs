﻿using Microsoft.AspNetCore.Mvc;

namespace Payment.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpGet("GetPayments")]
        public IActionResult Get()
        {
            return Ok("This is Payment Service");
        }
    }
}