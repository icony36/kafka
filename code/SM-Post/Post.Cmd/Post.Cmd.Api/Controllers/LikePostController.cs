﻿using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LikePostController : ControllerBase
    {
        private readonly ILogger<LikePostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public LikePostController(ILogger<LikePostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> LikePostAsync(Guid id)
        {
            try
            {
                await _commandDispatcher.SendAsync(new LikePostCommand
                {
                    Id = id
                });

                return Ok(new BaseResponse
                {
                    Message = "Like post completed."
                });
            }
            catch (InvalidOperationException exception)
            {
                _logger.Log(LogLevel.Warning, exception, "Client made a bad request");

                return BadRequest(new BaseResponse
                {
                    Message = exception.Message,
                });
            }
            catch (AggregateNotFoundException exception)
            {
                _logger.Log(LogLevel.Warning, exception, "Could not retrieve aggregate, client passed an invalid post id targeting the aggregate.");

                return BadRequest(new BaseResponse
                {
                    Message = exception.Message,
                });
            }
            catch (Exception exception)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to like a post.";
                _logger.Log(LogLevel.Error, exception, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
                {
                    Message = SAFE_ERROR_MESSAGE,
                });
            }
        }
    }
}
