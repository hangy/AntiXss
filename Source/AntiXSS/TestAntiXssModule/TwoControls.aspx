<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TwoControls.aspx.cs" Inherits="TestAntiXssModule.TwoControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
        <asp:Button ID="Button1"
            runat="server" Text="Button" onclick="Button1_Click" />
            <asp:Panel ID="Panel1" runat="server">
            </asp:Panel>
    </div>
    </form>
</body>
</html>
