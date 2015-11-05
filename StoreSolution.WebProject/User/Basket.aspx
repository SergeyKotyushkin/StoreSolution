<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Basket.aspx.cs" 
         Inherits="StoreSolution.WebProject.User.Basket" 
         MasterPageFile="~/Master/StoreMaster.Master" Title="Your order"%>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/Basket.css") %>" rel="stylesheet"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="StoreMasterContentId" runat="server">
    <br/>
    <asp:Label ID="labTitle" runat="server" Text="Your order"/>
    <br/>
    <br />
    <asp:GridView AllowPaging="True"
                  CellPadding="3"
                  HorizontalAlign="Center"
                  ID="gvTable"
                  OnPageIndexChanged="GV_table_PageIndexChanged"
                  OnPageIndexChanging="GV_table_PageIndexChanging"
                  runat="server"
                  Width="50%" OnDataBound="gvTable_DataBound" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
        <FooterStyle BackColor="White" ForeColor="#000066" />
        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        <PagerStyle HorizontalAlign="Left" VerticalAlign="Middle" BackColor="White" ForeColor="#000066"/>
        <RowStyle ForeColor="#000066" />
        <SortedAscendingCellStyle BackColor="#F1F1F1" />
        <SortedAscendingHeaderStyle BackColor="#007DBB" />
        <SortedDescendingCellStyle BackColor="#CAC9C9" />
        <SortedDescendingHeaderStyle BackColor="#00547E" />
        <PagerStyle HorizontalAlign="Center"/>
    </asp:GridView>
    <br/>
    <br/>
    <asp:Label ID="labTotal" runat="server"/>
    <br/>
    <br/>
    <asp:Button ID="btnBuy" runat="server" Text="Buy" OnClick="btnBuy_Click" Width="120px"/>
    <br/>
</asp:Content>
