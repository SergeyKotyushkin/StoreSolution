<%@ Page AutoEventWireup="true" CodeBehind="ProductCatalog.aspx.cs"
         Inherits="StoreSolution.WebProject.User.ProductCatalog" Language="C#"
         MasterPageFile="~/Master/StoreMaster.Master" Title="Product Catalog"%>

<asp:Content ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/ProductCatalog.css") %>" rel="stylesheet"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="StoreMasterContentId" runat="server">
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
                  Width="50%" OnRowCreated="gvTable_RowCreated" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3">
        <Columns>
            <asp:CommandField ButtonType="Button" HeaderText="Add" SelectText="+" ShowSelectButton="True">
            <ControlStyle CssClass="btnInGvTable" />
            </asp:CommandField>
            <asp:BoundField HeaderText="Count"/>
            <asp:CommandField ButtonType="Button" DeleteText="-" HeaderText="Del" ShowDeleteButton="True">
            <ControlStyle CssClass="btnInGvTable" />
            </asp:CommandField>
        </Columns>
        <FooterStyle BackColor="White" ForeColor="#000066" />
        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        <PagerStyle HorizontalAlign="Left" BackColor="White" ForeColor="#000066"/>
        <RowStyle ForeColor="#000066" />
        <SortedAscendingCellStyle BackColor="#F1F1F1" />
        <SortedAscendingHeaderStyle BackColor="#007DBB" />
        <SortedDescendingCellStyle BackColor="#CAC9C9" />
        <SortedDescendingHeaderStyle BackColor="#00547E" />
        <PagerStyle HorizontalAlign="Center"></PagerStyle>
    </asp:GridView>
    <br/>
    <br/>
    <br/>
    <asp:Button ID="btnBasket" OnClick="btnBasket_Click" runat="server" Text="To basket" Width="150px"/>
    <br />
</asp:Content>

