<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewUser.aspx.cs" Inherits="StoreSolution.WebProject.NotAuthenticated.NewUser" 
    MasterPageFile="~/Master/StoreMaster.Master" Title="<%$ Resources: Lang, NewUser_Title %>"%>


<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/NewUser.css") %>" rel="stylesheet"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="StoreMasterContentId" runat="server">
        <div>
            <asp:Label ID="labTitle" runat="server" Text="<%$ Resources: Lang, NewUser_Header %>"/>
            <br />
            <br />
        </div>
        <div id="main">
            <asp:Label ID="labLogin" runat="server" Text="<%$ Resources: Lang, NewUser_Login %>"/>
            <br />
            <asp:TextBox ID="tbLogin" runat="server" Width="200px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ControlToValidate="tbLogin" ErrorMessage="*" ForeColor="Red"/>
            <br />
            <div class="delimeter"></div>
            <asp:Label ID="labPassword" runat="server" Text="<%$ Resources: Lang, NewUser_Password %>"/>
            <br />
            <asp:TextBox ID="tbPassword" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="tbPassword" ErrorMessage="*" ForeColor="Red"/>
            <br />
            <div class="delimeter"></div>
            <asp:Label ID="labEmail" runat="server" Text="<%$ Resources: Lang, NewUser_Email %>"/>
            <br />
            <asp:TextBox ID="tbEmail" runat="server" Width="200px" TextMode="Email"/>
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="tbEmail" ErrorMessage="*" ForeColor="Red"/>
            <br />
            <div class="delimeter"></div>
            <asp:Label ID="labQuestion" runat="server" Text="<%$ Resources: Lang, NewUser_Question %>"/>
            <br />
            <asp:TextBox ID="tbQuestion" runat="server" Width="200px"/>
            <asp:RequiredFieldValidator ID="rfvQuestion" runat="server" ControlToValidate="tbQuestion" ErrorMessage="*" ForeColor="Red"/>
            <br />
            <div class="delimeter"></div>
            <asp:Label ID="labAnswer" runat="server" Text="<%$ Resources: Lang, NewUser_Answer %>"/>
            <br />
            <asp:TextBox ID="tbAnswer" runat="server" Width="200px"/>
            <asp:RequiredFieldValidator ID="rfvAnswer" runat="server" ControlToValidate="tbAnswer" ErrorMessage="*" ForeColor="Red"/>
            <br />
            <div class="delimeter"></div>
            <br />
            <asp:Button ID="btnSubmit" runat="server" Text="<%$ Resources: Lang, NewUser_SubmitButton %>" OnClick="btnSubmit_Click" />
        </div>
</asp:Content>
