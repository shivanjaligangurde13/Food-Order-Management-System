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

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // 1️⃣ Validate Captcha
            if (Session["CaptchaCode"] == null || txtCaptchaInput.Text.Trim() != Session["CaptchaCode"].ToString())
            {
                ShowAlert("Captcha Invalid!");
                GenerateCaptcha();
                return;
            }

            string username = txtEmployeeID.Text.Trim();
            string password = txtPassword1.Text;
            string strError = string.Empty;

            // 2️⃣ Step 1: Check Hardcoded Credentials (Quick bypass for testing)
            bool isAuthenticated = false;
            string userLower = username.ToLower();
            if ((userLower == "admin" && password == "50000") ||
                (userLower == "admin1" && password == "40000") ||
                (userLower == "admin2" && password == "30000") ||
                (userLower == "admin3" && password == "20000"))
            {
                isAuthenticated = true;
            }

            // 3️⃣ Step 2: If not hardcoded, try Active Directory
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
                catch { /* AD error, fallback to DB */ }
            }

            // 4️⃣ Step 3: If still not authenticated, try Oracle Database (FNS_USER_ACCESS)
            if (!isAuthenticated)
            {
                isAuthenticated = CheckDatabaseLogin(username, password);
            }

            // 5️⃣ Final Result
            if (isAuthenticated)
            {
                ExecuteUserRoleLogic(username);
            }
            else
            {
                ShowAlert("Invalid Credentials! Please check your Username and Password.");
                GenerateCaptcha();
            }
        }

        private bool CheckDatabaseLogin(string username, string password)
        {
            string connString = ConfigurationManager.ConnectionStrings["connectionStringEMD"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    conn.Open();
                    // We check against the table you mentioned earlier
                    string sql = "SELECT COUNT(*) FROM FNS_USER_ACCESS WHERE USERNAME = :u AND PASSWORD = :p";
                    OracleCommand cmd = new OracleCommand(sql, conn);
                    cmd.Parameters.Add(new OracleParameter("u", username));
                    cmd.Parameters.Add(new OracleParameter("p", password));
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch { return false; }
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

                        // We load multiple possible attributes where Department/OU might be stored
                        search.PropertiesToLoad.Add("department");
                        search.PropertiesToLoad.Add("distinguishedName");
                        search.PropertiesToLoad.Add("physicalDeliveryOfficeName");

                        SearchResult result = search.FindOne();
                        if (result != null)
                        {
                            string foundDept = "GENERAL";

                            // Priority 1: The Standard Department Field
                            if (result.Properties.Contains("department"))
                            {
                                foundDept = result.Properties["department"][0].ToString();
                            }
                            // Priority 2: The Office Name Field
                            else if (result.Properties.Contains("physicalDeliveryOfficeName"))
                            {
                                foundDept = result.Properties["physicalDeliveryOfficeName"][0].ToString();
                            }
                            // Priority 3: Extracting the OU from the Distinguished Name
                            else if (result.Properties.Contains("distinguishedName"))
                            {
                                string dn = result.Properties["distinguishedName"][0].ToString();
                                // This parses "CN=User,OU=IT_Dept,DC=corp" to get "IT_Dept"
                                int start = dn.IndexOf("OU=") + 3;
                                int end = dn.IndexOf(",", start);
                                if (start > 2 && end > start)
                                {
                                    foundDept = dn.Substring(start, end - start);
                                }
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
            string connString = ConfigurationManager.ConnectionStrings["connectionStringTrainee"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ROLE FROM FO_USER_ROLES WHERE USERNAME = :uName";
                
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(new OracleParameter("uName", username));

                    object result = command.ExecuteScalar();
                    string role = (result != null) ? result.ToString() : "Employee";

                    // Update or Insert logic for tracking logins
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

                    // Redirect logic
                    if (role == "GuestHouseAdmin")
                        Response.Redirect("FoodOrderRecords.aspx", false);
                    else
                        Response.Redirect("Emp_Dashboard.aspx", false);
                }
                catch (Exception ex)
                {
                    ShowAlert("Login Success, but Role Error: " + ex.Message);
                }
            }
        }

        private void ShowAlert(string msg)
        {
            string script = $"alert('{msg}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "UserAlert", script, true);
        }
    }
}