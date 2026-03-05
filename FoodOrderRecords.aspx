<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FoodOrderRecords.aspx.cs" Inherits="WebApplication4.FoodOrderRecords" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Food Order Records - RCF</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: Arial, sans-serif; background-color: #f5f5f5; min-height: 100vh; display: flex; flex-direction: column; }
        .top-green-strip { background-color: #28a745; padding: 10px 0; height:42px; }
        .top-strip-content { max-width: 1400px; margin: auto; padding: 0 30px; display: flex; justify-content: flex-end; }
        .header { background: #fff; padding: 20px 0; border-bottom: 1px solid #ddd; }
        .header-content { max-width: 1400px; margin: auto; text-align: center; }
        .header-content img { height: 110px; }
        .nav-bar { background-color: #28a745; padding: 0; }
        .nav-content { max-width: 1400px; margin: auto; display: flex; justify-content: center; align-items: center; gap: 0; }
        .nav-item { padding: 12px 30px; background-color: transparent; color: #fff; border: none; cursor: pointer; font-size: 15px; font-weight: 500; transition: background-color 0.3s; border-right: 1px solid rgba(255,255,255,0.2); }
        .nav-item:first-child { border-left: 1px solid rgba(255,255,255,0.2); }
        .nav-item:hover { background-color: #ffc107; color: #000; }
        .nav-item.active { background-color: rgba(0,0,0,0.15); }
        .content { max-width: 1400px; margin: auto; padding: 40px 30px; flex: 1; width: 100%; }
        .page-title { font-size: 26px; font-weight: 600; margin-bottom: 20px; border-bottom: 2px solid #28a745; padding-bottom: 8px; }
        .filter-box { background: #fff; border: 1px solid #ddd; padding: 25px; margin-bottom: 30px; }
        .filter-header { margin-bottom: 20px; }
        .filter-header h3 { font-size: 18px; font-weight: 600; color: #333; }
        .filter-section { display: grid; grid-template-columns: repeat(2, 1fr); gap: 20px 40px; }
        .filter-group { display: flex; flex-direction: column; gap: 6px; }
        .filter-group label { font-weight: 500; color: #333; }
        .filter-group input, .filter-group select { padding: 10px 12px; border: 1px solid #ccc; font-size: 14px; height: 44px; }
        .button-container { margin-top: 25px; display: flex; gap: 12px; }
        .btn-search { padding: 10px 36px; background-color: #28a745; color: #fff; border: none; font-weight: 600; cursor: pointer; transition: background-color 0.3s; }
        .btn-search:hover { background-color: #218838; }
        .btn-clear, .btn-download-pdf { padding: 10px 36px; background-color: #fff; color: #333; border: 1px solid #ccc; font-weight: 600; cursor: pointer; transition: all 0.3s; }
        .btn-clear:hover, .btn-download-pdf:hover { background-color: #f8f9fa; border-color: #999; }
        .data-table { width: 100%; border-collapse: collapse; background: #fff; }
        .data-table th, .data-table td { border: 1px solid #ddd; padding: 12px; text-align: left; }
        .data-table th { background: #f8f9fa; font-weight: 600; }
        .status-approved { color: #28a745; font-weight: 600; }
        .status-rejected { color: #dc3545; font-weight: 600; }
        .status-pending { color: #ffc107; font-weight: 600; }
        .pager-row { display: flex; justify-content: center; align-items: center; gap: 15px; margin-top: 20px; margin-bottom:20px; }
        .btn-pager { padding: 8px 20px; background-color: #1AA63A; color: white; border: none; border-radius: 4px; cursor: pointer; font-weight: bold; }
        .btn-pager:disabled { background-color: #ccc; }
        .footer { background-color: #28a745; color: #fff; text-align: center; padding: 14px; font-size: 13px; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <div class="top-green-strip"><div class="top-strip-content"></div></div>
    <div class="header">
        <div class="header-content">
            <img src="images/logo.png" alt="RCF Logo" />
        </div>
    </div>
    <div class="nav-bar">
        <div class="nav-content">
            <asp:Button ID="btnNavNewOrder" runat="server" Text="Place New Order" CssClass="nav-item" OnClick="btnNewOrder_Click" />
            <asp:Button ID="btnNavViewOrders" runat="server" Text="View Orders" CssClass="nav-item active" OnClick="btnViewOrders_Click" />
            <asp:Button ID="btnNavLogout" runat="server" Text="Logout" CssClass="nav-item" OnClick="btnNavLogout_Click" />
        </div>
    </div>
    <div class="content">
        <div class="page-title">Food Order Records</div>
        <div class="filter-box">
            <div class="filter-header"><h3>Filter Orders</h3></div>
            <div class="filter-section">
                <div class="filter-group">
                    <label>Ticket Number</label>
                    <asp:TextBox ID="txtTicketNumber" runat="server" />
                </div>
                <div class="filter-group">
                    <label>Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server">
                        <asp:ListItem Text="All Status" Value="" />
                        <asp:ListItem Text="PENDING" Value="PENDING" />
                        <asp:ListItem Text="APPROVED" Value="APPROVED" />
                        <asp:ListItem Text="REJECTED" Value="REJECTED" />
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label>From Date</label>
                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" />
                </div>
                <div class="filter-group">
                    <label>To Date</label>
                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" />
                </div>
            </div>
            <div class="button-container">
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-search" OnClick="btnSearch_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear Filters" CssClass="btn-clear" OnClick="btnClear_Click" />
                <asp:Button ID="btnDownloadPDF" runat="server" Text="Download PDF" CssClass="btn-download-pdf" OnClick="btnDownloadPDF_Click" />
            </div>
        </div>
        <asp:GridView ID="gvFoodOrders" runat="server" CssClass="data-table" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="SR_NO" HeaderText="Order ID" />
                <asp:BoundField DataField="TICKET_NO" HeaderText="Ticket No" />
                <asp:BoundField DataField="ORDERED_BY" HeaderText="Directive" />
                <asp:BoundField DataField="DEPARTMENT" HeaderText="Department" />
                <asp:BoundField DataField="MEAL_TYPE" HeaderText="Meal Type" />
                <asp:BoundField DataField="MEAL_CATEGORY" HeaderText="Category" />
                <asp:BoundField DataField="QUANTITY" HeaderText="Quantity" />
                <asp:BoundField DataField="ORDER_DATE" HeaderText="Order Date" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:BoundField DataField="ADD_ON" HeaderText="Add-on" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='status-<%# Eval("STATUS").ToString().ToLower() %>'><%# Eval("STATUS") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div class="pager-row">
        <asp:Button ID="btnPrev" runat="server" Text="Previous" OnClick="Pager_Click" CommandArgument="Prev" CssClass="btn-pager" />
        <asp:Label ID="lblPageNumber" runat="server" style="font-weight:bold;"></asp:Label>
        <asp:Button ID="btnNext" runat="server" Text="Next" OnClick="Pager_Click" CommandArgument="Next" CssClass="btn-pager" />
    </div>
    <div class="footer">© 2026 Rashtriya Chemicals and Fertilizers Limited.</div>
</form>
</body>
</html>