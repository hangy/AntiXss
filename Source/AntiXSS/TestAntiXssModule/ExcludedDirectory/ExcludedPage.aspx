<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExcludedPage.aspx.cs" Inherits="TestAntiXssModule.ExcludedPage1" validateRequest="false"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Anti XSS Module Test Page</title>
</head>
<body bgcolor="#ccffff">
    <form id="form1" runat="server">
    <div>
        Welcome back <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>!
    </div>
    </form>
</body>
</html>
