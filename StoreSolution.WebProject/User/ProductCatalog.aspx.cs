using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using StoreSolution.DatabaseProject.Model;
using StoreSolution.DatabaseProject.Realizations;
using StoreSolution.WebProject.Model;

namespace StoreSolution.WebProject.User
{
    public partial class ProductCatalog : System.Web.UI.Page
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

            if (Session["Bought"] != null)
            {
                var lMessage = new Label {Text = "Products are bought! Success!"};
                phForMessage.Controls.Clear();
                phForMessage.Controls.Add(lMessage);
                Session["Bought"] = null;
            }

            FillGridView();
            FillCountColumn();
        }

        protected void gvTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = GetIdFromRow(gvTable.SelectedIndex, 
                DbRepository.GetInstance().GetProducts().ToList());

            var orders = GetOrdersFromSession();

            AddToOrders(orders, id);
            LoadOrdersToSession(orders);
            FillCountColumn();
        }

        protected void btnBasket_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/User/Basket.aspx");
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void gvTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = GetIdFromRow(e.RowIndex,
                DbRepository.GetInstance().GetProducts().ToList());

            var orders = GetOrdersFromSession();
            RemoveFromOrders(orders, id);
            LoadOrdersToSession(orders);
            FillCountColumn();
        }

        protected void gvTable_PageIndexChanged(object sender, EventArgs e)
        {
            FillGridView();
            FillCountColumn();
        }

        protected void gvTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTable.PageIndex = e.NewPageIndex;
        }



        private void FillGridView()
        {
            var products = DbRepository.GetInstance().GetProducts();

            gvTable.DataSource = products.Select(p => new { p.Name, p.Category, p.Price }).ToList();
            gvTable.DataBind();

            if (gvTable.Columns.Count != 6) return;
            foreach (GridViewRow row in gvTable.Rows)
                row.Cells[5].Text = string.Format("{0:c}", double.Parse(row.Cells[5].Text));
        }

        private void FillCountColumn()
        {
            var orders = GetOrdersFromSession();
            var products = DbRepository.GetInstance().GetProducts().ToList();

            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                var id = GetIdFromRow(i, products);
                var foo = orders.Find(order => order.Id == id);
                if (foo == null)
                    gvTable.Rows[i].Cells[1].Text = (0).ToString();
                else
                {
                    gvTable.Rows[i].Cells[1].Text = (foo.Count).ToString();
                    gvTable.Rows[i].Cells[1].ID = "Record_" + id;
                }
            }
        }

        private List<Order> GetOrdersFromSession()
        {
            return Session["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

        private void LoadOrdersToSession(List<Order> orders)
        {
            Session["CurrentOrder"] = orders;
        }

        private int GetIdFromRow(int index, List<Product> products)
        {
            var name = gvTable.Rows[index].Cells[3].Text;
            var category = gvTable.Rows[index].Cells[4].Text;
            var price = double.Parse(gvTable.Rows[index].Cells[5].Text);

            var product = products.Find(p => p.Name == name && p.Category == category && Math.Abs(p.Price - price) < Tolerance);

            return product == null ? -1: product.Id;
        }

        private static void AddToOrders(List<Order> orders, int id)
        {
            var order = orders.Find(p => p.Id == id);
            if (order == null) orders.Add(new Order() { Id = id, Count = 1 });
            else order.Count++;
        }

        private static void RemoveFromOrders(List<Order> orders, int id)
        {
            var order = orders.Find(p => p.Id == id);
            if (order != null && order.Count != 0)
            {
                if (order.Count == 1) orders.Remove(order);
                else order.Count--;
            }
        }
        
        

    }
}