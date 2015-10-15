<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasketPage.aspx.cs" Inherits="StoreSolution.WebProject.BasketPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            font-size: x-large;
            text-align: center;
        }
        .auto-style2 {
            text-align: center;
        }
        .auto-style3 {
            text-align: left;
        }
    </style>
</head>
<body style="text-align: center">
    <form id="form1" runat="server">
    <div class="auto-style1">
    
        <div class="auto-style2">
            Your order</div>
        <br />
        <asp:Table ID="TableOrder" runat="server" CellPadding="10">
        </asp:Table>
        <br />
        <asp:Button ID="Button1" runat="server" style="font-size: medium" Text="Оплатить" OnClick="Button1_Click" />
        <br />
        <div class="auto-style3">
            <asp:HyperLink ID="HyperLinkBack" runat="server" style="text-align: left; font-size: medium;" NavigateUrl="~/StartPage.aspx">Назад</asp:HyperLink>
        </div>

    </div>
        <p>
            &nbsp;</p>
    </form>
</body>
</html>
