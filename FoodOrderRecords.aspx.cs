using iTextSharp.text;
using iTextSharp.text.pdf;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication4
{
    public partial class FoodOrderRecords : Page
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["connectionStringTrainee"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { BindGrid(); }
        }

        protected void btnNavNewOrder_Click(object sender, EventArgs e) { Response.Redirect("FoodOrders.aspx"); }
        protected void btnViewOrders_Click(object sender, EventArgs e) { BindGrid(); }
        protected void btnNavLogout_Click(object sender, EventArgs e) { Session.Clear(); Session.Abandon(); Response.Redirect("Login.aspx"); }
        protected void btnNewOrder_Click(object sender, EventArgs e) { Response.Redirect("FoodOrders.aspx"); }
        protected void btnSearch_Click(object sender, EventArgs e) { PageIndex = 0; BindGrid(); }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtTicketNumber.Text = ""; txtFromDate.Text = ""; txtToDate.Text = ""; ddlStatus.SelectedIndex = 0; PageIndex = 0; BindGrid();
        }

        private int PageSize { get { return 10; } }
        public int PageIndex
        {
            get { return ViewState["PageIndex"] != null ? (int)ViewState["PageIndex"] : 0; }
            set { ViewState["PageIndex"] = value; }
        }

        protected void Pager_Click(object sender, EventArgs e)
        {
            string direction = ((Button)sender).CommandArgument;
            if (direction == "Next") PageIndex++; else PageIndex--;
            BindGrid();
        }

        protected void btnDownloadPDF_Click(object sender, EventArgs e)
        {
            DataTable dt = GetFilteredData();
            if (dt.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No records found.');", true);
                return;
            }

            // ORDER BY NEW ORDER FIRST (Newest Sr No at the top)
            dt = dt.AsEnumerable().OrderByDescending(r => Convert.ToInt32(r["SR_NO"])).CopyToDataTable();

            DateTime fromDate = !string.IsNullOrEmpty(txtFromDate.Text) ? DateTime.Parse(txtFromDate.Text) : dt.AsEnumerable().Min(r => Convert.ToDateTime(r["ORDER_DATE"]));
            DateTime toDate = !string.IsNullOrEmpty(txtToDate.Text) ? DateTime.Parse(txtToDate.Text) : dt.AsEnumerable().Max(r => Convert.ToDateTime(r["ORDER_DATE"]));

            Document document = new Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 25, 25);
            MemoryStream ms = new MemoryStream();
            PdfWriter.GetInstance(document, ms);
            document.Open();

            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
            Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

            string logoPath = Server.MapPath("~/Images/logo.png");
            if (File.Exists(logoPath))
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleToFit(500f, 500f);
                logo.Alignment = Element.ALIGN_CENTER;
                document.Add(logo);
            }

            Paragraph title = new Paragraph("FOOD ORDER INVOICE", titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 12 };
            document.Add(title);

            PdfPTable infoTable = new PdfPTable(4) { WidthPercentage = 60, SpacingAfter = 15 };
            infoTable.AddCell(new Phrase("From Date:", labelFont));
            infoTable.AddCell(new Phrase(fromDate.ToString("dd-MM-yyyy"), valueFont));
            infoTable.AddCell(new Phrase("To Date:", labelFont));
            infoTable.AddCell(new Phrase(toDate.ToString("dd-MM-yyyy"), valueFont));
            document.Add(infoTable);

            PdfPTable table = new PdfPTable(10) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 6, 8, 12, 10, 10, 10, 6, 9, 11, 8 });

            string[] headers = { "Order ID", "Ticket No", "Directive", "Dept", "Meal Type", "Category", "Quantity", "Date", "Add-on", "Status" };
            foreach (string h in headers)
            {
                table.AddCell(new PdfPCell(new Phrase(h, headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = Color.LIGHT_GRAY });
            }

            foreach (DataRow row in dt.Rows)
            {
                table.AddCell(new Phrase(row["SR_NO"].ToString(), cellFont));
                table.AddCell(new Phrase(row["TICKET_NO"].ToString(), cellFont));
                table.AddCell(new Phrase(row["ORDERED_BY"].ToString(), cellFont)); // Directive Column
                table.AddCell(new Phrase(row["DEPARTMENT"].ToString(), cellFont));
                table.AddCell(new Phrase(row["MEAL_TYPE"].ToString(), cellFont));
                table.AddCell(new Phrase(row["MEAL_CATEGORY"].ToString(), cellFont));
                table.AddCell(new PdfPCell(new Phrase(row["QUANTITY"].ToString(), cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new Phrase(Convert.ToDateTime(row["ORDER_DATE"]).ToString("dd-MM-yyyy"), cellFont));
                table.AddCell(new Phrase(row["ADD_ON"] == DBNull.Value ? "-" : row["ADD_ON"].ToString(), cellFont));
                table.AddCell(new Phrase(row["STATUS"].ToString(), cellFont));
            }

            document.Add(table);
            document.Add(new Paragraph("\nGenerated on: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"), valueFont) { Alignment = Element.ALIGN_RIGHT });
            document.Close();

            byte[] bytes = ms.ToArray();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment;filename=Food_Order_Invoice.pdf");
            Response.BinaryWrite(bytes);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private DataTable GetFilteredData()
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(connString))
            {
                string sql = @"
                    SELECT SR_NO, TICKET_NO, ORDERED_BY, DEPARTMENT, 
                    CASE WHEN REGEXP_LIKE(MEAL_TYPE, '^[0-9]+$') THEN 'Lunch' ELSE MEAL_TYPE END AS MEAL_TYPE,
                    CASE WHEN REGEXP_LIKE(MEAL_CATEGORY, '^[0-9]+$') THEN 'VIP' ELSE MEAL_CATEGORY END AS MEAL_CATEGORY,
                    QUANTITY, ORDER_DATE, ADD_ON, STATUS
                    FROM FO_FOOD_ORDERS WHERE 1=1";

                OracleCommand cmd = new OracleCommand("", conn);
                if (!string.IsNullOrEmpty(txtTicketNumber.Text))
                {
                    sql += " AND TICKET_NO = :ticket";
                    cmd.Parameters.Add(":ticket", OracleDbType.Varchar2).Value = txtTicketNumber.Text.Trim();
                }
                if (!string.IsNullOrEmpty(txtFromDate.Text))
                {
                    sql += " AND ORDER_DATE >= TO_DATE(:fromDate,'YYYY-MM-DD')";
                    cmd.Parameters.Add(":fromDate", OracleDbType.Varchar2).Value = txtFromDate.Text;
                }
                if (!string.IsNullOrEmpty(txtToDate.Text))
                {
                    sql += " AND ORDER_DATE <= TO_DATE(:toDate,'YYYY-MM-DD')";
                    cmd.Parameters.Add(":toDate", OracleDbType.Varchar2).Value = txtToDate.Text;
                }
                if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                {
                    sql += " AND STATUS = :status";
                    cmd.Parameters.Add(":status", OracleDbType.Varchar2).Value = ddlStatus.SelectedValue.ToUpper();
                }
                sql += " ORDER BY SR_NO DESC";
                cmd.CommandText = sql;
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void BindGrid()
        {
            DataTable dt = GetFilteredData();
            if (dt.Rows.Count == 0)
            {
                gvFoodOrders.DataSource = null; gvFoodOrders.DataBind();
                lblPageNumber.Text = "No Records"; btnPrev.Enabled = false; btnNext.Enabled = false; return;
            }
            int totalPages = (int)Math.Ceiling((double)dt.Rows.Count / PageSize);
            if (PageIndex >= totalPages) PageIndex = totalPages - 1;
            if (PageIndex < 0) PageIndex = 0;
            gvFoodOrders.DataSource = dt.AsEnumerable().Skip(PageIndex * PageSize).Take(PageSize).CopyToDataTable();
            gvFoodOrders.DataBind();
            lblPageNumber.Text = "Page " + (PageIndex + 1) + " of " + totalPages;
            btnPrev.Enabled = PageIndex > 0;
            btnNext.Enabled = PageIndex < totalPages - 1;
        }
    }
}