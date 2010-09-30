<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SqlInjection.aspx.cs" Inherits="Web_Protection_Library_Sample_Web_App.SqlInjection" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        SQL Injection
    </h2>
    <p>The WPL comes with a plug-in which inspects all inbound parameters for potential SQL injection attacks.
       Try entering a potential SQL Injection string in the input box below (note no database access is performed). 
       The SqlInjectionRequestInspector should detect a possible attack and halt the request.
    </p>
    <p><asp:Label ssociatedControlID="InputString" Text="Enter Sql Injection Attempt:" runat="server" />&nbsp;
       <asp:TextBox ID="InputString" runat="server"  value="' or 1=1--"/><br />
       <asp:Button Text="Submit" runat="server" />
    </p>
</asp:Content>
