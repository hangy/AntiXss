<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
CodeBehind="UrlEncoding.aspx.cs" Inherits="Web_Protection_Library_Sample_Web_App.Encoding.UrlEncoding" ValidateRequest="false" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content  ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>URL Encoding</h2>
    <p>URL Encoding is a method of encoding data for use as URL query string parameters, as specified in <a href="http://tools.ietf.org/html/rfc3986">RFC8986</a>.
    URL encoding ensures that all browsers will correctly transmit text in URL strings. 
    Characters such as a question mark (?), ampersand (&amp;), slash mark (/), and spaces might be truncated or corrupted by some browsers. 
    As a result, these characters must be encoded in &lt;a&gt; tags or in query strings where the strings can be re-sent by a browser in a request string.
    </p>
    <p><asp:Label AssociatedControlID="QueryData" Text="Enter Query Data:" runat="server" />&nbsp;
       <asp:TextBox ID="QueryData" runat="server" ClientIDMode="Static" /><br />
       <asp:Button ID="Submit" Text="Submit" runat="server" />
    </p>
    <asp:Panel ID="EncodedResult" runat="server" Visible="false">
    <p>The AntiXSS URL Encoded string is <asp:Literal ID="EncodedOutput" runat="server" /></p>
    <p>The .NET Framework URL Encoded string is <asp:Literal ID="NetEncodedOutput" runat="server" /></p>
    </asp:Panel>
</asp:Content>
