using System;
using System.Threading.Tasks;

namespace Reversi.Persistence
{
    public interface IReversiDataAccess
    {
        Task<ReversiTable> LoadAsync(string path);

        Task SaveAsync(string path, ReversiTable table);
    }
}
