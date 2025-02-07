﻿using RomTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomTools.Services.Commands;

public interface ICreateHashedCollectionService
{
    public Task<int> CreateAsync(
        CreateHashedCollectionOptions options,
        CancellationToken cancellationToken);
}
