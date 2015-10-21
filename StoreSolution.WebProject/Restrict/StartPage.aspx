<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StartPage.aspx.cs" Inherits="StoreSolution.WebProject.Restrict.StartPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    </head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center">
    
        <div style="background-color: #808080; border-style: solid; text-align: right;">
            <asp:Label ID="L_user" runat="server" Font-Bold="True" ForeColor="#FFFF99" style="text-align: right"></asp:Label>
            &nbsp;
            <asp:Button ID="B_SignOut" 
                        runat="server" 
                        Font-Bold="False" 
                        Font-Underline="False" 
                        ForeColor="#990000" 
                        OnClick="B_SignOut_Click" 
                        style="font-size: x-small" 
                        Text="Sign Out" />
            &nbsp;&nbsp;&nbsp;            
            <br />
        </div>
        <strong><em>
        <asp:Label ID="L_title" runat="server" style="font-size: x-large" Text="Choose product for youself!"></asp:Label>
        </em>&nbsp;</strong><hr />
        <br />
        <br />
        <asp:GridView ID="GV_table" runat="server" HorizontalAlign="Center" OnRowDeleting="GV_table_RowDeleting" OnSelectedIndexChanged="GV_table_SelectedIndexChanged" Width="502px" AllowPaging="True" OnPageIndexChanged="GV_table_PageIndexChanged" OnPageIndexChanging="GV_table_PageIndexChanging">
            <Columns>
                <asp:CommandField ButtonType="Button" HeaderText="Add" SelectText="+" ShowSelectButton="True" />
                <asp:BoundField HeaderText="Count" />
                <asp:CommandField ButtonType="Button" DeleteText="-" HeaderText="Del" ShowDeleteButton="True" />
            </Columns>
            <PagerStyle HorizontalAlign="Center" />
        </asp:GridView>
        <br />
        <br />
        <br />
        <asp:Button ID="ButtonBasket" runat="server" OnClick="ButtonBasket_Click" style="font-size: medium" Text="To basket" Width="147px" />
        <br />
        <hr />
        <div class="auto-style2">
            <asp:PlaceHolder ID="PlaceHolderForMessage" runat="server"></asp:PlaceHolder>
        </div>
    </div>
    </form>
</body>
</html>
