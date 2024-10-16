using Npgsql;
using System.Threading.Tasks;

namespace Programa_de_Gerenciamento_Industrial
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string nomeLote = textBox1.Text;
            DateTime dataCriacao = dateTimePicker1.Value;
            string status = textBox3.Text;
            int quantidade = Convert.ToInt32(textBox2.Text);

            string connectionString = "Host=aws-0-sa-east-1.pooler.supabase.com;Port=6543;Username=postgres.stjotefgyhrhlobwldqs;Password=Q9nWPZV8.reuyMC;Database=postgres";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "INSERT INTO lotes (nome_lote, data_criação, status, quantidade) VALUES (@nomeLote, @dataCriacao, @status, @quantidade)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nomeLote", nomeLote);
                        cmd.Parameters.AddWithValue("@dataCriacao", dataCriacao);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@quantidade", quantidade);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    MessageBox.Show("Lote adicionado com sucesso!");
                    this.Close();
                }
                catch (NpgsqlException)
                {
                    MessageBox.Show("Erro ao adicionar lote. Aguarde um momento e tente novamente.");
                    await Task.Delay(5000); // Aguarda 5 segundos antes de permitir nova tentativa
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
        }
    }
}
