<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" 
    Inherits="StoreSolution.WebProject.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="~/Content/Index.css" rel="stylesheet"/>
    <link href="~/Content/Common.css" rel="stylesheet"/>
    <title>Welcome to Your Store!</title>
</head>
<body>
<form id="form1" runat="server">
    <div id="header">
        <asp:Label CssClass="cellCenter" ID="labWelcome" runat="server" Text="Hello! Nice to see You here. Please sign in to shop." />
    </div>
    <br/>
    <div id="center"
         class="cellCenter">
        <div class="cellCenter">
            <asp:Image ID="logo" ImageUrl="~/Content/Images/11logo.jpg" runat="server"/>
            <br/>
            <div id="credentials">
                <div class="delimeter"></div>
                <asp:Label ID="labLogin" runat="server" Text="Login"/>
                <br/>
                <asp:TextBox CssClass="tbCredentials" ID="tbLogin" runat="server"/>
                <asp:RequiredFieldValidator ControlToValidate="tbLogin" EnableClientScript="False" ErrorMessage="*" 
                                            ForeColor="Red" ID="rfvLogin" runat="server"/>
                <br/>
                <div class="delimeter"></div>
                <asp:Label CssClass="passwordClass" ID="labPassword" runat="server" Text="Password"/>
                <br/>
                <asp:TextBox CssClass="tbCredentials" ID="tbPassword" runat="server" TextMode="Password"/>
                <asp:RequiredFieldValidator ControlToValidate="tbPassword" EnableClientScript="False" ErrorMessage="*" 
                                            ForeColor="Red" ID="rfvPassword" runat="server"/>
                <br/>
                <br/>
                <asp:Button CssClass="btnCredentials" ID="btnSubmit" OnClick="btnSubmit_Click" runat="server" Text="Log In"/>
                <br/>
                <br />
                <asp:HyperLink ID="hlCreateUser" NavigateUrl="~/NotAuthenticated/NewUser.aspx" runat="server" Text="Not registered yet?"/>
            </div>
        </div>
    </div>
    <div id="footer">
        <div id="inFooter">
            <hr/>
            <asp:Label ID="labMessage" runat="server" Text=""/>
        </div>
    </div>
</form>
</body>
</html>
