using System;
using System.Linq;
using System.Windows;
using CuratorRatingApp.Models;

namespace CuratorRatingApp
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public MainWindow()
        {
            InitializeComponent();
            LoadCurators();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCurators();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCurators();
            StatusText.Text = "✅ Данные обновлены!";
        }

        // Кнопка "Добавить"
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddCuratorWindow();
            addWindow.Owner = this;
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                LoadCurators(); // Обновляем таблицу
                StatusText.Text = "✅ Куратор добавлен!";
            }
        }

        // Кнопка "Удалить"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли куратор
            if (CuratorsGrid.SelectedItem == null)
            {
                MessageBox.Show("❌ Сначала выберите куратора в таблице!",
                                "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем выбранного куратора
            var selected = (Curator)CuratorsGrid.SelectedItem;

            // Спрашиваем подтверждение
            var result = MessageBox.Show($"Удалить куратора \"{selected.FullName}\"?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = new System.Data.SqlClient.SqlConnection(
                        @"Server=localhost;Database=curatorragingg;Trusted_Connection=True;Encrypt=False;"))
                    {
                        conn.Open();
                        string query = "DELETE FROM Curators WHERE Id = @Id";
                        var cmd = new System.Data.SqlClient.SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", selected.Id);
                        cmd.ExecuteNonQuery();
                    }

                    LoadCurators();
                    StatusText.Text = $"✅ Куратор \"{selected.FullName}\" удален!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении:\n{ex.Message}",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Кнопка "Статистика"
        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var curators = dbHelper.GetCurators();

                if (curators.Count == 0)
                {
                    MessageBox.Show("Нет данных для статистики.", "Информация",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int maxRating = curators.Max(c => c.Rating);
                int minRating = curators.Min(c => c.Rating);
                double avgRating = curators.Average(c => c.Rating);
                var bestCurator = curators.First();

                string stats = $"📊 СТАТИСТИКА РЕЙТИНГА\n" +
                               $"{new string('═', 30)}\n" +
                               $"📌 Всего кураторов: {curators.Count}\n" +
                               $"🏆 Максимальный рейтинг: {maxRating}\n" +
                               $"📉 Минимальный рейтинг: {minRating}\n" +
                               $"📊 Средний рейтинг: {avgRating:F1}\n" +
                               $"{new string('═', 30)}\n" +
                               $"🥇 Лучший куратор:\n" +
                               $"   {bestCurator.FullName} ({bestCurator.Rating} баллов)";

                MessageBox.Show(stats, "📊 Статистика", MessageBoxButton.OK, MessageBoxImage.Information);
                StatusText.Text = "📊 Статистика показана!";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подсчете статистики:\n{ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод загрузки данных
        private void LoadCurators()
        {
            try
            {
                var curators = dbHelper.GetCurators();
                CuratorsGrid.ItemsSource = curators;
                this.Title = $"Рейтинг кураторов (всего: {curators.Count})";
                StatusText.Text = $"✅ Загружено {curators.Count} кураторов";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных:\n{ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "❌ Ошибка подключения";
            }
        }
    }
}