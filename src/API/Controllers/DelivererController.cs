using Application.DTOs;
using Application.UseCases.Interfaces;
using Application.ViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class DelivererController : MainController
{
    private readonly IDelivererUseCase _delivererUseCase;
    private readonly UserManager<User> _userManager;

    public DelivererController(IDelivererUseCase delivererUseCase, UserManager<User> userManager)
    {
        _delivererUseCase = delivererUseCase;
        _userManager = userManager;
    }

    /// <summary>
    /// Creates a new deliverer.
    /// </summary>
    /// <param name="model">The deliverer's information.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPost]
    [Route("deliverer/create")]
    public IActionResult CreateDeliverer(DelivererDto? model)
    {
        if (model == null)
            return BadRequest("Need to fill in the information");

        var userId = _userManager.GetUserAsync(HttpContext.User).Result?.Id;

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("User not logged in");

        var deliverer = _delivererUseCase.CreateDeliverer(model, Guid.Parse(userId));

        if (deliverer == null)
            return BadRequest("An unexpected error has occurred");

        return Ok("Success when creating deliverer");
    }

    /// <summary>
    /// Uploads a document for the deliverer.
    /// </summary>
    /// <param name="file">The file to be uploaded.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPost]
    [Route("deliverer/post/document")]
    public IActionResult UploadDocument(IFormFile file)
    {
        var extension = System.IO.Path.GetExtension(file.FileName);

        if (extension.ToLower() != ".png" && extension.ToLower() != ".bmp") 
        {
            return BadRequest("Invalid file extension. Only png and bmp files are allowed");
        }

        var delivererId = _userManager.GetUserAsync(User).Result?.Id;

        var result = _delivererUseCase.UploadDocument(file, Guid.Parse(delivererId));

        if (!result)
            return BadRequest("An unexpected error has occurred");
        
        return Ok($"File {file.FileName} has been uploaded successfully.");
    }
}