using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using openstig_api_controls.Models;
using openstig_api_controls.Database;
using System.IO;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace openstig_api_controls.Controllers
{
    [Route("/")]
    public class ControlsController : Controller
    {
        private readonly ILogger<ControlsController> _logger;
        private ControlsDBContext _context;

        public ControlsController(ILogger<ControlsController> logger, ControlsDBContext context)
        {
            _logger = logger;
            _context = context; // pass in the database in memory
        }

        // GET the compliance listing for a system
        [HttpGet]
        public async Task<IActionResult> GetAllControls()
        {
            try {
                var result = await _context.Controls.ToListAsync();
                if (result != null)
                    return Ok(result);
                else
                    return NotFound(); // nothing loaded yet
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error listing all controls...check the in memory database and XML file load.");
                return BadRequest();
            }
        }

    }
}
