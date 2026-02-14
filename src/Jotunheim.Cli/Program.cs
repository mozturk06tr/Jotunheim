using System;
using System.Threading.Tasks;

namespace Jotunheim.Cli;

internal sealed class Program
{
    internal static Task<int> Main(string[] args)
    {
        Console.WriteLine("Jotunheim CLI");
        return Task.FromResult(0);
    }
}
