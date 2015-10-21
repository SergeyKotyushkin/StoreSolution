<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductManagementPage.aspx.cs" 
    Inherits="StoreSolution.WebProject.SuperThings.ProductManagementPage"
    EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style6 {
            font-size: x-large;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div style="background-color: #808080; border-style: solid; text-align: right;">
            <asp:Label ID="L_user" runat="server" Font-Bold="True" ForeColor="#FFFF99" style="text-align: right"></asp:Label>
            &nbsp;
            <asp:Button ID="B_SignOut" runat="server" Font-Bold="False" Font-Underline="False" ForeColor="#990000" OnClick="B_SignOut_Click" style="font-size: x-small" Text="Sign Out" />
            &nbsp;&nbsp;&nbsp;            
            <br />
        </div>
        
    <div style="text-align: center">     
        <span class="auto-style6">Products managment</span><br />
        <br />
        <asp:Panel runat="server" ID="myDialogBox" EnableTheming="True" Visible="False">
            <p>Are you sure you want to continue?</p>
            <div>
                <asp:Button runat="server" ID="B_yes" Text="Yes" OnClick="B_yes_Click"/>
                <asp:Button runat="server" ID="B_no" Text="No" OnClick="B_no_Click"/>
            </div>
        </asp:Panel>

        <br />
        <asp:GridView ID="GV_table" runat="server" CellPadding="5" HorizontalAlign="Center" OnRowEditing="GV_table_RowEditing" Width="346px" OnRowUpdating="GV_table_RowUpdating" OnRowCancelingEdit="GV_table_RowCancelingEdit" AllowPaging="True" OnPageIndexChanging="GV_table_PageIndexChanging" OnPreRender="GV_table_PreRender" OnRowDeleting="GV_table_RowDeleting" PageSize="7" ShowFooter="True">
            <Columns>
                <asp:CommandField ButtonType="Button" ShowEditButton="True" CausesValidation="False" />
                <asp:CommandField ButtonType="Button" ShowDeleteButton="True" />
            </Columns>
            <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" />
        </asp:GridView>
        <br />
        <asp:PlaceHolder ID="PH_message" runat="server"></asp:PlaceHolder>
        <br />
        <br />
    
    </div>
    </form>
</body>
</html>
