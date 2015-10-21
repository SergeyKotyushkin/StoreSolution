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

        <div class="auto-style1">
    
        <div class="auto-style2">
            Your order</div>
        <br />
        <asp:GridView ID="GV_table" runat="server" CellPadding="5" HorizontalAlign="Center" Width="476px" AllowPaging="True" OnPageIndexChanged="GV_table_PageIndexChanged" OnPageIndexChanging="GV_table_PageIndexChanging">
            <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" />
        </asp:GridView>
        <br />
        <asp:PlaceHolder ID="PH_total" runat="server"></asp:PlaceHolder>
        <br />
        <br />
        <asp:Button ID="B_buy" runat="server" style="font-size: medium" Text="Buy" OnClick="B_buy_Click" Width="117px" />
        <br />
        <div class="auto-style3">
            <asp:Button ID="B_back" runat="server" BackColor="#C1C1FF" OnClick="B_back_Click" Text="Back" Font-Bold="False" Font-Names="Calibri" Font-Size="Medium" ForeColor="#000066" Height="41px" Width="118px" />
        </div>

    </div>
        <p>
            &nbsp;</p>
    </form>
</body>
</html>
