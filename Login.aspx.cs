using System;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.DirectoryServices;
using System.Data.OracleClient;

namespace WebApplication4
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GenerateCaptcha();
            }
        }

        private void GenerateCaptcha()
        {
            Random res = new Random();
            string captchaCode = res.Next(10000, 99999).ToString();
            Session["CaptchaCode"] = captchaCode;
            lblCaptcha.Text = captchaCode;
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlModal.Visible = false;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // 1. Captcha Check
            if (Session["CaptchaCode"] == null || txtCaptchaInput.Text.Trim() != Session["CaptchaCode"].ToString())
            {
                ShowAlert("Captcha Invalid!");
                GenerateCaptcha();
                return;
            }

            string username = txtEmployeeID.Text.Trim();
            string password = txtPassword1.Text;
            bool isAuthenticated = false;
            string strError = string.Empty;

            // 2. Hardcoded Check
            string userLower = username.ToLower();
            if ((userLower == "admin" && password == "50000") ||
                (userLower == "admin1" && password == "40000") ||
                (userLower == "admin2" && password == "30000") ||
                (userLower == "admin3" && password == "20000"))
            {
                isAuthenticated = true;
            }

            // 3. AD Check
            if (!isAuthenticated)
            {
                try
                {
                    string domainName = ConfigurationManager.AppSettings["DirectoryDomain"];
                    string adPath = ConfigurationManager.AppSettings["DirectoryPath"];
                    if (!string.IsNullOrEmpty(adPath))
                    {
                        isAuthenticated = AuthenticateUser(domainName, username, password, adPath, out strError);
                    }
                }
                catch { /* AD Silently fails to allow DB fallback */ }
            }

            // 4. DB Check
            if (!isAuthenticated)
            {
                isAuthenticated = CheckDatabaseLogin(username, password);
            }

            // 5. Evaluation
            if (isAuthenticated)
            {
                ExecuteUserRoleLogic(username);
            }
            else
            {
                // Shows dialogue for wrong Username or Password
                ShowAlert("Invalid Credentials! Please check your Username and Password.");
                GenerateCaptcha();
            }
        }

        private bool CheckDatabaseLogin(string username, string password)
        {
            try
            {
                // Check if the connection string entry exists in web.config
                var connSettings = ConfigurationManager.ConnectionStrings["connectionStringTrainee"];
                if (connSettings == null)
                {
                    // Log this internally if needed: "Connection string 'connectionStringEMD' is missing."
                    return false;
                }

                string connString = connSettings.ConnectionString;
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();
                    string sql = "SELECT COUNT(*) FROM FNS_USER_ACCESS WHERE USERNAME = :u AND PASSWORD = :p";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("u", username));
                        cmd.Parameters.Add(new OracleParameter("p", password));
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception)
            {
                // If database is down or table doesn't exist, we return false to trigger the "Invalid Credentials" popup
                return false;
            }
        }

        public bool AuthenticateUser(string domain, string username, string password, string ldapPath, out string Errmsg)
        {
            Errmsg = "";
            string domainAndUsername = domain + @"\" + username;
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry(ldapPath, domainAndUsername, password))
                {
                    object nativeObject = entry.NativeObject;
                    using (DirectorySearcher search = new DirectorySearcher(entry))
                    {
                        search.Filter = $"(SAMAccountName={username})";
                        search.PropertiesToLoad.Add("department");
                        search.PropertiesToLoad.Add("distinguishedName");
                        search.PropertiesToLoad.Add("physicalDeliveryOfficeName");

                        SearchResult result = search.FindOne();
                        if (result != null)
                        {
                            string foundDept = "GENERAL";
                            if (result.Properties.Contains("department"))
                                foundDept = result.Properties["department"][0].ToString();
                            else if (result.Properties.Contains("physicalDeliveryOfficeName"))
                                foundDept = result.Properties["physicalDeliveryOfficeName"][0].ToString();
                            else if (result.Properties.Contains("distinguishedName"))
                            {
                                string dn = result.Properties["distinguishedName"][0].ToString();
                                int start = dn.IndexOf("OU=") + 3;
                                int end = dn.IndexOf(",", start);
                                if (start > 2 && end > start)
                                    foundDept = dn.Substring(start, end - start);
                            }
                            Session["UserDepartment"] = foundDept;
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Errmsg = ex.Message;
                return false;
            }
        }

        private void ExecuteUserRoleLogic(string username)
        {
            try
            {
                var connSettings = ConfigurationManager.ConnectionStrings["connectionStringTrainee"];
                if (connSettings == null)
                {
                    ShowAlert("Configuration Error: Connection string 'connectionStringTrainee' not found.");
                    return;
                }

                using (OracleConnection connection = new OracleConnection(connSettings.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT ROLE FROM FO_USER_ROLES WHERE USERNAME = :uName";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(new OracleParameter("uName", username));

                    object result = command.ExecuteScalar();
                    string role = (result != null) ? result.ToString() : "Employee";

                    // Update or Insert logic
                    if (result != null)
                    {
                        string update = "UPDATE FO_USER_ROLES SET LAST_LOGIN_DATE = SYSDATE WHERE USERNAME = :uName";
                        OracleCommand uCmd = new OracleCommand(update, connection);
                        uCmd.Parameters.Add(new OracleParameter("uName", username));
                        uCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string insert = "INSERT INTO FO_USER_ROLES (USERNAME, ROLE, FIRST_LOGIN_DATE, LAST_LOGIN_DATE) VALUES (:uName, :role, SYSDATE, SYSDATE)";
                        OracleCommand iCmd = new OracleCommand(insert, connection);
                        iCmd.Parameters.Add(new OracleParameter("uName", username));
                        iCmd.Parameters.Add(new OracleParameter("role", role));
                        iCmd.ExecuteNonQuery();
                    }

                    Session["UserLoggedIn"] = username;
                    Session["Role"] = role;

                    if (role == "GuestHouseAdmin")
                        Response.Redirect("FoodOrderRecords.aspx", false);
                    else
                        Response.Redirect("Emp_Dashboard.aspx", false);
                }
            }
            catch (Exception ex)
            {
                // Instead of crashing the page, show the error in your custom dialogue box
                ShowAlert("Login successful, but role assignment failed: " + ex.Message);
            }
        }

        private void ShowAlert(string msg)
        {
            lblModalMessage.Text = msg;
            pnlModal.Visible = true;
        }
    }
}