using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Npgsql;
using System.Data;

namespace Programa_de_Gerenciamento_Industrial
{
    public partial class Form6 : Form
    {
        private readonly string connectionString = "Host=aws-0-sa-east-1.pooler.supabase.com;Port=6543;Username=postgres.stjotefgyhrhlobwldqs;Password=Q9nWPZV8.reuyMC;Database=postgres;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100;";

        public Form6()
        {
            InitializeComponent();
            LoadClientesAsync();
        }

        private async void LoadClientesAsync()
        {
            string query = "SELECT id_cliente, nome FROM cliente";
            await LoadDataAsync(query, dtClientes =>
            {
                comboBox1.DataSource = dtClientes;
                comboBox1.DisplayMember = "nome";
                comboBox1.ValueMember = "id_cliente";
            }, "Erro ao carregar clientes");
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione um cliente.");
                return;
            }

            int idClienteSelecionado = Convert.ToInt32(comboBox1.SelectedValue);
            await LoadLotesAsync(idClienteSelecionado);
        }

        private async Task LoadLotesAsync(int idClienteSelecionado)
        {
            string query = @"
                SELECT l.id_lote, l.nome_lote, l.data_criação, c.nome as cliente, l.status
                FROM cliente_lote cl
                JOIN lotes l ON cl.id_lote = l.id_lote
                JOIN cliente c ON cl.id_cliente = c.id_cliente
                WHERE cl.id_cliente = @id_cliente";

            await LoadDataAsync(query, dtLotes =>
            {
                dataGridView1.DataSource = dtLotes;
            }, "Erro ao carregar lotes: Abra esta tela novamente", new NpgsqlParameter("@id_cliente", idClienteSelecionado));
        }

        private async Task LoadDataAsync(string query, Action<DataTable> setDataSource, string errorMessage, params NpgsqlParameter[] parameters)
        {
            try
            {
                using var con = new NpgsqlConnection(connectionString);
                await con.OpenAsync();
                using var cmd = new NpgsqlCommand(query, con);
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using var da = new NpgsqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                setDataSource(dataTable);
            }
            catch (Exception)
            {
                MessageBox.Show(errorMessage);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            dataGridView1.DataSource = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExportDataToPdf();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ExportDataToExcel();
        }

        private void ExportDataToPdf()
        {
            if (dataGridView1.Rows.Count == 0)
            {
                ShowNoDataWarning();
                return;
            }

            using (var doc = new iTextSharp.text.Document())
            {
                PdfWriter.GetInstance(doc, new FileStream("relatorio.pdf", FileMode.Create));
                doc.Open();
                PdfPTable table = CreatePdfTable();
                doc.Add(table);
                doc.Close();
            }

            MessageBox.Show("Relatório exportado para PDF com sucesso!");
        }

        private void ExportDataToExcel()
        {
            if (dataGridView1.Rows.Count == 0)
            {
                ShowNoDataWarning();
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Relatório");
                FillExcelWorksheet(worksheet);
                workbook.SaveAs("relatorio.xlsx");
            }

            MessageBox.Show("Relatório exportado para Excel com sucesso!");
        }

        private void FillExcelWorksheet(IXLWorksheet worksheet)
        {
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = dataGridView1.Columns[i].HeaderText;
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = dataGridView1.Rows[i].Cells[j].Value?.ToString() ?? string.Empty;
                }
            }
        }

        private PdfPTable CreatePdfTable()
        {
            var table = new PdfPTable(dataGridView1.Columns.Count);
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                table.AddCell(new Phrase(column.HeaderText));
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    table.AddCell(new Phrase(cell.Value?.ToString() ?? string.Empty));
                }
            }

            return table;
        }

        private void ShowNoDataWarning()
        {
            MessageBox.Show("Não há dados para exportar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form6_Load(object sender, EventArgs e) { }
    }
}
