﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;

public class TradeLoader : ITradeLoader
{
    private readonly ApplicationDbContext _context;

    // Track last Trade for loading next one
    public Guid? _lastLoadedTradeId { get; set; }
    public Guid? _lastLoadedTradeIdd = null;

    public TradeLoader(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Trade>> LoadAllTradesAsync()
    {
        return await _context.Trades.ToListAsync();
    }
    public async Task<Trade> LoadOneTradeAtTimeAsync()
    {
        return await _context.Trades.FirstOrDefaultAsync();
    }
}
