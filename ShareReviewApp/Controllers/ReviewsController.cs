using Microsoft.AspNetCore.Mvc;
using ShareReviewApp.DTOs.Reviews;
using ShareReviewApp.Exceptions;
using ShareReviewApp.Services;

namespace ShareReviewApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetAll()
    {
        var reviews = await _reviewService.GetAllAsync();
        return Ok(reviews);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReviewResponse>> GetById(Guid id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        if (review is null)
            return NotFound();

        return Ok(review);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewResponse>> Create([FromBody] CreateReviewRequest request)
    {
        try
        {
            var created = await _reviewService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DuplicateReviewException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/helpful")]
    public async Task<ActionResult<ReviewResponse>> VoteHelpful(Guid id)
    {
        var review = await _reviewService.VoteHelpfulAsync(id);
        if (review is null)
            return NotFound();

        return Ok(review);
    }

    // Leading slash overrides the controller-level route prefix
    [HttpGet("/api/products/{productId:guid}/reviews")]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetByProductId(Guid productId)
    {
        var reviews = await _reviewService.GetByProductIdAsync(productId);
        return Ok(reviews);
    }
}
