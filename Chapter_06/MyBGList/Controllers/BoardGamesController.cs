﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.DTO;
using MyBGList.Models;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.ComponentModel.DataAnnotations;
using MyBGList.Attributes;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(
            ApplicationDbContext context,
            ILogger<BoardGamesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //[HttpGet(Name = "GetBoardGames")]
        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        //public async Task<RestDTO<BoardGame[]>> Get(
        //    int pageIndex = 0,
        //    [Range(1, 100)] int pageSize = 10,
        //    [SortColumnValidator(typeof(BoardGameDTO))] string? sortColumn = "Name",
        //    // [RegularExpression("ASC|DESC")] string? sortOrder = "ASC",
        //    [SortOrderValidator] string? sortOrder = "ASC",
        //    string? filterQuery = null
        //    )
        //{
        //    var query = _context.BoardGames.AsQueryable();
        //    if (!string.IsNullOrEmpty(filterQuery))
        //        query = query.Where(b => b.Name.Contains(filterQuery));
        //    query = query
        //            .OrderBy($"{sortColumn} {sortOrder}")
        //            .Skip(pageIndex * pageSize)
        //            .Take(pageSize);

        //    return new RestDTO<BoardGame[]>()
        //    {
        //        Data = await query.ToArrayAsync(),
        //        PageIndex = pageIndex,
        //        PageSize = pageSize,
        //        RecordCount = await _context.BoardGames.CountAsync(),
        //        Links = new List<LinkDTO> {
        //            new LinkDTO(
        //                Url.Action(
        //                    null,
        //                    "BoardGames",
        //                    new { pageIndex, pageSize },
        //                    Request.Scheme)!,
        //                "self",
        //                "GET"),
        //        }
        //    };
        //}

        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<RestDTO<BoardGame[]>> Get(
            [FromQuery] RequestDTO<BoardGameDTO> input)
        {
            throw new Exception("ok");

            var query = _context.BoardGames.AsQueryable();
            if (!string.IsNullOrEmpty(input.FilterQuery))
                query = query.Where(b => b.Name.Contains(input.FilterQuery));
            query = query
                    .OrderBy($"{input.SortColumn} {input.SortOrder}")
                    .Skip(input.PageIndex * input.PageSize)
                    .Take(input.PageSize);

            return new RestDTO<BoardGame[]>()
            {
                Data = await query.ToArrayAsync(),
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = await _context.BoardGames.CountAsync(),
                Links = new List<LinkDTO> {
                    new LinkDTO(
                        Url.Action(
                            null,
                            "BoardGames",
                            new { input.PageIndex, input.PageSize },
                            Request.Scheme)!,
                        "self",
                        "GET"),
                }
            };
        }

        [HttpPost(Name = "UpdateBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Post(BoardGameDTO bgDTO)
        {
            var boardgame = await _context.BoardGames
                .Where(b => b.Id == bgDTO.Id)
                .FirstOrDefaultAsync();
            if (boardgame != null)
            {
                if (!string.IsNullOrEmpty(bgDTO.Name))
                    boardgame.Name = bgDTO.Name;
                if (bgDTO.Year.HasValue && bgDTO.Year.Value > 0)
                    boardgame.Year = bgDTO.Year.Value;
                boardgame.LastModifiedDate = DateTime.Now;
                _context.BoardGames.Update(boardgame);
                await _context.SaveChangesAsync();
            };

            return new RestDTO<BoardGame?>()
            {
                Data = boardgame,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(
                            Url.Action(
                                null,
                                "BoardGames",
                                bgDTO,
                                Request.Scheme)!,
                            "self",
                            "POST"),
                }
            };
        }

        [HttpDelete(Name = "DeleteBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Delete(int id)
        {
            var boardgame = await _context.BoardGames
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
            if (boardgame != null)
            {
                _context.BoardGames.Remove(boardgame);
                await _context.SaveChangesAsync();
            };

            return new RestDTO<BoardGame?>()
            {
                Data = boardgame,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(
                            Url.Action(
                                null,
                                "BoardGames",
                                id,
                                Request.Scheme)!,
                            "self",
                            "DELETE"),
                }
            };
        }
    }
}