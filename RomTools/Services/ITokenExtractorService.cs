using RomTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomTools.Services
{
    public interface ITokenExtractorService
    {
        public List<string> ExtractTokens(
            FileEnvelope file,
            params string[] braces);
    }
}
