﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Contracts;
public interface IWatcher
{
    Task StartAsync(CancellationToken token = default);
}