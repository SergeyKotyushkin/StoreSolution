<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductManagement.aspx.cs" 
    Inherits="StoreSolution.WebProject.Admin.ProductManagement"
    EnableEventValidation="false" MasterPageFile="~/Master/StoreMaster.Master"
    Title="<%$ Resources: Lang, ProductManagement_Title %>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/ProductManagement.css") %>" rel="stylesheet"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="StoreMasterContentId" runat="server">
    <br/>
    <asp:Label ID="labTitle" runat="server" Text="<%$ Resources: Lang, ProductManagement_Title %>"/>
    <br/>
    <asp:Panel runat="server" ID="myDialogBox" EnableTheming="True" Visible="False">
        <hr/>
        <br/>
        <asp:Label ID="labMessageBoxTitle" runat="server" Text="<%$ Resources: Lang, ProductManagement_InsertQuestion %>"/>
        <br/>
        <br/>
        <div>
            <asp:Button runat="server" ID="btnYes" Text="<%$ Resources: Lang, ProductManagement_InsertAnswerYes %>" OnClick="btnYes_Click"/>
            <asp:Button runat="server" ID="btnNo" Text="<%$ Resources: Lang, ProductManagement_InsertAnswerNo %>" OnClick="btnNo_Click"/>
        </div>
        <br/>
        <hr/>
    </asp:Panel>
    <br/>
    <asp:GridView AllowPaging="True" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" 
                  BorderWidth="1px" CellPadding="3" HorizontalAlign="Center" ID="gvTable" 
                  OnDataBound="gvTable_DataBound" OnPageIndexChanging="gvTable_PageIndexChanging" 
                  OnPreRender="gvTable_PreRender" OnRowCancelingEdit="gvTable_RowCancelingEdit" 
                  OnRowDataBound="gvTable_RowDataBound" OnRowDeleting="gvTable_RowDeleting" 
                  OnRowEditing="gvTable_RowEditing" OnRowUpdating="gvTable_RowUpdating" 
                  PageSize="7" runat="server" ShowFooter="True" Width="50%" OnRowCreated="gvTable_RowCreated">
        <Columns>
            <asp:CommandField ButtonType="Button" ShowEditButton="True" CausesValidation="False" 
                EditText="<%$ Resources: Lang, ProductManagement_EditButton %>"
                UpdateText="<%$ Resources: Lang, ProductManagement_UpdateButton %>"
                CancelText="<%$ Resources: Lang, ProductManagement_CancelButton %>">
            <ControlStyle CssClass="btnInGvTable" />
            </asp:CommandField>
            <asp:CommandField ButtonType="Button" ShowDeleteButton="True" DeleteText="<%$ Resources: Lang, ProductManagement_DeleteButton %>">
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
