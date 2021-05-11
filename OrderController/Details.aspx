<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="OrderController.Details" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div>

            <asp:Label ID="lbProvider" runat="server" Text="Provider name:"></asp:Label>
            <asp:TextBox ID="tbProvider" runat="server" Width="173px"></asp:TextBox>
            <br />
            <asp:Label ID="lbDescription" runat="server" Text="Description:"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="tbDescription" runat="server" Width="173px"></asp:TextBox>
            <br />
            <asp:Label ID="lbManager" runat="server" Text="Manager:"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="tbManager" runat="server" Width="173px"></asp:TextBox>
            <br />
            <asp:Label ID="lbQuantity" runat="server" Text="Quantity:"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="tbQuantity" runat="server" Width="173px"></asp:TextBox>
            <br />
            <asp:Label ID="lbAmount" runat="server" Text="Amount:"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="tbAmount" runat="server" Width="173px"></asp:TextBox>
            <asp:PlaceHolder ID="phError" runat="server"></asp:PlaceHolder>
            <br />

            <asp:PlaceHolder ID="phOrders" runat="server"></asp:PlaceHolder>
        </div>
</asp:Content>
