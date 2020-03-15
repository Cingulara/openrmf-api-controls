// Copyright (c) Cingulara LLC 2020 and Tutela LLC 2020. All rights reserved.
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
                _logger.LogInformation("Calling GetAllControls({0}, {1})", impactlevel, pii.ToString());
                var listing = NATSClient.GetControlRecords(impactlevel, pii);
                if (listing != null) {
                    _logger.LogInformation("Called GetAllControls({0}, {1}) successfully", impactlevel, pii.ToString());
                    return Ok(listing);
                }
                else {
                    _logger.LogWarning("Called GetAllControls({0}, {1}) but no control records listing returned", impactlevel, pii.ToString());
                    return NotFound(); // nothing loaded yet
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "GetAllControls() Error listing all control sets. Please check the in memory database and XML file load.");
                return BadRequest();
            }
        }

        /// <summary>
        /// GET the full listing of NIST 800-53 major controls based on impact level and PII boolean
        /// </summary>
        /// <returns>
        /// HTTP Status showing they were found and a list of control records for the NIST controls.
        /// </returns>
        /// <response code="200">Returns the newly updated item</response>
        /// <response code="400">If the get did not work correctly</response>
        /// <response code="404">If the impact passed is not valid</response>
        [HttpGet("majorcontrols")]
        [Authorize(Roles = "Administrator,Reader,Editor,Assessor")]
        public async Task<IActionResult> GetAllMajorControls()
        {
            try {
                _logger.LogInformation("Calling GetAllMajorControls()");
                var listing = NATSClient.GetControlRecords("high", true);
                if (listing != null) {
                    _logger.LogInformation("Called GetAllMajorControls() successfully");
                    // get just the id, number, and title
                    listing = listing.GroupBy(x => new {x.number, x.title})
                            .Select(g => new ControlSet {
                                number = g.Key.number, 
                                title = g.Key.title}).ToList();
                    // return the distint listing
                    return Ok(listing.Distinct().OrderBy(x => x.indexsort).ToList());
                }
                else {
                    _logger.LogWarning("Called GetAllMajorControls() but no control records listing returned");
                    return NotFound(); // nothing loaded yet
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "GetAllMajorControls() Error listing all control sets. Please check the in memory database and XML file load.");
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
            _logger.LogInformation("Calling GetControl({0})", term);
            if (!string.IsNullOrEmpty(term)) {
                try {
                    var record = NATSClient.GetControlRecord(term);
                    if (record == null) {
                        _logger.LogWarning("GetControl({0}) term not found", term);
                        return NotFound();
                    }
                    _logger.LogInformation("Called GetControl({0}) successfully", term);
                    return Ok(record);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "GetControl() Error listing all control sets. Please check the in memory database and XML file load.");
                    return BadRequest();
                }
            }
            else {
                _logger.LogWarning("GetControl({0}) term was not sent in the API call");
                return BadRequest("No valid control term sent");
            }
        }
    }
}
