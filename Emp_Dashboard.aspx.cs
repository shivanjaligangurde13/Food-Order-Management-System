using System;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class Emp_Dashboard : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["connectionStringTrainee"].ConnectionString;

        public int PageIndex
        {
            get { return ViewState["PageIndex"] != null ? (int)ViewState["PageIndex"] : 0; }
            set { ViewState["PageIndex"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserLoggedIn"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblEmployeeID.Text = Session["UserLoggedIn"].ToString();

                if (Session["UserDepartment"] != null && !string.IsNullOrEmpty(Session["UserDepartment"].ToString()))
                {
                    lblDepartment.Text = Session["UserDepartment"].ToString().ToUpper();
                }
                else
                {
                    string user = lblEmployeeID.Text.ToLower();
                    if (user == "admin" || user == "admin1") lblDepartment.Text = "IT DEPARTMENT";
                    else if (user == "admin2") lblDepartment.Text = "HR DEPARTMENT";
                    else lblDepartment.Text = "GENERAL";
                }

                BindGrid();
            }
        }

        private void BindGrid()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    string dbSearchID = lblEmployeeID.Text;
                    if (dbSearchID.ToLower() == "admin1") dbSearchID = "6001";
                    else if (dbSearchID.ToLower() == "admin") dbSearchID = "123";
                    else if (dbSearchID.ToLower() == "admin2") dbSearchID = "12345";

                    conn.Open();

                    string countSql = "SELECT COUNT(*) FROM FO_FOOD_ORDERS WHERE TICKET_NO = :p_tid OR MODIFIED_BY_IP = :p_tid";
                    OracleCommand countCmd = new OracleCommand(countSql, conn);
                    countCmd.Parameters.Add(new OracleParameter("p_tid", dbSearchID));
                    int totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());

                    string dataSql = @"SELECT * FROM (
                                        SELECT a.*, ROWNUM rnum FROM (
                                            SELECT * FROM FO_FOOD_ORDERS 
                                            WHERE TICKET_NO = :p_tid OR MODIFIED_BY_IP = :p_tid
                                            ORDER BY SR_NO DESC
                                        ) a WHERE ROWNUM <= :p_max
                                       ) WHERE rnum > :p_min";

                    OracleCommand cmd = new OracleCommand(dataSql, conn);
                    cmd.Parameters.Add(new OracleParameter("p_tid", dbSearchID));
                    cmd.Parameters.Add(new OracleParameter("p_max", (PageIndex + 1) * 10));
                    cmd.Parameters.Add(new OracleParameter("p_min", PageIndex * 10));

                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptOrders.DataSource = dt;
                    rptOrders.DataBind();

                    bool showPagination = totalRecords > 10;
                    btnPrev.Visible = showPagination;
                    btnNext.Visible = showPagination;
                    lblPageNumber.Visible = showPagination;

                    if (showPagination)
                    {
                        lblPageNumber.Text = "Page " + (PageIndex + 1);
                        btnPrev.Enabled = (PageIndex > 0);
                        btnNext.Enabled = ((PageIndex + 1) * 10) < totalRecords;
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error loading grid: " + ex.Message);
                }
            }
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            UpdateStatus("APPROVED");
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            UpdateStatus("REJECTED");
        }

        private void UpdateStatus(string status)
        {
            lblStatusMessage.Visible = false;
            bool anySelected = false;
            bool success = false;

            foreach (RepeaterItem item in rptOrders.Items)
            {
                CheckBox chk = (CheckBox)item.FindControl("chkRow");
                if (chk != null && chk.Checked)
                {
                    anySelected = true;
                    Label lblSrNo = (Label)item.FindControl("lblSrNo");
                    TextBox txtRemark = (TextBox)item.FindControl("txtRemark");

                    if (lblSrNo != null && txtRemark != null)
                    {
                        using (OracleConnection conn = new OracleConnection(connString))
                        {
                            string sql = @"UPDATE FO_FOOD_ORDERS 
                                           SET STATUS = :p_status, 
                                               REMARK = :p_remark, 
                                               MODIFIED_DATE_TIME = SYSDATE,
                                               STATUS_UPDATE_DATE_TIME = SYSDATE
                                           WHERE SR_NO = :p_srno";

                            OracleCommand cmd = new OracleCommand(sql, conn);
                            cmd.Parameters.Add(new OracleParameter("p_status", status));
                            cmd.Parameters.Add(new OracleParameter("p_remark", txtRemark.Text.Trim()));
                            cmd.Parameters.Add(new OracleParameter("p_srno", lblSrNo.Text));

                            try
                            {
                                conn.Open();
                                if (cmd.ExecuteNonQuery() > 0) success = true;
                            }
                            catch (Exception ex)
                            {
                                ShowAlert("Update Error: " + ex.Message);
                            }
                        }
                    }
                }
            }

            if (!anySelected)
            {
                ShowAlert("Please select at least one particular order.");
            }
            else if (success)
            {
                // Trigger Swiggy-style modal instead of standard label
                litModalMessage.Text = "The selected orders have been marked as <b>" + status + "</b> successfully.";
                pnlSuccessModal.Visible = true;
                BindGrid();
            }
        }

        private void ShowAlert(string msg)
        {
            lblStatusMessage.Text = msg;
            lblStatusMessage.Visible = true;
            lblStatusMessage.ForeColor = msg.ToLower().Contains("success") ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlSuccessModal.Visible = false;
        }

        protected void Pager_Click(object sender, EventArgs e)
        {
            string direction = ((Button)sender).CommandArgument;
            if (direction == "Next") PageIndex++; else PageIndex--;
            lblStatusMessage.Visible = false;
            BindGrid();
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}