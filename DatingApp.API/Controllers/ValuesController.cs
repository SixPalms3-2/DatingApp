using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Authorize] // Ensure the user is authorized when using ValuesController
    [ApiController]
    [Route("API/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<Value> _logger;

        public ValuesController(DataContext context, ILogger<Value> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await _context.Values.ToListAsync();

            // return HTTP response
            return Ok(values);
        }

        [AllowAnonymous] // No authorization needed
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await _context.Values.FirstOrDefaultAsync(v => v.Id == id);

            // return HTTP response
            return Ok(value);
        }

    }
}

