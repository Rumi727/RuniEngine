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
                Vector3 rotation = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

                if (InputManager.GetKey(KeyCode.A))
                    transform.position -= transform.right * speed;
                if (InputManager.GetKey(KeyCode.D))
                    transform.position += transform.right * speed;
                if (InputManager.GetKey(KeyCode.S))
                    transform.position -= transform.forward * speed;
                if (InputManager.GetKey(KeyCode.W))
                    transform.position += transform.forward * speed;
                if (InputManager.GetKey(KeyCode.LeftShift))
                    transform.position -= transform.up * speed;
                if (InputManager.GetKey(KeyCode.Space))
                    transform.position += transform.up * speed;

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
