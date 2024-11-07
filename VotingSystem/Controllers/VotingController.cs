using Microsoft.AspNetCore.Mvc;
using VotingSystem.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VotingSystem.Controllers
{
    public class VotingController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VotingController> _logger;

        public VotingController(IConfiguration configuration, ILogger<VotingController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Step 1: Basic Information
        public IActionResult Step1()
        {
            return View(new Step1Model());
        }

        [HttpPost]
        public IActionResult Step1(Step1Model model)
        {
            if (ModelState.IsValid)
            {
                // Store Step 1 data in TempData for later steps
                TempData["Name"] = model.Name;
                TempData["Contact"] = model.Contact;
                TempData["Age"] = model.Age;
                TempData["CNIC"] = model.CNIC;
                TempData.Keep(); // Ensure TempData persists across multiple requests
                return RedirectToAction("Step2");
            }
            return View(model);
        }

        // Step 2: Assembly Type
        public IActionResult Step2()
        {
            TempData.Keep(); // Keep TempData for future steps
            return View();
        }

        [HttpPost]
        public IActionResult Step2(string assemblyType)
        {
            if (!string.IsNullOrEmpty(assemblyType))
            {
                TempData["AssemblyType"] = assemblyType;
                TempData.Keep();
                return RedirectToAction("Step3");
            }
            ModelState.AddModelError("", "Please select an assembly type.");
            return View();
        }

        // Step 3: Party Selection
        public IActionResult Step3()
        {
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public IActionResult Step3(string selectedParty)
        {
            if (!string.IsNullOrEmpty(selectedParty))
            {
                TempData["SelectedParty"] = selectedParty;
                TempData.Keep();
                return RedirectToAction("Step4");
            }
            ModelState.AddModelError("", "Please select a party.");
            return View();
        }
        public IActionResult ThankYou()
        {
            return View();
        }


        // Step 4: Confirm and Submit
        public IActionResult Step4()
        {
            var model = new VotingModel
            {
                Name = TempData["Name"]?.ToString(),
                Contact = TempData["Contact"]?.ToString(),
                Age = int.TryParse(TempData["Age"]?.ToString(), out var age) ? age : 0,
                CNIC = TempData["CNIC"]?.ToString(),
                AssemblyType = TempData["AssemblyType"]?.ToString(),
                SelectedParty = TempData["SelectedParty"]?.ToString()
            };

            TempData.Keep(); // Keep TempData if the user goes back to previous steps

            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitStep4(VotingModel model)
        {
            // Log the values of the model to see if they are correctly populated
            _logger.LogInformation("Name: {Name}, Contact: {Contact}, Age: {Age}, CNIC: {CNIC}, AssemblyType: {AssemblyType}, SelectedParty: {SelectedParty}",
                model.Name, model.Contact, model.Age, model.CNIC, model.AssemblyType, model.SelectedParty);

            if (model == null || model.Name == null)
            {
                ModelState.AddModelError("", "Invalid voting data submitted.");
                return View("Step4", model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = "INSERT INTO Table_1 (Name, Contact, Age, CNIC, AssemblyType, SelectedParty) " +
                                   "VALUES (@Name, @Contact, @Age, @CNIC, @AssemblyType, @SelectedParty)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Name", model.Name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Contact", model.Contact ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Age", model.Age);
                        cmd.Parameters.AddWithValue("@CNIC", model.CNIC ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssemblyType", model.AssemblyType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SelectedParty", model.SelectedParty ?? (object)DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            _logger.LogInformation("Data inserted successfully.");
                        }
                        else
                        {
                            _logger.LogWarning("No rows were inserted.");
                        }
                    }
                }

                return RedirectToAction("ThankYou");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting voting data into database.");
                ModelState.AddModelError("", "An error occurred while saving your vote. Please try again.");
                return View("Step4", model);
            }
        }
    }
}