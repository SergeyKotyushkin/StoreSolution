using StoreSolution.DatabaseProject.Model;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.WebProject.Currency;
using StoreSolution.WebProject.Lang;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;
using StructureMap;

namespace StoreSolution.WebProject.Admin
{
    public partial class ProductManagement : System.Web.UI.Page
    {
        private StoreMaster _master;
        private readonly IProductRepository _iProductRepository;

        protected ProductManagement()
            : this(ObjectFactory.GetInstance<IProductRepository>())
        {
        }

        protected ProductManagement(IProductRepository iIProductRepository)
        {
            _iProductRepository = iIProductRepository;
        }

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (cookie == null)
            {
                cookie = new HttpCookie("language", "en-US");
                Response.Cookies.Add(cookie);
            }
            Page.Culture = cookie.Value;
            Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");
            
            _master.BtnBackVisibility = false;

            var user = Membership.GetUser();
            if (user == null)
            {
                _master.SignOut(false);
                return;
            }

            SetTitles(user);

            FillProductsGridView(false);
        }
        
        protected void gvTable_RowEditing(object sender, GridViewEditEventArgs e)
        {
            _master.LabMessageText = "";
            gvTable.EditIndex = e.NewEditIndex;

            gvTable.PagerSettings.Visible = false;
            FillProductsGridView(true);
            SetButtonsEnabled(false);
        }

        protected void gvTable_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var rgx = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z]+[a-zA-Z0-9_ ]*$");
            var name = e.NewValues["Name"].ToString().Trim();
            var category = e.NewValues["Category"].ToString().Trim();
            decimal price;

            var b1 = rgx.IsMatch(name);
            var b2 = rgx.IsMatch(category);
            var b3 = decimal.TryParse(e.NewValues["Price"].ToString().Trim(), out price);

            if (b1 && b2 && b3)
            {
                var product = new Product()
                {
                    Id = int.Parse(e.NewValues["Id"].ToString()),
                    Name = name,
                    Category = category,
                    Price = price
                };

                if (_iProductRepository.AddOrUpdateProduct(product))
                {
                    _master.LabMessageForeColor = Color.DarkGreen;
                    _master.LabMessageText = LangSetter.Set("ProductManagement_ProductWasUpdated");
                    Logger.Log.Info(string.Format("Product {0} successfully updated.", product.Name));
                }
                else
                {
                    _master.LabMessageForeColor = Color.Red;
                    _master.LabMessageText = LangSetter.Set("ProductManagement_ProductWasNotUpdated");
                }
            }
            else
            {
                _master.LabMessageForeColor = Color.Red;
                _master.LabMessageText = LangSetter.Set("ProductManagement_ProductWasNotUpdated");
            }

            gvTable.EditIndex = -1;

            gvTable.PagerSettings.Visible = true;
            FillProductsGridView(true);
            SetButtonsEnabled(true);
        }

        protected void gvTable_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            _master.LabMessageText = "";
            gvTable.EditIndex = -1;

            gvTable.PagerSettings.Visible = true;
            FillProductsGridView(true);
            SetButtonsEnabled(true);
        }

        protected void gvTable_PreRender(object sender, EventArgs e)
        {
            if (gvTable.EditIndex != -1) 
                gvTable.Rows[gvTable.EditIndex].Cells[2].Enabled = false;
        }

        protected void gvTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = int.Parse(e.Values["Id"].ToString());

            var product = _iProductRepository.GetProductById(id);
            if (_iProductRepository.RemoveProduct(id))
            {
                _master.LabMessageForeColor = Color.DarkGreen;
                _master.LabMessageText = LangSetter.Set("ProductManagement_ProductWasRemoved");
                Logger.Log.Info(string.Format("Product {0} successfully added.", product.Name));
            }
            else
            {
                _master.LabMessageForeColor = Color.Red;
                _master.LabMessageText = LangSetter.Set("ProductManagement_ProductWasNotRemoved");
            }

            FillProductsGridView(true);
        }

        protected void gvTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            _master.LabMessageText = "";
            gvTable.PageIndex = e.NewPageIndex;
            FillProductsGridView(true);
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            var textBox1 = gvTable.FooterRow.Cells[3].Controls[0] as TextBox;
            var textBox2 = gvTable.FooterRow.Cells[4].Controls[0] as TextBox;
            var textBox3 = gvTable.FooterRow.Cells[5].Controls[0] as TextBox;

            if (textBox1 != null && textBox2 != null && textBox3 != null)
            {
                var name = textBox1.Text.Trim();
                var category = textBox2.Text.Trim();
                var priceStr = textBox3.Text.Trim();
                decimal price;

                var rgx = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z]+[a-zA-Z0-9_ ]*$");

                var cultureInfo = _master.GetCurrencyCultureInfo();
                var b1 = rgx.IsMatch(name);
                var b2 = rgx.IsMatch(category);
                var b3 = decimal.TryParse(priceStr, NumberStyles.AllowDecimalPoint, cultureInfo, out price);
                
                if (b1 && b2 && b3)
                {

                    var culturePrice = CurrencyConverter.ConvertToRu(price, cultureInfo.Name);
                    var product = new Product()
                    {
                        Id = -1,
                        Name = name,
                        Category = category,
                        Price = culturePrice
                    };

                    if (_iProductRepository.AddOrUpdateProduct(product))
                    {
                        _master.LabMessageForeColor = Color.DarkGreen;
                        _master.LabMessageText = LangSetter.Set("ProductManagement_ProductWasAdded");
                        Logger.Log.Info(string.Format("Product {0} successfully added.", product.Name));
                    }
                    else
                    {
                        _master.LabMessageForeColor = Color.Red;
                        _master.LabMessageText = LangSetter.Set("ProductManagement_ProductWasNotAdded");
                    }
                }
                else
                {
                    _master.LabMessageForeColor = Color.Red;
                    _master.LabMessageText = LangSetter.Set("ProductManagement_ProductWasNotAdded");
                }
            }

            ControlVisibilityOfMessageBox();
            FillProductsGridView(true);
        }

        protected void btnNo_Click(object sender, EventArgs e)
        {
            ControlVisibilityOfMessageBox();
            _master.LabMessageText = "";
        }

        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            var cultureInfo = _master.GetCurrencyCultureInfo();

            var rate = CurrencyConverter.GetRate(cultureInfo.Name);
            foreach (GridViewRow row in gvTable.Rows)
            {
                if (row.Cells[2].Controls.Count != 0) continue;
                var price = CurrencyConverter.ConvertFromRu(decimal.Parse(row.Cells[5].Text), rate);
                row.Cells[5].Text = price.ToString("C", cultureInfo);
            }
        }

        protected void gvTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Header) return;
            e.Row.Cells[2].Text = LangSetter.Set("ProductManagement_HeaderId");
            e.Row.Cells[3].Text = LangSetter.Set("ProductManagement_HeaderName");
            e.Row.Cells[4].Text = LangSetter.Set("ProductManagement_HeaderCategory");
            e.Row.Cells[5].Text = LangSetter.Set("ProductManagement_HeaderPrice");
        }


        private void SetButtonsEnabled(bool enabled)
        {
            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                if (i == gvTable.EditIndex) continue;
                gvTable.Rows[i].Cells[0].Enabled = enabled;
                gvTable.Rows[i].Cells[1].Enabled = enabled;
            }
            gvTable.FooterRow.Visible = enabled;
        }

        private void ControlVisibilityOfMessageBox()
        {
            myDialogBox.Visible = !myDialogBox.Visible;
            gvTable.Enabled = !myDialogBox.Visible;
        }

        private void SetTitles(MembershipUser user)
        {
            var hlUserText = LangSetter.Set("Master_ToProfile");
            if (hlUserText != null) _master.HlUserText = string.Format(hlUserText, user.UserName);
        }

        private void FillProductsGridView(bool bind)
        {
            var products = _iProductRepository.Products.ToList();
            products.Insert(0, new Product {Id = 0, Name = "0", Category = "0", Price = 0});

            gvTable.DataSource = products;
            if(!Page.IsPostBack || bind)
                gvTable.DataBind();
            
            ShowFooter();

            if (gvTable.PageIndex == 0)
                gvTable.Rows[0].Visible = false;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            ControlVisibilityOfMessageBox();
        }

        private void ShowFooter()
        {
            var bInsert = new Button { Text = LangSetter.Set("ProductManagement_InsertButton") };
            var tbName = new TextBox();
            var tbCategory = new TextBox();
            var tbPrice = new TextBox();

            foreach (TableCell cell in gvTable.FooterRow.Cells)
                cell.Controls.Clear();

            gvTable.FooterRow.Cells[1].Controls.Add(bInsert);
            gvTable.FooterRow.Cells[3].Controls.Add(tbName);
            gvTable.FooterRow.Cells[4].Controls.Add(tbCategory);
            gvTable.FooterRow.Cells[5].Controls.Add(tbPrice);

            bInsert.CssClass = "btnInGvTable";
            bInsert.Click += btnInsert_Click;
        }
    }
}