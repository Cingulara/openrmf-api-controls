using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace openstig_api_controls.Models
{

  public class ControlSet {

    public ControlSet () {
      id = Guid.NewGuid(); // pk generated
    }
    public string family { get; set;}
    public string number { get; set;}
    public string title { get; set;}
    public string priority { get; set;}
    public bool lowimpact { get; set;}
    public bool moderateimpact { get; set;}
    public bool highimpact { get; set;}
    public string supplementalGuidance { get; set;}
    public List<ChildControl> childControls { get; set; }
    [Key]
    public Guid id { get; set;}

    public string subControlDescription { get; set;}
    public string subControlNumber { get; set;}
  }

}