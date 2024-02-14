using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuniEngine
{
    public class Test : MonoBehaviour
    {
        void Update()
        {
            if (Vector3.zero != Input.mousePositionDelta)
            {
                Debug.Log($"Input System : {Pointer.current.delta.value}");
                Debug.Log($"Input : {Input.mousePositionDelta}");
            }
        }
    }
}
