<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="WebApplication4.AdminDashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Admin Dashboard - Guest House Ordering</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: Arial, Helvetica, sans-serif;
            background: #f5f5f5;
            min-height: 100vh;
        }

        /* Top Header Bar */
        .top-bar {
            background: #28a745;
            padding: 8px 0;
/*            border-bottom: 3px solid #ff9933;*/
        }

        .top-bar-content {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            color: white;
            font-size: 0.85rem;
        }

        /* Main Header */
        .main-header {
            background: white;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            padding: 20px 0;
        }

        .header-content {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 20px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .logo-section {
            display: flex;
            align-items: center;
            gap: 15px;
        }

        
        .site-title {
            color: #003d82;
        }

        .site-title h1 {
            font-size: 1.6rem;
            font-weight: 600;
            margin-bottom: 3px;
        }

        .site-title p {
            font-size: 0.9rem;
            color: #666;
        }

        /* Navigation */
        .nav-bar {
            background: #28a745;
            border-top: 1px solid #003d82;
        }

        .nav-content {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 20px;
        }

        .breadcrumb {
            padding: 12px 0;
            color: white;
            font-size: 0.9rem;
        }

        /* Main Content */
        .container {
            width: 100%;
            max-width: 1200px;
            margin: 40px auto;
            padding: 0 20px;
        }

        .page-title {
            text-align: center;
            margin-bottom: 40px;
        }

        .page-title h2 {
            font-size: 2rem;
            color: #003d82;
            margin-bottom: 8px;
            font-weight: 600;
        }

        .page-title p {
            color: #666;
            font-size: 1rem;
        }

        .dashboard-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 30px;
            padding: 20px 0;
        }

        .dashboard-card {
            background: white;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            padding: 40px 30px;
            text-align: center;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            transition: all 0.3s ease;
            cursor: pointer;
            text-decoration: none;
            color: inherit;
            display: block;
        }

        .dashboard-card:hover {
            border-color: #0056b3;
            box-shadow: 0 4px 12px rgba(0, 86, 179, 0.2);
            transform: translateY(-3px);
        }

        .icon-container {
            width: 90px;
            height: 90px;
            margin: 0 auto 25px;
            background: #0056b3;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .icon-container svg {
            width: 45px;
            height: 45px;
            fill: white;
        }

        .dashboard-card h2 {
            font-size: 1.4rem;
            color: #003d82;
            margin-bottom: 12px;
            font-weight: 600;
        }

        .dashboard-card p {
            color: #555;
            font-size: 0.95rem;
            line-height: 1.6;
        }

        .card-new-order .icon-container {
            background: #28a745;
        }

        .card-new-order:hover {
            border-color: #28a745;
        }

        .card-new-order:hover .icon-container {
            background: #218838;
        }

        .card-view-orders .icon-container {
            background: #0056b3;
        }

        .card-view-orders:hover {
            border-color: #0056b3;
        }

        .card-view-orders:hover .icon-container {
            background: #004494;
        }

        .logout-btn {
            padding: 10px 32px;
            background-color: #dc3545;
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-left: auto;
            margin-right:30px;
        }

        .logout-btn:hover {
            background-color: #c82333;
        }

        /* Footer */
        .footer {
            background: #28a745;
            color: white;
            text-align: center;
            padding: 20px;
            margin-top: 60px;
            font-size: 0.9rem;
        }

        @media (max-width: 768px) {
            .header-content {
                flex-direction: column;
                gap: 15px;
                text-align: center;
            }

            .site-title h1 {
                font-size: 1.3rem;
            }

            .page-title h2 {
                font-size: 1.6rem;
            }

            .dashboard-grid {
                grid-template-columns: 1fr;
                gap: 20px;
            }

            .dashboard-card {
                padding: 30px 20px;
            }

            .top-bar-content {
                flex-direction: column;
                gap: 5px;
                text-align: center;
            }
        }

        @media (max-width: 480px) {
            .site-title h1 {
                font-size: 1.1rem;
            }

            .page-title h2 {
                font-size: 1.4rem;
            }

            .icon-container {
                width: 75px;
                height: 75px;
            }

            .icon-container svg {
                width: 38px;
                height: 38px;
            }

            .dashboard-card h2 {
                font-size: 1.2rem;
            }
        }
        .container{
            flex:1;
        }
        form{
            min-height:100vh;
            flex-direction:column;
            display:flex;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Top Bar -->
        <div class="top-bar">
            <div class="top-bar-content">
                <div>Rashtriya Chemicals and Fertilizers Limited</div>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="logout-btn" OnClick="btnLogout_Click" />
            </div>
        </div>

        <!-- Main Header -->
        <div class="main-header">
            <div class="header-content">
                <div class="logo-section">
                    
                    <div class="site-title">
                        <h1>Guest House Ordering System</h1>
                        <p>Admin Portal</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Navigation Bar -->
        <div class="nav-bar">
            <div class="nav-content">
                <div class="breadcrumb">Home > Admin Dashboard</div>
            </div>
        </div>

        <!-- Main Content -->
        <div class="container">
            <div class="page-title">
                <h2>Admin Dashboard</h2>
                <p>Select an option to proceed</p>
            </div>

            <div class="dashboard-grid">
                <!-- Place New Order Card -->
                <asp:HyperLink ID="lnkPlaceOrder" runat="server" NavigateUrl="~/Foodorders.aspx" CssClass="dashboard-card card-new-order">
                    <div class="icon-container">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                            <path d="M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z"/>
                        </svg>
                    </div>
                    <h2>Place New Order</h2>
                    <p>Fill and submit new orders received from employees</p>
                </asp:HyperLink>

                <!-- View All Orders Card -->
                <asp:HyperLink ID="lnkViewOrders" runat="server" NavigateUrl="~/FoodOrderRecords.aspx" CssClass="dashboard-card card-view-orders">
                    <div class="icon-container">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                            <path d="M9 3h6v2H9zm0 4h6v2H9zm0 4h6v2H9zm0 4h6v2H9zM5 3h2v18H5zm12 0h2v18h-2z"/>
                            <path d="M3 1h18a2 2 0 0 1 2 2v18a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V3a2 2 0 0 1 2-2zm0 2v18h18V3H3z"/>
                        </svg>
                    </div>
                    <h2>View All Orders</h2>
                    <p>Monitor and manage all placed orders </p>
                </asp:HyperLink>
            </div>
        </div>

        <!-- Footer -->
        <div class="footer">
            <p>&copy; 2025 Rashtriya Chemicals and Fertilizers Limited. All rights reserved.</p>
        </div>
    </form>
</body>
</html>
