#nullable enable
using Newtonsoft.Json;
using RuniEngine.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuniEngine.Inputs
{
    public static class InputManager
    {
        [ProjectData]
        public struct ProjectData
        {
            [JsonProperty] public static Dictionary<string, KeyCode[]> controlList { get; set; } = new();
        }

        [UserData]
        public struct UserData
        {
            [JsonProperty] public static Dictionary<string, KeyCode[]> controlList { get; set; } = new();
        }



        /// <summary>
        /// <para>KeyCode의 모든 값</para>
        /// <para>Any value of KeyCode</para>
        /// </summary>
        public static KeyCode[] unityKeyCodeList { get; } = (KeyCode[])Enum.GetValues(typeof(KeyCode));



        public static bool anyKeyDown => Input.anyKeyDown;
        public static bool anyKey => Input.anyKey;



        public static Vector2 pointerPosition => Pointer.current.position.value;
        public static Vector2 pointerPositionDelta => Pointer.current.delta.value;



        public static bool GetKeyDown(string key, int priority = 0) => !IsInputLocked(priority) && InternalGeyKey(key, true, false);
        public static bool GetKey(string key, int priority = 0) => !IsInputLocked(priority) && InternalGeyKey(key, false, false);
        public static bool GetKeyUp(string key, int priority = 0) => !IsInputLocked(priority) && InternalGeyKey(key, false, true);

        public static bool GetKeyDown(string key, params InputLocker[] locks) => !IsInputLocked(locks) && InternalGeyKey(key, true, false);
        public static bool GetKey(string key, params InputLocker[] locks) => !IsInputLocked(locks) && InternalGeyKey(key, false, false);
        public static bool GetKeyUp(string key, params InputLocker[] locks) => !IsInputLocked(locks) && InternalGeyKey(key, false, true);

        public static bool GetKeyDown(KeyCode keyCode, int priority = 0) => !IsInputLocked(priority) && Input.GetKeyDown(keyCode);
        public static bool GetKey(KeyCode keyCode, int priority = 0) => !IsInputLocked(priority) && Input.GetKey(keyCode);
        public static bool GetKeyUp(KeyCode keyCode, int priority = 0) => !IsInputLocked(priority) && Input.GetKeyUp(keyCode);

        public static bool GetKeyDown(KeyCode keyCode, params InputLocker[] locks) => !IsInputLocked(locks) && Input.GetKeyDown(keyCode);
        public static bool GetKey(KeyCode keyCode, params InputLocker[] locks) => !IsInputLocked(locks) && Input.GetKey(keyCode);
        public static bool GetKeyUp(KeyCode keyCode, params InputLocker[] locks) => !IsInputLocked(locks) && Input.GetKeyUp(keyCode);


        internal static bool InternalGeyKey(string key, bool down, bool up)
        {
            KeyCode[]? keyCodes = null;
            if (UserData.controlList.ContainsKey(key))
                keyCodes = UserData.controlList[key];
            else if (ProjectData.controlList.ContainsKey(key))
                keyCodes = ProjectData.controlList[key];

            if (keyCodes == null)
                return false;
            
            for (int i = 0; i < keyCodes.Length; i++)
            {
                KeyCode keyCode = keyCodes[i];
                if (i < keyCodes.Length - 1)
                {
                    if (Input.GetKey(keyCode))
                        continue;

                    return false;
                }
                else
                {
                    if (down)
                        return Input.GetKeyDown(keyCode);
                    else if (up)
                        return Input.GetKeyUp(keyCode);

                    return Input.GetKey(keyCode);
                }
            }
            
            return false;
        }



        public static bool IsInputLocked() => InputLocker.instances.Count > 0;

        public static bool IsInputLocked(int priority)
        {
            if (InputLocker.instances.Count <= 0)
                return false;

            return priority < InputLocker.instances.Max();
        }

        public static bool IsInputLocked(params InputLocker[] locks)
        {
            if (locks.Length <= 0 || InputLocker.instances.Count <= 0)
                return false;

            return locks.Max() < InputLocker.instances.Max();
        }
    }
}
