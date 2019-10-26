﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dersa.Interfaces
{
    public interface IEntityComparerProvider
    {
        IComparer<ICompiledEntity> GetEntityComparer();

    }
}