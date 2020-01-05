// Copyright (c) Cingulara LLC 2020 and Tutela LLC 2020. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using NATS.Client;
using openrmf_api_controls.Models;
using Newtonsoft.Json;
using NLog;

namespace openrmf_api_controls.Classes
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

            var logger = LogManager.GetLogger("openrmf_api_controls");

            // add the options for the server, reconnecting, and the handler events
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.MaxReconnect = -1;
            opts.ReconnectWait = 1000;
            opts.Name = "openrmf-api-controls";
            opts.Url = Environment.GetEnvironmentVariable("NATSSERVERURL");
            opts.AsyncErrorEventHandler += (sender, events) =>
            {
                logger.Info("NATS client error. Server: {0}. Message: {1}. Subject: {2}", events.Conn.ConnectedUrl, events.Error, events.Subscription.Subject);
            };

            opts.ServerDiscoveredEventHandler += (sender, events) =>
            {
                logger.Info("A new server has joined the cluster: {0}", events.Conn.DiscoveredServers);
            };

            opts.ClosedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Closed: {0}", events.Conn.ConnectedUrl);
            };

            opts.ReconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Reconnected: {0}", events.Conn.ConnectedUrl);
            };

            opts.DisconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Disconnected: {0}", events.Conn.ConnectedUrl);
            };
            
            // Creates a live connection to the NATS Server with the above options
            IConnection c = cf.CreateConnection(opts);

            // send the message with data of the filter serialized
            Msg reply = c.Request("openrmf.controls", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(controlFilter)), 10000);
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

            var logger = LogManager.GetLogger("openrmf_api_controls");

            // add the options for the server, reconnecting, and the handler events
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.MaxReconnect = -1;
            opts.ReconnectWait = 1000;
            opts.Name = "openrmf-api-controls";
            opts.Url = Environment.GetEnvironmentVariable("NATSSERVERURL");
            opts.AsyncErrorEventHandler += (sender, events) =>
            {
                logger.Info("NATS client error. Server: {0}. Message: {1}. Subject: {2}", events.Conn.ConnectedUrl, events.Error, events.Subscription.Subject);
            };

            opts.ServerDiscoveredEventHandler += (sender, events) =>
            {
                logger.Info("A new server has joined the cluster: {0}", events.Conn.DiscoveredServers);
            };

            opts.ClosedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Closed: {0}", events.Conn.ConnectedUrl);
            };

            opts.ReconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Reconnected: {0}", events.Conn.ConnectedUrl);
            };

            opts.DisconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Disconnected: {0}", events.Conn.ConnectedUrl);
            };
            
            // Creates a live connection to the NATS Server with the above options
            IConnection c = cf.CreateConnection(opts);

            // send the message with data of the filter serialized
            Msg reply = c.Request("openrmf.controls.search", Encoding.UTF8.GetBytes(term), 10000);
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