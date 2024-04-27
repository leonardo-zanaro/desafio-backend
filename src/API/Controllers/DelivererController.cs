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

    [HttpPost]
    [Route("deliverer/create")]
    public IActionResult CreateDeliverer(DelivererDTO? model)
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

    [HttpPost]
    [Route("deliverer/document")]
    public IActionResult UploadDocument(IFormFile file, Guid delivererId)
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
}