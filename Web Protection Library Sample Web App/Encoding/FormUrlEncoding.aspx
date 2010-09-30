<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FormUrlEncoding.aspx.cs" Inherits="Web_Protection_Library_Sample_Web_App.Encoding.FormUrlEncoding" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content  ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>application/x-www-form-urlencoded Encoding</h2>
    <p>application/x-www-form-urlencoded Encoding is a method of encoding data for use in HTML form submissions, 
    as specified in <a href="http://tools.ietf.org/html/rfc3986">RFC8986</a> and the 
    <a href="http://www.w3.org/TR/html401/appendix/notes.html#non-ascii-chars">HTML 4.01 specification</a>.
    Form encoding differs subtly from <a href="UrlEncoding.aspx">URL encoding</a> in that it encodes a space character, " " as a plus sign, +.
    Characters such as a question mark (?), ampersand (&amp;), slash mark (/), and spaces might be truncated or corrupted by some browsers. 
    </p>
    <p>There is no .NET framework equivalent of this function.</p>
    <p><asp:Label ID="Label1" AssociatedControlID="QueryData" Text="Enter Query Data:" runat="server" />&nbsp;
       <asp:TextBox ID="QueryData" runat="server" ClientIDMode="Static" /><br />
       <asp:Button ID="Submit" Text="Submit" runat="server" />
    </p>
    <asp:Panel ID="EncodedResult" runat="server" Visible="false">
    <p>The Form URL Encoded string is <asp:Literal ID="EncodedOutput" runat="server" /></p>
    </asp:Panel>
</asp:Content>

