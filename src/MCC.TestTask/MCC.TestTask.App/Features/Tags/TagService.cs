using MCC.TestTask.App.Features.Tags.Dto;
using MCC.TestTask.Domain;
using MCC.TestTask.Persistance;
using MCC.TestTask.Infrastructure;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using MCC.TestTask.App.Features.Communities.Dto;
using FluentResults.Extensions;

namespace MCC.TestTask.App.Features.Tags;

public class TagService
{
    private readonly BlogDbContext _blogDbContext;

    public TagService(BlogDbContext blogDbContext)
    {
        _blogDbContext = blogDbContext;
    }

    public async Task<Result<List<TagDto>>> GetAllTagsAsync()
    {
        return await _blogDbContext.Tags
            .Select(t => t.ToDto())
            .ToListAsync();
    }

    public async Task<Result<Guid>> CreateTag(Guid userId, string name)
    {
        return await CheckUserExistsAsync(userId).Bind(async Task<Result<Guid>> () =>
        {
            if (await _blogDbContext.Tags.AnyAsync(t => t.Name == name))
                return CustomErrors.ValidationError("Tag already exists");

            var tag = new Tag
            {
                CreateTime = DateTime.UtcNow,
                Name = name,
                UserId = userId
            };

            _blogDbContext.Tags.Add(tag);
            await _blogDbContext.SaveChangesAsync();
            return tag.Id;

        });
    }

    private async Task<Result> CheckUserExistsAsync(Guid userId)
    {
        return await _blogDbContext.Users.AnyAsync(u => u.Id == userId)
            ? Result.Ok()
            : CustomErrors.NotFound("Non-existent user");
    }
}