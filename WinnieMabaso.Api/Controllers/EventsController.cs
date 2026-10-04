using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WinnieMabaso.Application.DTOs.Events;
using WinnieMabaso.Application.Interfaces;

namespace WinnieMabaso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;


    public EventsController(
        IEventService eventService)
    {
        _eventService = eventService;
    }


    // ==================================================
    // GET PUBLISHED EVENTS
    // Mobile users
    // ==================================================

    [HttpGet("published")]
    [Authorize]
    public async Task<ActionResult<List<EventResponse>>>
        GetPublishedEvents()
    {
        List<EventResponse> events =
            await _eventService
                .GetPublishedAsync();

        return Ok(events);
    }


    // ==================================================
    // GET ALL EVENTS
    // Administrators
    // ==================================================

    [HttpGet]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<ActionResult<List<EventResponse>>>
        GetAllEvents()
    {
        List<EventResponse> events =
            await _eventService
                .GetAllAsync();

        return Ok(events);
    }


    // ==================================================
    // GET EVENT BY ID
    // Authenticated users
    // ==================================================

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<EventResponse>>
        GetEventById(string id)
    {
        EventResponse? eventResponse =
            await _eventService
                .GetByIdAsync(id);


        if (eventResponse == null)
        {
            return NotFound(
                new
                {
                    message = "Event not found."
                });
        }


        return Ok(eventResponse);
    }


    // ==================================================
    // CREATE EVENT
    // Administrator only
    // ==================================================

    [HttpPost]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<ActionResult<EventResponse>>
        CreateEvent(
            [FromBody] CreateEvent request)
    {
        string? userId =
            User.FindFirst("userId")?.Value;


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "Authenticated user ID could not be determined."
                });
        }


        try
        {
            EventResponse response =
                await _eventService
                    .CreateAsync(
                        userId,
                        request);


            return CreatedAtAction(
                nameof(GetEventById),
                new
                {
                    id = response.Id
                },
                response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
    }


    // ==================================================
    // UPDATE EVENT
    // Administrator only
    // ==================================================

    [HttpPut("{id}")]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<ActionResult<EventResponse>>
        UpdateEvent(
            string id,
            [FromBody] UpdateEvent request)
    {
        string? userId =
            User.FindFirst("userId")?.Value;


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "Authenticated user ID could not be determined."
                });
        }


        try
        {
            EventResponse response =
                await _eventService
                    .UpdateAsync(
                        id,
                        userId,
                        request);


            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(
                new
                {
                    message = ex.Message
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
    }


    // ==================================================
    // PUBLISH EVENT
    // Administrator only
    // ==================================================

    [HttpPut("{id}/publish")]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult>
        PublishEvent(string id)
    {
        return await UpdatePublishedStatus(
            id,
            true);
    }


    // ==================================================
    // UNPUBLISH EVENT
    // Administrator only
    // ==================================================

    [HttpPut("{id}/unpublish")]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult>
        UnpublishEvent(string id)
    {
        return await UpdatePublishedStatus(
            id,
            false);
    }


    // ==================================================
    // DELETE EVENT
    // Administrator only
    // ==================================================

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdministrationOnly")]
    public async Task<IActionResult>
        DeleteEvent(string id)
    {
        try
        {
            await _eventService
                .DeleteAsync(id);


            return Ok(
                new
                {
                    message =
                        "Event deleted successfully."
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(
                new
                {
                    message = ex.Message
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
    }


    // ==================================================
    // UPDATE PUBLISHED STATUS
    // ==================================================

    private async Task<IActionResult>
        UpdatePublishedStatus(
            string id,
            bool published)
    {
        string? userId =
            User.FindFirst("userId")?.Value;


        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                new
                {
                    message =
                        "Authenticated user ID could not be determined."
                });
        }


        try
        {
            await _eventService
                .UpdatePublishedStatusAsync(
                    id,
                    published,
                    userId);


            return Ok(
                new
                {
                    message = published
                        ? "Event published successfully."
                        : "Event unpublished successfully."
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(
                new
                {
                    message = ex.Message
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(
                new
                {
                    message = ex.Message
                });
        }
    }
}