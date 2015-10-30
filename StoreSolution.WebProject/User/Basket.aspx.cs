using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using StoreSolution.WebProject.Model;
using System.Web.Security;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.MyIoC;

namespace StoreSolution.WebProject.User
{
    public partial class Basket : System.Web.UI.Page
    {
        private const double Tolerance = double.Epsilon;

        private readonly IProductRepository _productRepository;

        protected Basket()
            : this(SimpleContainer.Resolve<IProductRepository>())
        {
            
        }

        protected Basket(IProductRepository iProductRepository)
        {
            _productRepository = iProductRepository;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Membership.GetUser();
            if (user == null) SignOut();

            SetTitles(user);

            if (!Page.IsPostBack)
                FillOrdersGridView();
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            Session["Bought"] = 1;
            Session["CurrentOrder"] = null;
            Response.Redirect("~/User/ProductCatalog.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/User/ProductCatalog.aspx");
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            SignOut();
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
            var products = _productRepository.Products.ToList();

            var orders = GetOrdersFromSession();

            var list = products.Join(orders, p => p.Id, q => q.Id, (p, q) => new { p.Name, p.Price, q.Count, Total = (q.Count * p.Price) }).ToList();
            gvTable.DataSource = list;               
            gvTable.DataBind();
            
            var sum = list.Sum(p => p.Total);

            if (Math.Abs(sum) < Tolerance)
            {
                btnBuy.Enabled = false;
                labTotal.Text = "Your basket is empty.";
            }
            else
            {
                btnBuy.Enabled = true;
                labTotal.Text = "Total: " + string.Format("{0:c}", sum);
            }
        }

        private void SetTitles(MembershipUser user)
        {
            hlUser.Text = "Good day, " + user.UserName + "!";
        }

        private void SignOut()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            Response.End();
        }

        private IEnumerable<Order> GetOrdersFromSession()
        {
            return Session["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                gvTable.Rows[i].Cells[1].Text = string.Format("{0:c}", double.Parse(gvTable.Rows[i].Cells[1].Text));
                gvTable.Rows[i].Cells[3].Text = string.Format("{0:c}", double.Parse(gvTable.Rows[i].Cells[3].Text));
            }
        }

    }
}