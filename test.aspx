<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication4.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        body {
            height: 100vh;
            display: flex;
            flex-direction: column;
            align-items: center;
            background: linear-gradient(135deg, #81c784, #4caf50);
            padding-top: 50px;
            animation: formFade 1s ease forwards;
        }

        @keyframes formFade {
            from { opacity: 0; transform: translateY(-20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        img {
            margin-bottom: 30px;
            width: 650px;
            height: 120px;
        }

        form {
            margin-top:60px;
            background: white;
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0 8px 20px rgba(0,0,0,0.2);
            width: 350px;
            text-align: center;
            font-weight: 500;
        }

        h2 {
            margin-bottom: 30px;
            color: #000000;
            font-family: 'Lora', serif;
        }

        .uname {
            text-align: left;
            color: black;
        }

        input[type=text], input[type=password], .loginbtn {
            width: 100%;
            padding: 10px;
            margin-bottom: 20px;
            border-radius: 8px;
            border: 1px solid #ccc;
            font-size: 1rem;
            transition: all 0.5s ease;
        }

        input[type=text]:focus, input[type=password]:focus {
            border-color: #008047;
            box-shadow: 0 0 5px #81c784;
            outline: none;
        }

        .loginbtn {
            background-color: #008047;
            color: white;
            border: none;
            cursor: pointer;
            font-size: 1rem;
            transition: all 0.3s ease;
        }

        .loginbtn:hover {
            background-color: #006622;
            transform: scale(1.05);
        }

        .checkbox {
            font-size: 0.85rem;
            display: flex;
            align-items: center;
            justify-content: flex-start;
            margin-bottom: 10px;
            font-weight: 100;
        }

        .checkbox input {
            margin-right: 5px;
        }
    </style>
    <script type="text/javascript">
        function togglePassword(cb, txtId) {
            var pwd = document.getElementById(txtId);
            if (pwd) pwd.type = cb.checked ? 'text' : 'password';
        }
    </script>
</head>
<body>
    <div>
        <img alt="RCF LOGO" src="Images/logo.png" />
    </div>

    <form id="form1" runat="server">
        <h2>Food Service Order</h2>

        <div class="uname">
            <asp:Label ID="lblUsername" runat="server" Text="Username:" AssociatedControlID="txtEmployeeID"></asp:Label>
            <asp:TextBox ID="txtEmployeeID" runat="server" placeholder="Enter Username" required></asp:TextBox>
        </div>

        <div class="uname">
            <asp:Label ID="lblPassword" runat="server" Text="Password:" AssociatedControlID="txtPassword1"></asp:Label>
            <asp:TextBox ID="txtPassword1" runat="server" TextMode="Password" placeholder="Enter Password" required></asp:TextBox>
        </div>

        <div class="checkbox">
            <input type="checkbox" onclick="togglePassword(this, '<%= txtPassword1.ClientID %>')" /> Show Password
        </div>
        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="loginbtn" OnClick="btnLogin_Click" />
    </form>
</body>
</html>
