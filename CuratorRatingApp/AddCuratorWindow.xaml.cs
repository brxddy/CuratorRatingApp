using System;
using System.Data.SqlClient;
using System.Windows;

namespace CuratorRatingApp
{
    public partial class AddCuratorWindow : Window
    {
        private string connectionString = @"Server=localhost;Database=curatorragingg;Trusted_Connection=True;Encrypt=False;";

        public AddCuratorWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка: ФИО обязательно
            if (string.IsNullOrWhiteSpace(tbFullName.Text))
            {
                ErrorText.Text = "❌ Введите ФИО куратора!";
                return;
            }

            // Проверка: рейтинг - число
            int rating;
            if (!int.TryParse(tbRating.Text, out rating))
            {
                ErrorText.Text = "❌ Рейтинг должен быть числом!";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Curators (FullName, Department, Rating) 
                                    VALUES (@FullName, @Department, @Rating)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FullName", tbFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Department", tbDepartment.Text.Trim());
                    cmd.Parameters.AddWithValue("@Rating", rating);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("✅ Куратор успешно добавлен!", "Успех",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"❌ Ошибка: {ex.Message}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}