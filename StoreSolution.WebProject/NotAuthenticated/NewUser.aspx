<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewUser.aspx.cs" Inherits="StoreSolution.WebProject.NewUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="~/Content/Common.css" rel="stylesheet" />
    <link href="~/Content/NewUser.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="labTitle" runat="server" Text="Enter your credentials"/>
            <br />
            <br />
        </div>
        <div style="text-align: center; font-size: large">
            <asp:Label ID="labLogin" runat="server" Text="Login"></asp:Label>
            <br />
            <asp:TextBox ID="tbLogin" runat="server" Width="200px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ControlToValidate="tbLogin" ErrorMessage="*" CssClass="validate"></asp:RequiredFieldValidator>
            <br />
            <div class="delimeter"></div>
            <asp:Label ID="labPassword" runat="server" Text="Password"></asp:Label>
            <br />
            <asp:TextBox ID="tbPassword" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="tbPassword" ErrorMessage="*" CssClass="validate"></asp:RequiredFieldValidator>
            <br />
            <div class="delimeter"></div>
            <asp:Label ID="labEmail" runat="server" Text="Email"></asp:Label>
            <br />
            <asp:TextBox ID="tbEmail" runat="server" Width="200px" TextMode="Email"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="tbEmail" ErrorMessage="*" CssClass="validate"></asp:RequiredFieldValidator>
            <br />
            <div class="delimeter"></div>
            <asp:Label ID="labQuestion" runat="server" Text="Question"></asp:Label>
            <br />
            <asp:TextBox ID="tbQuestion" runat="server" Width="200px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvQuestion" runat="server" ControlToValidate="tbQuestion" ErrorMessage="*" CssClass="validate"></asp:RequiredFieldValidator>
            <br />
            <div class="delimeter"></div>
            <asp:Label ID="labAnswer" runat="server" Text="Answer"></asp:Label>
            <br />
            <asp:TextBox ID="tbAnswer" runat="server" Width="200px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvAnswer" runat="server" ControlToValidate="tbAnswer" ErrorMessage="*" CssClass="validate"></asp:RequiredFieldValidator>
            <br />
            <div class="delimeter"></div>
            <br />
            <asp:Button ID="btnSubmit" runat="server" Text="Create" OnClick="btnSubmit_Click" />
        </div>
        <div id="back">
            <asp:Button ID="btnBack" OnClick="btnBack_Click" runat="server" Text="Back" CausesValidation="False" />
        </div>
        <div id="footer">
            <div id="inFooter">
                <hr/>
                <asp:Label CssClass="validate" ID="labMessage" runat="server" Text=""/>
            </div>
        </div>
    </form>
</body>
</html>
