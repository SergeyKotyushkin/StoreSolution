using StoreSolution.DatabaseProject.Model;
using StoreSolution.DatabaseProject.Realizations;
using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace StoreSolution.WebProject.SuperThings
{
    public partial class ProductManagementPage : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                Response.Redirect("~/LoginPage.aspx");
                return;
            }
            L_user.Text = "Good day, " + user.UserName + "!";

            FillProductsGridView(false);
        }

        protected void B_SignOut_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("~/LoginPage.aspx");
            //FormsAuthentication.RedirectToLoginPage();
        }

        protected void GV_table_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GV_table.EditIndex = e.NewEditIndex;
            FillProductsGridView(true);
        }

        protected void GV_table_RowUpdating(object sender, GridViewUpdateEventArgs e)
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

                DbRepository.GetInstance().UpdateProduct(product);
            }
            else
            {
                Session["Message"] = "Error. Not valid value. Please check entered values.";
                ShowMessage();
            }

            GV_table.EditIndex = -1;

            FillProductsGridView(true);
        }

        protected void GV_table_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GV_table.EditIndex = -1;
            FillProductsGridView(true);
        }

        protected void GV_table_PreRender(object sender, EventArgs e)
        {
            if (GV_table.EditIndex != -1)
                GV_table.Rows[GV_table.EditIndex].Cells[2].Enabled = false;
        }

        protected void GV_table_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = int.Parse(e.Values["Id"].ToString());

            if (DbRepository.GetInstance().RemoveProduct(id)) Session["Message"] = "Product was removed.";
            else  Session["Message"] = "Error. Product wasn't removed.";

            FillProductsGridView(true);
        }
        
        protected void GV_table_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_table.PageIndex = e.NewPageIndex;
            FillProductsGridView(true);
        }

        protected void B_yes_Click(object sender, EventArgs e)
        {
            var textBox1 = GV_table.FooterRow.Cells[3].Controls[0] as TextBox;
            var textBox2 = GV_table.FooterRow.Cells[4].Controls[0] as TextBox;
            var textBox3 = GV_table.FooterRow.Cells[5].Controls[0] as TextBox;

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
                        Id = 0,
                        Name = name,
                        Category = category,
                        Price = price
                    };

                    DbRepository.GetInstance().AddProduct(product);
                }
                else
                {
                    Session["Message"] = "Error. Not valid value. Please check entered values.";
                    //ShowMessage();
                }
            }

            B_no_Click(null, null);
            FillProductsGridView(true);
        }
        
        protected void B_no_Click(object sender, EventArgs e)
        {
            myDialogBox.Visible = !myDialogBox.Visible;
            GV_table.Enabled = !myDialogBox.Visible;
        }



        private void FillProductsGridView(bool bind)
        {
            var products = DbRepository.GetInstance().GetProducts();
            products.Insert(0, new Product() {Id = 0, Name = "0", Category = "0", Price = 0});

            GV_table.DataSource = products.ToList();
            if(!Page.IsPostBack || bind)
                GV_table.DataBind();

            if(GV_table.Columns.Count == 6)
                foreach (GridViewRow row in GV_table.Rows)
                    row.Cells[5].Text = string.Format("{0:c}", double.Parse(row.Cells[5].Text));


            ShowMessage();
            ShowFooter();


            GV_table.Rows[0].Visible = false;
        }

        private void ShowMessage()
        {
            if (Session["Message"] != null)
            {
                var label = new Label {Text = Session["Message"] as string};
                label.Font.Size = FontUnit.Larger;

                PH_message.Controls.Add(label);
                Session["Message"] = null;
            }
            else
            {
                PH_message.Controls.Clear();
            }
        }

        private void b_insert_Click(object sender, EventArgs e)
        {
            myDialogBox.Visible = !myDialogBox.Visible;
            GV_table.Enabled = !myDialogBox.Visible;
        }

        private void ShowFooter()
        {
            var bInsert = new Button {Text = "Insert"};
            var tbName = new TextBox();
            var tbCategory = new TextBox();
            var tbPrice = new TextBox();

            foreach (TableCell cell in GV_table.FooterRow.Cells)
                cell.Controls.Clear();

            GV_table.FooterRow.Cells[1].Controls.Add(bInsert);
            GV_table.FooterRow.Cells[3].Controls.Add(tbName);
            GV_table.FooterRow.Cells[4].Controls.Add(tbCategory);
            GV_table.FooterRow.Cells[5].Controls.Add(tbPrice);

            bInsert.Click += b_insert_Click;
        }
    }
}