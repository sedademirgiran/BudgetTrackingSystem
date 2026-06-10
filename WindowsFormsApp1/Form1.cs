using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SqlConnection connection = new SqlConnection(
        @"Data Source=(localdb)\MSSQLLocalDB;
        Initial Catalog=budgetDB;
        Integrated Security=True");

        public Form1()
        {
            InitializeComponent();

            LoadTransactions();
            CalculateSummary();
        }

        private void LoadTransactions()
        {
            SqlDataAdapter adapter = new SqlDataAdapter(
         "SELECT Id AS 'ID', " +
         "TransactionDate AS 'Tarih', " +
         "Type AS 'Tür', " +
         "Category AS 'Kategori', " +
         "Amount AS 'Tutar', " +
         "Description AS 'Açıklama' " +
         "FROM Transactions",
         connection);

            DataTable table = new DataTable();

            adapter.Fill(table);

            dgvTransactions.DataSource = table;

            dgvTransactions.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            if (!dgvTransactions.Columns.Contains("Delete"))
            {
                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();

                btnDelete.Name = "Delete";
                btnDelete.HeaderText = "Sil";
                btnDelete.Text = "SİL";
                btnDelete.UseColumnTextForButtonValue = true;

                dgvTransactions.Columns.Add(btnDelete);
            }

            dgvTransactions.Columns["Delete"].DefaultCellStyle.BackColor = Color.Red;
            dgvTransactions.Columns["Delete"].DefaultCellStyle.ForeColor = Color.White;
        }
        private void CalculateSummary()
        {
            decimal totalIncome = 0;
            decimal totalExpense = 0;

            connection.Open();

            SqlCommand incomeCommand = new SqlCommand(
                "SELECT ISNULL(SUM(Amount),0) FROM Transactions WHERE Type = N'Gelir'",
                connection);

            totalIncome = Convert.ToDecimal(incomeCommand.ExecuteScalar());

            SqlCommand expenseCommand = new SqlCommand(
                "SELECT ISNULL(SUM(Amount),0) FROM Transactions WHERE Type = N'Gider'",
                connection);

            totalExpense = Convert.ToDecimal(expenseCommand.ExecuteScalar());

            connection.Close();

            decimal balance = totalIncome - totalExpense;

            label7.Text = totalIncome.ToString("N2") + " ₺";
            label9.Text = totalExpense.ToString("N2") + " ₺";
            label11.Text = balance.ToString("N2") + " ₺";
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(
                    "INSERT INTO Transactions " +
                    "(TransactionDate, Type, Category, Amount, Description) " +
                    "VALUES (@date,@type,@category,@amount,@description)",
                    connection);

                command.Parameters.AddWithValue(
                    "@date",
                    dtpDate.Value.Date);

                command.Parameters.AddWithValue(
                    "@type",
                    cmbType.Text);

                command.Parameters.AddWithValue(
                    "@category",
                    cmbCategory.Text);

                command.Parameters.AddWithValue(
                    "@amount",
                    decimal.Parse(txtAmount.Text));

                command.Parameters.AddWithValue(
                    "@description",
                    txtDescription.Text);

                command.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Kayıt başarıyla eklendi.");

                LoadTransactions();
                CalculateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 &&
    dgvTransactions.Columns[e.ColumnIndex].Name == "Delete")
            {
                int id = Convert.ToInt32(
                    dgvTransactions.Rows[e.RowIndex].Cells["ID"].Value
                );

                DialogResult result = MessageBox.Show(
                    "Bu kaydı silmek istiyor musunuz?",
                    "Silme Onayı",
                    MessageBoxButtons.YesNo
                );

                if (result == DialogResult.Yes)
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                        "DELETE FROM Transactions WHERE Id=@id",
                        connection
                    );

                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();

                    connection.Close();

                    LoadTransactions();
                    CalculateSummary();

                    MessageBox.Show("Kayıt silindi.");
                }
            }
        }
    }
}
    