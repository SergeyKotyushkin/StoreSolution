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
    public partial class BasketPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowOrder();
        }

        private void ShowOrder()
        {
            DbRepository repo = DbRepository.GetInstance();
            var products = repo.GetProducts();

            TableOrder.Rows.Add(new TableRow());
            for (int i = 0; i < 4; i++)
            {
                TableOrder.Rows[TableOrder.Rows.Count - 1].Cells.Add(new TableCell());
            }

            TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[0].Text = "Название";
            TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[1].Text = "Количество";
            TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[2].Text = "Стоимость одного";
            TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[3].Text = "Стоимость общая";

            TableOrder.Rows[TableOrder.Rows.Count - 1].Font.Bold = true;

            if (Session["CurrentOrder"] == null) Session["CurrentOrder"] = new List<Order>();
            var orders = Session["CurrentOrder"] as List<Order>;

            double sum = 0;
            foreach (var item in orders)
            {
                Product product = repo.GetProductById(item.Id);
                if (product == null) continue;

                TableOrder.Rows.Add(new TableRow());
                for (int i = 0; i < 4; i++)
                {
                    TableOrder.Rows[TableOrder.Rows.Count - 1].Cells.Add(new TableCell());
                    TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[i].HorizontalAlign = HorizontalAlign.Center;
                }

                TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[0].Text = product.Name.ToString();
                TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[1].Text = item.Count.ToString();
                TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[2].Text = String.Format("{0:c}", product.Price);
                sum += item.Count * product.Price;
                TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[3].Text = String.Format("{0:c}", (item.Count * product.Price));
            }

            TableOrder.Rows.Add(new TableRow());
            for (int i = 0; i < 4; i++)
            {
                TableOrder.Rows[TableOrder.Rows.Count - 1].Cells.Add(new TableCell());
            }

            TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[0].Text = "Итого";
            TableOrder.Rows[TableOrder.Rows.Count - 1].Cells[TableOrder.Rows[0].Cells.Count - 1].Text = String.Format("{0:c}", sum);
            TableOrder.Rows[TableOrder.Rows.Count - 1].Font.Bold = true;


            TableOrder.GridLines = GridLines.Both;
            TableOrder.BorderStyle = BorderStyle.Solid;
            TableOrder.HorizontalAlign = HorizontalAlign.Center;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Session["Bought"] = "1";
            Session["CurrentOrder"] = null;
            Response.Redirect("StartPage.aspx");
        }
    }
}