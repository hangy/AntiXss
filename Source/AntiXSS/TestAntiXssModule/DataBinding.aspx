<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataBinding.aspx.cs" Inherits="TestAntiXssModule.DataBinding" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <Asp:DataGrid id = "mygrid" runat= "server" CellPadding="4" AutoGenerateColumns="false"
            DataSourceID="SqlDataSource1" ForeColor="#333333" GridLines="None">
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <EditItemStyle BackColor="#2461BF" />
        <SelectedItemStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <AlternatingItemStyle BackColor="White" />
        <ItemStyle BackColor="#EFF3FB" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <Columns>
        <asp:BoundColumn DataField="StartDate" HeaderText="StartDate" SortExpression="StartDate" />
        <asp:BoundColumn DataField="EndDate" HeaderText="EndDate" SortExpression="EndDate" />
        <asp:TemplateColumn HeaderText="MenuType" SortExpression="MenuType">
        <ItemTemplate>
            <asp:Label ID="NameLabel2" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MenuType").ToString()%>' />      
        </ItemTemplate>
        </asp:TemplateColumn>
        </Columns>
        </asp:DataGrid>
        
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        CellPadding="4" DataSourceID="SqlDataSource1" ForeColor="#333333" 
        GridLines="None">
        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
        <Columns>
            <asp:BoundField DataField="StartDate" HeaderText="StartDate" SortExpression="StartDate" />
            <asp:BoundField DataField="MenuType" HeaderText="MenuType" SortExpression="MenuType" />
        </Columns>
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
       </asp:GridView>
<asp:DataList ID="DataList1" runat="server" DataSourceID="SqlDataSource1">
        <EditItemTemplate>
            Edit Item Template<asp:Label ID="Label3" runat="server" 
                Text='<%# Eval("StartDate") %>'></asp:Label>
        </EditItemTemplate>
        <ItemTemplate>
            ID:
            <asp:Literal ID="IDLiteral" runat="server" Text='<%# Eval("StartDate") %>' />
            Name:
            <asp:Label ID="NameLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MenuType").ToString()%>' />      
            <br />

        </ItemTemplate>
    </asp:DataList>
        <asp:ListView ID="ListView1" runat="server" DataSourceID="SqlDataSource1">
         <LayoutTemplate>
    <div runat="server" id="itemPlaceholder" ></div>

        </LayoutTemplate>

        <ItemTemplate>
        <div>
            ID:
            <asp:Label ID="IDLabel10" runat="server" Text='<%# Eval("StartDate") %>' />
            
            Name:
            <asp:Label ID="NameLabel10" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MenuType").ToString()%>' />      
            <br />
</div>
        </ItemTemplate>
        </asp:ListView>
<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
            ConnectionString="server=anilkr3\sqlexpress;Initial Catalog=DinnerNow;Integrated Security=True" 
            SelectCommand="SELECT * FROM [Menu]"></asp:SqlDataSource>

    </div>
    </form>
</body>
</html>
