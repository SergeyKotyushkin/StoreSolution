using StoreSolution.DatabaseProject.Model;
using System;
using System.Drawing;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.MyIoC;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.Admin
{
    public partial class ProductManagement : System.Web.UI.Page
    {
        private StoreMaster _master;
        private readonly IProductRepository _productRepository;

        protected ProductManagement()
            : this(SimpleContainer.Resolve<IProductRepository>())
        {
            
        }

        protected ProductManagement(IProductRepository iProductRepository)
        {
            _productRepository = iProductRepository;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master != null) _master.BtnBackVisibility = true;

            var user = Membership.GetUser();
            if (user == null) SignOut();

            SetTitles(user);

            FillProductsGridView(false);
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            SignOut();
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
            double price;

            var b1 = rgx.IsMatch(name);
            var b2 = rgx.IsMatch(category);
            var b3 = double.TryParse(e.NewValues["Price"].ToString().Trim(), out price);

            if (b1 && b2 && b3)
            {
                var product = new Product()
                {
                    Id = int.Parse(e.NewValues["Id"].ToString()),
                    Name = name,
                    Category = category,
                    Price = price
                };

                var result = _productRepository.AddOrUpdateProduct(product);
                if (result)
                {
                    _master.LabMessageForeColor = Color.DarkGreen;
                    _master.LabMessageText = "Product successfully updated.";
                    Logger.Log.Info("Product " + product.Name + " successfully updated.");
                }
                else
                {
                    _master.LabMessageForeColor = Color.Red;
                    _master.LabMessageText = "Error. Not valid value. Please check entered values.";
                }
            }
            else
            {
                _master.LabMessageForeColor = Color.Red;
                _master.LabMessageText = "Error. Not valid value. Please check entered values.";
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

            var product = _productRepository.GetProductById(id);
            if (_productRepository.RemoveProduct(id))
            {
                _master.LabMessageForeColor = Color.DarkGreen;
                _master.LabMessageText = "Product was removed.";
                Logger.Log.Info("Product " + product.Name + " successfully added.");
            }
            else
            {
                _master.LabMessageForeColor = Color.Red;
                _master.LabMessageText = "Error. Product wasn't removed.";
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
                double price;

                var rgx = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z]+[a-zA-Z0-9_ ]*$");

                var b1 = rgx.IsMatch(name);
                var b2 = rgx.IsMatch(category);
                var b3 = double.TryParse(priceStr, out price);
                
                if (b1 && b2 && b3)
                {
                    var product = new Product()
                    {
                        Id = -1,
                        Name = name,
                        Category = category,
                        Price = price
                    };

                    var result = _productRepository.AddOrUpdateProduct(product);
                    if (result)
                    {
                        _master.LabMessageForeColor = Color.DarkGreen;
                        _master.LabMessageText = "Product successfully added.";
                        Logger.Log.Info("Product " + product.Name + " successfully added.");
                    }
                    else
                    {
                        _master.LabMessageForeColor = Color.Red;
                        _master.LabMessageText = "Error. Not valid value. Please check entered values.";
                    }
                }
                else
                {
                    _master.LabMessageForeColor = Color.Red;
                    _master.LabMessageText = "Error. Not valid value. Please check entered values.";
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
            foreach (GridViewRow row in gvTable.Rows)
            {
                if (row.Cells[2].Controls.Count == 0)
                    row.Cells[5].Text = string.Format("{0:c}", double.Parse(row.Cells[5].Text));
            }
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
            _master.HlUserText = "Good day, " + user.UserName + "!";
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

        private void FillProductsGridView(bool bind)
        {
            var products = _productRepository.Products.ToList();
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
            var bInsert = new Button {Text = "Insert"};
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