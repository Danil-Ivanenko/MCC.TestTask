using System.Net;
using MCC.TestTask.App.Features.Tags.Dto;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCC.TestTask.App.Services.Auth;
using MCC.TestTask.App.Features.Communities;

namespace MCC.TestTask.App.Features.Tags;

[Route("api/tag")]
public class TagController : ControllerBase
{
    private readonly TagService _tagService;
    private readonly UserAccessor _userAccessor;

    public TagController(TagService tagService, UserAccessor userAccessor)
    {
        _tagService = tagService;
        _userAccessor = userAccessor;
    }

    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags()
    {
        return await _tagService.GetAllTagsAsync().ToActionResult();
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Guid>> CreateTag([FromBody] CreateTagModel model)
    {
        return await _userAccessor.GetUserId()
        .Bind(userId => _tagService.CreateTag(userId, model.Name))
        .ToActionResult();
        //return await _tagService.CreateTag(model.Name).ToActionResult();
    }
}