using Application.DTOs;
using Application.UseCases.Interfaces;
using Application.ViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class DelivererController : MainController
{
    private readonly IDelivererUseCase _delivererUseCase;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<DelivererController> _logger;
    
    public DelivererController(
        IDelivererUseCase delivererUseCase,
        UserManager<User> userManager,
        ILogger<DelivererController> logger)
    {
        _delivererUseCase = delivererUseCase;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a list of all deliverers in the system.
    /// </summary>
    /// <param name="pageNumber">The page number of the results (optional).</param>
    /// <param name="pageQuantity">The number of results per page (optional).</param>
    /// <returns>
    /// The list of deliverers.
    /// </returns>
    [HttpGet]
    [Route("/deliverer")]
    public IActionResult GetAll(int? pageNumber = null, int? pageQuantity = null)
    {
        var list = _delivererUseCase.GetAll(pageNumber, pageQuantity);

        return Ok(list);
    }
    
    /// <summary>
    /// Creates a deliverer in the system.
    /// </summary>
    /// <param name="model">The deliverer data.</param>
    /// <returns>
    /// Returns an IActionResult representing the result of the create operation.
    /// If the create operation is successful, the IActionResult will contain the created deliverer object.
    /// If there is an error during the create operation, the IActionResult will contain a BadRequest result with an error message.
    /// </returns>
    [HttpPost]
    [Route("/deliverer")]
    public IActionResult CreateDeliverer(DelivererDTO? model)
    {
        try
        {
            if (model == null)
                return BadRequest("Need to fill in the information");

            var userId = _userManager.GetUserAsync(HttpContext.User).Result?.Id;

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User not logged in");

            var resultCreate = _delivererUseCase.CreateDeliverer(model, Guid.Parse(userId));

            if (!resultCreate.Success)
                return BadRequest(resultCreate.Message);

            return Ok(resultCreate.Object);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Uploads a document for a deliverer.
    /// </summary>
    /// <param name="file">The document file to upload.</param>
    /// <param name="delivererId">The ID of the deliverer.</param>
    /// <returns>
    /// Returns an IActionResult representing the result of the upload operation.
    /// If the upload is successful, the IActionResult will contain a message indicating the file has been uploaded successfully.
    /// If there is an error during the upload, the IActionResult will contain a BadRequest result with an error message.
    /// </returns>
    [HttpPost]
    [Route("deliverer/document")]
    public IActionResult UploadDocument(IFormFile file, Guid delivererId)
    {
        try
        {
            var extension = System.IO.Path.GetExtension(file.FileName);

            if (extension.ToLower() != ".png" && extension.ToLower() != ".bmp") 
            {
                return BadRequest("Invalid file extension. Only png and bmp files are allowed");
            }

            var result = _delivererUseCase.UploadDocument(file, delivererId);

            if (!result.Success)
                return BadRequest(result.Message);
            
            return Ok($"File {file.FileName} has been uploaded successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }
}