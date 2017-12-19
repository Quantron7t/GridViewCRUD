using GridViewOnSteroids.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GridViewOnSteroids
{
    public partial class CarList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCarList();
            }
        }

        private void LoadCarList()
        {
            string connectionString = "Data Source=(local);Initial Catalog=Demo;Integrated Security=True";
            string query = "Select id,name,type from Cars";

            CarGridView.DataSource = GetData(connectionString,query);
            CarGridView.DataBind();            
        }       

        private DataTable GetData(string connectionString,string query)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }           
        }

        protected void CarGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[0].Visible = false;
        }

        protected void CarGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Int32.Parse(CarGridView.DataKeys[e.RowIndex].Value.ToString());
            string connectionString = "Data Source=(local);Initial Catalog=Demo;Integrated Security=True";
            string query = "Delete from Cars where id=@ID";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query,sqlConnection);
                sqlCommand.Parameters.AddWithValue("@ID",id);
                sqlCommand.ExecuteNonQuery();
            }
            LoadCarList();
        }

        protected void CarGridView_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            int id = Int32.Parse(CarGridView.DataKeys[e.NewSelectedIndex].Value.ToString());
            string connectionString = "Data Source=(local);Initial Catalog=Demo;Integrated Security=True";
            string query = "Select name,type,price,countryoforigin from Cars where id=@ID";
            Car car = new Car();
            car = GetCar(connectionString, query, id);
            if (car != null)
            {
                carNameLabel.Text = car.Name;
                carTypeLabel.Text = car.Type;
                carPriceLabel.Text = car.Price.ToString("C2", CultureInfo.CreateSpecificCulture("fr-FR"));
                carCountryLabel.Text = car.Country;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SelectCarModal", "openSelectedCar();", true);
            }
        }

        protected void CarGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName=="EditRow")
            {
                int id = Int32.Parse(CarGridView.DataKeys[Int32.Parse(e.CommandArgument.ToString())].Value.ToString());
                UpdateButton.CommandArgument = id.ToString();
                string connectionString = "Data Source=(local);Initial Catalog=Demo;Integrated Security=True";
                string query = "Select name,type,price,countryoforigin from Cars where id=@ID";
                Car car = new Car();
                car = GetCar(connectionString, query, id);

                if (car!=null)
                {
                    updateCarNameLabel.Text = car.Name;
                    updateTypeTextBox.Text = car.Type;
                    updatePriceTextBox.Text = car.Price.ToString();
                    updateCountryTextBox.Text = car.Country.ToString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "EditCarModal", "editCar();", true);
                }        
            }
        }

        private Car GetCar(string connectionString, string query,int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataReader sqlDataReader;
                sqlCommand.Parameters.AddWithValue("@ID", id);
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    Car car = new Car();
                    while (sqlDataReader.Read())
                    {
                        car.Name = sqlDataReader[0].ToString();
                        car.Type = sqlDataReader[1].ToString();
                        car.Price = Convert.ToDecimal(sqlDataReader[2]);
                        car.Country = sqlDataReader[3].ToString();
                    }
                    return car;
                }               
            }
            return null;
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=(local);Initial Catalog=Demo;Integrated Security=True";
            string query = "Update Cars set price=@CarPrice,type=@CarType,countryoforigin=@CarCountry where id=@ID";
            var id = ((Button)sender).CommandArgument;
            string price, type, country;
            type = updateTypeTextBox.Text;
            price = updatePriceTextBox.Text;
            country = updateCountryTextBox.Text;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@ID", id);
                    sqlCommand.Parameters.AddWithValue("@CarType", type);
                    sqlCommand.Parameters.AddWithValue("@CarPrice", price);
                    sqlCommand.Parameters.AddWithValue("@CarCountry", country);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DeleteException", "alert(" + ex + ");", true);
            }
            LoadCarList();
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=(local);Initial Catalog=Demo;Integrated Security=True";
            string query = "Insert into Cars values (@CarName,@CarType,@CarPrice,@CarCountry)";
            string name,price, type, country;
            name = addCarNameTextBox.Text;
            type = addCarTypeTextBox.Text;
            price = addCarPriceTextBox.Text;
            country = addCarCountryTextBox.Text;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query,sqlConnection);
                sqlCommand.Parameters.AddWithValue("@CarName", name);
                sqlCommand.Parameters.AddWithValue("@CarType", type);
                sqlCommand.Parameters.AddWithValue("@CarPrice", price);
                sqlCommand.Parameters.AddWithValue("@CarCountry", country);
                sqlCommand.ExecuteNonQuery();
            }
            LoadCarList();
        }
        
        protected void AddWindowPopupButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddCarModalOpener", "addCar();", true);
        }
    }
}