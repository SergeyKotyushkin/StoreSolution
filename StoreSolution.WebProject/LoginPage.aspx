<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginPage.aspx.cs" Inherits="StoreSolution.WebProject.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 137px;
        }
        .auto-style3 {
            width: 137px;
            height: 23px;
        }
        .auto-style8 {
            width: 207px;
            text-align: center;
        }
        .auto-style10 {
            text-align: center;
            width: 111px;
        }
        .auto-style11 {
            text-align: center;
            width: 111px;
            height: 30px;
        }
        .auto-style12 {
            width: 207px;
            text-align: center;
            height: 30px;
        }
        .auto-style13 {
            width: 137px;
            height: 30px;
        }
        .auto-style14 {
            width: 207px;
            text-align: left;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center">
    
        <br />
        <table style="margin: auto; width: 51%;">
            <tr>
                <td class="auto-style10">&nbsp;</td>
                <td class="auto-style8">
    
        Please login<br />
                    <br />
                </td>
                <td class="auto-style1">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style11">Login</td>
                <td class="auto-style12">
                    <asp:TextBox ID="TB_login" runat="server" Width="200px"></asp:TextBox>
                </td>
                <td class="auto-style13">
                    <asp:Button ID="B_action" runat="server" OnClick="B_action_Click" Text="Log In" />
                </td>
            </tr>
            <tr>
                <td class="auto-style10">&nbsp;</td>
                <td class="auto-style14">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TB_login" ErrorMessage="Required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
&nbsp;
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TB_login" ErrorMessage=" Not valid login." ForeColor="#3333FF" ValidationExpression="^[a-zA-Z0-9_-]{3,16}$" Display="Dynamic"></asp:RegularExpressionValidator>
                </td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style10">Password</td>
                <td class="auto-style8">
                    <asp:TextBox ID="TB_password" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
                </td>
                <td class="auto-style1">
                    <asp:CheckBox ID="CB_create" runat="server" AutoPostBack="True" OnCheckedChanged="CB_create_CheckedChanged" Text="Create new user?" />
                </td>
            </tr>
            <tr>
                <td class="auto-style10">&nbsp;</td>
                <td class="auto-style14">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TB_password" ErrorMessage="Required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
&nbsp;
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TB_password" ErrorMessage=" Not valid password." ForeColor="#3333FF" ValidationExpression="^[a-zA-Z0-9_!@#$%&amp;*-]{6,18}$" Display="Dynamic"></asp:RegularExpressionValidator>
                </td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style10">
                    <asp:Label ID="L_email" runat="server" Text="E-mail" Visible="False"></asp:Label>
                </td>
                <td class="auto-style8">
                    <asp:TextBox ID="TB_email" runat="server" Visible="False" Width="200px"></asp:TextBox>
                </td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style10">&nbsp;</td>
                <td class="auto-style14">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TB_email" ErrorMessage="Required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
&nbsp;
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="TB_email" ErrorMessage=" Not valid email." ForeColor="#3333FF" ValidationExpression="^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$" Display="Dynamic"></asp:RegularExpressionValidator>
                </td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style10">
                    <asp:Label ID="L_question" runat="server" Text="Question" Visible="False"></asp:Label>
                </td>
                <td class="auto-style8">
                    <asp:TextBox ID="TB_question" runat="server" Visible="False" Width="200px"></asp:TextBox>
                </td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style10">&nbsp;</td>
                <td class="auto-style14">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TB_question" ErrorMessage="Required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style10">
                    <asp:Label ID="L_answer" runat="server" Text="Answer" Visible="False"></asp:Label>
                </td>
                <td class="auto-style8">
                    <asp:TextBox ID="TB_answer" runat="server" Visible="False" Width="200px"></asp:TextBox>
                </td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style10">&nbsp;</td>
                <td class="auto-style14">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TB_answer" ErrorMessage="Required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style10">&nbsp;</td>
                <td class="auto-style8">&nbsp;</td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style10">&nbsp;</td>
                <td class="auto-style8">
                    <asp:Label ID="L_message" runat="server"></asp:Label>
                </td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
