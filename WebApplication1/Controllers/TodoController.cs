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
        public async Task<IActionResult> GetTasks()
        {
            string query = "SELECT * FROM todo";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'MyConnection' not found.");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
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

        [HttpPost("add_task")]
        public async Task<IActionResult> AddTask([FromForm] string task)
        {
            string query = "INSERT INTO todo VALUES (@task)";
            string sqlDataSource = _configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'MyConnection' not found.");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@task", task);
                        await myCommand.ExecuteNonQueryAsync();
                    }
                    myCon.Close();
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


    }
}
