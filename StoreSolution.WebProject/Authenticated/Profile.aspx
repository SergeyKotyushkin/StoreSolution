<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="StoreSolution.WebProject.Authenticated.Profile"
Title="<%$ Resources: Lang, Profile_Title %>" MasterPageFile="~/Master/StoreMaster.Master" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/Profile.css") %>" rel="stylesheet"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="StoreMasterContentId" runat="server">
    <asp:Label ID="labTitle" runat="server"/>
    <div id="iconDiv" class="cellCenter">
        <asp:Image ID="icon" runat="server"/>
    </div>
    <br/>
    <asp:FileUpload CssClass="btnProfile" ID="btnChooseIcon" runat="server"/>
    <asp:Button CausesValidation="False" CssClass="btnProfile" ID="btnNewIcon" OnClick="btnNewIcon_Click" runat="server" Text="<%$ Resources: Lang, Profile_CommitIcon %>"/>
    <hr/>
    <asp:Label ID="labPersonData" runat="server" Text="<%$ Resources: Lang, Profile_PersonalData %>"/>
    <br/>
    <asp:Label ID="labName" runat="server" Text="<%$ Resources: Lang, Profile_PersonalName %>"/>
    <asp:TextBox ID="tbName" runat="server"/>
    <asp:Label ID="labSecondName" runat="server" Text="<%$ Resources: Lang, Profile_PersonalSecondName %>"/>
    <asp:TextBox ID="tbSecondName" runat="server"/>
    <asp:Button CssClass="btnProfile" ID="btnSubmit" runat="server" Text="<%$ Resources: Lang, Profile_SubmitButton %>" OnClick="btnSubmit_Click" CausesValidation="False"/>
    <br/>
    <hr/>
    <asp:Label ID="labPassword" runat="server" Text="<%$ Resources: Lang, Profile_ChangePassword %>"/>
    <br/>
    <asp:Label ID="labOldPassword" runat="server" Text="<%$ Resources: Lang, Profile_OldPassword %>"/>
    <asp:TextBox ID="tbOldPassword" runat="server"/>
    <asp:RequiredFieldValidator ID="rfvOldPassword" runat="server" Display="Dynamic" ErrorMessage="*" ForeColor="Red" ControlToValidate="tbOldPassword"/>
    <asp:Label ID="labNewPassword" runat="server" Text="<%$ Resources: Lang, Profile_NewPassword %>"/>
    <asp:TextBox ID="tbNewPassword" runat="server"/>
    <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" Display="Dynamic" ErrorMessage="*" ForeColor="Red" ControlToValidate="tbNewPassword"/>
    <asp:Button CssClass="btnProfile" ID="btnNewPassword" runat="server" OnClick="btnNewPassword_Click" Text="<%$ Resources: Lang, Profile_ChangeButton %>"/>
    <br/>
    <hr/>
    <br />
    <div id="history">
        <asp:Label ID="labOrderHistory" runat="server" Text="<%$ Resources: Lang, Profile_OrderHistoryHeader %>"/>
        <br/>
        <br/>
        <asp:GridView ID="gvOrderHistory" runat="server" OnDataBound="gvOrderHistory_OnDataBound" OnRowDataBound="gvOrderHistory_RowDataBound" AllowPaging="True" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2" PageSize="3" OnPageIndexChanging="gvOrderHistory_PageIndexChanging">
            <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510"/>
            <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White"/>
            <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" VerticalAlign="Middle"/>
            <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510"/>
            <SortedAscendingCellStyle BackColor="#FFF1D4"/>
            <SortedAscendingHeaderStyle BackColor="#B95C30"/>
            <SortedDescendingCellStyle BackColor="#F1E5CE"/>
            <SortedDescendingHeaderStyle BackColor="#93451F"/>
        </asp:GridView>
    </div>

</asp:Content>
