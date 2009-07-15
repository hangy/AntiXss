<%@ Page Language="C#" ValidateRequest="false" AutoEventWireup="true" CodeBehind="TestMe.aspx.cs" Inherits="AntiXSSTestApp.TestMe" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:TextBox ID="txtBox1" Rows="4" Width="500" TextMode="MultiLine" runat="server"></asp:TextBox><asp:Button ID="btnSubmit" runat="server" Text="Run Test" OnClick="Submit_Click" />
    </div>
    </form>
</body>
</html>
