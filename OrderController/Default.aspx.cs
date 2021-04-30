using Newtonsoft.Json;
using OrderController.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OrderController
{
    public partial class _Default : Page
    {
        private List<Order> Orders = new List<Order>();
        private string redirect = "Default.aspx?";
        private string sortBy = "id";
        protected void Page_Load(object sender, EventArgs e)
        {
            Button btnSaveOrders = new Button();
            btnSaveOrders.Text = "Save orders to file";
            btnSaveOrders.ID = "btnSaveOrders";
            btnSaveOrders.Click += new EventHandler(btnevent_SaveToFile);
            phOrders.Controls.Add(btnSaveOrders);


            int page = 0;
            if (Request.QueryString["page"] != null)
            {
                page = Convert.ToInt32(Request.QueryString["page"]);
            }

            int pageSize = 5;
            if (Session["pageSize"] != null && !Page.IsPostBack)
            {
                int radiobutton = (int)Session["pageSize"];
                RadioButtonList1.Items[radiobutton].Selected = true;
            }
            if(RadioButtonList1.SelectedItem != null)
            {
                pageSize = Convert.ToInt32(RadioButtonList1.SelectedItem.Text);
            }
            int pageStart = page * pageSize;
            int pageFinish = pageStart + pageSize;
            
            if (Session["orders"] != null)
            {
                Orders = JsonConvert.DeserializeObject<List<Order>>(Session["orders"].ToString());
            }
            else
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\TextFile1.txt");
                Orders = JsonConvert.DeserializeObject<List<Order>>(File.ReadAllText(path));
                
                Session["orders"] = JsonConvert.SerializeObject(Orders);
            }
            if(Session["sortBy"] != null)
            {
                sortBy = Session["sortBy"].ToString();
            }

            List<PropertyInfo> props = new List<PropertyInfo>(Orders[0].GetType().GetProperties());

            foreach (PropertyInfo prop in props)
            {
                if (!(prop.GetValue(Orders[0], null) is IList<Modification>) && sortBy == prop.Name.ToString())
                {
                    Orders = Orders.OrderBy(o => o.GetType().GetProperty(sortBy).GetValue(o, null)).ToList();
                    Session["orders"] = JsonConvert.SerializeObject(Orders);
                }
                else if(!(prop.GetValue(Orders[0], null) is IList<Modification>) && sortBy.Split('-')[0] == prop.Name.ToString())
                {
                    Orders = Orders.OrderByDescending(o => o.GetType().GetProperty(sortBy.Split('-')[0]).GetValue(o, null)).ToList();
                    Session["orders"] = JsonConvert.SerializeObject(Orders);
                }
            }


            TableRow head = new TableRow();

            foreach (PropertyInfo prop in props)
            {
                if (!(prop.GetValue(Orders[0], null) is IList<Modification>))
                {
                    TableCell headCell = new TableCell();

                    Button sort = new Button();
                    sort.Text = prop.Name.ToString();
                    sort.ID = "btnSort-" + sort.Text;
                    sort.Click += new EventHandler(btnevent_Sort);

                    headCell.Controls.Add(sort);
                    head.Cells.Add(headCell);
                }
            }

            TableOrders.Rows.Add(head);

            for (int i = pageStart; i < Orders.Count && i < pageFinish; i++)
            {

                TableRow tableRow = new TableRow();
                if (Orders[i].modificationData.Count > 0)
                {
                    tableRow.BackColor = Color.Aquamarine;
                }
                
                foreach (PropertyInfo prop in props)
                {
                    object PropValue = prop.GetValue(Orders[i], null);
                    if (!(PropValue is IList<Modification>))
                    {
                        TableCell cell = new TableCell();
                        cell.Controls.Add(new LiteralControl(PropValue.ToString()));
                        tableRow.Cells.Add(cell);
                    }
                }

                TableCell btncell = new TableCell();
                Button btn = new Button();
                btn.ID = "btnEdit-" + Orders[i].id;
                btn.Text = "Edit";
                btn.Click += new EventHandler(btnevent_Click);

                btncell.Controls.Add(btn);
                tableRow.Cells.Add(btncell);
                TableOrders.Rows.Add(tableRow);
            }

            Button prev = new Button();
            prev.Text = "Previous";
            prev.ID = "btnPrev";
            if (page == 0)
            {
                prev.Enabled = false;
            }
            prev.Click += new EventHandler(btnevent_Previous);
            phOrders.Controls.Add(prev);

            Button next = new Button();
            next.Text = "Next";
            next.ID = "btnNext";
            if((page+1)*pageSize > Orders.Count)
            {
                next.Enabled = false;
            }
            next.Click += new EventHandler(btnevent_Next);
            phOrders.Controls.Add(next);

            int totalQuantity = 0;
            int totalAmount = 0;

            foreach (Order order in Orders)
            {
                totalQuantity += order.quantity;
                totalAmount += order.amount * order.quantity;
            }
            lbQuantity.Text += totalQuantity;
            lbAmount.Text += totalAmount;
        }

        protected void btnevent_SaveToFile(object sender, EventArgs e)
        {
            foreach(Order order in Orders)
            {
                order.modificationData = new List<Modification>();
            }
            string text = JsonConvert.SerializeObject(Orders);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\TextFile1.txt");

            Session["orders"] = text;

            File.WriteAllText(path, text);

            Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
        }

        protected void btnevent_Click(object sender, EventArgs e)
        {
            Button senderButton = sender as Button;
            string id = senderButton.ID.Split('-')[1];
            Response.Redirect("Details.aspx?id=" + id);
        }

        protected void btnevent_Sort(object sender, EventArgs e)
        {
            Button senderButton = sender as Button;
            string property = senderButton.ID.Split('-')[1];
            if(Session["sortBy"] != null && Session["sortBy"].ToString() == property)
            {
                Session["sortBy"] = property + "-descending";
            }
            else
            {
                Session["sortBy"] = property;
            }
            
            Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
        }

        protected void btnevent_Previous(object sender, EventArgs e)
        {
            int newPage = 0;
            if (Request.QueryString["page"] != null)
            {
                newPage = Convert.ToInt32(Request.QueryString["page"]) - 1;
            }
            Response.Redirect(redirect + "page=" + newPage);
        }

        protected void btnevent_Next(object sender, EventArgs e)
        {
            int newPage = 1;
            if (Request.QueryString["page"] != null)
            {
                newPage = Convert.ToInt32(Request.QueryString["page"]) + 1;
            }
            Response.Redirect(redirect + "page=" + newPage);
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["pageSize"] = RadioButtonList1.SelectedIndex;
            Response.Redirect(redirect + "page=0");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            List<Order> Searched = new List<Order>();
            List<PropertyInfo> props = new List<PropertyInfo>(Orders[0].GetType().GetProperties());

            foreach (Order order in Orders)
            {
                foreach (PropertyInfo prop in props)
                {
                    if(prop.GetValue(order,null).ToString() == tbSearch.Text)
                    {
                        if(!Searched.Any(o => o.id == order.id))
                        {
                            Searched.Add(order);
                        }
                    }
                }
            }

            Session["search"] = JsonConvert.SerializeObject(Searched);
            Response.Redirect("Search.aspx");
        }
    }
}