using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardgamesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardgamesApi.Data
{
    public class BoardgameDbContext : DbContext
    {
        public BoardgameDbContext(DbContextOptions<BoardgameDbContext> options) : base(options)
        {
        }

        public DbSet<Boardgame> Boardgames => Set<Boardgame>();
    }
}