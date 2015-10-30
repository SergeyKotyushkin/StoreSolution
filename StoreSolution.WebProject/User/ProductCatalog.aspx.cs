using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.MyIoC;
using StoreSolution.WebProject.Model;

namespace StoreSolution.WebProject.User
{
    public partial class ProductCatalog : System.Web.UI.Page
    {
        private readonly IProductRepository _productRepository;

        protected ProductCatalog()
            : this(SimpleContainer.Resolve<IProductRepository>())
        {
        }

        protected ProductCatalog(IProductRepository iProductRepository)
        {
            _productRepository = iProductRepository;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Membership.GetUser();
            if (user == null) SignOut();

            SetTitles(user);

            FillGridView(false);
            FillCountColumn();
        }

        protected void gvTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = GetIdFromRow(gvTable.SelectedIndex);

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
            SignOut();
        }

        protected void gvTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = GetIdFromRow(e.RowIndex);

            var orders = GetOrdersFromSession();
            RemoveFromOrders(orders, id);
            LoadOrdersToSession(orders);
            FillCountColumn();
        }

        protected void gvTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTable.PageIndex = e.NewPageIndex;
        }

        protected void gvTable_PageIndexChanged(object sender, EventArgs e)
        {
            FillGridView(true);
            FillCountColumn();
        }


        private void SignOut()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            Response.End();
        }

        private void SetTitles(MembershipUser user)
        {
            hlUser.Text = "Good day, " + user.UserName + "!";

            labMessage.Text = "";
            if (Session["Bought"] == null) return;
            labMessage.ForeColor = Color.DarkGreen;
            labMessage.Text = "Products were bought successfully.";
            Session["Bought"] = null;
        }

        private void FillGridView(bool bind)
        {
            var products = _productRepository.Products;

            gvTable.DataSource = products.Select(p => new {p.Id, p.Name, p.Category, p.Price}).ToList();
            if (!Page.IsPostBack || bind)
                gvTable.DataBind();
        }

        private void FillCountColumn()
        {
            var orders = GetOrdersFromSession();

            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                var id = GetIdFromRow(i);
                var foo = orders.Find(order => order.Id == id);
                gvTable.Rows[i].Cells[1].Text = foo == null ? (0).ToString() : (foo.Count).ToString();
            }
        }

        private void LoadOrdersToSession(List<Order> orders)
        {
            Session["CurrentOrder"] = orders;
        }

        private List<Order> GetOrdersFromSession()
        {
            return Session["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

        private int GetIdFromRow(int index)
        {
            int id;
            if (!int.TryParse(gvTable.Rows[index].Cells[3].Text, out id)) return -1;

            var product = _productRepository.GetProductById(id);
            return product == null ? -1 : product.Id;
        }

        private void AddToOrders(List<Order> orders, int id)
        {
            var order = orders.Find(o => o.Id == id);
            if (order == null) orders.Add(new Order {Id = id, Count = 1});
            else order.Count++;

            labMessage.Text = "";
            labMessage.ForeColor = Color.DarkGreen;
            labMessage.Text = "Product '" + _productRepository.GetProductById(id).Name + "' was added to order.";
        }

        private void RemoveFromOrders(List<Order> orders, int id)
        {
            var order = orders.Find(o => o.Id == id);
            if (order == null || order.Count == 0) return;
            if (order.Count == 1) orders.Remove(order);
            else order.Count--;

            labMessage.Text = "";
            labMessage.ForeColor = Color.DarkBlue;
            labMessage.Text = "Product '" + _productRepository.GetProductById(id).Name + "' was removed from order.";
        }

        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvTable.Rows)
                row.Cells[6].Text = string.Format("{0:c}", double.Parse(row.Cells[6].Text));
        }

        protected void gvTable_RowCreated(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    e.Row.Cells[3].CssClass = "hiddencol";
                    break;
                case DataControlRowType.Header:
                    e.Row.Cells[3].CssClass = "hiddencol";
                    break;
            }
        }
    }
}