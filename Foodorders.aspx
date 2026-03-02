<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FoodOrders.aspx.cs" Inherits="WebApplication4.Foodorders" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Place Food Order - RCF</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            min-height: 100vh;
            display: flex;
            flex-direction: column;
        }

        /* Top strip */
        .top-green-strip {
            background-color: #28a745;
            padding: 10px 0;
            height:41px;
        }

        .top-strip-content {
            max-width: 1400px;
            margin: auto;
            padding: 0 30px;
            display: flex;
            justify-content: flex-end;
        }

        .btn-logout {
            padding: 8px 22px;
            background-color: #dc3545;
            color: #fff;
            border: none;
            cursor: pointer;
            font-size: 14px;
        }

        /* Header */
        .header {
            background: #fff;
            padding: 20px 0;
            border-bottom: 1px solid #ddd;
        }

        .header-content {
            max-width: 1400px;
            margin: auto;
            text-align: center;
        }

        .header-content img {
            height: 110px;
        }

        /* Navigation Bar */
        .nav-bar {
            background-color: #28a745;
            padding: 0;
        }

        .nav-content {
            max-width: 1400px;
            margin: auto;
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 0;
        }

        .nav-item {
            padding: 12px 30px;
            background-color: transparent;
            color: #fff;
            border: none;
            cursor: pointer;
            font-size: 15px;
            font-weight: 500;
            transition: background-color 0.3s;
            border-right: 1px solid rgba(255,255,255,0.2);
        }

        .nav-item:first-child {
            border-left: 1px solid rgba(255,255,255,0.2);
        }

        .nav-item:hover {
            background-color: #ffc107;
            color: #000;
        }

        .nav-item.active {
            background-color: rgba(0,0,0,0.15);
        }

        /* Content */
        .content {
            max-width: 1400px;
            margin: auto;
            padding: 40px 30px;
            flex: 1;
            width: 100%;
        }

        .page-title {
            font-size: 26px;
            font-weight: 600;
            margin-bottom: 20px;
            border-bottom: 2px solid #28a745;
            padding-bottom: 8px;
        }

        /* Form Box */
        .form-box {
            background: #fff;
            border: 1px solid #ddd;
            padding: 30px;
            margin-bottom: 30px;
        }

        .form-section {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 25px 40px;
        }

        .form-group {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .form-group label {
            font-weight: 500;
            color: #333;
        }

        .form-group input,
        .form-group select,
        .form-group textarea {
            padding: 10px 12px;
            border: 1px solid #ccc;
            font-size: 14px;
            font-family: Arial, sans-serif;
        }

        .form-group input,
        .form-group select {
            height: 44px;
        }

        .form-group textarea {
            min-height: 44px;
            resize: vertical;
        }

        .error-label {
            color: #dc3545;
            font-size: 13px;
            min-height: 18px;
        }

        /* Buttons */
        .button-container {
            margin-top: 30px;
            display: flex;
            gap: 12px;
            justify-content: flex-start;
        }

        .btn-submit {
            padding: 12px 40px;
            background-color: #28a745;
            color: #fff;
            border: none;
            font-weight: 600;
            cursor: pointer;
            font-size: 15px;
            transition: background-color 0.3s;
        }

        .btn-submit:hover {
            background-color: #218838;
        }

        .btn-cancel {
            padding: 12px 40px;
            background-color: #fff;
            color: #333;
            border: 1px solid #ccc;
            font-weight: 600;
            cursor: pointer;
            font-size: 15px;
            transition: all 0.3s;
        }

        .btn-cancel:hover {
            background-color: #f8f9fa;
            border-color: #999;
        }

        /* Footer */
        .footer {
            background-color: #28a745;
            color: #fff;
            text-align: center;
            padding: 14px;
            font-size: 13px;
        }

        /* Responsive */
        @media (max-width: 768px) {
            .nav-content {
                flex-direction: column;
                gap: 0;
            }

            .nav-item {
                width: 100%;
                text-align: center;
                border-right: none;
                border-bottom: 1px solid rgba(255,255,255,0.2);
            }

            .nav-item:first-child {
                border-left: none;
            }

            .form-section {
                grid-template-columns: 1fr;
            }

            .button-container {
                flex-direction: column;
            }

            .btn-submit,
            .btn-cancel {
                width: 100%;
            }
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="top-green-strip">
            <div class="top-strip-content">
               
            </div>
        </div>

        <div class="header">
            <div class="header-content">
                <img src="images/logo.png" alt="RCF Logo" />
            </div>
        </div>

        <!-- Navigation Bar with 3 options -->
        <div class="nav-bar">
            <div class="nav-content">
                <asp:Button ID="btnNavNewOrder" runat="server"
                    Text="Place New Order"
                    CssClass="nav-item active"
                    OnClick="btnNavNewOrder_Click" />
                <asp:Button ID="btnNavViewOrders" runat="server"
                    Text="View Orders"
                    CssClass="nav-item"
                    OnClick="btnViewRecords_Click" />
                <asp:Button ID="btnNavLogout" runat="server"
                    Text="Logout"
                    CssClass="nav-item"
                    OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="content">
            <div class="page-title">Place New Food Order</div>

            <div class="form-box">
                <div class="form-section">
                    
                    <div class="form-group">
                        <label>Ticket Number <span style="color: red;">*</span></label>
                        <asp:TextBox ID="txtTicket" runat="server" />
                        <asp:Label ID="lblTicketError" runat="server" CssClass="error-label" />
                    </div>

                    <div class="form-group">
                        <label>Department <span style="color: red;">*</span></label>
                        <asp:TextBox ID="txtDepartment" runat="server" />
                        <asp:Label ID="lblDepartmentError" runat="server" CssClass="error-label" />
                    </div>

                    <div class="form-group">
                        <label>Meal Category <span style="color: red;">*</span></label>
                        <asp:DropDownList ID="ddlCategory" runat="server" />
                        <asp:Label ID="lblCategoryError" runat="server" CssClass="error-label" />
                    </div>

                    <div class="form-group">
                        <label>Meal Type <span style="color: red;">*</span></label>
                        <asp:DropDownList ID="ddlMealType" runat="server" />
                        <asp:Label ID="lblMealTypeError" runat="server" CssClass="error-label" />
                    </div>

                    <div class="form-group">
                        <label>Quantity <span style="color: red;">*</span></label>
                        <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" />
                        <asp:Label ID="lblQuantityError" runat="server" CssClass="error-label" />
                    </div>

                    <div class="form-group">
                        <label>Order Date <span style="color: red;">*</span></label>
                        <asp:TextBox ID="txtDate" runat="server" TextMode="Date" />
                        <asp:Label ID="lblDateError" runat="server" CssClass="error-label" />
                    </div>

                    <div class="form-group" style="grid-column: 1 / -1;">
                        <label>Add-on / Special Instructions</label>
                        <asp:TextBox ID="txtAddOn" runat="server" TextMode="MultiLine" Rows="3" />
                    </div>

                </div>

                <div class="button-container">
                    <asp:Button ID="btnSubmit" runat="server"
                        Text="Submit Order"
                        CssClass="btn-submit"
                        OnClick="btnSubmit_Click" />
                    <asp:Button ID="btnCancel" runat="server"
                        Text="Cancel"
                        CssClass="btn-cancel"
                        OnClick="btnCancel_Click" />
                </div>

            </div>

        </div>

        <div class="footer">
            © 2026 Rashtriya Chemicals and Fertilizers Limited.
        </div>

    </form>
</body>
</html>
