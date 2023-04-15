using System;
using System.IO;
using System.Threading.Tasks;

namespace Reversi.Persistence
{
    public class ReversiFileDataAccess : IReversiDataAccess
    {
        public async Task<ReversiTable> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line = await reader.ReadLineAsync();
                    int tableSize = int.Parse(line);
                    
                    ReversiTable table = new ReversiTable(tableSize);

                    line = await reader.ReadLineAsync();
                    string[] values = line.Split(' ');

                    table.TimeWhite = int.Parse(values[0]);
                    table.TimeBlack = int.Parse(values[1]);

                    line = await reader.ReadLineAsync();
                    values = line.Split(' ');

                    table.ScoreWhite = int.Parse(values[0]);
                    table.ScoreBlack = int.Parse(values[1]);

                    line = await reader.ReadLineAsync();

                    table.CurrentPlayer = (Player)int.Parse(line);

                    for (int i = 0; i < tableSize; i++)
                    {
                        line = await reader.ReadLineAsync();
                        values = line.Split(' ');

                        for (int j = 0; j < tableSize; j++)
                        {
                            table.SetValue(i, j, (Player)int.Parse(values[j]));
                        }
                    }

                    return table;
                }
            }
            catch
            {
                throw new ReversiDataException();
            }
        }

        public async Task SaveAsync(string path, ReversiTable table)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(table.Size);
                    await writer.WriteAsync(table.TimeWhite.ToString());
                    await writer.WriteLineAsync(" " + table.TimeBlack);
                    await writer.WriteAsync(table.ScoreWhite.ToString());
                    await writer.WriteLineAsync(" " + table.ScoreBlack);
                    await writer.WriteLineAsync(((int)table.CurrentPlayer).ToString());

                    for (int i = 0; i < table.Size; i++)
                    {
                        for (int j = 0; j < table.Size; j++)
                        {             
                            await writer.WriteAsync((int)table[i, j] + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new ReversiDataException();
            }
        }
    }
}
