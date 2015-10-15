<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StartPage.aspx.cs" Inherits="StoreSolution.WebProject.StartPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            font-size: x-large;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center">
    
        <strong><em><span class="auto-style1">Choose product for youself!</span></em><hr />
        </strong>
        <br />
        <asp:Table ID="TableProducts" runat="server" CellPadding="10" style="font-size: large">
        </asp:Table>
        <br />
        <asp:Button ID="ButtonBasket" runat="server" OnClick="ButtonBasket_Click" style="font-size: medium" Text="Перейти к корзине" />
        <br />
        <hr />
        <div class="auto-style2">
            <asp:PlaceHolder ID="PlaceHolderForMessage" runat="server"></asp:PlaceHolder>
        </div>
    </div>
    </form>
</body>
</html>
