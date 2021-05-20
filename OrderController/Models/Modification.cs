using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderController.Models
{
    public class Modification //вынести в другой класс
    {
        public string modTime { get; set; }
        public string providerName { get; set; }
        public string description { get; set; }
        public string manager { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
        public string region { get; set; }

        /// <summary>
        /// Keeps the info about Order, before it was edited
        /// </summary>
        /// <param name="dt">Time of modification</param>
        /// <param name="bc">Order that was modificated</param>
        public Modification()
        {

        }
        public Modification(string dt, Order bc)
        {
            modTime = dt;
            providerName = bc.providerName;
            description = bc.description;
            manager = bc.manager;
            quantity = bc.quantity;
            amount = bc.amount;
            region = bc.region;
        }
    }
}