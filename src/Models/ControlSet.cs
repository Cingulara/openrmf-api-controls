// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace openrmf_api_controls.Models
{

  public class ControlSet {

    public ControlSet () {
      id = Guid.NewGuid(); // pk generated
    }
    [Key]
    public Guid id { get; set;}
    public string family { get; set;}
    public string number { get; set;}
    public string title { get; set;}
    public string priority { get; set;}
    public bool lowimpact { get; set;}
    public bool moderateimpact { get; set;}
    public bool highimpact { get; set;}
    public string supplementalGuidance { get; set;}

    public string subControlDescription { get; set;}
    public string subControlNumber { get; set;}
  }

}