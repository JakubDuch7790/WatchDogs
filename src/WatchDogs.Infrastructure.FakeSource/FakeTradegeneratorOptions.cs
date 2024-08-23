using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Infrastructure.FakeSource;

public record FakeTradegeneratorOptions
{
    public int GeneratedTradesTop { get; set; }
    public int GeneratedTradesBottom { get; set; }
}
