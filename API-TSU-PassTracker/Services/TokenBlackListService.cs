﻿using API_TSU_PassTracker.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace API_TSU_PassTracker.Services
{
    public interface ITokenBlackListService
    {
        Task AddTokenToBlackList(string token);
        Task<bool> iSTokenRevoked(string token);
    }
    public class TokenBlackListService : ITokenBlackListService
    {
        private readonly TsuPassTrackerDBContext _context;

        public TokenBlackListService(TsuPassTrackerDBContext context)
        {
            _context = context;
        }

        public async Task AddTokenToBlackList(string token)
        {
            var DbToken = new TokenBlackList
            {
                token = token,
            };

            await _context.TokenBlackList.AddAsync(DbToken);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> iSTokenRevoked(string token)
        {
            return await _context.TokenBlackList.AnyAsync(t => t.token == token);
        }
    }
}
