using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        Vector2 movement = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) movement.y += 1f;
            if (Keyboard.current.sKey.isPressed) movement.y -= 1f;
            if (Keyboard.current.aKey.isPressed) movement.x -= 1f;
            if (Keyboard.current.dKey.isPressed) movement.x += 1f;
        }

        if (movement != Vector2.zero)
        {
            movement.Normalize();
        }

        transform.position += (Vector3)(movement * speed * Time.deltaTime);
    }
}
