using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using openrmf_api_controls.Models;
using openrmf_api_controls.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace openrmf_api_controls.Controllers
{
    [Route("/")]
    public class ControlsController : Controller
    {
        private readonly ILogger<ControlsController> _logger;

        public ControlsController(ILogger<ControlsController> logger)
        {
            _logger = logger;
        }

        // GET the full listing of NIST 800-53 controls
        [HttpGet]
        [Authorize(Roles = "Administrator,Reader,Editor,Assessor")]
        public async Task<IActionResult> GetAllControls(string impactlevel = "", bool pii = false)
        {
            try {
                var listing = NATSClient.GetControlRecords(impactlevel, pii);
                var result = new List<ControlSet>(); // put all results in here
                if (listing != null) {
                    // figure out the impact level filter
                    if (impactlevel.Trim().ToLower() == "low")
                        result = listing.Where(x => x.lowimpact).ToList();
                    else if (impactlevel.Trim().ToLower() == "moderate")
                        result = listing.Where(x => x.moderateimpact).ToList();
                    else if (impactlevel.Trim().ToLower() == "high")
                        result = listing.Where(x => x.highimpact).ToList();

                    // include things that are not P0 meaning not used, and that there is no low/moderate/high designation
                    // these should always be included where the combination of all "false" and not P0 = include them
                    result.AddRange(listing.Where(x => x.priority != "P0" && 
                        !x.lowimpact && !x.moderateimpact && !x.highimpact ).ToList());

                    // see if the PII is true, and if so add in the PII family by appending that to the result from above
                    if (pii) {
                        result.AddRange(listing.Where(x => !string.IsNullOrEmpty(x.family) && x.family.ToLower() == "pii").ToList());
                    }
                    // return whatever is in here
                    return Ok(result);
                }
                else
                    return NotFound(); // nothing loaded yet
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error listing all control sets. Please check the in memory database and XML file load.");
                return BadRequest();
            }
        }
                // GET the text of a control passed in from the compliance page when you click on an individual 
        // item on a single-checklist page that is filtered based on compliance
        [HttpGet("{term}")]
        [Authorize(Roles = "Administrator,Reader,Editor,Assessor")]
        public async Task<IActionResult> GetControl(string term)
        {
            if (!string.IsNullOrEmpty(term)) {
                try {
                    var record = NATSClient.GetControlRecord(term);
                    return Ok(record);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error listing all control sets. Please check the in memory database and XML file load.");
                    return BadRequest();
                }
            }
            else
                return NotFound();
        }
    }
}
