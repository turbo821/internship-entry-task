
using TickiTackToe.Application.Interfaces;

namespace TickiTackToe.Infrastructure.Services
{
    public class LuckyService : ILuckyService
    {
        private static readonly Random _random = new Random();

        public bool IsLucky(int percent)
        {
            if (percent < 0 || percent > 100)
                throw new ArgumentOutOfRangeException("The percentage must be between 0 and 100");

            if (percent == 0)
                return false;
            if (percent == 100)
                return true;

            int randomValue = _random.Next(1, 101);
            return randomValue <= percent;
        }
    }
}
