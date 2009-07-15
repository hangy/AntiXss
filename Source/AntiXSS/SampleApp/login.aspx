<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Feedback.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>TechEd 2008 - Login to provide feedback</title>
    <style type="text/css">

table {font-family:verdana;font-size:11px;line-height:140%;}
.center_orange_top {height:100px;background-image: url('images/orange_C05602.png');
        }


.center_nav {height:19px;background-image: url('images/black_nav_center.png');
background-repeat: repeat-x;
        }
.center_main {border:1px solid #D7D7D7;background-color:#F1F1F3;}


#contentTable {width:90%;
            height: 82px;
        }
        .style1
        {
            width: 90%;
        }
        
       
/*  Footer  Styles*/
.ShellFooterTable
{
	height:40px;width: 100%; text-decoration: none; vertical-align: middle; white-space: nowrap;
	border-top:solid 5px #003366;
}    
        .style2
        {
            width: 100%;
            height: 14px;
        }
        
        .boldoblique 
        {
            font-weight:bolder; 
            font-style:oblique;
        }

    </style>
</head>
<body bgcolor="#979797">
    
    <form id="form2" runat="server">
    <div height="90%">
    
                            <table id="centerTable" border="0" cellpadding="0" cellspacing="0" 
                                height="100%" width="845px" align="center">

                                <tr>
                                    <td id="nav_dev" class="center_nav">
                                        <!-- BEGIN Nav Area1 --><!-- END Nav Area1 -->
                                    </td>
                                </tr>
                                <tr>
                                    <td class="center_main">
                                        <table id="contentTable" border="0" cellpadding="0" cellspacing="0" 
                                            width="100%">
                                            <tr>
                                                <td id="contentTableNav2">
                                                    <!-- BEGIN Nav Area2 --><!-- END Nav Area2 -->
                                                </td>
                                                <td>
                                                    <div id="divContent">
                                                        <!-- BEGIN Main Content Area -->
                                                        <table align="center" cellpadding="0" cellspacing="0" class="style1">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    Welcome to feedback tool. You are currently at the 
                                                                    <asp:Label ID="lblVenue" runat="server" Text="USA"></asp:Label> Venue. Click <a href="Login.aspx?location=Other">here</a> to browse to other locations.
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <div>
    
                                                                        <h3>
                                                                            &nbsp;</h3>
                                                                        <h3>
            Login to provide feedback</h3>
        <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
        <br />
        <br />
        Username:
        <asp:TextBox ID="txtUsername" runat="server" Text="johndoe"></asp:TextBox>
        <br />
        Password:&nbsp;
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Login" />
        <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:CheckBox ID="chkRememberMe" runat="server" Text="Remember Me" />
    </div>
    
    </form>
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                    <!-- END Main Content Area -->
                                                </td>
                                            </tr>
                                        </table>
<!-- Disclaimer -- Begin -->                                        
<table class="ShellFooterTable" cellspacing="0" cellpadding="0" border="0" width="100%" style="max-height:40px" >		
    <tr >
        <td class="style2">&nbsp;&nbsp;
            <span id="ctl01_ShellFooterContent_ctl00_MicrosoftConfidential">Copyright © Microsoft Corporation.  All rights reserved. </span>
        </td>
    </tr> 
</table>
<!-- Disclaimer -- End -->                                                
                                 
                                    </td>
                                </tr>
                            </table>
    </div>
    </form>
</body>
</html>
