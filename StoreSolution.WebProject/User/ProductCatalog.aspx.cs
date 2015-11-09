using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.MyIoC;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;
using StoreSolution.WebProject.Model;

namespace StoreSolution.WebProject.User
{
    public partial class ProductCatalog : System.Web.UI.Page
    {
        private StoreMaster _master;
        private readonly IProductRepository _productRepository;

        protected ProductCatalog()
            : this(SimpleContainer.Resolve<IProductRepository>())
        {
        }

        protected ProductCatalog(IProductRepository iProductRepository)
        {
            _productRepository = iProductRepository;
        }

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (null == cookie) return;
            Page.Culture = cookie.Value;
            Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master != null) _master.BtnBackVisibility = false;

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
            var user = Membership.GetUser();
            if (user == null) return;

            Logger.Log.Debug("User " + user.UserName + " redirected to Basket page.");
            Response.Redirect("~/User/Basket.aspx");
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
                    e.Row.Cells[3].CssClass = "hiddenСolumn";
                    break;
                case DataControlRowType.Header:
                    e.Row.Cells[3].CssClass = "hiddenСolumn";
                    break;
            }
        }

        protected void gvTable_PageIndexChanged(object sender, EventArgs e)
        {
            FillGridView(true);
            FillCountColumn();
        }

        protected void gvTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Header) return;
            e.Row.Cells[1].Text = (string)HttpContext.GetGlobalResourceObject("Lang", "ProductCatalog_HeaderCount");
            e.Row.Cells[4].Text = (string)HttpContext.GetGlobalResourceObject("Lang", "ProductCatalog_HeaderName");
            e.Row.Cells[5].Text = (string)HttpContext.GetGlobalResourceObject("Lang", "ProductCatalog_HeaderCategory");
            e.Row.Cells[6].Text = (string)HttpContext.GetGlobalResourceObject("Lang", "ProductCatalog_HeaderPrice");
        }


        private void SignOut()
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                Logger.Log.Error("No user at Product Management page start.");
                Logger.Log.Error("Sign out.");
            }
            else Logger.Log.Error("User " + user.UserName + " sing out.");

            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            Response.End();
        }

        private void SetTitles(MembershipUser user)
        {
            var hlUserText = (string)HttpContext.GetGlobalResourceObject("Lang", "Master_ToProfile");
            if (hlUserText != null) _master.HlUserText = string.Format(hlUserText, user.UserName);

            _master.LabMessageText = "";
            if (Session["Bought"] == null) return;
            _master.LabMessageForeColor = Color.DarkGreen;
            _master.LabMessageText = (string)HttpContext.GetGlobalResourceObject("Lang", "ProductCatalog_ProductsBought");
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

            _master.LabMessageForeColor = Color.DarkGreen;
            var text = (string) HttpContext.GetGlobalResourceObject("Lang", "ProductCatalog_ProductAdded");
            if (text != null) _master.LabMessageText = string.Format(text, _productRepository.GetProductById(id).Name);
        }

        private void RemoveFromOrders(List<Order> orders, int id)
        {
            var order = orders.Find(o => o.Id == id);
            if (order == null || order.Count == 0) return;
            if (order.Count == 1) orders.Remove(order);
            else order.Count--;

            _master.LabMessageForeColor = Color.DarkBlue;
            var text = (string)HttpContext.GetGlobalResourceObject("Lang", "ProductCatalog_ProductRemoved");
            if (text != null) _master.LabMessageText = string.Format(text, _productRepository.GetProductById(id).Name);
        }
    }
}