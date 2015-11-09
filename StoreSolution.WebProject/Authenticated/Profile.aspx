<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="StoreSolution.WebProject.Authenticated.Profile"
    Title="<%$ Resources: Lang, Profile_Title %>" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/Content/Common.css" rel="stylesheet" />
    <link href="~/Content/Profile.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="labTitle" runat="server" />
        <div id="iconDiv" class="cellCenter">
            <asp:Image ID="icon" runat="server"/>
        </div>
        <br />
        <asp:FileUpload CssClass="btnProfile" ID="btnChooseIcon" runat="server" />
        <asp:Button CausesValidation="False" CssClass="btnProfile" ID="btnNewIcon" OnClick="btnNewIcon_Click" runat="server" Text="<%$ Resources: Lang, Profile_CommitIcon %>" />
        <hr />
        <asp:Label ID="labPersonData" runat="server" Text="<%$ Resources: Lang, Profile_PersonalData %>" />
        <br />
        <asp:Label ID="labName" runat="server" Text="<%$ Resources: Lang, Profile_PersonalName %>" />
        <br />
        <asp:TextBox ID="tbName" runat="server" />
        <br />
        <asp:Label ID="labSecondName" runat="server" Text="<%$ Resources: Lang, Profile_PersonalSecondName %>" />
        <br />
        <asp:TextBox ID="tbSecondName" runat="server" />
        <br />
        <br />
        <asp:Button CssClass="btnProfile" ID="btnSubmit" runat="server" Text="<%$ Resources: Lang, Profile_SubmitButton %>" OnClick="btnSubmit_Click" CausesValidation="False" />
        <br />
        <hr />
        <asp:Label ID="labPassword" runat="server" Text="<%$ Resources: Lang, Profile_ChangePassword %>" />
        <br />
        <asp:Label ID="labOldPassword" runat="server" Text="<%$ Resources: Lang, Profile_OldPassword %>"/>
        <br />
        <asp:TextBox ID="tbOldPassword" runat="server"/>
        <asp:RequiredFieldValidator ID="rfvOldPassword" runat="server" Display="Dynamic" ErrorMessage="*" ForeColor="Red" ControlToValidate="tbOldPassword"/>
        <br />
        <asp:Label ID="labNewPassword" runat="server" Text="<%$ Resources: Lang, Profile_NewPassword %>" />
        <br />
        <asp:TextBox ID="tbNewPassword" runat="server" />
        <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" Display="Dynamic" ErrorMessage="*" ForeColor="Red" ControlToValidate="tbNewPassword"/>
        <asp:Button CssClass="btnProfile" ID="btnNewPassword" runat="server" OnClick="btnNewPassword_Click" Text="<%$ Resources: Lang, Profile_ChangeButton %>" />
        <br />
        <hr />
    </div>
    <div id="footer">
        <div id="inFooter">
            <hr/>
            <asp:Label ID="labMessage" runat="server"/>
        </div>
    </div>
    <asp:Button ID="btnBack" runat="server" Text="<%$ Resources: Lang, Profile_ExitButton %>" CausesValidation="False" OnClick="btnBack_Click" />
    </form>
</body>
</html>
