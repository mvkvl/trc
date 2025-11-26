using Library;

# region === pragma: off ===
#pragma warning disable CS8321 // Локальная функция объявлена, но не используется
#endregion

try
{
    //LevelsNetworkTest();
    DelegatesTest();
} 
catch (Exception e)
{
    Console.WriteLine($"Application exception: {e}");
}


# region ==== delegates test ====

static void DelegatesTest()
{

    MockConnector c = new();
    
    static void thd1(TradeEvent t)
    {
        Console.WriteLine($"th1: new trade {t.Time:T} {t.SecCode} {t.Price} {t.Volume}");
    }
    static void thd2(TradeEvent t)
    {
        Console.WriteLine($"th2: new trade {t.Time:T} {t.SecCode} {t.Price} {t.Volume}");
    }

    c.Connect();
    c.TradeEvent += thd1;
    Thread.Sleep(5000);
    c.TradeEvent += thd2;
    Thread.Sleep(5000);
    c.TradeEvent -= thd2;
    Thread.Sleep(5000);
    c.TradeEvent -= thd1;
    Thread.Sleep(5000);
    c.Disconnect();
    Thread.Sleep(5000);

}

#endregion

#region ==== network tests ====

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

#region === test methods ====
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

# region === pragma: on ===
#pragma warning restore CS8321 // Локальная функция объявлена, но не используется
#endregion
