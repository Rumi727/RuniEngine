#nullable enable
using System.Reflection;
using System;
using System.Linq;
using HarmonyLib;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace Rumi.VisualStudio.Nullable.Editor
{
    sealed class VisualStudioEditorPackagePatches
    {
        static readonly Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().First(static x => x.GetName().Name == "Unity.VisualStudio.Editor");
        static readonly Type type = assembly.GetType("Microsoft.Unity.VisualStudio.Editor.VisualStudioForWindowsInstallation");

        static readonly Type legacyType = assembly.GetType("Microsoft.Unity.VisualStudio.Editor.LegacyStyleProjectGeneration");
        static readonly Type sdkType = assembly.GetType("Microsoft.Unity.VisualStudio.Editor.SdkStyleProjectGeneration");

        static IGenerator legacy = (IGenerator)Activator.CreateInstance(legacyType);
        static IGenerator sdk = (IGenerator)Activator.CreateInstance(sdkType);

        static readonly Harmony harmony = new Harmony("Rumi.VisualStudio.Nullable.Editor");

        static bool isPathed = false;
        public static void Patch()
        {
            if (isPathed)
                return;

            isPathed = true;

            MethodInfo org = AccessTools.PropertyGetter(type, "ProjectGenerator");
            MethodInfo post = typeof(VisualStudioEditorPackagePatches).GetMethod("Post", BindingFlags.NonPublic | BindingFlags.Static);

            harmony.Patch(org, null, post);
        }

        static void Post(ref IGenerator __result)
        {
            if (CsprojSettingAssets.instance.enableSDKStyle)
                __result = sdk;
            else
                __result = legacy;
        }
    }
}