using Base;
using Util;


# region ==== initialization ====

var startLevel = ConsoleUtil.ReadInput("enter initial level");
var stepSize   = ConsoleUtil.ReadInput("enter stepSize");
var stepsCount = ConsoleUtil.ReadInput("enter number of levels");

# endregion

# region ==== tests ====

//Console.WriteLine("\nTest non-uniform short network:");
//TestNonUniformNetwork(Side.Sell, startLevel, stepSize, stepsCount);

//Console.WriteLine("\nTest non-uniform long network:");
//TestNonUniformNetwork(Side.Buy, startLevel, stepSize, stepsCount);

//Console.WriteLine("\nTest uniform short network:");
//TestUniformNetwork(Side.Sell, startLevel, stepSize, stepsCount);

Console.WriteLine("\nTest uniform long network:");
TestUniformNetwork(Side.Buy, startLevel, stepSize, stepsCount);

# endregion

# region === test methods ====
static void TestNonUniformNetwork(Side side, decimal sl, decimal ss, decimal sc)
{
    var net = new Network(side);
    var p = sl;
    for (int i = 0; i < sc; i++)
    {
        net.Add(p);
        if (side == Side.Sell)
        {
            p -= ss;
            if (p < 0)
            {
                p = 0;
            }
        } else
        {
            p += ss;
        }
    }
    Console.WriteLine(net.ToString());
}

static void TestUniformNetwork(Side side, decimal sl, decimal ss, decimal sc)
{
    var net = new Network(side, ss);
    net.Add(sl);
    for (int i = 1; i < sc; i++)
    {
        net.Add();
    }
    Console.WriteLine(net.ToString());
}

# endregion

