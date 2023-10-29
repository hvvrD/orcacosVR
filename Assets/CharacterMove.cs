using UnityEngine;

/// <summary>
/// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class CharacterMove : MonoBehaviour
{
    private Vector3 charVelocity;
    private float charSpeed = 100;
    private float charJumpHeight = 1;
    private float gravity = 9;
    private CharacterController charController;

    private Vector3 charRotation;
    private float sensitivity = 2f;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //GameManager.Instance.OnMove += SetMove;
        GameManager.Instance.OnMoveAxis += SetMoveAxis;
        GameManager.Instance.OnJump += SetJump;
        GameManager.Instance.OnLookAxis += SetLook;
    }

    private void OnDestroy()
    {
        //GameManager.Instance.OnMove -= SetMove;
        GameManager.Instance.OnMoveAxis -= SetMoveAxis;
        GameManager.Instance.OnJump -= SetJump;
        GameManager.Instance.OnLookAxis -= SetLook;
    }

    private void SetMove(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                charController.Move(new Vector3(0, 0, -1));
                break;
            case KeyCode.A:
                charController.Move(new Vector3(-1, 0, 0));
                break;
            case KeyCode.S:
                charController.Move(new Vector3(0, 0, 1));
                break;
            case KeyCode.D:
                charController.Move(new Vector3(1, 0, 0));
                break;
        }
    }

    private void SetMoveAxis(Vector3 move)
    {
        move = transform.TransformDirection(move);
        move.y = 0;

        charController.Move(move * Time.deltaTime * charSpeed);
    }

    private void SetJump()
    {
        
    }

    private void SetLook(float mouseX, float mouseY)
    {
        charRotation.x += mouseX * sensitivity;
        charRotation.y += mouseY * sensitivity;
        charRotation.y = Mathf.Clamp(charRotation.y, -90f, 90f);
        var xQuat = Quaternion.AngleAxis(charRotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(charRotation.y, Vector3.left);

        transform.localRotation = xQuat * yQuat;
    }
}