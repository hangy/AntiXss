<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Web_Protection_Library_Sample_Web_App.DefaultHomePage" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to the Web Protection Library
    </h2>
    <p>
        To learn more about the Web Protection Library visit <a href="http://wpl.codeplex.com" title="WPL Website">wpl.codeplex.com</a>.
    </p>
    <h3>Encoding Methods</h3>
    <p>The AntiXSS Component of the WPL contains numerous a myriad of encoding functions for user input, including HTML, HTML attributes, XML, CSS and JavaScript.</p>
    <ul>
        <li>White Lists: AntiXSS differs from the standard .NET framework encoding by using a white list approach. All characters not on the white list will be encoded using the correct rules for the encoding type. Whilst this comes at a performance cost AntiXSS has been written with performance in mind.</li>
        <li>Secure Globalization: The web is a global market place, and cross-site scripting is a global issue. An attack can be coded anywhere, and Anti-XSS now protects against XSS attacks coded in dozens of languages.</li>
    </ul>
    <p>The following Encoding methods are provided by AntiXSS.</p>
    <ul>
        <li><a href="Encoding\UrlEncoding.aspx">URL Encoding</a></li>    
        <li><a href="Encoding\FormUrlEncoding.aspx">application/x-www-form-urlencoded Encoding</a></li>
    </ul>
    
    <h3>Security Runtime Engine</h3>
    <p>The Security Runtime Engine (SRE) provides a wrapper around your existing web sites, ensuring that common attack vectors to not make it to your application. Protection is provided as standard for</p>
    <ul>
        <li><a href="AutomaticEncoding.aspx">Automatic HTML Encoding</a></li>
        <li><a href="CreditCardFiltering.aspx">Credit Card Filtering</a></li>
        <li><a href="SqlInjection.aspx">SQL Injection</a></li>
    </ul>
</asp:Content>
