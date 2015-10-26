<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Basket.aspx.cs" Inherits="StoreSolution.WebProject.User.Basket" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Basket</title>
    <link href="~/Content/Basket.css" rel="stylesheet" />
    <link href="~/Content/Common.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="cap">
            <asp:Label ID="labUser" runat="server"/>
            <asp:Button ID="btnSignOut" OnClick="btnSignOut_Click" runat="server" Text="Sign Out" />
            <br/>
        </div>
        <div>
            <div>
                <asp:Label ID="labTitle" runat="server" Text="Your order"/>
            </div>
            <br/>
            <asp:GridView AllowPaging="True" 
                          CellPadding="5" 
                          HorizontalAlign="Center" 
                          ID="gvTable" 
                          OnPageIndexChanged="GV_table_PageIndexChanged" 
                          OnPageIndexChanging="GV_table_PageIndexChanging" 
                          runat="server"
                          Width="50%">
                <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
            </asp:GridView>
            <br/>
            <asp:PlaceHolder ID="phTotal" runat="server"/>
            <br/>
            <br/>
            <asp:Button ID="btnBuy" runat="server" Text="Buy" OnClick="btnBuy_Click" Width="120px"/>
            <br/>
            <div id="back">
                <asp:Button ID="btnBack" OnClick="btnBack_Click" runat="server" Text="Back" />
            </div>
        </div>
    </form>
</body>
</html>
