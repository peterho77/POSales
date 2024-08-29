using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSales
{
    internal class DataProvider
    {
        
         //Lấy đường dẫn đến cơ sở dữ liệu
        static private string connectionstr;
        static public string myConnection()
        {
            connectionstr = @"Data Source=LAPTOP-99SD070M\CHITRONG;Initial Catalog=Supermarket_Management;Integrated Security=True";
            return connectionstr; 
        }
        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(myConnection());
        }

        public DataTable LoadCSDL(string sql)    // Hàm đọc cơ sở dữ liệu 
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = GetSqlConnection())
            {
                using (SqlCommand cm = new SqlCommand(sql, cn))
                {
                    try
                    {
                        cn.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cm))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return dt;
        }

        public DataSet LoadCSDLDTS(string sql)  // Hàm thực hiện câu lệnh SQL với các thông số được thêm trực tiếp vào câu lệnh
        {
            DataSet dts = new DataSet();
            using (SqlConnection cn = GetSqlConnection())
            {
                using (SqlCommand cm = new SqlCommand(sql, cn))
                {
                    try
                    {
                        cn.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cm))
                        {
                            da.Fill(dts);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return dts;
        }

        public int Change(string sql)
        {
            int kq = 0;
            using (SqlConnection cn = GetSqlConnection())
            {
                using (SqlCommand cm = new SqlCommand(sql, cn))
                {
                    try
                    {
                        cn.Open();
                        kq = cm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return kq;
        }
        static public DataTable getTable(string query) // Hàm đưa dữ liệu từ vào datatable mục đích đưa vào combobox
        {
            SqlConnection connection = new SqlConnection(connectionstr);
            SqlCommand command = new SqlCommand(query, connection);

            connection = new SqlConnection(myConnection());

            connection.Open();

            command = new SqlCommand(query, connection);

            SqlDataAdapter adapter = new SqlDataAdapter(command);

            DataTable table = new DataTable();

            adapter.Fill(table);

            connection.Close();

            return table;
        }

        static public string getPassword(string username)   // Hàm lấy mật khẩu từ bảng tài khoản CSDL
        {
            string password = "";
            SqlConnection connection = new SqlConnection(connectionstr);
            connection.Open();
            SqlCommand command = new SqlCommand("Select Password from UserAccount where Username = '" + username + "'", connection);
            var reader = command.ExecuteReader();
            if(reader.Read())
            {
                password = reader["Password"].ToString();
            }
            connection.Close();
            return password;
        }

        static public double ExtractData(string sql)  // Hảm trích xuất dữ liệu từ các thành phần trong bảng
        {
            double data = 0;
            SqlConnection connection = new SqlConnection(myConnection());
            connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);
            data = double.Parse(command.ExecuteScalar().ToString());
            connection.Close();
            return data;
            
        }
    }
}
