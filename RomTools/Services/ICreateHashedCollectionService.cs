using RomTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomTools.Services
{
    public interface ICreateHashedCollectionService
    {
        public Task<int> Create(CreateHashedCollectionOptions options);
    }
}
