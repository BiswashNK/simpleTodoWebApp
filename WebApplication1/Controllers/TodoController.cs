using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TodoController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet("get_tasks")]
        public async Task<IActionResult> GetTasks([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Username is required" });
            }

            string query = "SELECT * FROM todo WHERE username=@username";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'MyConnection' not found.");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@username", username);

                        using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                        {
                            table.Load(myReader);
                            myReader.Close();
                        }
                    }
                    await myCon.CloseAsync();
                }
                return new JsonResult(table);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        
        [HttpPost("add_task")]
        public async Task<IActionResult> AddTask([FromForm] string task, [FromForm] string username)
        {
            if (string.IsNullOrEmpty(task) || string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Task and username are required" });
            }

            string query = "INSERT INTO todo (task, username) VALUES (@task, @username)";
            string sqlDataSource = _configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'MyConnection' not found.");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@task", task);
                        myCommand.Parameters.AddWithValue("@username", username);
                        await myCommand.ExecuteNonQueryAsync();
                    }
                    await myCon.CloseAsync();
                }
                return new JsonResult(new { message = "Added Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("delete_task")]
        public async Task<IActionResult> DeleteTask([FromForm] string id)
        {
            string query = "DELETE FROM todo WHERE id=@id";
            string sqlDataSource = _configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'MyConnection' not found.");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@id", id);
                        await myCommand.ExecuteNonQueryAsync();
                    }
                    myCon.Close();
                }
                return new JsonResult(new { message = "Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            string query = "SELECT * FROM users WHERE username=@username AND password=@password";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'MyConnection' not found.");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@username", username);
                        myCommand.Parameters.AddWithValue("@password", password);
                        using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                        {
                            table.Load(myReader);
                            myReader.Close();
                        }
                    }
                    myCon.Close();
                }
                return new JsonResult(table);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] string username, [FromForm] string password, [FromForm] string email)
        {
            string queryCheck = "SELECT COUNT(*) FROM users WHERE username = @username";
            string queryInsert = "INSERT INTO users VALUES (@username, @password, @email)";
            string sqlDataSource = _configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'MyConnection' not found.");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();

                    // Check if the username already exists
                    using (SqlCommand checkCommand = new SqlCommand(queryCheck, myCon))
                    {
                        checkCommand.Parameters.AddWithValue("@username", username);
                        int userCount = (int)await checkCommand.ExecuteScalarAsync();
                        if (userCount > 0)
                        {
                            return BadRequest(new { message = "Username already exists" });
                        }
                    }

                    // Insert new user if username doesn't exist
                    using (SqlCommand insertCommand = new SqlCommand(queryInsert, myCon))
                    {
                        insertCommand.Parameters.AddWithValue("@username", username);
                        insertCommand.Parameters.AddWithValue("@password", password);
                        insertCommand.Parameters.AddWithValue("@email", email);
                        await insertCommand.ExecuteNonQueryAsync();
                    }

                    myCon.Close();
                }
                return new JsonResult(new { message = "Registered Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
