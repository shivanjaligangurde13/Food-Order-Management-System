<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Emp_Dashboard.aspx.cs" Inherits="WebApplication4.Emp_Dashboard" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Your Orders - Food Service</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        body { background-color: #f4f7f6; min-height: 100vh; padding-top: 55px; }
        .header-accent { width: 100%; height: 55px; background: #1AA63A; top: 0; position: fixed; left: 0; z-index: 1001; }
        .logo-wrapper { width: 100%; display: flex; justify-content: center; padding: 20px 0; background: white; }
        .logo-wrapper img { width: 600px; max-width: 90%; height: auto; }
        .nav-bar { width: 100%; height: 45px; background-color: #1AA63A; display: flex; justify-content: center; align-items: center; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }
        .btn-logout-nav { padding: 8px 40px; background-color: #1AA63A; color: #fff !important; border: none; cursor: pointer; font-size: 15px; font-weight: 600; text-decoration: none; }
        .container { width: 98%; max-width: 1400px; margin: 30px auto 80px auto; background: white; padding: 25px; border-radius: 8px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }
        .status-msg { color: #dc3545; font-weight: bold; display: block; margin-bottom: 15px; text-align: center; }
        .info-section { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; padding: 25px; background: #fafafa; border: 1px solid #eee; border-radius: 8px; margin-bottom: 25px; }
        .info-label { font-weight: bold; color: #2e7d32; font-size: 0.8rem; text-transform: uppercase; margin-bottom: 5px; }
        .info-box { background: #fff; padding: 12px; border: 1px solid #ddd; border-radius: 4px; font-weight: 500; }
        .table-wrapper { width: 100%; overflow-x: auto; border: 1px solid #eee; border-radius: 8px; }
        table { width: 100%; border-collapse: collapse; }
        th { background-color: #f8f9fa; color: #2e7d32; padding: 15px; text-align: center; border-bottom: 2px solid #4caf50; }
        td { padding: 12px; text-align: center; border-bottom: 1px solid #f0f0f0; }
        .pager-row { display: flex; justify-content: center; align-items: center; gap: 15px; margin-top: 20px; }
        .btn-pager { padding: 8px 20px; background-color: #1AA63A; color: white; border: none; border-radius: 4px; cursor: pointer; font-weight: bold; }
        .btn-pager:disabled { background-color: #ccc; }
        .footer-accent { width: 100%; height: 40px; background: #1AA63A; position: fixed; bottom: 0; display: flex; justify-content: center; align-items: center; color: white; font-size: 0.8rem; }
        .remark-box { width: 95%; min-width: 200px; padding: 8px; border: 1px solid #ccc; border-radius: 4px; font-size: 14px; color: #333; }
        .remark-box:focus { border-color: #1AA63A; outline: none; box-shadow: 0 0 5px rgba(26, 166, 58, 0.3); }

        /* SWIGGY STYLE MODAL CSS */
        .modal-overlay {
            position: fixed; top: 0; left: 0; width: 100%; height: 100%;
            background: rgba(0,0,0,0.6); display: flex; justify-content: center; align-items: center; z-index: 2000;
        }
        .modal-box {
            background: white; padding: 40px; border-radius: 12px; text-align: center;
            box-shadow: 0 10px 25px rgba(0,0,0,0.2); max-width: 400px; width: 90%;
            border-top: 8px solid #1AA63A;
        }
        .modal-box h2 { color: #1AA63A; margin-bottom: 15px; font-size: 24px; }
        .modal-box p { color: #555; margin-bottom: 25px; line-height: 1.5; font-size: 16px; }
        .btn-modal-close {
            background-color: #1AA63A; color: white; padding: 12px 35px;
            border: none; border-radius: 6px; font-weight: bold; cursor: pointer; text-transform: uppercase;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <asp:Panel ID="pnlSuccessModal" runat="server" Visible="false" CssClass="modal-overlay">
            <div class="modal-box">
                <div style="font-size: 50px; color: #1AA63A; margin-bottom: 10px;">✔</div>
                <h2>Update Successful</h2>
                <p><asp:Literal ID="litModalMessage" runat="server"></asp:Literal></p>
                <asp:Button ID="btnCloseModal" runat="server" Text="CONTINUE" CssClass="btn-modal-close" OnClick="btnCloseModal_Click" />
            </div>
        </asp:Panel>

        <div class="header-accent"></div>
        <div class="logo-wrapper"><img src="Images/logo.png" alt="RCF Logo"/></div>
        <div class="nav-bar">
            <asp:LinkButton ID="lnkLogoutNav" runat="server" CssClass="btn-logout-nav" OnClick="lnkLogout_Click">Logout</asp:LinkButton>
        </div>
        <div class="container">
            <h1>YOUR ORDERS</h1>
           <asp:Label ID="lblStatusMessage" runat="server" Visible="false" Font-Bold="true" CssClass="status-msg"></asp:Label>
            
            <div class="info-section">
                <div class="info-group">
                    <div class="info-label">Username</div>
                    <div class="info-box"><asp:Label ID="lblEmployeeID" runat="server"></asp:Label></div>
                </div>
                <div class="info-group">
                    <div class="info-label">Department</div>
                    <div class="info-box"><asp:Label ID="lblDepartment" runat="server"></asp:Label></div>
                </div>
                <div style="grid-column: 1/-1; text-align:center;">
                    <asp:Button ID="btnAccept" runat="server" Text="APPROVE" OnClick="btnAccept_Click" style="background:#28a745; color:white; padding:12px 35px; border:none; border-radius:5px; cursor:pointer;" />
                    <asp:Button ID="btnReject" runat="server" Text="REJECT" OnClick="btnReject_Click" style="background:#dc3545; color:white; padding:12px 35px; border:none; border-radius:5px; cursor:pointer; margin-left:15px;" />
                </div>
            </div>
            
            <div class="table-wrapper">
                <table>
                    <thead>
                        <tr>
                            <th><input type="checkbox" onclick="toggleCheckboxes(this)" /></th>
                            <th>Sr.No</th><th>Meal Type</th><th>Category</th><th>Date</th><th>Quantity</th><th>Add-Ons</th><th>Remark</th><th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptOrders" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><asp:CheckBox ID="chkRow" runat="server" Enabled='<%# Eval("STATUS").ToString() == "PENDING" %>' /></td>
                                    <td><asp:Label ID="lblSrNo" runat="server" Text='<%# Eval("SR_NO") %>'></asp:Label></td>
                                    <td><%# Eval("MEAL_TYPE") %></td>
                                    <td><%# Eval("MEAL_CATEGORY") %></td>
                                    <td><%# Eval("ORDER_DATE", "{0:dd-MM-yyyy}") %></td>
                                    <td><%# Eval("QUANTITY") %></td>
                                    <td><%# Eval("ADD_ON") %></td>
                                    <td><asp:TextBox placeholder="Enter Remark" ID="txtRemark" runat="server" CssClass="remark-box" Text='<%# Eval("REMARK") %>' Enabled='<%# Eval("STATUS").ToString() == "PENDING" %>'></asp:TextBox></td>
                                    <td><asp:Label ID="lblStat" runat="server" Text='<%# Eval("STATUS") %>' Style='<%# "font-weight:bold; color:" + (Eval("STATUS").ToString() == "APPROVED" ? "green" : (Eval("STATUS").ToString() == "REJECTED" ? "red" : "#ffc107")) %>'></asp:Label></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            
            <div class="pager-row">
                <asp:Button ID="btnPrev" runat="server" Text="Previous" OnClick="Pager_Click" CommandArgument="Prev" CssClass="btn-pager" />
                <asp:Label ID="lblPageNumber" runat="server" style="font-weight:bold;"></asp:Label>
                <asp:Button ID="btnNext" runat="server" Text="Next" OnClick="Pager_Click" CommandArgument="Next" CssClass="btn-pager" />
            </div>
        </div>
        <div class="footer-accent">© 2026 Rashtriya Chemicals and Fertilizers Limited</div>
    </form>
    <script>function toggleCheckboxes(source) { var checkboxes = document.querySelectorAll('input[type="checkbox"]'); checkboxes.forEach(c => { if (c != source && !c.disabled) c.checked = source.checked; }); }</script>
</body>
</html>