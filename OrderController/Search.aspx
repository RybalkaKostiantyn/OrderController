<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="OrderController.Search" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div>

            

            <asp:Table ID="TableSearch" runat="server" BorderStyle="Solid" BorderWidth="3px" GridLines="Both">
            </asp:Table>
            <asp:Label ID="lbError" runat="server" Font-Bold="True" Font-Size="XX-Large" Text="Not found"></asp:Label>

            

        </div>
</asp:Content>
        
