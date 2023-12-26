namespace ScrappyChests;

public static class ConfigPresetDescriptions
{

    public static readonly string Default = """
        Default configuration for the mod:
        - Interactables and monsters drop scrap instead of items
        - Bonus printer spawn rate
        - Void items can be found in printers and cauldrons
        - Extra white and yellow cauldrons in the Bazaar and on the moon
        - Interactables that costs lunar coins, cost white items instead
        """;


    public static readonly string Easy = """
        Default configuration except:
        - Adaptive chests, legendary chests, void potential, void cradles and bosses drop items
        - Lunar interactables cost lunar coins
        - Red items are never replaced with scrap
        """;


    public static readonly string AllTheChoices = """
        Default configuration except:
        - Multishops, adaptive chests and void potentials drop items
        """;


    public static readonly string DefaultPrinterSpawnRate = """
        Default configuration except:
        - Default printer spawn rate
        """;


    public static readonly string NoPrinters = """
        Default configuration except:
        - Printers are disabled
        """;


    public static readonly string Hardcore = """
        Default configuration except:
        - Every chests and enemies drops scrap
        - Default printers spawn rate
        - No void printers in void seeds
        - No extra cauldrons
        - No speed item bonus
        """;


    public static readonly string V1 = """
        Equivalent to the 1.0 version of the mod.

        Default configuration except:
        - Default printers spawn rate
        - No void printers in void seeds
        - No extra cauldrons
        - No void items in printers and cauldrons
        - No speed items bonus
        - Lunar interactables cost lunar coins
        - Void cradles drops void items
        """;


    public static readonly string V1_1 = """
        Equivalent to the 1.1 version of the mod.
        
        Default configuration except:
        - No void printers in void seeds
        - No extra cauldrons
        - Lunar interactables cost lunar coins
        """;


    public static readonly string V1_2 = """
        Equivalent to the 1.2 version of the mod.
        
        Default configuration except:
        - Lunar interactables cost lunar coins (newt altars and lunar seers) 
        """;


    public static readonly string Vanilla = """
        Every changes are disabled.
        """;


    public static readonly string Custom = """
        Custom configuration
        """;

}
