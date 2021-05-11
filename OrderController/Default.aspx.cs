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
    //добавить обработку ошибок, ввести tryPage. В каком классе, методе, месте произошла ошибка. Возможно входные параметры в Data. Error Handling - логи. Serilog. UseTorBllCommon
    public partial class _Default : Page
    {
        private List<Order> Orders = new List<Order>();
        private string redirect = "Default.aspx?";
        private string sortBy = "id";
        string path = "none";
        protected void Page_Load(object sender, EventArgs e)
        {
            bool error = false;
            try
            {
                if (Session["orders"] != null)
                {
                    Orders = JsonConvert.DeserializeObject<List<Order>>(Session["orders"].ToString());
                }
                else
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\OrdersDBFile.txt");
                    Orders = JsonConvert.DeserializeObject<List<Order>>(File.ReadAllText(path));

                    Session["orders"] = JsonConvert.SerializeObject(Orders);
                }
            }
            catch
            {
                lbHeader.Text = "Куда-то делся документ с заказами, без него работа невозможна";
                
                string errorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\Error " + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt");

                string errorText = "File not found" + '\n' + "Searched adress: " + path;
                
                File.WriteAllText(errorPath, errorText);

                error = true;
            }

            if(!error)
            {
                int page = 0;
                if (Request.QueryString["page"] != null)
                {
                    page = Convert.ToInt32(Request.QueryString["page"]);
                }

                int pageSize = 5;
                if (Session["pageSize"] != null && !IsPostBack) //без page, page size записать в property (совет),
                {
                    int radiobutton = (int)Session["pageSize"];
                    rblPageSize.Items[radiobutton].Selected = true;
                }
                if (rblPageSize.SelectedItem != null)
                {
                    pageSize = Convert.ToInt32(rblPageSize.SelectedItem.Text);
                }
                int pageStart = page * pageSize;
                int pageFinish = pageStart + pageSize;


                if (Session["sortBy"] != null)
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
                    else if (!(prop.GetValue(Orders[0], null) is IList<Modification>) && sortBy.Split('-')[0] == prop.Name.ToString())
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
                        sort.ID = "btnSort" + sort.Text; //специальные символы не нужны, текст запихни в Тэг
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

                    /*
                    foreach (PropertyInfo prop in props)
                    {
                        object PropValue = prop.GetValue(Orders[i], null);
                        if (!(PropValue is IList<Modification>))
                        {
                            TableCell cell = new TableCell();
                            cell.Controls.Add(new LiteralControl(PropValue.ToString()));
                            tableRow.Cells.Add(cell);
                        }
                    }*/
                    TableCell cell = new TableCell();
                    cell.Controls.Add(new LiteralControl(Orders[i].id.ToString()));
                    tableRow.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Controls.Add(new LiteralControl(Orders[i].providerName.ToString()));
                    tableRow.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Controls.Add(new LiteralControl(Orders[i].description.ToString()));
                    tableRow.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Controls.Add(new LiteralControl(Orders[i].creationData.ToString()));
                    tableRow.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Controls.Add(new LiteralControl(Orders[i].manager.ToString()));
                    tableRow.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Controls.Add(new LiteralControl(Orders[i].quantity.ToString()));
                    tableRow.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Controls.Add(new LiteralControl(Orders[i].amount.ToString()));
                    tableRow.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Controls.Add(new LiteralControl(Orders[i].region.ToString()));
                    tableRow.Cells.Add(cell);

                    TableCell btncell = new TableCell();
                    Button btn = new Button();
                    btn.ID = "btnEdit" + Orders[i].id;
                    btn.Text = "Edit";
                    btn.Click += new EventHandler(btnevent_Edit);

                    btncell.Controls.Add(btn);
                    tableRow.Cells.Add(btncell);
                    TableOrders.Rows.Add(tableRow);
                }

                Button btnSaveOrders = new Button();
                btnSaveOrders.Text = "Save orders to file";
                btnSaveOrders.ID = "btnSaveOrders";
                btnSaveOrders.Click += new EventHandler(btnevent_SaveToFile);
                phOrders.Controls.Add(btnSaveOrders);

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
                if ((page + 1) * pageSize > Orders.Count)
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
        }

        protected void btnevent_SaveToFile(object sender, EventArgs e)
        {
            foreach(Order order in Orders)
            {
                order.modificationData = new List<Modification>();
            }
            string text = JsonConvert.SerializeObject(Orders);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\OrdersDBFile.txt");

            Session["orders"] = text;

            File.WriteAllText(path, text);

            Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
        }

        protected void btnevent_Edit(object sender, EventArgs e)
        {
            Button senderButton = sender as Button;
            string id = senderButton.ID.Replace("btnEdit","");
            Response.Redirect("Details.aspx?id=" + id);
        }

        protected void btnevent_Sort(object sender, EventArgs e)
        {
            Button senderButton = sender as Button;
            string property = senderButton.Text;
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
            Session["pageSize"] = rblPageSize.SelectedIndex;
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