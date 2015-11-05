<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="StoreSolution.WebProject.Authenticated.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profile</title>
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
        <asp:Button CausesValidation="False" CssClass="btnProfile" ID="btnNewIcon" OnClick="btnNewIcon_Click" runat="server" Text="Commit this icon" />
        <hr />
        <asp:Label ID="labPersonData" runat="server" Text="Personal data" />
        <br />
        <asp:Label ID="labName" runat="server" Text="Name" />
        <br />
        <asp:TextBox ID="tbName" runat="server" />
        <br />
        <asp:Label ID="labSecondName" runat="server" Text="SecondName" />
        <br />
        <asp:TextBox ID="tbSecondName" runat="server" />
        <br />
        <hr />
        <asp:Label ID="labPassword" runat="server" Text="Change password" />
        <br />
        <asp:Label ID="labOldPassword" runat="server" Text="Old password"/>
        <br />
        <asp:TextBox ID="tbOldPassword" runat="server"/>
        <asp:RequiredFieldValidator ID="rfvOldPassword" runat="server" Display="Dynamic" ErrorMessage="*" ForeColor="Red" ControlToValidate="tbOldPassword"></asp:RequiredFieldValidator>
        <br />
        <asp:Label ID="labNewPassword" runat="server" Text="New password" />
        <br />
        <asp:TextBox ID="tbNewPassword" runat="server" />
        <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" Display="Dynamic" ErrorMessage="*" ForeColor="Red" ControlToValidate="tbNewPassword"></asp:RequiredFieldValidator>
        <asp:Button CssClass="btnProfile" ID="btnNewPassword" runat="server" OnClick="btnNewPassword_Click" Text="Change" />
        <br />
        <hr />
        <div>
            <asp:Button CssClass="btnProfile" ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" CausesValidation="False" />
        </div>
    </div>
    <div id="footer">
        <div id="inFooter">
            <hr/>
            <asp:Label ID="labMessage" runat="server"/>
        </div>
    </div>
    <asp:Button ID="btnBack" runat="server" Text="Exit" CausesValidation="False" OnClick="btnBack_Click" />
    </form>
</body>
</html>
