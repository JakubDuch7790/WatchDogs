using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Domain;

/// <summary>
/// There are different types of lot sizes, which allow traders to control how large their positions are.
/// Here we are storing them because we need them in calculating volume-to-balance ratio.
/// Because this app should be scaleable we provide all standardized lot types in financial markets, particularly in forex trading
/// </summary>
public record SuspiciousDealDetectorOptions
{
    public int StandardLot { get; set; }
    public int MiniLot { get; set; }
    public int MicroLot { get; set; }
    public int NanoLot { get; set; }
}
