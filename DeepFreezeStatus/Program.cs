using System;
using System.Collections.Generic;
using Utility.ModifyRegistry;
using Microsoft.Win32;   

namespace DeepFreezeStatus
{
  class Program
  {
    public static ModifyRegistry MyRegistry = new ModifyRegistry();
    public static DeepFreezeRegLocation KeyLocation;

    static void Main()
    {
      Console.WriteLine(GetDeepFreezeStatus());
#if DEBUG
      Console.ReadLine();
#endif
    }

    /// <summary>
    /// Searches for DeepFreeze and returns true if it is installed
    /// </summary>
    /// <returns></returns>
    public static bool IsDeepFreezeInstalled()
    {
      var locations = new List<DeepFreezeRegLocation>
                        {
                          new DeepFreezeRegLocation
                            {
                              BaseRegistryKey =
                                BaseRegistryKeyType.LocalMachine,
                              Path =
                                "SOFTWARE\\Faronics\\Deep Freeze 6",
                              Key = "DF Status"
                            },
                          new DeepFreezeRegLocation
                            {
                              BaseRegistryKey =
                                BaseRegistryKeyType.LocalMachine,
                              Path =
                                "SOFTWARE\\Faronics\\Deep Freeze 7",
                              Key = "DF Status"
                            }
                        };

      string value = null;
      var i = 0;
      while (value == null && i < locations.Count)
      {
        KeyLocation = locations[i];
        SetBaseRegistryKey(KeyLocation.BaseRegistryKey);
        MyRegistry.SubKey = KeyLocation.Path;
        value = MyRegistry.Read(KeyLocation.Key);
        i++;
      }

      return value != null;
    }

    public static void SetBaseRegistryKey(BaseRegistryKeyType type)
    {
      switch (type)
      {
        case BaseRegistryKeyType.LocalMachine:
          MyRegistry.BaseRegistryKey = Registry.LocalMachine;
          break;
        case BaseRegistryKeyType.CurrentUser:
          MyRegistry.BaseRegistryKey = Registry.CurrentUser;
          break;
      }
    }


    public static DeepFreezeStatus GetDeepFreezeStatus()
    {
      if (IsDeepFreezeInstalled())
      {
        var value = MyRegistry.Read(KeyLocation.Key);
        switch (value)
          {
          case "Thawed":
            return DeepFreezeStatus.Thawed;

          case "Maintenance mode":
            return DeepFreezeStatus.Maintenance;

          case "Frozen":
            return DeepFreezeStatus.Frozen;

              default:
            return DeepFreezeStatus.Unknown;
          }
      }
      return DeepFreezeStatus.NotInstalled;
    }
  }
}
