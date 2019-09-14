using System;
using System.Collections.Generic;
using System.Text;
using NATS.Client;
using openstig_api_controls.Models;
using Newtonsoft.Json;
using System.Linq;

namespace openstig_api_controls.Classes
{
    public static class NATSClient
    {        
        /// <summary>
        /// Return a list of controls based on filter and PII
        /// </summary>
        /// <param name="impactlevel">The impact level of the controls to return.</param>
        /// <param name="pii">Boolean to return the PII elements or not.</param>
        /// <returns></returns>
        public static List<ControlSet> GetControlRecords(string impactlevel, bool pii){
            // get the result ready to receive the info and send on
            List<ControlSet> controls = new List<ControlSet>();
            // setup the filter for impact level and PII for controls
            Filter controlFilter = new Filter() {impactLevel = impactlevel, pii = pii};
            // Create a new connection factory to create a connection.
            ConnectionFactory cf = new ConnectionFactory();
            // Creates a live connection to the default NATS Server running locally
            IConnection c = cf.CreateConnection(Environment.GetEnvironmentVariable("natsserverurl"));
            // send the message with data of the filter serialized
            Msg reply = c.Request("openrmf.controls", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(controlFilter)), 30000);
            // save the reply and get back the checklist to score
            if (reply != null) {
                controls = JsonConvert.DeserializeObject<List<ControlSet>>(Compression.DecompressString(Encoding.UTF8.GetString(reply.Data)));
                c.Close();
                return controls;
            }
            c.Close();
            return controls;
        }


        /// <summary>
        /// Return a control based on the search term sent
        /// </summary>
        /// <param name="term">The control title or search to get a control record back.</param>
        /// <returns></returns>
        public static ControlSet GetControlRecord(string term){
            // get the result ready to receive the info and send on
            ControlSet control = new ControlSet();
            // Create a new connection factory to create a connection.
            ConnectionFactory cf = new ConnectionFactory();
            // Creates a live connection to the default NATS Server running locally
            IConnection c = cf.CreateConnection(Environment.GetEnvironmentVariable("natsserverurl"));
            // send the message with data of the filter serialized
            Msg reply = c.Request("openrmf.controls.search", Encoding.UTF8.GetBytes(term), 30000);
            // save the reply and get back the checklist to score
            if (reply != null) {
                control = JsonConvert.DeserializeObject<ControlSet>(Compression.DecompressString(Encoding.UTF8.GetString(reply.Data)));
                c.Close();
                return control;
            }
            c.Close();
            return control;
        }
    }
}