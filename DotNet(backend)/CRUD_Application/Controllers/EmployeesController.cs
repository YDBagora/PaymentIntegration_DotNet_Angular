using CRUD_Application.Data;
using CRUD_Application.Models;
using CRUD_Application.Models.Entity;
using CRUD_Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CRUD_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //inherting PreDefine Controller
    public class EmployeesController : ControllerBase
    {
        //private variable is created by using ApplicationContext
        private readonly ApplicationContext dbContext;
        private readonly IMemoryCache cache;
        private readonly PayPalServices payPalServices;
        private readonly RazorpayService razorpayService;

        //EmployeeController Class Constructor is called and set the value in dbContext variable
        public EmployeesController(ApplicationContext dbContext, IMemoryCache cache, PayPalServices payPalServices, RazorpayService razorpayService)
        {
            this.dbContext = dbContext;
            this.cache = cache;
            this.payPalServices = payPalServices;
            this.razorpayService = razorpayService;
        }

        [HttpGet("GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            try
            {
                var getEmployees = dbContext.Employees.ToList();

                return Ok(getEmployees);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error {e.Message}");
            }
        }

        /// <summary>
        /// POST Request to Create the Employee in the Table
        /// </summary>
        [HttpPost("add-employee")]
        public IActionResult AddEmployee(AddEmployeeDto addEmployeeDto)
        {
            try
            {
                //Employee class object is created
                var getEmployees = new Employee()
                {
                    Name = addEmployeeDto.Name,
                    Email = addEmployeeDto.Email,
                    Phone = addEmployeeDto.Phone,
                    Salary = addEmployeeDto.Salary
                };
                dbContext.Employees.Add(getEmployees);
                dbContext.SaveChanges();

                HttpContext.Session.SetString("EmployeeEmail", getEmployees.Email);
                //cache.Set("abcd", getEmployees.Email, TimeSpan.FromMinutes(30));

                var rapid = getEmployees.Email;

                return Ok(getEmployees);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error {e.Message}");
            }
        }

        /// <summary>
        //GET Request is created and in that method id parameter is passed
        /// </summary>
        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetEmployeeById(Guid id)
        {
            try
            {
                var employee = dbContext.Employees.Find(id);

                if (employee is null)
                {
                    return NotFound();
                }

                return Ok(employee);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error {e.Message}");
            }
        }

        /// <summary>
        //PUT Request is created to update the Employee and in method employee id and UpdateEmployee Model is passed
        /// </summary>
        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateEmployeeById(Guid id, UpdateEmployeeDto updateEmployee)
        {
            try
            {
                var employee = dbContext.Employees.Find(id);

                if (employee is null)
                {
                    return NotFound();
                }

                employee.Name = updateEmployee.Name;
                employee.Email = updateEmployee.Email;
                employee.Phone = updateEmployee.Phone;
                employee.Salary = updateEmployee.Salary;

                dbContext.SaveChanges();

                return Ok(employee);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error {e.Message}");
            }
        }

        /// <summary>
        //DELETE Request is created to delete the employee and in parameter employee id is passed
        /// </summary>
        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteEmployeeById(Guid id)
        {
            try
            {
                var employee = dbContext.Employees.Find(id);

                if (employee is null)
                {
                    return NotFound();
                }

                dbContext.Employees.Remove(employee);
                dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal Server Error {e.Message}");
            }
        }


        [HttpPost("createpayment")]
        public IActionResult CreatePayment([FromQuery] string amount)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var payment = payPalServices.CreatePayment(baseUrl, amount);

            var approveUrl = payment.links.FirstOrDefault(x => x.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;
            return Ok(new { approveUrl });
        }


        [HttpGet("executepayment")]
        public IActionResult ExecutePayment([FromQuery] string paymentId, [FromQuery] string payerId)
        {
            var executedPayment = payPalServices.ExecutePayment(paymentId, payerId);
            return Ok(executedPayment);
        }

        [HttpPost("verify")]
        public IActionResult VerifyPayment([FromBody] RazorpayVerificationModel model)
        {
            try
            {
                razorpayService.VerifyPayment(model.OrderId, model.PaymentId, model.Signature);
                return Ok(new { status = "success" });
            }
            catch
            {
                return BadRequest(new { status = "failed" });
            }
        }
    }
}
