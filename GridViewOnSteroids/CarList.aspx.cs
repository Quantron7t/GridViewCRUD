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
            string query = "Select id,name,availability from Cars";

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
            string query = "Select name,type,price,countryoforigin,availability,instock from Cars where id=@ID";
            Car car = new Car();
            car = GetCar(connectionString, query, id);
            if (car != null)
            {
                carNameLabel.Text = car.Name;
                carTypeLabel.Text = car.Type;
                carPriceLabel.Text = car.Price.ToString("C2", CultureInfo.CreateSpecificCulture("fr-FR"));
                carCountryLabel.Text = car.Country;
                carInStockLabel.Text = car.InStock.ToString();
                if (car.Availability)
                {
                    selectAvailabilityImage.ImageUrl = "~/images/available.png";
                    selectAvailabilityImage.ToolTip = "Car available";
                }
                else
                {
                    selectAvailabilityImage.ImageUrl = "~/images/unavailable.png";
                    selectAvailabilityImage.ToolTip = "Car unavailable";
                }
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
                string query = "Select name,type,price,countryoforigin,availability,instock from Cars where id=@ID";
                Car car = new Car();
                car = GetCar(connectionString, query, id);

                if (car!=null)
                {
                    updateCarNameLabel.Text = car.Name;
                    updateTypeTextBox.Text = car.Type;
                    updatePriceTextBox.Text = car.Price.ToString();
                    updateCountryTextBox.Text = car.Country.ToString();
                    updateAvailableCheckBox.Checked = car.Availability;
                    updateInStockTextBox.Text = car.InStock.ToString();
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
                        car.Availability = Convert.ToBoolean(sqlDataReader[4]);
                        car.InStock = Convert.ToInt32(sqlDataReader[5]);
                    }
                    return car;
                }               
            }
            return null;
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=(local);Initial Catalog=Demo;Integrated Security=True";
            string query = "Update Cars set price=@CarPrice,type=@CarType,countryoforigin=@CarCountry,availability=@Available,instock=@InStock where id=@ID";
            var id = ((Button)sender).CommandArgument;
            string price, type, country;
            bool available;
            int inStock;
            type = updateTypeTextBox.Text;
            price = updatePriceTextBox.Text;
            country = updateCountryTextBox.Text;
            available = updateAvailableCheckBox.Checked;
            inStock = Convert.ToInt32(updateInStockTextBox.Text);
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
                    sqlCommand.Parameters.AddWithValue("@Available", available);
                    sqlCommand.Parameters.AddWithValue("@InStock", inStock);
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
            string query = "Insert into Cars values (@CarName,@CarType,@CarPrice,@CarCountry,@Availability,@Instock)";
            string name,price, type, country;
            bool available;
            int instock;
            name = addCarNameTextBox.Text;
            type = addCarTypeTextBox.Text;
            price = addCarPriceTextBox.Text;
            country = addCarCountryTextBox.Text;
            available = addCarAvailableCheckBox.Checked;
            instock = Convert.ToInt32(addCarInStockTextBox.Text);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query,sqlConnection);
                sqlCommand.Parameters.AddWithValue("@CarName", name);
                sqlCommand.Parameters.AddWithValue("@CarType", type);
                sqlCommand.Parameters.AddWithValue("@CarPrice", price);
                sqlCommand.Parameters.AddWithValue("@CarCountry", country);
                sqlCommand.Parameters.AddWithValue("@Availability", available);
                sqlCommand.Parameters.AddWithValue("@Instock", instock);
                sqlCommand.ExecuteNonQuery();
            }
            LoadCarList();
        }
        
        protected void AddWindowPopupButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddCarModalOpener", "addCar();", true);
        }

        protected void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this,this.GetType(),"checkboxscript","alert('Fired')",true);
            foreach (GridViewRow row in CarGridView.Rows)
            {
                int id = Convert.ToInt32(CarGridView.DataKeys[row.RowIndex].Value);
                CheckBox chk = (CheckBox)row.FindControl("CheckBox");
                if (chk.Checked)
                {
                    updateAvailability(id, true);
                }
                else
                {
                    updateAvailability(id, false);
                }
            }
        }

        private void updateAvailability(int id, bool v)
        {
            string connectionString = "Data Source=(local);Initial Catalog=Demo;Integrated Security=true";
            string query = "update Cars set availability=@available where id=@ID;";

            using (SqlConnection sqlConnection= new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(query,sqlConnection);
                sqlCommand.Parameters.AddWithValue("@available",v);
                sqlCommand.Parameters.AddWithValue("@ID", id);
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}