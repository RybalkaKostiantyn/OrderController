using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderController.Models
{
    public class Order
    {
        public int id { get; set; }
        public string providerName { get; set; }
        public string description { get; set; }
        public string creationData { get; set; }
        public List<Modification> modificationData { get; set; }
        public string manager { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
        public string region { get; set; }

        public Order()
        {

        }
        public Order(int i, string p, string d, string c, string ma, int q, int a, string r)
        {
            id = i;
            providerName = p;
            description = d;
            creationData = c;
            modificationData = new List<Modification>();
            manager = ma;
            quantity = q;
            amount = a;
            region = r;
        }

        public Order(Order o)
        {
            id = o.id;
            providerName = o.providerName;
            description = o.description;
            creationData = o.creationData;
            modificationData = o.modificationData;
            manager = o.manager;
            quantity = o.quantity;
            amount = o.amount;
            region = o.region;
        }

        /// <summary>
        /// Replacing data in Order
        /// </summary>
        /// <param name="m">Data to replace</param>
        public void Undo(Modification m)
        {
            providerName = m.providerName;
            description = m.description;
            manager = m.manager;
            quantity = m.quantity;
            amount = m.amount;
            region = m.region;
        }
    }

    public class Modification
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