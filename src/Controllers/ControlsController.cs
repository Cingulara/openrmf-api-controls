// Copyright (c) Cingulara 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.

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

        /// <summary>
        /// GET the full listing of NIST 800-53 controls based on impact level and PII boolean
        /// </summary>
        /// <param name="impactlevel">The impact level of low, medium, high to filter the controls returned</param>
        /// <param name="pii">A boolean to include the PII items or not</param>
        /// <returns>
        /// HTTP Status showing they were found and a list of control records for the NIST controls.
        /// </returns>
        /// <response code="200">Returns the newly updated item</response>
        /// <response code="400">If the get did not work correctly</response>
        /// <response code="404">If the impact passed is not valid</response>
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

        /// <summary>
        /// GET the text of a control passed in from the compliance page when you click on an individual 
        /// item on a single-checklist page that is filtered based on compliance
        /// </summary>
        /// <param name="term">The term/string to search for to get the description of the control</param>
        /// <returns>
        /// HTTP Status showing it was found and the record with the control.
        /// </returns>
        /// <response code="200">Returns the newly updated item</response>
        /// <response code="400">If the get did not work correctly</response>
        /// <response code="404">If the term passed in is not valid</response>
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
