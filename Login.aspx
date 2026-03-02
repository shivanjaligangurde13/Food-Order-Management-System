<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication4.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; font-family: 'Segoe UI', sans-serif; }
        
        body {
            height: 100vh; 
            display: flex; 
            flex-direction: column; 
            align-items: center;
            background: linear-gradient(135deg, #f5f7f5, #e8f5e9); /* Softened background to make the green lines pop */
            overflow: hidden; 
        }

        /* NEW: TOP GREEN STRIP */
        .top-green-strip { 
            width: 100%;
            height: 48px; 
            background-color: #1AA63A; 
            display: flex; 
            align-items: center; 
            padding: 0 25px;
            color: white;
            font-size: 16px;
            font-weight: 500;
        }

        /* Logo Header */
        .logo-header {
            width: 600px; 
            max-width: 90%;
            height: auto;
            margin: 20px 0;
            display: block;
        }

        /* NEW: 0.5CM GREEN LINE BELOW LOGO */
        .logo-underline {
            width: 100%;
            height: 0.6cm;
            background-color: #1AA63A;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        /* Form Centering Wrapper */
        .form-wrapper {
            flex: 1; 
            display: flex;
            align-items: center; 
            justify-content: center;
            width: 100%;
        }

        form {
            background: white; 
            padding: 40px; 
            border-radius: 15px;
            box-shadow: 0 8px 30px rgba(0,0,0,0.15); 
            width: 380px; 
            text-align: center;
            border-top: 4px solid #1aa63a; /* Matches the top strip */
        }

        h2 { margin-bottom: 25px; color: #333; font-weight: 600; }
        .uname { text-align: left; color: #444; margin-bottom: 15px; }
        .uname label { font-size: 0.9rem; font-weight: 600; display: block; margin-bottom: 5px; }

        input[type=text], input[type=password], .loginbtn {
            width: 100%; padding: 12px; border-radius: 8px; border: 1px solid #ddd; font-size: 1rem;
        }

        .captcha-container {
            display: flex; align-items: center; justify-content: space-between;
            margin-bottom: 15px; background: #f9f9f9; padding: 10px; border-radius: 8px;
            border: 1px dashed #4caf50;
        }

        .captcha-image {
            font-weight: bold; font-style: italic; font-size: 1.4rem; color: #2e7d32;
            letter-spacing: 5px; user-select: none; background: #e8f5e9; padding: 5px 15px;
        }

        .refresh-btn { cursor: pointer; color: #1aa63a; font-size: 0.85rem; text-decoration: none; font-weight: bold; }
        .refresh-btn:hover { text-decoration: underline; }
        
        .loginbtn { 
            background-color: #1aa63a; color: white; border: none; cursor: pointer; 
            transition: 0.3s; margin-top: 10px; font-weight: 600;
        }
        .loginbtn:hover { background-color: #148a30; transform: translateY(-1px); }

        /* --- STABLE FOOTER --- */
        .footer-wrapper {
            width: 100%;
            height: 40px;
            background: #1AA63A;
            position: fixed;
            bottom: 0;
            left: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            box-shadow: 0 -2px 10px rgba(0,0,0,0.1);
        }

        .footer-text {
            color: white;
            font-size: 0.85rem;
        }
    </style>
</head>
<body>
    <div class="top-green-strip">
      
    </div>

    <img alt="RCF LOGO" src="Images/logo.png" class="logo-header" />

    <div class="logo-underline"></div>

    <div class="form-wrapper">
        <form id="form1" runat="server">
            <h2>Food Service Login</h2>

            <div class="uname">
                <asp:Label runat="server" AssociatedControlID="txtEmployeeID">Username</asp:Label>
                <asp:TextBox ID="txtEmployeeID" runat="server" placeholder="Enter Username" required></asp:TextBox>
            </div>

            <div class="uname">
                <asp:Label runat="server" AssociatedControlID="txtPassword1">Password</asp:Label>
                <asp:TextBox ID="txtPassword1" runat="server" TextMode="Password" placeholder="Enter Password" required></asp:TextBox>
            </div>

            <div class="captcha-container">
                <asp:Label ID="lblCaptcha" runat="server" CssClass="captcha-image"></asp:Label>
                <asp:LinkButton ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" CausesValidation="false" CssClass="refresh-btn">Refresh</asp:LinkButton>
            </div>

            <div class="uname">
                <asp:Label runat="server" AssociatedControlID="txtCaptchaInput">Captcha Verification</asp:Label>
                <asp:TextBox ID="txtCaptchaInput" runat="server" placeholder="Enter code above" required></asp:TextBox>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="loginbtn" OnClick="btnLogin_Click" />
        </form>
    </div>

    <div class="footer-wrapper">
        <div class="footer-text">© 2026 Rashtriya Chemicals and Fertilizers Limited.</div>
    </div>
</body>
</html>