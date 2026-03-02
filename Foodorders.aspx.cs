using System;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


namespace WebApplication4
{
    public partial class Foodorders : System.Web.UI.Page
    {
        // Connection string
        string connString = ConfigurationManager
                            .ConnectionStrings["connectionStringTrainee"]
                            .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserLoggedIn"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                txtDate.Text = "";
                LoadMealCategories();
                LoadMealTypes();
            }
        }

        // ================= NAVIGATION BUTTON EVENTS =================

        protected void btnNavNewOrder_Click(object sender, EventArgs e)
        {
            // Already on this page, just refresh
            Response.Redirect("FoodOrders.aspx");
        }

        protected void btnNavViewOrders_Click(object sender, EventArgs e)
        {
            Response.Redirect("FoodOrderRecords.aspx");
        }

        protected void btnNavLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        // ================= FORM EVENTS =================

        // Load meal type 
        private void LoadMealTypes()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                string sql = @"SELECT TYPE_ID, TYPE_NAME
                       FROM FO_MEAL_TYPE
                       WHERE IS_ACTIVE = 'Y'
                       ORDER BY TYPE_NAME";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlMealType.DataSource = dt; //assign data
                        ddlMealType.DataTextField = "TYPE_NAME"; // Lunch / Dinner
                        ddlMealType.DataValueField = "TYPE_ID";  // 1 / 2
                        ddlMealType.DataBind(); //show data
                    }
                }
            }

            ddlMealType.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Meal Type --", ""));
        }

        // Load Meal Category Dropdown
        private void LoadMealCategories()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                string sql = @"SELECT CATEGORY_ID, CATEGORY_NAME
                               FROM FO_MEAL_CATEGORY
                               WHERE IS_ACTIVE = 'Y'
                               ORDER BY CATEGORY_NAME";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlCategory.DataSource = dt;  //take this table as input
                        ddlCategory.DataTextField = "CATEGORY_NAME";
                        ddlCategory.DataValueField = "CATEGORY_ID";
                        ddlCategory.DataBind();
                    }
                }
            }

            ddlCategory.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Category --", ""));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                if (SaveOrder())
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "showalert",
                        "alert('Order Placed Successfully!'); window.location='FoodOrderRecords.aspx';",
                        true
                    );
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("FoodOrderRecords.aspx");
        }

        private DataTable GetFilteredData()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                string sql = @"SELECT 
                        SR_NO,
                        TICKET_NO,
                        DEPARTMENT,
                        MEAL_TYPE,
                        MEAL_CATEGORY,
                        QUANTITY,
                        ORDER_DATE,
                        ADD_ON,
                        STATUS
                       FROM FO_FOOD_ORDERS
                       ORDER BY ORDER_DATE DESC";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }


        private bool ValidateForm()
        {
            bool valid = true;

            lblTicketError.Text = string.IsNullOrEmpty(txtTicket.Text) ? "Ticket number is required" : "";
            lblDepartmentError.Text = string.IsNullOrEmpty(txtDepartment.Text) ? "Department is required" : "";
            lblCategoryError.Text = string.IsNullOrEmpty(ddlCategory.SelectedValue) ? "Please select a category" : "";
            lblMealTypeError.Text = ddlMealType.SelectedIndex == 0 ? "Please select meal type" : "";
            lblQuantityError.Text = string.IsNullOrEmpty(txtQuantity.Text) ? "Quantity is required" : "";
            lblDateError.Text = string.IsNullOrEmpty(txtDate.Text) ? "Date is required" : "";

            if (lblTicketError.Text != "" ||
                lblDepartmentError.Text != "" ||
                lblCategoryError.Text != "" ||
                lblMealTypeError.Text != "" ||
                lblQuantityError.Text != "" ||
                lblDateError.Text != "")
            {
                valid = false;
            }

            return valid;
        }

        protected void btnViewRecords_Click(object sender, EventArgs e)
        {
            Response.Redirect("FoodOrderRecords.aspx");
        }


        private bool SaveOrder()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    string sql = @"INSERT INTO FO_FOOD_ORDERS
                   (SR_NO, TICKET_NO, DEPARTMENT, MEAL_CATEGORY, MEAL_TYPE,
                    QUANTITY, ADD_ON, ORDER_DATE, STATUS, 
                    MODIFIED_BY_IP, REMARK, SUBMIT_DATE_TIME)
                   VALUES
                   (FOOD_ORDER_SEQ.NEXTVAL, :ticket, :dept, :cat, :mtype,
                    :qty, :addon, TO_DATE(:odate, 'YYYY-MM-DD'),
                    'PENDING', :ip, '', SYSDATE)";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(":ticket", OracleDbType.Varchar2)
                            .Value = txtTicket.Text.Trim();

                        cmd.Parameters.Add(":dept", OracleDbType.Varchar2)
                            .Value = txtDepartment.Text.Trim();

                        cmd.Parameters.Add(":cat", OracleDbType.Varchar2)
                            .Value = ddlCategory.SelectedItem.Text.Trim();

                        cmd.Parameters.Add(":mtype", OracleDbType.Varchar2)
                            .Value = ddlMealType.SelectedItem.Text.Trim();

                        cmd.Parameters.Add(":qty", OracleDbType.Int32)
                            .Value = Convert.ToInt32(txtQuantity.Text.Trim());

                        cmd.Parameters.Add(":addon", OracleDbType.Varchar2)
                            .Value = txtAddOn.Text.Trim();

                        cmd.Parameters.Add(":odate", OracleDbType.Varchar2)
                            .Value = txtDate.Text;

                        cmd.Parameters.Add(":ip", OracleDbType.Varchar2)
                            .Value = GetUserIP();

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "alert",
                    "alert('Error saving order: " + ex.Message.Replace("'", "\\'") + "');",
                    true
                );
                return false;
            }
        }



        private string GetUserIP()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = Request.ServerVariables["REMOTE_ADDR"];

            return ip;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

    }
}