using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using StoreSolution.DatabaseProject.Realizations;
using StoreSolution.WebProject.Model;
using System.Web.Security;

namespace StoreSolution.WebProject.User
{
    public partial class Basket : System.Web.UI.Page
    {
        private const double Tolerance = double.Epsilon;

        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                btnSignOut_Click(null, null);
                return;
            }

            labUser.Text = "Good day, " + user.UserName + "!";

            FillOrdersGridView();
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            Session["Bought"] = "1";
            Session["CurrentOrder"] = null;
            Response.Redirect("~/User/ProductCatalog.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/User/ProductCatalog.aspx");
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void GV_table_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTable.PageIndex = e.NewPageIndex;
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
            gvTable.DataSource = list;               
            gvTable.DataBind();

            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                gvTable.Rows[i].Cells[1].Text = string.Format("{0:c}", double.Parse(gvTable.Rows[i].Cells[1].Text));
                gvTable.Rows[i].Cells[3].Text = string.Format("{0:c}", double.Parse(gvTable.Rows[i].Cells[3].Text));
            }

            var sum = list.Sum(p => p.Total);

            var label = new Label();

            if (Math.Abs(sum) < Tolerance)
            {
                btnBuy.Enabled = false;
                label.Text = "Your basket is empty.";
            }
            else
            {
                btnBuy.Enabled = true;
                label.Text = "Total: " + string.Format("{0:c}", sum);
            }
            label.Font.Size = FontUnit.Larger;
            phTotal.Controls.Clear();
            phTotal.Controls.Add(label);
        }

        private IEnumerable<Order> GetOrdersFromSession()
        {
            return Session["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

    }
}