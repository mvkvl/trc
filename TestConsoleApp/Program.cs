using Base;
using Util;

LevelsNetworkTest();

# region ==== network tests ====

static void LevelsNetworkTest()
{
    #region ==== initialization ====

    var startLevel = ConsoleUtil.ReadDecimal("enter initial level");
    var stepsCount = ConsoleUtil.ReadDecimal("enter number of levels");
    var stepSize   = ConsoleUtil.ReadDecimal("enter stepSize");
    var direction = ConsoleUtil.ReadSide("enter network direction (S/L)");

    #endregion
    #region ==== tests ====

    //Console.WriteLine("\nTest non-uniform network:\n");
    //TestNonUniformNetwork(direction, startLevel, stepSize, stepsCount);

    Console.WriteLine("\nTest uniform network:\n");
    TestUniformNetwork(direction, startLevel, stepSize, stepsCount);

    #endregion
}

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
        }
        else
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

# endregion
