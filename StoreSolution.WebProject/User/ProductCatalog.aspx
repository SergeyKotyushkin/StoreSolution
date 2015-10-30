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
            <asp:HyperLink ID="hlUser" NavigateUrl="~/Authenticated/Profile.aspx" runat="server" />
            <asp:Button ID="btnSignOut" OnClick="btnSignOut_Click" runat="server" Text="Sign Out" />
            <br/>
        </div>
        <br />
        <asp:Label ID="labTitle" runat="server" Text="Choose products for youself!"/>
        <br/>
        <br/>
        <asp:GridView AllowPaging="True" 
                      DataKeyNames="Id"
                      HorizontalAlign="Center" 
                      ID="gvTable" 
                      OnDataBound="gvTable_DataBound" 
                      OnPageIndexChanged="gvTable_PageIndexChanged" 
                      OnPageIndexChanging="gvTable_PageIndexChanging" 
                      OnRowDeleting="gvTable_RowDeleting" 
                      OnSelectedIndexChanged="gvTable_SelectedIndexChanged" 
                      runat="server" 
                      Width="50%" OnRowCreated="gvTable_RowCreated">
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
    </div>
    <div id="footer">
        <div id="inFooter">
            <hr/>
            <asp:Label ID="labMessage" runat="server" Text=""/>
        </div>
    </div>
</form>
</body>
</html>
