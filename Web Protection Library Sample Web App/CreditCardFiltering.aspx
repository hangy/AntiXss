<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreditCardFiltering.aspx.cs" Inherits="Web_Protection_Library_Sample_Web_App.CreditCardFiltering" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Credit Card Response Inspector
    </h2>
    <p>The WPL comes with a plug-in which inspects text based output for credit card numbers.
       Try entering a credit card number in the input box and submitting the request. The text in the box is reflected back in the response,
       and the credit card inspector should stop the response. You can also see
       <a href="Unprotected/CreditCardFiltering.aspx">unprotected version of this page</a>.
    </p>
    <p><asp:Label AssociatedControlID="CreditCardNumber" Text="Enter Credit Card:" runat="server" />&nbsp;
       <asp:TextBox ID="CreditCardNumber" runat="server" ClientIDMode="Static" /><br />
       <asp:Button ID="Submit" Text="Submit" runat="server" />
    </p>
    <asp:Panel ID="Reflected" runat="server" Visible="false">
    <p>You entered <asp:Literal ID="ReflectedOutput" runat="server" /></p>
    </asp:Panel>
    <script type="text/javascript" language="javascript">
        // Setup a test input, chopped up so we don't trigger the filter.
        var ccInputField = document.getElementById('CreditCardNumber');
        if (ccInputField.value == '') {
            ccInputField.value = '4111-';
            ccInputField.value += '1111-';
            ccInputField.value += '1111-';
            ccInputField.value += '1111';
        }
    </script>
</asp:Content>
