namespace Util
{
    internal class ConsoleUtil
    {
        public static decimal ReadInput(string q)
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
    }
}
