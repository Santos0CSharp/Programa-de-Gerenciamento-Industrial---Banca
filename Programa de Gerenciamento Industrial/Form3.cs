using Npgsql;
using System.Data;
using System.Threading.Tasks;

namespace Programa_de_Gerenciamento_Industrial
{
    public partial class Form3 : Form
    {
        private string connectionString = "Host=aws-0-sa-east-1.pooler.supabase.com;Port=6543;Username=postgres.stjotefgyhrhlobwldqs;Password=Q9nWPZV8.reuyMC;Database=postgres;Timeout=60;";
        private const int delayMilliseconds = 5000; // 5 segundos de atraso

        public Form3()
        {
            InitializeComponent();
        }

        private async void Form3_Load(object sender, EventArgs e)
        {
            dataGridViewLotes.Rows.Clear();
            await CarregarLotesAsync();
        }

        private async Task CarregarLotesAsync()
        {
            string query = "SELECT id_lote, nome_lote AS conteudo, data_criação AS dataCriacao, status, quantidade FROM lotes";

            while (true)
            {
                try
                {
                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        await conn.OpenAsync();
                        using (var cmd = new NpgsqlCommand(query, conn))
                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridViewLotes.DataSource = dt;
                        }
                    }
                    break; // Saia do loop se a operação for bem-sucedida
                }
                catch (NpgsqlException)
                {
                    MessageBox.Show("Aguarde para atualizar novamente.");
                    await Task.Delay(delayMilliseconds); // Espera 5 segundos antes de tentar novamente
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro inesperado: {ex.Message}");
                    break; // Sai do loop se houver uma exceção inesperada
                }
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            dataGridViewLotes.DataSource = null;
            await CarregarLotesAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8();
            form8.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form9 form9 = new Form9();
            form9.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load_1(object sender, EventArgs e)
        {
        }
    }
}
