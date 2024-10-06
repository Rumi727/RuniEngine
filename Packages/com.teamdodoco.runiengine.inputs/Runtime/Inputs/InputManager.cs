using RuniEngine.Accounts;
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
            public static Dictionary<string, List<KeyCode[]>> controlList { get; set; } = new();
        }

        [UserData]
        public struct UserData
        {
            public static Dictionary<string, List<KeyCode[]>> controlList { get; set; } = new();
        }



        /// <summary>
        /// <para>KeyCode의 모든 값</para>
        /// <para>Any value of KeyCode</para>
        /// </summary>
        public static KeyCode[] unityKeyCodeList { get; } = (KeyCode[])Enum.GetValues(typeof(KeyCode));



        public static bool anyKeyDown => Input.anyKeyDown;
        public static bool anyKey => Input.anyKey;

        public static Vector2 pointerPosition
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Pointer.current.position.value;
#else
                return Input.mousePosition;
#endif
            }
        }

        public static Vector2 pointerPositionDelta
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Pointer.current.delta.value;
#else
                return Input.mousePositionDelta;
#endif
            }
        }

        public static Vector2 mouseScrollDelta
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current.scroll.value;
#else
                return Input.mouseScrollDelta;
#endif
            }
        }

        public static bool GetKeyDown(string key, string nameSpace = "", int priority = 0) => !IsInputLocked(priority) && InternalGeyKey(key, nameSpace, true, false);
        public static bool GetKey(string key, string nameSpace = "", int priority = 0) => !IsInputLocked(priority) && InternalGeyKey(key, nameSpace, false, false);
        public static bool GetKeyUp(string key, string nameSpace = "", int priority = 0) => !IsInputLocked(priority) && InternalGeyKey(key, nameSpace, false, true);

        public static bool GetKeyDown(string key, string nameSpace = "", params InputLocker[] locks) => !IsInputLocked(locks) && InternalGeyKey(key, nameSpace, true, false);
        public static bool GetKey(string key, string nameSpace = "", params InputLocker[] locks) => !IsInputLocked(locks) && InternalGeyKey(key, nameSpace, false, false);
        public static bool GetKeyUp(string key, string nameSpace = "", params InputLocker[] locks) => !IsInputLocked(locks) && InternalGeyKey(key, nameSpace, false, true);

        public static bool GetKeyDown(KeyCode keyCode, int priority = 0) => !IsInputLocked(priority) && Input.GetKeyDown(keyCode);
        public static bool GetKey(KeyCode keyCode, int priority = 0) => !IsInputLocked(priority) && Input.GetKey(keyCode);
        public static bool GetKeyUp(KeyCode keyCode, int priority = 0) => !IsInputLocked(priority) && Input.GetKeyUp(keyCode);

        public static bool GetKeyDown(KeyCode keyCode, params InputLocker[] locks) => !IsInputLocked(locks) && Input.GetKeyDown(keyCode);
        public static bool GetKey(KeyCode keyCode, params InputLocker[] locks) => !IsInputLocked(locks) && Input.GetKey(keyCode);
        public static bool GetKeyUp(KeyCode keyCode, params InputLocker[] locks) => !IsInputLocked(locks) && Input.GetKeyUp(keyCode);


        static bool InternalGeyKey(string key, string nameSpace, bool down, bool up)
        {
            List<KeyCode[]>? keyCodesList = null;
            if (UserData.controlList.ContainsKey(key))
                keyCodesList = UserData.controlList[key];
            else if (ProjectData.controlList.ContainsKey(key))
                keyCodesList = ProjectData.controlList[key];

            if (keyCodesList == null)
                return false;

            for (int i = 0; i < keyCodesList.Count; i++)
            {
                if (InternalGetKey(keyCodesList[i], down, up))
                    return true;
            }

            return false;
        }

        static bool InternalGetKey(KeyCode[] keyCodes, bool down, bool up)
        {
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



        public static bool IsInputLocked() => IsInputLocked(int.MinValue);

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
