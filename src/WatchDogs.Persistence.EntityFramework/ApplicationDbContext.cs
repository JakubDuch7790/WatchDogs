﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDogs.Contracts;

namespace WatchDogs.Persistence.EntityFramework;
public class ApplicationDbContext : DbContext
{
    public DbSet<Trade> Trades { get; set; }
}
