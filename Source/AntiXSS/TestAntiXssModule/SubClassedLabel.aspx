<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubClassedLabel.aspx.cs" Inherits="TestAntiXssModule.SubClassedLabel" %>

<%@ Register assembly="TestAntiXssModule" namespace="TestAntiXssModule" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
<cc1:RedLabel ID="RedLabel1" runat="server"></cc1:RedLabel>
    </div>
    
    </form>
</body>
</html>
