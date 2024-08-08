﻿using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogusTest;

public class SuspiciousDealsDetector
{
    /* Similar deals are deals where: 
     * open time differs no more than one second,
     * currency pair is the same and
     * the difference in volume-to-balance ratio is no more than 5%. */

    private const int TimeDifferenceBetweenDeals = 1;
}