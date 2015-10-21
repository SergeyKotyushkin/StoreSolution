using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using StoreSolution.DatabaseProject.Model;
using StoreSolution.DatabaseProject.Interfaces;
using StoreSolution.DatabaseProject.Realizations;
using StoreSolution.WebProject.Model;
using System.Web.Security;

namespace StoreSolution.WebProject
{
    public partial class BasketPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            L_user.Text = "Good day, " + Membership.GetUser().UserName + "!";

            FillOrdersGridView();
        }

        protected void B_buy_Click(object sender, EventArgs e)
        {
            Session["Bought"] = "1";
            Session["CurrentOrder"] = null;
            Response.Redirect("~/Restrict/StartPage.aspx");
        }

        protected void B_back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Restrict/StartPage.aspx");
        }

        protected void B_SignOut_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("~/LoginPage.aspx");
        }

        protected void GV_table_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_table.PageIndex = e.NewPageIndex;
        }

        protected void GV_table_PageIndexChanged(object sender, EventArgs e)
        {
            FillOrdersGridView();
        }
        

        private void FillOrdersGridView()
        {
            var products = DbRepository.GetInstance().GetProducts();

            var orders = GetOrdersFromSession();

            var list = products.Join(orders, p => p.Id, q => q.Id, (p, q) => new { p.Name, p.Price, q.Count, Total = (q.Count * p.Price) }).ToList();
            GV_table.DataSource = list;               
            GV_table.DataBind();

            for (var i = 0; i < GV_table.Rows.Count; i++)
            {
                GV_table.Rows[i].Cells[1].Text = String.Format("{0:c}", double.Parse(GV_table.Rows[i].Cells[1].Text));
                GV_table.Rows[i].Cells[3].Text = String.Format("{0:c}", double.Parse(GV_table.Rows[i].Cells[3].Text));
            }

            var sum = list.Sum(p => p.Total);

            var label = new Label();

            if (sum == 0)
            {
                B_buy.Enabled = false;
                label.Text = "Your basket is empty.";
            }
            else
            {
                B_buy.Enabled = true;
                label.Text = "Total: " + String.Format("{0:c}", sum);
            }
            label.Font.Size = FontUnit.Larger;
            PH_total.Controls.Clear();
            PH_total.Controls.Add(label);
        }

        private List<Order> GetOrdersFromSession()
        {
            return Session["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

    }
}