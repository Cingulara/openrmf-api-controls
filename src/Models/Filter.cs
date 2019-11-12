namespace openrmf_api_controls.Models
{

  public class Filter {

    public Filter () {
        impactLevel = "low";
    }
    public string impactLevel { get; set;}
    public bool pii { get; set;}
  }

}