<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FoodOrders.aspx.cs" Inherits="WebApplication4.Foodorders" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Place Food Order - RCF</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: Arial, sans-serif; background-color: #f5f5f5; min-height: 100vh; display: flex; flex-direction: column; }

        .top-green-strip { background-color: #28a745; padding: 10px 0; height:41px; }
        .top-strip-content { max-width: 1400px; margin: auto; padding: 0 30px; display: flex; justify-content: flex-end; }
        .btn-logout { padding: 8px 22px; background-color: #dc3545; color: #fff; border: none; cursor: pointer; font-size: 14px; }
        .header { background: #fff; padding: 20px 0; border-bottom: 1px solid #ddd; }
        .header-content { max-width: 1400px; margin: auto; text-align: center; }
        .header-content img { height: 110px; }
        .nav-bar { background-color: #28a745; padding: 0; }
        .nav-content { max-width: 1400px; margin: auto; display: flex; justify-content: center; align-items: center; gap: 0; }
        .nav-item { padding: 12px 30px; background-color: transparent; color: #fff; border: none; cursor: pointer; font-size: 15px; font-weight: 500; transition: background-color 0.3s; border-right: 1px solid rgba(255,255,255,0.2); }
        .nav-item:hover { background-color: #ffc107; color: #000; }
        .nav-item.active { background-color: rgba(0,0,0,0.15); }
        .content { max-width: 1400px; margin: auto; padding: 40px 30px; flex: 1; width: 100%; }
        .page-title { font-size: 26px; font-weight: 600; margin-bottom: 20px; border-bottom: 2px solid #28a745; padding-bottom: 8px; }
        .form-box { background: #fff; border: 1px solid #ddd; padding: 30px; margin-bottom: 30px; }
        .form-section { display: grid; grid-template-columns: repeat(2, 1fr); gap: 25px 40px; }
        .form-group { display: flex; flex-direction: column; gap: 6px; }
        .form-group label { font-weight: 500; color: #333; }
        .form-group input, .form-group select, .form-group textarea { padding: 10px 12px; border: 1px solid #ccc; font-size: 14px; }
        .form-group input, .form-group select { height: 44px; }
        .error-label { color: #dc3545; font-size: 13px; min-height: 18px; }
        .button-container { margin-top: 30px; display: flex; gap: 12px; }
        .btn-submit { padding: 12px 40px; background-color: #28a745; color: #fff; border: none; font-weight: 600; cursor: pointer; }
        .btn-cancel { padding: 12px 40px; background-color: #fff; color: #333; border: 1px solid #ccc; font-weight: 600; cursor: pointer; }
        .footer { background-color: #28a745; color: #fff; text-align: center; padding: 14px; font-size: 13px; }

        .modal-overlay {
            position: fixed; top: 0; left: 0; width: 100%; height: 100%;
            background: rgba(0,0,0,0.6); display: flex; justify-content: center; align-items: center; z-index: 1000;
        }
        .modal-box {
            background: white; padding: 40px; border-radius: 12px; text-align: center;
            box-shadow: 0 10px 25px rgba(0,0,0,0.2); max-width: 400px; width: 90%;
            border-top: 8px solid #28a745;
        }
        .modal-box h2 { color: #28a745; margin-bottom: 15px; font-size: 24px; }
        .modal-box p { color: #555; margin-bottom: 25px; line-height: 1.5; }
        .btn-modal-close {
            background-color: #28a745; color: white; padding: 12px 30px;
            border: none; border-radius: 6px; font-weight: bold; cursor: pointer;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <asp:Panel ID="pnlSuccessModal" runat="server" Visible="false" CssClass="modal-overlay">
            <div class="modal-box">
                <div style="font-size: 50px; color: #28a745; margin-bottom: 10px;">✔</div>
                <h2>Order Placed!</h2>
                <p>Your meal request has been recorded successfully. You can track the status in the records section.</p>
                <asp:Button ID="btnCloseModal" runat="server" Text="OK, GOT IT" CssClass="btn-modal-close" OnClick="btnCancel_Click" />
            </div>
        </asp:Panel>

        <div class="top-green-strip">
            <div class="top-strip-content"></div>
        </div>

        <div class="header">
            <div class="header-content">
                <img src="images/logo.png" alt="RCF Logo" />
            </div>
        </div>

        <div class="nav-bar">
            <div class="nav-content">
                <asp:Button ID="btnNavNewOrder" runat="server" Text="Place New Order" CssClass="nav-item active" OnClick="btnNavNewOrder_Click" />
                <asp:Button ID="btnNavViewOrders" runat="server" Text="View Orders" CssClass="nav-item" OnClick="btnViewRecords_Click" />
                <asp:Button ID="btnNavLogout" runat="server" Text="Logout" CssClass="nav-item" OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="content">
            <div class="page-title">Place New Food Order</div>
            <asp:Label ID="lblMessage" runat="server" Visible="false" Font-Bold="true"></asp:Label>
            
            <div class="form-box">
                <div class="form-section">
                    <div class="form-group">
                        <label>Directive <span style="color: red;">*</span></label>
                        <asp:TextBox ID="txtOrderedBy" runat="server" />
                        <asp:Label ID="lblOrderedByError" runat="server" CssClass="error-label" />
                    </div>
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
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Order" CssClass="btn-submit" OnClick="btnSubmit_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-cancel" OnClick="btnCancel_Click" />
                </div>
            </div>
        </div>

        <div class="footer">
            © 2026 Rashtriya Chemicals and Fertilizers Limited.
        </div>
    </form>
</body>
</html>