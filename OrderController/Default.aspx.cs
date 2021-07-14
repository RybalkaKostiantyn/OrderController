using Newtonsoft.Json;
using OrderController.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OrderController
{
    /*
     1 метод 1 скрин! В отдельные методы закинь или в region закинуть
     branches
        file/Database
         */

    public partial class _Default : Page
    {

        string Verify()
        {
            string errorText = "";
            string connString = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = OrdersDB; Integrated Security = True";
            /*try
            {
                
                if (Session["orders"] != null)
                {
                    Orders = JsonConvert.DeserializeObject<List<Order>>(Session["orders"].ToString());
                }
                else
                {
                    string sql = "SELECT * FROM OrderTable";

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        var rdr = cmd.ExecuteReader();

                        conn.Close();
                    }
                }
            }
            catch
            {
                errorText += "Database not found" + '\n' + "Searched adress: " + connString + '\n' + '\n';
            }*/
            if (Request["page"] != null)
            {
                try
                {
                    page = Convert.ToInt32(Request["page"]);
                }
                catch
                {
                    errorText += "Page number (pagination) can't be converted to Int" + '\n' + "Request['page']: " + Request["page"] + '\n' + '\n';
                }
            }

            if (Session["pageSize"] != null)
            {
                try
                {
                    int radiobutton = (int)Session["pageSize"];
                    rblPageSize.Items[radiobutton].Selected = true;
                }
                catch
                {
                    errorText += "Var PageSize (pagination) can't be converted to Int" + '\n' + "Request['pageSize']: " + Request["pageSize"] + '\n' + '\n';
                }
            }

            return errorText;
        }

        private List<Order> Orders = new List<Order>();
        private string redirect = "Default.aspx?";
        private string sortBy;
        string path = "none";
        int page = 0;
        int pageSize = 5;
        protected void Page_Load(object sender, EventArgs e)
        {
            bool error = false;
            string errorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\Error " + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt");
            string errorText = Verify();

            if (errorText != "")
            {
                error = true;
            }

            if (!error)
            {
                if (Request["page"] != null)
                {
                    page = Convert.ToInt32(Request["page"]);
                }
                if (Session["pageSize"] != null && !IsPostBack)
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
                
                
                    string connString = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = OrdersDB; Integrated Security = True";
                    if(sortBy == null)
                    {
                        sortBy = "id";
                    }
                    if (Session["sortBy"] != null)
                    {
                        sortBy = Session["sortBy"].ToString();
                    }

                string sql = "SELECT * FROM OrderTable ORDER BY " +
                    sortBy +
                    " OFFSET @pageStart ROWS " +
                    "FETCH NEXT @pageSize ROWS ONLY";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@pageStart", page * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize * 2);
                    cmd.Parameters.AddWithValue("@sortBy", sortBy);
                    var rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Order or = new Order(
                                Convert.ToInt32(rdr[0]),
                                rdr[1].ToString(),
                                rdr[2].ToString(),
                                rdr[3].ToString(),
                                rdr[4].ToString(),
                                Convert.ToInt32(rdr[5]),
                                Convert.ToInt32(rdr[6]),
                                rdr[7].ToString()
                                );
                        if (!Orders.Any(o => o.id == or.id))
                        {
                            Orders.Add(or);
                        }
                    }
                }
                    

                    Session["orders"] = JsonConvert.SerializeObject(Orders);
                

                

                List<PropertyInfo> props = new List<PropertyInfo>(Orders[0].GetType().GetProperties());
                
                foreach (PropertyInfo prop in props)
                {
                    if (!(prop.GetValue(Orders[0], null) is IList<Modification>) && sortBy == prop.Name.ToString())
                    {
                        Orders = Orders.OrderBy(o => o.GetType().GetProperty(sortBy).GetValue(o, null)).ToList();
                        Session["orders"] = JsonConvert.SerializeObject(Orders);
                    }
                    else if (!(prop.GetValue(Orders[0], null) is IList<Modification>) && sortBy.Split(' ')[0] == prop.Name.ToString())
                    {
                        Orders = Orders.OrderByDescending(o => o.GetType().GetProperty(sortBy.Split(' ')[0]).GetValue(o, null)).ToList();
                        Session["orders"] = JsonConvert.SerializeObject(Orders);
                    }
                }


                TableRow head = new TableRow();

                foreach (PropertyInfo prop in props)
                {
                    if (!(prop.Name.ToString() == "modificationData"))
                    {
                        TableCell headCell = new TableCell();

                        Button sort = new Button();
                        sort.Text = prop.Name.ToString();
                        sort.ID = "btnSort" + sort.Text;
                        sort.Click += new EventHandler(btnevent_Sort);

                        headCell.Controls.Add(sort);
                        head.Cells.Add(headCell);
                    }
                }


                TableOrders.Rows.Add(head);

                for (int i = pageStart; i < Orders.Count && i < pageFinish; i++)
                {
                    TableRow tableRow = new TableRow();
                    if (Orders[i].modificationData != null && Orders[i].modificationData.Count > 0)
                    {
                        tableRow.BackColor = Color.Aquamarine;
                    }
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
                btnSaveOrders.Click += new EventHandler(btnevent_SaveToDB);
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
            else
            {
                lbHeader.Text = "ERROR!!!";

                File.WriteAllText(errorPath, errorText);
            }
        }

        protected void btnevent_SaveToDB(object sender, EventArgs e)
        {
            foreach (Order order in Orders)
            {
                order.modificationData = new List<Modification>();
            }
            string text = JsonConvert.SerializeObject(Orders);

            Session["orders"] = text;

            string connString = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = OrdersDB; Integrated Security = True";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                foreach (Order order in Orders)
                {
                    string sql = "UPDATE OrderTable SET providerName = @providerName" + 
                        ", description = @description" +
                        ", creationData = @creationData" +
                        ", manager = @manager" +
                        ", quantity = @quantity" +
                        ", amount = @amount" +
                        ", region = @region" +
                        " WHERE id = @id;";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@providerName", order.providerName);
                    cmd.Parameters.AddWithValue("@description", order.description);
                    cmd.Parameters.AddWithValue("@creationData", order.creationData);
                    cmd.Parameters.AddWithValue("@manager", order.manager);
                    cmd.Parameters.AddWithValue("@quantity", order.quantity);
                    cmd.Parameters.AddWithValue("@amount", order.amount);
                    cmd.Parameters.AddWithValue("@region", order.region);
                    cmd.Parameters.AddWithValue("@id", order.id);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }

            Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
        }

        protected void btnevent_Edit(object sender, EventArgs e)
        {
            Button senderButton = sender as Button;
            string id = senderButton.ID.Replace("btnEdit", "");
            Response.Redirect("Details.aspx?id=" + id);
        }

        protected void btnevent_Sort(object sender, EventArgs e)
        {
            Button senderButton = sender as Button;
            string property = senderButton.Text;
            if (Session["sortBy"] != null && Session["sortBy"].ToString() == property)
            {
                Session["sortBy"] = property + " DESC";
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
                    if (prop.GetValue(order, null).ToString() == tbSearch.Text)
                    {
                        if (!Searched.Any(o => o.id == order.id))
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