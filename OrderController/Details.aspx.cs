using Newtonsoft.Json;
using OrderController.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OrderController
{
    public partial class Details : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["orders"] != null)
            {
                int id = Convert.ToInt32(Request.QueryString["id"]);
                List<Order> OrderList = JsonConvert.DeserializeObject<List<Order>>(Session["orders"].ToString());

                Order order = OrderList.Find(o => o.id == id);

                if (!IsPostBack)
                {
                    tbProvider.Text = order.providerName;
                    tbDescription.Text = order.description;
                    tbManager.Text = order.manager;
                    tbQuantity.Text = order.quantity.ToString();
                    tbAmount.Text = order.amount.ToString();
                }

                Button btn = new Button();
                btn.Text = "Save";
                btn.ID = "btnSave";
                btn.Click += new EventHandler(btnevent_Save);
                phOrders.Controls.Add(btn);

                if (order.modificationData != null)
                {
                    for (int i = order.modificationData.Count - 1; i >= 0; i--)
                    {
                        Label lb = new Label();
                        lb.Text = "<p>" + order.modificationData[i].modTime + ") provider: " + order.modificationData[i].providerName + "</p>"
                            + "<p>" + order.modificationData[i].description + "</p>"
                            + "<p>" + "quantity: " + order.modificationData[i].quantity + ", amount: " + order.modificationData[i].amount + "$</p>";
                        phOrders.Controls.Add(lb);

                        Button undo = new Button();
                        undo.Text = "Undo changes";
                        undo.ID = "btnUndo" + i;
                        undo.Click += new EventHandler(btnevent_Undo);
                        phOrders.Controls.Add(undo);
                    }
                }
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }

        protected void btnevent_Save(object sender, EventArgs e)
        {
            bool error = false;
            int id = Convert.ToInt32(Request.QueryString["id"]);
            List<Order> OrderList = JsonConvert.DeserializeObject<List<Order>>(Session["orders"].ToString());
            Order order = OrderList.Find(o => o.id == id);

            int orderIndex = OrderList.IndexOf(OrderList.Find(o => o.id == id));
            
                Order updated = new Order(order);
                updated.providerName = tbProvider.Text;
                updated.description = tbDescription.Text;
                updated.manager = tbManager.Text;
            try
            {
                updated.quantity = Convert.ToInt32(tbQuantity.Text);
                updated.amount = Convert.ToInt32(tbAmount.Text);
            }
            catch
            {
                Label warning = new Label();
                warning.Text = @"<p style='color: red'>"+ "Integers, please." +"</p>";
                phError.Controls.Add(warning);
                error = true;
            }

            if(!error)
            {
                if (updated.modificationData == null)
                {
                    updated.modificationData = new List<Modification>();
                }
                updated.modificationData.Add(new Modification(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), order));

                OrderList.RemoveAt(orderIndex);
                OrderList.Insert(orderIndex, updated);
                Session["orders"] = JsonConvert.SerializeObject(OrderList);
                Response.Redirect("Default.aspx");
            }
        }

        protected void btnevent_Undo(object sender, EventArgs e)
        {
            Button senderButton = sender as Button;
            int undoNum = Convert.ToInt32(senderButton.ID.Replace("btnUndo",""));

            int id = Convert.ToInt32(Request.QueryString["id"]);
            List<Order> OrderList = JsonConvert.DeserializeObject<List<Order>>(Session["orders"].ToString());
            Order order = OrderList.Find(o => o.id == id);
            int orderIndex = OrderList.IndexOf(order);

            order.Undo(order.modificationData[undoNum]);

            for (int i = undoNum; i < order.modificationData.Count; i++)
            {
                order.modificationData.RemoveAt(i);
            }

            OrderList.RemoveAt(orderIndex);
            OrderList.Insert(orderIndex, order);
            Session["orders"] = JsonConvert.SerializeObject(OrderList);
            Response.Redirect("Default.aspx");
        }
    }
}