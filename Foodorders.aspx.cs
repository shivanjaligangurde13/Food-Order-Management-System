using System;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class Foodorders : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["connectionStringTrainee"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserLoggedIn"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                txtDate.Text = DateTime.Now.ToString("YYYY-MM-DD");
                LoadMealCategories();
                LoadMealTypes();
            }
        }

        private void LoadMealTypes()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                string sql = "SELECT TYPE_ID, TYPE_NAME FROM FO_MEAL_TYPE WHERE IS_ACTIVE = 'Y' ORDER BY TYPE_NAME";
                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlMealType.DataSource = dt;
                ddlMealType.DataTextField = "TYPE_NAME";
                ddlMealType.DataValueField = "TYPE_ID";
                ddlMealType.DataBind();
            }
            ddlMealType.Items.Insert(0, new ListItem("-- Select Meal Type --", ""));
           
        }

        private void LoadMealCategories()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                string sql = "SELECT CATEGORY_ID, CATEGORY_NAME FROM FO_MEAL_CATEGORY WHERE IS_ACTIVE = 'Y' ORDER BY CATEGORY_NAME";
                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlCategory.DataSource = dt;
                ddlCategory.DataTextField = "CATEGORY_NAME";
                ddlCategory.DataValueField = "CATEGORY_ID";
                ddlCategory.DataBind();
            }
            ddlCategory.Items.Insert(0, new ListItem("-- Select Category --", ""));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                if (SaveOrder())
                {
                    pnlSuccessModal.Visible = true;
                    lblMessage.Visible = false;
                }
            }
        }

        private bool SaveOrder()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    // Note: Ensure your table has a column for the person ordering (e.g., ORDERED_BY)
                    string sql = @"INSERT INTO FO_FOOD_ORDERS
                                   (SR_NO, TICKET_NO, DEPARTMENT, MEAL_CATEGORY, MEAL_TYPE,
                                    QUANTITY, ADD_ON, ORDER_DATE, STATUS, 
                                    MODIFIED_BY_IP, REMARK, SUBMIT_DATE_TIME, ORDERED_BY)
                                   VALUES
                                   (FOOD_ORDER_SEQ.NEXTVAL, :ticket, :dept, :cat, :mtype,
                                    :qty, :addon, TO_DATE(:odate, 'YYYY-MM-DD'),
                                    'PENDING', :ip, '', SYSDATE, :orderedBy)";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("ticket", OracleDbType.Varchar2).Value = txtTicket.Text.Trim();
                        cmd.Parameters.Add("dept", OracleDbType.Varchar2).Value = txtDepartment.Text.Trim();
                        cmd.Parameters.Add("cat", OracleDbType.Varchar2).Value = ddlCategory.SelectedItem.Text;
                        cmd.Parameters.Add("mtype", OracleDbType.Varchar2).Value = ddlMealType.SelectedItem.Text;
                        cmd.Parameters.Add("qty", OracleDbType.Int32).Value = Convert.ToInt32(txtQuantity.Text);
                        cmd.Parameters.Add("addon", OracleDbType.Varchar2).Value = txtAddOn.Text.Trim();
                        cmd.Parameters.Add("odate", OracleDbType.Varchar2).Value = txtDate.Text;
                        cmd.Parameters.Add("ip", OracleDbType.Varchar2).Value = GetUserIP();
                        cmd.Parameters.Add("orderedBy", OracleDbType.Varchar2).Value = txtOrderedBy.Text.Trim();

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Visible = true;
                return false;
            }
        }

        private bool ValidateForm()
        {
            bool valid = true;
            lblOrderedByError.Text = string.IsNullOrEmpty(txtOrderedBy.Text) ? "Name is required" : "";
            lblTicketError.Text = string.IsNullOrEmpty(txtTicket.Text) ? "Ticket number is required" : "";
            lblDepartmentError.Text = string.IsNullOrEmpty(txtDepartment.Text) ? "Department is required" : "";
            lblCategoryError.Text = ddlCategory.SelectedIndex == 0 ? "Please select a category" : "";
            lblMealTypeError.Text = ddlMealType.SelectedIndex == 0 ? "Please select meal type" : "";
            lblQuantityError.Text = string.IsNullOrEmpty(txtQuantity.Text) ? "Quantity is required" : "";
            lblDateError.Text = string.IsNullOrEmpty(txtDate.Text) ? "Date is required" : "";

            if (lblOrderedByError.Text != "" || lblTicketError.Text != "" || lblDepartmentError.Text != "" ||
                lblCategoryError.Text != "" || lblMealTypeError.Text != "" || lblQuantityError.Text != "" || lblDateError.Text != "")
                valid = false;

            return valid;
        }

        private string GetUserIP()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip)) ip = Request.ServerVariables["REMOTE_ADDR"];
            return ip;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("FoodOrderRecords.aspx");
        }

        protected void btnNavNewOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("FoodOrders.aspx");
        }

        protected void btnViewRecords_Click(object sender, EventArgs e)
        {
            Response.Redirect("FoodOrderRecords.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}