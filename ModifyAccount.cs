using Microsoft.ReportingServices.Diagnostics.Internal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSales
{
    internal class ModifyAccount
    {
        SqlCommand sqlCommand;
        SqlDataReader dataReader;
        public List<TaiKhoan> TaiKhoans(string query)
        {
            List<TaiKhoan> taiKhoans = new List<TaiKhoan>(); //Kiểm tra tài khoản
            using (SqlConnection sqlConnection = new SqlConnection(DataProvider.myConnection()))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand(query, sqlConnection);
                dataReader = sqlCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    int id = dataReader.GetInt32(0); // Assuming the first column is an integer
                    string name = dataReader.GetString(1); // Assuming the second column is a string
                    taiKhoans.Add(new TaiKhoan(id.ToString(), name));
                }
                sqlConnection.Close();
            }
            return taiKhoans;
        }
        public void Command(string query)  // Dùng để đăng ký tài khoản
        {
            using (SqlConnection sqlConnection = new SqlConnection(DataProvider.myConnection()))
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }
    }
}
