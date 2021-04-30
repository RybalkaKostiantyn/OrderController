<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OrderController._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Orders</h1>

        <asp:TextBox ID="tbSearch" runat="server" Width="200px"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" />
        <br />

        <asp:Label ID="Label1" runat="server" Text="Show on page: "></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged">
                <asp:ListItem>5</asp:ListItem>
                <asp:ListItem>10</asp:ListItem>
                <asp:ListItem>25</asp:ListItem>
            </asp:RadioButtonList>
        <br />
        <asp:Table ID="TableOrders" runat="server" BorderStyle="Solid" BorderWidth="3px" GridLines="Both">
        </asp:Table>
        <br />
        <br />

        <asp:PlaceHolder ID="phOrders" runat="server"></asp:PlaceHolder>
        <br />
        <br />
        <asp:Label ID="lbQuantity" runat="server" Text="Total Quantity: "></asp:Label>
        <br />
        <asp:Label ID="lbAmount" runat="server" Text="Total Amount: "></asp:Label>
    </div>
    

</asp:Content>
