<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AntiXSSTestApp._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">

    <title>Untitled Page</title>
    <script language ="javascript" type ="text/javascript" >
    function SHOW()
    {
    var v = <%=note%>
    v = v + 1;
    alert(v);
    }
    
    </script>
    
    <meta http-equiv="Content-Type" content="text/html; charset=Shift-JIS" />
</head>


<body>
    <form id="form1" runat="server">
    <asp:TextBox runat ="server" ID ="txtData"  Text ="<mouse> @#@# " ></asp:TextBox>
    &#x3042;&#x3043;&#x3044;&#x3045;&#x3046;
    Check for Label
    <br />
        <asp:Label runat ="server" ID="lblMessage"  ></asp:Label>
    
    Check for HyperLink
    <br />    
        <a href='#' id="lnkId" runat ="server" ></a>
    
    Check for Link Button &#x10000;
    <br />    
        <asp:LinkButton runat ="server" ID ="lnkbtn" Text ="" ></asp:LinkButton>
    
    Check for Table &#39641&#55364;&#56893;
    <br />    
    <table style ="background-color : Red ;" border = "2" width ="100%" >
    <tr>
    <td>
    &nbsp;
    </td>
    </tr>
    <tr>
    <td>
        <asp:Label runat ="server" ID="Label1"  ></asp:Label>
        <input type ="button" value ="CHECK" onclick ="SHOW()" ></input>
    </td>
    </tr>
      <tr>
    <td>
    &nbsp;
    </td>
    </tr>
    </table>
    <a href="/file.ext?val=%82%A0">Go to second page</a> 
    <br />
    <a href="/file.ext?val=&#12354;">Go to second page UNICODE</a>  
    
    &#97;&#198;&#126;&#219; &#12354; &#X3052;
    
    </form>
</body>
</html>
