using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuniEngine
{
    public class Pos : MonoBehaviour
    {
        Vector2 rotation = Vector2.zero;

        void Update()
        {
            float speed;
            if (Input.GetKey(KeyCode.LeftControl))
                speed = 0.25f * Kernel.fpsUnscaledSmoothDeltaTime;
            else
                speed = 0.125f * Kernel.fpsUnscaledSmoothDeltaTime;

            {
                Vector3 rotation = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

                if (Input.GetKey(KeyCode.A))
                    transform.position -= transform.right * speed;
                if (Input.GetKey(KeyCode.D))
                    transform.position += transform.right * speed;
                if (Input.GetKey(KeyCode.S))
                    transform.position -= transform.forward * speed;
                if (Input.GetKey(KeyCode.W))
                    transform.position += transform.forward * speed;
                if (Input.GetKey(KeyCode.LeftShift))
                    transform.position -= transform.up * speed;
                if (Input.GetKey(KeyCode.Space))
                    transform.position += transform.up * speed;

                transform.localEulerAngles = rotation;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                Vector2 rotation = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 10;
                this.rotation += new Vector2(-rotation.y, rotation.x);
            }

            transform.localEulerAngles = rotation;
        }
    }
}
