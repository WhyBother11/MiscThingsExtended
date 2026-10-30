using System.Reflection;
using HarmonyLib;
using MiscThingsExtended.Framework.Util;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;

namespace MiscThingsExtended;

public class ModEntry : Mod
{
    #nullable disable
    public static ModEntry Instance { get; private set; }
    
    #nullable restore

    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Harmony harmony = new(ModManifest.UniqueID);
        harmony.PatchAll();
        
        var annotatedMethods = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsDefined(typeof(TriggerActionGSQ_Holder)))
            .SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)).ToList();
        RegisterMethods(annotatedMethods);
        
        var dataAssetClasses = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsDefined(typeof(DataAsset)));
        RegisterDataAssets(dataAssetClasses);
        
        var registrableClasses = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsDefined(typeof(Registrable)));
        Register(registrableClasses);
    }
    
    private static void Register(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var registerMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == "Register");
            registerMethod.Invoke(null, new object[]{ ModBindings.Helper });
        }
    }
    
    private static void RegisterMethods(IEnumerable<MethodInfo> methods)
    {
        foreach (var method in methods)
        {
            switch (method)
            {
                case var _ when method.IsDefined(typeof(GSQ)): RegisterGameStateQuery(method); break;
                case var _ when method.IsDefined(typeof(Trigger)): RegisterTrigger(method); break;
                case var _ when method.IsDefined(typeof(TriggerAction)): RegisterTriggerAction(method); break;
            }
        }
    }

    private static void RegisterGameStateQuery(MethodInfo method)
    { 
        GameStateQuery.Register(method.Name.PrependId(), method.CreateDelegate<GameStateQueryDelegate>());
    }

    private static void RegisterTrigger(MethodInfo method)
    {
        TriggerActionManager.RegisterTrigger(method.Name.PrependId());
    }

    private static void RegisterTriggerAction(MethodInfo method)
    {
        TriggerActionManager.RegisterAction(method.Name.PrependId(), method.CreateDelegate<TriggerActionDelegate>());
    }
    
    private static void RegisterDataAssets(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var registerMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == "RegisterAsset");
            registerMethod.Invoke(null, new object[]
            {
                ModBindings.Helper, $"Mods/{Id}/{type.GetCustomAttribute<DataAsset>()!.Name}"
            });
        }
    }
}
