#nullable enable
using RuniEngine.Inputs;
using UnityEngine;

namespace RuniEngine
{
    public class Test : MonoBehaviour
    {
        Vector2 rotation = Vector2.zero;

        void Update()
        {
            float speed;
            if (InputManager.GetKey(KeyCode.LeftControl))
                speed = 0.25f * Kernel.fpsUnscaledSmoothDeltaTime;
            else
                speed = 0.125f * Kernel.fpsUnscaledSmoothDeltaTime;

            {
                Vector3 motion = Vector3.zero;
                Vector3 rotation = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
                
                if (InputManager.GetKey(KeyCode.A))
                    motion -= transform.right * speed;
                if (InputManager.GetKey(KeyCode.D))
                    motion += transform.right * speed;
                if (InputManager.GetKey(KeyCode.S))
                    motion -= transform.forward * speed;
                if (InputManager.GetKey(KeyCode.W))
                    motion += transform.forward * speed;
                if (InputManager.GetKey(KeyCode.LeftShift))
                    motion -= transform.up * speed;
                if (InputManager.GetKey(KeyCode.Space))
                    motion += transform.up * speed;

                transform.position += motion;
                transform.localEulerAngles = rotation;
            }

            if (InputManager.GetKey(KeyCode.Mouse0))
            {
                Vector2 rotation = InputManager.pointerPositionDelta * 0.5f;
                this.rotation += new Vector2(-rotation.y, rotation.x);
            }

            transform.localEulerAngles = rotation;
        }
    }
}
