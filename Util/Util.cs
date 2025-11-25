using Base;
namespace Util
{
    public class ConsoleUtil
    {
        public static decimal ReadDecimal(string q)
        {
            Console.Write(q + ": ");
            var s = Console.ReadLine();
            try
            {
                return decimal.Parse(s);
            }
            catch (Exception e)
            {
                Console.WriteLine("input error: " + e.Message);
                System.Environment.Exit(1);
            }
            return 0;
        }
        public static Side ReadSide(string q)
        {
            Console.Write(q + ": ");
            var s = Console.ReadLine();
            var ss = s?.ToLower();

            if (ss == "s")
            {
                return Side.Sell;
            }
            if (ss == "l")
            {
                return Side.Buy;
            }
            throw new InvalidDataException("please enter S for short side or L for long side");
        }
    }
}
