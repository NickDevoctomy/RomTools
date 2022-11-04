namespace RomTools.Services
{
    public class PruneRomsService : IPruneRomsService
    {
        public async Task<int> Process(PruneRomsOptions options)
        {
            await Task.Yield();
            return -1;
        }
    }
}
