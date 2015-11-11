<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" 
    Inherits="StoreSolution.WebProject.Index" MasterPageFile="~/Master/StoreMaster.Master"
    Title="<%$ Resources: Lang, Index_Title%>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/Index.css") %>" rel="stylesheet"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="StoreMasterContentId" runat="server">
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
</asp:Content>
