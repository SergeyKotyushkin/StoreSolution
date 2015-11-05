<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductManagement.aspx.cs" 
    Inherits="StoreSolution.WebProject.Admin.ProductManagement"
    EnableEventValidation="false" MasterPageFile="~/Master/StoreMaster.Master" 
    Title="Product management" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/ProductManagement.css") %>" rel="stylesheet"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="StoreMasterContentId" runat="server">
    <br/>
    <asp:Label ID="labTitle" runat="server" Text="Products management"/>
    <br/>
    <asp:Panel runat="server" ID="myDialogBox" EnableTheming="True" Visible="False">
        <hr/>
        <br/>
        <asp:Label ID="labMessageBoxTitle" runat="server" Text="Are you sure you want to continue?"/>
        <br/>
        <br/>
        <div>
            <asp:Button runat="server" ID="btnYes" Text="Yes" OnClick="btnYes_Click"/>
            <asp:Button runat="server" ID="btnNo" Text="No" OnClick="btnNo_Click"/>
        </div>
        <br/>
        <hr/>
    </asp:Panel>
    <br/>
    <asp:GridView AllowPaging="True"
                  CellPadding="3"
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
                  Width="50%" OnDataBound="gvTable_DataBound" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
        <Columns>
            <asp:CommandField ButtonType="Button" ShowEditButton="True" CausesValidation="False">
            <ControlStyle CssClass="btnInGvTable" />
            </asp:CommandField>
            <asp:CommandField ButtonType="Button" ShowDeleteButton="True">
            <ControlStyle CssClass="btnInGvTable" />
            </asp:CommandField>
        </Columns>
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
</asp:Content>
