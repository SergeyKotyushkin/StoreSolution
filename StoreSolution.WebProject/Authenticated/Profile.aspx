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
    <br/>
    <asp:TextBox ID="tbName" runat="server"/>
    <br/>
    <asp:Label ID="labSecondName" runat="server" Text="<%$ Resources: Lang, Profile_PersonalSecondName %>"/>
    <br/>
    <asp:TextBox ID="tbSecondName" runat="server"/>
    <br/>
    <br/>
    <asp:Button CssClass="btnProfile" ID="btnSubmit" runat="server" Text="<%$ Resources: Lang, Profile_SubmitButton %>" OnClick="btnSubmit_Click" CausesValidation="False"/>
    <br/>
    <hr/>
    <asp:Label ID="labPassword" runat="server" Text="<%$ Resources: Lang, Profile_ChangePassword %>"/>
    <br/>
    <asp:Label ID="labOldPassword" runat="server" Text="<%$ Resources: Lang, Profile_OldPassword %>"/>
    <br/>
    <asp:TextBox ID="tbOldPassword" runat="server"/>
    <asp:RequiredFieldValidator ID="rfvOldPassword" runat="server" Display="Dynamic" ErrorMessage="*" ForeColor="Red" ControlToValidate="tbOldPassword"/>
    <br/>
    <asp:Label ID="labNewPassword" runat="server" Text="<%$ Resources: Lang, Profile_NewPassword %>"/>
    <br/>
    <asp:TextBox ID="tbNewPassword" runat="server"/>
    <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" Display="Dynamic" ErrorMessage="*" ForeColor="Red" ControlToValidate="tbNewPassword"/>
    <asp:Button CssClass="btnProfile" ID="btnNewPassword" runat="server" OnClick="btnNewPassword_Click" Text="<%$ Resources: Lang, Profile_ChangeButton %>"/>
    <br/>
    <hr/>
</asp:Content>
