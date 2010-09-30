<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AutomaticEncoding.aspx.cs" Inherits="Web_Protection_Library_Sample_Web_App.AutomaticEncoding" ValidateRequest="false" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Automatic Encoding
    </h2>
    <p>The WPL comes with a plug-in which inspects WebForms based pages and encodes control properties without the need for user code.
       Try entering a potential XSS string in the input box below. The text is reflected back in the code behind page into a label control,
       without any HTML encoding. The AntiXssPageInspector should encode the text property of the label control and prevent XSS.
    </p>
    <p><asp:Label AssociatedControlID="InputString" Text="Enter XSS Attempt:" runat="server" />&nbsp;
       <asp:TextBox ID="InputString" runat="server"  value="<script>window.alert('Hello XSS');</script>"/><br />
       <asp:Button ID="Submit" Text="Submit" runat="server" />
    </p>
    <asp:Panel ID="Reflected" runat="server" Visible="false">
    <p>You entered <asp:Label ID="ReflectedOutput" runat="server" /></p>
    </asp:Panel>
</asp:Content>
