<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="summary.aspx.cs" Inherits="Feedback.summary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>TechEd 2008 - Feedback Summary</title>
    <style type="text/css">

table {font-family:verdana;font-size:11px;line-height:140%;}
.center_orange_top {height:100px;background-image: url('images/orange_C05602.png');
        }


.center_nav {height:19px;background-image: url('images/black_nav_center.png');
background-repeat: repeat-x;
        }
.center_main {border:1px solid #D7D7D7;background-color:#F1F1F3;}


#contentTable {width:90%}
        .style1
        {
            width: 90%;
        }
    input{font-size:9pt;font-family:verdana;}
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
    </style>
</head>
<body bgcolor="#979797">
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
                                                        <table align="center" cellpadding="0" cellspacing="0" class="style1">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;</td>
                                                                <td  align="right" >
                                                                    &nbsp;</td>
                                                            </tr> 
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblFeedback" runat="server" Font-Bold="True" Font-Size="Small" 
                                                                        ForeColor="#0066FF"></asp:Label>
                                                                </td>
                                                                <td  align="right" >
                                                                    <a href="logout.aspx">Logout</a>
                                                                </td>
                                                            </tr> 
                                                        </table> 
                                                        <!-- BEGIN Main Content Area -->
                                                        <table align="center" cellpadding="0" cellspacing="0" class="style1">
                                                            <tr>
                                                                <td>
                                                                    <br />
                                                                    The following feedback is for session code
                                                                    <asp:Label ID="lblSessionName" runat="server" Font-Bold="True"></asp:Label>
                                                                    .&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Repeater ID="repFeedback" runat="server" 
                                                                        OnItemDataBound="repFeedback_ItemDataBound">
                                                                    <ItemTemplate>
                                                                    <p><asp:Label runat="server" ID="CommentsLabel"/> <br />
                                                                        - <i><asp:Label runat="server" ID="NameLabel" /> (<asp:Label runat="server" ID="EmailLabel" />
                                                                        )</i></p>
                                                                    </ItemTemplate>
                                                                    </asp:Repeater>
                                                                    <br />
                                                                    <b>Please provide your feedback:</b> <br/>
                                                                    <table cellpadding="2" id="commentsTable">
                                                                        <tr>
                                                                            <td id="namelabel">
                                                                                Name:
                                                                            </td>
                                                                            <td id="name">
                                                                                <asp:Label ID="lblUser" runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td id="emaillabel">
                                                                                Email Address:</td>
                                                                            <td id="email">
                                                                                <asp:TextBox ID="txtEmail" runat="server" Width="300px"></asp:TextBox>
                                                                            </td>
                                                                            
                                                                        </tr>
                                                                        <tr>
                                                                            <td valign="top" id="commentslabel">
                                                                                Comments:<br />
                                                                            </td>
                                                                            <td style="margin-left: 40px" id="comments">
                                                                                <asp:TextBox ID="txtComments" runat="server" Height="100px" 
                                                                                    TextMode="MultiLine" Width="300px"></asp:TextBox>
                                                                                </td>
                                                                            
                                                                        </tr>
                                                                        <tr>
                                                                            <td valign="top" id="spacer1">
                                                                                &nbsp;</td>
                                                                            <td style="margin-left: 40px" id="spacer2">
                                                                                &nbsp;</td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td valign="top">
                                                                                &nbsp;</td>
                                                                            <td style="margin-left: 40px">
                                                                                <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
                                                                                    onclick="btnSubmit_Click" CausesValidation="true"  />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    &nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    &nbsp;</td>
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
