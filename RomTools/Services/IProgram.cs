using System.Threading.Tasks;

namespace RomTools.Services;

public interface IProgram
{
    Task<int> Run(string[] arguments, CancellationToken cancellationToken);
}
