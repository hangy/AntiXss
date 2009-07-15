<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Feedback._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>TechEd 2008 - Feedback Page</title>
    <style type="text/css">

#grayTable {background-color:#A0A0A0;background-image: url('images/gray_upper_center.png');
background-repeat: repeat-x;
        }
table {font-family:verdana;font-size:11px;line-height:140%;}
.gray_left {background-image: url('images/gray_side_left.png');
background-repeat: repeat-y;width:40px;
        }
.gray_upper {background-image: url('images/gray_upper_center.png');
background-repeat: repeat-x;height:55px;
        }
.center_orange_top {height:100px;background-image: url('images/orange_C05602.png');
        }


.center_nav {height:19px;background-image: url('images/black_nav_center.png');
background-repeat: repeat-x;
        }
.center_main {border:1px solid #D7D7D7;background-color:#F1F1F3;}


#contentTable {width:90%}
input{font-size:9pt;font-family:verdana;}
.banner {font-family:verdana;font-size:15px;line-height:140%;font-weight:bold}

.error { font-weight: bold;font-size: 10pt;color: #dc143c;}
.btn {font-size:9pt;font-family:verdana;}
.gray_lower {background-image: url('images/gray_lower_center.png');
background-repeat: repeat-x;height:103px;
        }


.gray_right{ background-image: url('images/gray_side_right.png'); 
background-repeat: repeat-y;width: 40px; background-color:#979797;
        }
        .style1
        {
            width: 90%;
        }
        .style2
        {
            width: 66px;
        }
        .style3
        {
            width: 320px;
        }
        .style4
        {
            width: 320px;
            font-weight: bold;
        }
        .style5
        {
            width: 118px;
            font-weight: bold;
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
        .style6
        {
            width: 118px;
            height: 14px;
        }
    </style>
</head>
<body bgcolor="#979797" onload="welcomeUserMessage();">
    <form id="form1" runat="server">
    <div>
    
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
                                            height="100%" width="100%">
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
                                                                    <br />
                                                                    Sessions that you have attended are listed below.
                                                                    <br />
                                                                    <br />
                                                                    <table cellpadding="0" cellspacing="0" class="style1">
                                                                        <tr>
                                                                            <td class="style5">
                                                                                Code</td>
                                                                            <td class="style4">
                                                                                Name</td>
                                                                            <td>
                                                                                <b></b>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="style6">
                                                                                WIN120</td>
                                                                            <td class="style3">
                                                                                Secure Application Development</td>
                                                                            <td>
                                                                                <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
                                                                                    Text="Provide Feedback" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <br />
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
