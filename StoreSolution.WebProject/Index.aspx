<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" 
    Inherits="StoreSolution.WebProject.Index" Title="<%$ Resources: Lang, Index_Title%>" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="~/Content/Index.css" rel="stylesheet"/>
    <link href="~/Content/Common.css" rel="stylesheet"/>
    <title></title>
</head>
<body>
<form id="form1" runat="server">
    <div id="header">
        <asp:Label CssClass="cellCenter" ID="labWelcome" runat="server" Text="<%$ Resources: Lang, Index_Header %>"/>
    </div>
    <br/>
    <div id="center">
        <div class="cellCenter">
            <asp:Image ID="logo" ImageUrl="~/Content/Images/11logo.jpg" runat="server"/>
            <br/>
            <div id="credentials">
                <div class="delimeter"></div>
                <asp:Label ID="labLogin" runat="server" Text="<%$ Resources: Lang, Index_Login %>"/>
                <br/>
                <asp:TextBox CssClass="tbCredentials" ID="tbLogin" runat="server"/>
                <asp:RequiredFieldValidator ControlToValidate="tbLogin" EnableClientScript="False" ErrorMessage="*" 
                                            ForeColor="Red" ID="rfvLogin" runat="server"/>
                <br/>
                <div class="delimeter"></div>
                <asp:Label CssClass="passwordClass" ID="labPassword" runat="server" Text="<%$ Resources: Lang, Index_Password %>"/>
                <br/>
                <asp:TextBox CssClass="tbCredentials" ID="tbPassword" runat="server" TextMode="Password"/>
                <asp:RequiredFieldValidator ControlToValidate="tbPassword" EnableClientScript="False" ErrorMessage="*" 
                                            ForeColor="Red" ID="rfvPassword" runat="server"/>
                <br/>
                <br/>
                <asp:Button CssClass="btnCredentials" ID="btnSubmit" OnClick="btnSubmit_Click" runat="server" Text="<%$ Resources: Lang, Index_LoginButon %>"/>
                <br/>
                <br />
                <asp:HyperLink ID="hlCreateUser" NavigateUrl="~/NotAuthenticated/NewUser.aspx" runat="server" Text="<%$ Resources: Lang, Index_NewUser %>" />
            </div>
        </div>
    </div>
    <div id="footer">
        <div id="inFooter">
            <hr/>
            <asp:Label ID="labMessage" runat="server"/>
        </div>
    </div>
</form>
</body>
</html>
