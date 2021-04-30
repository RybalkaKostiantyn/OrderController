using Newtonsoft.Json;
using OrderController.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OrderController
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["search"] != null)
            {
                lbError.Text = "";
                List<Order> SearchList = JsonConvert.DeserializeObject<List<Order>>(Session["search"].ToString());
                if (SearchList.Any())
                {
                    List<PropertyInfo> props = new List<PropertyInfo>(SearchList[0].GetType().GetProperties());
                    foreach (Order order in SearchList)
                    {
                        TableRow tableRow = new TableRow();
                        if (order.modificationData.Count > 0)
                        {
                            tableRow.BackColor = Color.Aquamarine;
                        }

                        foreach (PropertyInfo prop in props)
                        {
                            object PropValue = prop.GetValue(order, null);
                            if (!(PropValue is IList<Modification>))
                            {
                                TableCell cell = new TableCell();
                                cell.Controls.Add(new LiteralControl(PropValue.ToString()));
                                tableRow.Cells.Add(cell);
                            }
                        }

                        TableCell btncell = new TableCell();
                        Button btn = new Button();
                        btn.ID = "btnEdit-" + order.id;
                        btn.Text = "Edit";
                        btn.Click += new EventHandler(btnevent_Click);

                        btncell.Controls.Add(btn);
                        tableRow.Cells.Add(btncell);
                        TableSearch.Rows.Add(tableRow);
                    }
                }
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }

        protected void btnevent_Click(object sender, EventArgs e)
        {
            Button senderButton = sender as Button;
            string id = senderButton.ID.Split('-')[1];
            Response.Redirect("Details.aspx?id=" + id);
        }
    }
}