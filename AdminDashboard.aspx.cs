using System;
using System.Web.UI;

namespace WebApplication4
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: Uncomment and update the login page path when you have a login page
            // Check if admin is logged in
            //if (Session["AdminLoggedIn"] == null || !(bool)Session["AdminLoggedIn"])
            //{
            //    // Redirect to login page if not logged in
            //    Response.Redirect("~/YourLoginPage.aspx");
            //}

            if (!IsPostBack)
            {
                // You can add any initialization code here
                // For example, display admin name if stored in session
                if (Session["AdminName"] != null)
                {
                    // Could display welcome message with admin name
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // TODO: Update this to redirect to your actual login page
            // Response.Redirect("~/YourLoginPage.aspx");

            // For now, just redirect back to this page or home
            Response.Redirect("~/Login.aspx");
        }
    }
}