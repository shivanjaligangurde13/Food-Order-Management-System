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
        private readonly string connString =
            ConfigurationManager.ConnectionStrings["connectionStringTrainee"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        // ================= NAVIGATION BUTTON EVENTS =================

        protected void btnNavNewOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("FoodOrders.aspx");
        }

        protected void btnViewOrders_Click(object sender, EventArgs e)
        {
            // Already on this page, so just refresh the grid
            BindGrid();
        }

        protected void btnNavLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        // ================= FILTER BUTTON EVENTS =================

        protected void btnNewOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("FoodOrders.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtTicketNumber.Text = "";
            txtFromDate.Text = "";
            txtToDate.Text = "";
            ddlStatus.SelectedIndex = 0;
            BindGrid();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        private int PageSize
        {
            get { return 10; }   // Change number of records per page here
        }

        public int PageIndex
        {
            get { return ViewState["PageIndex"] != null ? (int)ViewState["PageIndex"] : 0; }
            set { ViewState["PageIndex"] = value; }
        }
        protected void Pager_Click(object sender, EventArgs e)
        {
            string direction = ((Button)sender).CommandArgument;

            if (direction == "Next")
                PageIndex++;
            else
                PageIndex--;

            BindGrid();
        }


        // ================= PDF =================

        protected void btnDownloadPDF_Click(object sender, EventArgs e)
        {
            DataTable dt = GetFilteredData();
            dt = dt.AsEnumerable().OrderBy(r => r.Field<DateTime>("ORDER_DATE")).CopyToDataTable();


            if (dt.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "alert", "alert('No records found.');", true);
                return;
            }

            // ================= DATE RANGE =================
            DateTime fromDate;
            DateTime toDate;

            if (!string.IsNullOrEmpty(txtFromDate.Text) && !string.IsNullOrEmpty(txtToDate.Text))
            {
                fromDate = DateTime.Parse(txtFromDate.Text);
                toDate = DateTime.Parse(txtToDate.Text);
            }
            else
            {
                fromDate = dt.AsEnumerable().Min(r => r.Field<DateTime>("ORDER_DATE"));
                toDate = dt.AsEnumerable().Max(r => r.Field<DateTime>("ORDER_DATE"));
            }

            Document document = new Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 25, 25);

            MemoryStream ms = new MemoryStream();
            PdfWriter.GetInstance(document, ms);
            document.Open();

            // ================= FONTS =================
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            // ================= LOGO =================
            string logoPath = Server.MapPath("~/Images/logo.png"); // CHANGE IF NEEDED
            if (File.Exists(logoPath))
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleToFit(500f, 500f);
                logo.Alignment = Element.ALIGN_CENTER;
                logo.SpacingAfter = 8f;
                document.Add(logo);
            }

            // ================= TITLE =================
            Paragraph title = new Paragraph("FOOD ORDER INVOICE", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 12;
            document.Add(title);

            // ================= DATE INFO =================
            PdfPTable infoTable = new PdfPTable(4);
            infoTable.WidthPercentage = 60;
            infoTable.SpacingAfter = 15;

            infoTable.AddCell(new Phrase("From Date:", labelFont));
            infoTable.AddCell(new Phrase(fromDate.ToString("dd-MM-yyyy"), valueFont));
            infoTable.AddCell(new Phrase("To Date:", labelFont));
            infoTable.AddCell(new Phrase(toDate.ToString("dd-MM-yyyy"), valueFont));

            document.Add(infoTable);

            // ================= MAIN TABLE =================
            PdfPTable table = new PdfPTable(9);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 7, 10, 12, 10, 12, 6, 10, 12, 9 });

            string[] headers =
            {
        "Order ID","Ticket No","Department","Meal Type",
        "Category","Quantity","Order Date","Add-on","Status"
    };

            foreach (string h in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(h, headerFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 6
                };
                table.AddCell(cell);
            }

            // ================= DATA ROWS =================
            foreach (DataRow row in dt.Rows)
            {
                string mealType = row["MEAL_TYPE"].ToString();
                string category = row["MEAL_CATEGORY"].ToString();

                mealType = int.TryParse(mealType, out _) ? "-" : mealType;
                category = int.TryParse(category, out _) ? "-" : category;

                table.AddCell(new Phrase(row["SR_NO"].ToString(), cellFont));
                table.AddCell(new Phrase(row["TICKET_NO"].ToString(), cellFont));
                table.AddCell(new Phrase(row["DEPARTMENT"].ToString(), cellFont));
                table.AddCell(new Phrase(mealType, cellFont));
                table.AddCell(new Phrase(category, cellFont));

                table.AddCell(new PdfPCell(new Phrase(row["QUANTITY"].ToString(), cellFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER
                });

                table.AddCell(new Phrase(
                    Convert.ToDateTime(row["ORDER_DATE"]).ToString("dd-MM-yyyy"), cellFont));

                table.AddCell(new Phrase(
                    row["ADD_ON"] == DBNull.Value ? "-" : row["ADD_ON"].ToString(), cellFont));

                PdfPCell statusCell = new PdfPCell(
                    new Phrase(row["STATUS"].ToString().ToUpper(), cellFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(statusCell);
            }

            document.Add(table);

            // ================= FOOTER =================
            Paragraph footer = new Paragraph(
                "\nGenerated on: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
                valueFont);
            footer.Alignment = Element.ALIGN_RIGHT;
            document.Add(footer);

            document.Close();

            byte[] bytes = ms.ToArray();
            ms.Close();

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition",
                "attachment;filename=Food_Order_Invoice.pdf");
            Response.BinaryWrite(bytes);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }




        // ================= DATA =================

        private DataTable GetFilteredData()
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connString))
            {
                string sql = @"
SELECT
    SR_NO,
    TICKET_NO,
    DEPARTMENT,

    CASE 
        WHEN REGEXP_LIKE(MEAL_TYPE, '^[0-9]+$') THEN 'Lunch'
        ELSE MEAL_TYPE
    END AS MEAL_TYPE,

    CASE 
        WHEN REGEXP_LIKE(MEAL_CATEGORY, '^[0-9]+$') THEN 'VIP'
        ELSE MEAL_CATEGORY
    END AS MEAL_CATEGORY,

    QUANTITY,
    ORDER_DATE,
    ADD_ON,
    STATUS
FROM FO_FOOD_ORDERS
WHERE 1=1";

                OracleCommand cmd = new OracleCommand(sql, conn);

                if (!string.IsNullOrEmpty(txtTicketNumber.Text))
                {
                    sql += " AND TICKET_NO = :ticket";
                    cmd.Parameters.Add(":ticket", OracleDbType.Varchar2)
                        .Value = txtTicketNumber.Text.Trim();
                }


                if (!string.IsNullOrEmpty(txtFromDate.Text))
                {
                    sql += " AND ORDER_DATE >= TO_DATE(:fromDate,'YYYY-MM-DD')";
                    cmd.Parameters.Add(":fromDate", OracleDbType.Varchar2)
                        .Value = txtFromDate.Text;
                }

                if (!string.IsNullOrEmpty(txtToDate.Text))
                {
                    sql += " AND ORDER_DATE <= TO_DATE(:toDate,'YYYY-MM-DD')";
                    cmd.Parameters.Add(":toDate", OracleDbType.Varchar2)
                        .Value = txtToDate.Text;
                }

                if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                {
                    sql += " AND STATUS = :status";
                    cmd.Parameters.Add(":status", OracleDbType.Varchar2)
                        .Value = ddlStatus.SelectedValue.ToUpper();
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
                gvFoodOrders.DataSource = null;
                gvFoodOrders.DataBind();

                lblPageNumber.Text = "No Records";
                btnPrev.Enabled = false;
                btnNext.Enabled = false;
                return;
            }

            int totalRecords = dt.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            // Prevent page overflow
            if (PageIndex >= totalPages)
                PageIndex = totalPages - 1;

            if (PageIndex < 0)
                PageIndex = 0;

            int startIndex = PageIndex * PageSize;

            DataTable pagedTable = dt.AsEnumerable()
                .Skip(startIndex)
                .Take(PageSize)
                .CopyToDataTable();

            gvFoodOrders.DataSource = pagedTable;
            gvFoodOrders.DataBind();

            // Update Page Number Label
            lblPageNumber.Text = "Page " + (PageIndex + 1) + " of " + totalPages;

            // Enable / Disable buttons
            btnPrev.Enabled = PageIndex > 0;
            btnNext.Enabled = PageIndex < totalPages - 1;
        }

    }
}