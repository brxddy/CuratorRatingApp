using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CuratorRatingApp.Models
{
    public class DatabaseHelper
    {
        // ВАЖНО: имя базы - curatorragingg (именно так, как вы создали!)
        private string connectionString = @"Server=localhost;Database=curatorragingg;Trusted_Connection=True;Encrypt=False;";

        public List<Curator> GetCurators()
        {
            List<Curator> curators = new List<Curator>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Id, FullName, Department, Rating FROM Curators ORDER BY Rating DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    curators.Add(new Curator
                    {
                        Id = reader.GetInt32(0),
                        FullName = reader.GetString(1),
                        Department = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        Rating = reader.GetInt32(3)
                    });
                }
            }

            return curators;
        }
    }
}