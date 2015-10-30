<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductManagement.aspx.cs" 
    Inherits="StoreSolution.WebProject.Admin.ProductManagement"
    EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product managment</title>
    <link href="~/Content/Common.css" rel="stylesheet" />
    <link href="~/Content/ProductManagment.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="cap">
            <asp:HyperLink ID="hlUser" NavigateUrl="~/Authenticated/Profile.aspx" runat="server" />
            <asp:Button ID="btnSignOut" OnClick="btnSignOut_Click" runat="server" Text="Sign Out" />
            <br />
        </div>
        <div>
            <asp:Label ID="labTitle" runat="server" Text="Products managment" />
            <br />
            <asp:Panel runat="server" ID="myDialogBox" EnableTheming="True" Visible="False">
                <hr />
                <br />
                <asp:Label ID="labMessageBoxTitle" runat="server" Text="Are you sure you want to continue?" />
                <br />
                <br />
                <div>
                    <asp:Button runat="server" ID="btnYes" Text="Yes" OnClick="btnYes_Click"/>
                    <asp:Button runat="server" ID="btnNo" Text="No" OnClick="btnNo_Click"/>
                </div>
                <br />
                <hr />
            </asp:Panel>
            <br />
            <asp:GridView AllowPaging="True" 
                          CellPadding="5" 
                          HorizontalAlign="Center" 
                          ID="gvTable" 
                          OnPageIndexChanging="gvTable_PageIndexChanging" 
                          OnPreRender="gvTable_PreRender" 
                          OnRowCancelingEdit="gvTable_RowCancelingEdit" 
                          OnRowDeleting="gvTable_RowDeleting" 
                          OnRowEditing="gvTable_RowEditing" 
                          OnRowUpdating="gvTable_RowUpdating" 
                          PageSize="7" 
                          runat="server" 
                          ShowFooter="True" 
                          Width="50%" OnDataBound="gvTable_DataBound">
                <Columns>
                    <asp:CommandField ButtonType="Button" ShowEditButton="True" CausesValidation="False" />
                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" />
                </Columns>
                <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            </asp:GridView>
            <br />
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
