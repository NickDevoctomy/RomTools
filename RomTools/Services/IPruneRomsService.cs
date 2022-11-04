namespace RomTools.Services;

public interface IPruneRomsService
{
    public Task<int> Process(PruneRomsOptions options);
}
