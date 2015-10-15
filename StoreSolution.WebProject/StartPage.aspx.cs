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

namespace StoreSolution.WebProject
{
    public partial class StartPage : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowAllProducts();
            RenameBtns();

            if (Session["Bought"] != null)
            {
                Label l_message = new Label();
                l_message.Text = "Products have bought! Success!";
                PlaceHolderForMessage.Controls.Add(l_message);
                Session["Bought"] = null;
            }
        }

        private void ShowAllProducts()
        {
            DbRepository repo = DbRepository.GetInstance();
            var products = repo.GetProducts();

            TableProducts.Rows.Add(new TableRow());
            for (int i = 0; i < 4; i++)
            {
                TableProducts.Rows[TableProducts.Rows.Count - 1].Cells.Add(new TableCell());
            }

            TableProducts.Rows[TableProducts.Rows.Count - 1].Cells[0].Text = "Название";
            TableProducts.Rows[TableProducts.Rows.Count - 1].Cells[1].Text = "Категория";
            TableProducts.Rows[TableProducts.Rows.Count - 1].Cells[2].Text = "Цена";
            TableProducts.Rows[TableProducts.Rows.Count - 1].Cells[3].Text = "Купить";

            TableProducts.Rows[TableProducts.Rows.Count - 1].Font.Bold = true;

            foreach (var item in products)
            {
                TableProducts.Rows.Add(new TableRow());
                for (int i = 0; i < 4; i++)
                {
                    TableProducts.Rows[TableProducts.Rows.Count - 1].Cells.Add(new TableCell());
                    TableProducts.Rows[TableProducts.Rows.Count - 1].Cells[i].HorizontalAlign = HorizontalAlign.Center;
                }
                
                TableProducts.Rows[TableProducts.Rows.Count - 1].Cells[0].Text = item.Name.ToString();
                TableProducts.Rows[TableProducts.Rows.Count - 1].Cells[1].Text = item.Category.ToString();
                TableProducts.Rows[TableProducts.Rows.Count - 1].Cells[2].Text = String.Format("{0:c}", item.Price);

                Button btn = new Button();
                btn.Text = "+";
                btn.ID = "tableBtn_" + item.Id;
                btn.Click += btn_Click;
                TableProducts.Rows[TableProducts.Rows.Count - 1]
                    .Cells[TableProducts.Rows[TableProducts.Rows.Count - 1]
                    .Cells.Count - 1].Controls.Add(btn);
            }

            TableProducts.GridLines = GridLines.Both;
            TableProducts.BorderStyle = BorderStyle.Solid;
            TableProducts.HorizontalAlign = HorizontalAlign.Center;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button b = (sender as Button);
            int id = int.Parse(b.ID.Substring(b.ID.IndexOf('_') + 1));

            if (Session["CurrentOrder"] == null) Session["CurrentOrder"] = new List<Order>();
            var orders = Session["CurrentOrder"] as List<Order>;

            addToOrders(orders, id);
            RenameBtns();
        }

        private void addToOrders(List<Order> orders, int id)
        {
            bool b = false;
            foreach (var item in orders)
            {
                if (item.Id == id)
                {
                    item.Count++;
                    b = true;
                    break;
                }
            }

            if (!b) orders.Add(new Order() { Id = id, Count = 1 });
        }

        private void RenameBtns()
        {
            if (Session["CurrentOrder"] == null) Session["CurrentOrder"] = new List<Order>();
            var orders = Session["CurrentOrder"] as List<Order>;

            for (int i = 1; i < TableProducts.Rows.Count; i++)
            {
                string id_str = (TableProducts.Rows[i].Cells[TableProducts.Rows[i].Cells.Count - 1].Controls[0] as Button).ID;
                int id = int.Parse(id_str.Substring(id_str.IndexOf('_') + 1));

                string text = "+";
                foreach (var item in orders)
                {
                    if (item.Id == id)
                    {
                        text = "ok (" + item.Count.ToString() + ")";
                        break;
                    }
                }

                (TableProducts.Rows[i].Cells[TableProducts.Rows[i].Cells.Count - 1].Controls[0] as Button).Text = text;
            }
        }

        protected void ButtonBasket_Click(object sender, EventArgs e)
        {
            Response.Redirect("BasketPage.aspx");
        }
    }
}