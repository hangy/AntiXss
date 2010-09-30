<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="Web_Protection_Library_Sample_Web_App.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        About the Microsoft Web Protection Library
    </h2>
    <p>
        The Microsoft Web Protection Library (WPL) is an open source project consisting of a set of .NET assemblies which will help you protect your web sites, current, future and past. The WPL includes
    </p>
    <h3>AntiXSS</h3>
    <p>AntiXSS provides a myriad of encoding functions for user input, including HTML, HTML attributes, XML, CSS and JavaScript.</p>
    <ul>
        <li>White Lists: AntiXSS differs from the standard .NET framework encoding by using a white list approach. All characters not on the white list will be encoded using the correct rules for the encoding type. Whilst this comes at a performance cost AntiXSS has been written with performance in mind.</li>
        <li>Secure Globalization: The web is a global market place, and cross-site scripting is a global issue. An attack can be coded anywhere, and Anti-XSS now protects against XSS attacks coded in dozens of languages.</li>
    </ul>
    <h3>Security Runtime Engine</h3>
    <p>The Security Runtime Engine (SRE) provides a wrapper around your existing web sites, ensuring that common attack vectors to not make it to your application. Protection plug-ins include</p>
    <ul>
        <li>Cross Site Scripting for Web Forms</li>
        <li>SQL Injection</li>
        <li>Credit Card number information leaks</li>
    </ul>

    <p>As with all web security the WPL is part of a defense in depth strategy, adding an extra layer to any validation or secure coding practices you have already adopted.</p>

    <h3>Solid Foundation for Developers</h3>
    <p>No matter your development experience level, the documentation, example code, unit tests, and calling schemes make it easy for you to know how to protect your applications from XSS attacks. Additionally, a performance data sheet helps you plan your secure deployment with full knowledge of how AntiXSS will likely perform in your environment.</p>
    <p>For downloads, documentation and source visit <a href="http://wpl.codeplex.com" title="WPL Website">wpl.codeplex.com</a>.</p>
</asp:Content>
