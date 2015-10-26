<%@ Page AutoEventWireup="true" CodeBehind="ProductCatalog.aspx.cs" 
         Inherits="StoreSolution.WebProject.User.ProductCatalog" Language="C#" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product catalog</title>
    <link href="~/Content/Common.css" rel="stylesheet"/>
    <link href="~/Content/ProductCatalog.css" rel="stylesheet"/>
</head>
<body>
<form id="form1" runat="server">
    <div style="text-align: center">
        <div id="cap">
            <asp:Label ID="labUser" runat="server" />
            <asp:Button ID="btnSignOut" OnClick="btnSignOut_Click" runat="server" Text="Sign Out" />
            <br/>
        </div>
        <asp:Label ID="labTitle" runat="server" Text="Choose product for youself!"/>
        <hr/>
        <br/>
        <br/>
        <asp:GridView AllowPaging="True" 
                      HorizontalAlign="Center" 
                      ID="gvTable" 
                      OnPageIndexChanged="gvTable_PageIndexChanged" 
                      OnPageIndexChanging="gvTable_PageIndexChanging" 
                      OnRowDeleting="gvTable_RowDeleting" 
                      OnSelectedIndexChanged="gvTable_SelectedIndexChanged" 
                      runat="server" 
                      Width="50%">
            <Columns>
                <asp:CommandField ButtonType="Button" HeaderText="Add" SelectText="+" ShowSelectButton="True"/>
                <asp:BoundField HeaderText="Count"/>
                <asp:CommandField ButtonType="Button" DeleteText="-" HeaderText="Del" ShowDeleteButton="True"/>
            </Columns>
            <PagerStyle HorizontalAlign="Center"/>
        </asp:GridView>
        <br/>
        <br/>
        <br/>
        <asp:Button ID="btnBasket" OnClick="btnBasket_Click" runat="server" Text="To basket" Width="150px" />
        <br/>
        <hr/>
        <div id="message">
            <asp:PlaceHolder ID="phForMessage" runat="server"/>
        </div>
    </div>
</form>
</body>
</html>
