<%@ Page AutoEventWireup="true" CodeBehind="ProductCatalog.aspx.cs"
         Inherits="StoreSolution.WebProject.User.ProductCatalog" Language="C#"
         MasterPageFile="~/Master/StoreMaster.Master" Title="<%$ Resources: Lang, ProductCatalog_Title %>"%>

<asp:Content ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/ProductCatalog.css") %>" rel="stylesheet"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="StoreMasterContentId" runat="server">
    <div>
        <div>
            <br/>
            <asp:Label ID="labTitle" runat="server" Text="<%$ Resources: Lang, ProductCatalog_Header %>"/>
            <br/>
            <br/>
            <asp:GridView AllowPaging="True" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                          HorizontalAlign="Center" ID="gvTable" OnDataBound="gvTable_DataBound" OnPageIndexChanged="gvTable_PageIndexChanged"
                          OnPageIndexChanging="gvTable_PageIndexChanging" OnRowCreated="gvTable_RowCreated" OnRowDataBound="gvTable_RowDataBound"
                          OnRowDeleting="gvTable_RowDeleting" OnSelectedIndexChanged="gvTable_SelectedIndexChanged" runat="server" Width="50%">
                <Columns>
                    <asp:CommandField ButtonType="Button" SelectText="+" ShowSelectButton="True">
                        <ControlStyle CssClass="btnInGvTable"/>
                    </asp:CommandField>
                    <asp:BoundField/>
                    <asp:CommandField ButtonType="Button" DeleteText="-" ShowDeleteButton="True">
                        <ControlStyle CssClass="btnInGvTable"/>
                    </asp:CommandField>
                </Columns>
                <FooterStyle BackColor="White" ForeColor="#000066"/>
                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White"/>
                <PagerStyle HorizontalAlign="Left" BackColor="White" ForeColor="#000066"/>
                <RowStyle ForeColor="#000066"/>
                <SortedAscendingCellStyle BackColor="#F1F1F1"/>
                <SortedAscendingHeaderStyle BackColor="#007DBB"/>
                <SortedDescendingCellStyle BackColor="#CAC9C9"/>
                <SortedDescendingHeaderStyle BackColor="#00547E"/>
                <PagerStyle HorizontalAlign="Center"></PagerStyle>
            </asp:GridView>
            <br/>
            <div style="text-align: center; display: inline-block">
                <asp:CheckBox ID="cbSearchHeader" runat="server" Text="<%$ Resources: Lang, ProductCatalog_SearchHeader %>"
                              Checked="False" AutoPostBack="True" OnCheckedChanged="cbSearchHeader_CheckedChanged"/>
                <div style="text-align: left;">
                    <asp:Panel ID="pSearchingBoard" runat="server">
                        <asp:Label ID="labSearchName" runat="server" Text="<%$ Resources: Lang, ProductCatalog_SearchName %>"/>
                        <asp:TextBox ID="tbSearchName" runat="server"/>
                        <asp:Label ID="labSearchCategory" runat="server" Text="<%$ Resources: Lang, ProductCatalog_SearchCategory %>"/>
                        <asp:DropDownList ID="ddlSearchCategory" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchCategory_SelectedIndexChanged"/>
                        <asp:Button ID="btnSearch" runat="server" Text="<%$ Resources: Lang, ProductCatalog_SearchButton %>" OnClick="btnSearch_Click"/>
                    </asp:Panel>
                </div>
            </div>
            <br/>
            <br/>
            <br/>
            <asp:Button ID="btnBasket" OnClick="btnBasket_Click" runat="server" Text="<%$ Resources: Lang, ProductCatalog_BusketButton %>" Width="150px"/>
            <br/>
        </div>
        <br />
    </div>
</asp:Content>