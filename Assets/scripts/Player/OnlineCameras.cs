using UnityEngine;
using Unity.Netcode;
public class OnlineCameras : NetworkBehaviour
{
    [Header("Camera Refs")]
    public Camera cam;
    public AudioListener listener;

    [Header("Orbit")]
    public float mouseSensitivity = 0.12f;
    public float stickSensitivity = 120f;
    public bool invertY = false;
    public float minPitch = -30f;
    public float maxPitch = 70f;
    public float orbitDistance = 10f;
    public float orbitHeight = 6f;
    public float followSmooth = 6f;

    float yaw, pitch;
    PlayerScript_Multi move;

    void Awake()
    {
        move = GetComponent<PlayerScript_Multi>();
        if (!cam)      cam = GetComponentInChildren<Camera>(true);
        if (!listener) listener = GetComponentInChildren<AudioListener>(true);
    }

    public override void OnNetworkSpawn()
    {
        bool on = IsOwner;
        if (cam)      cam.enabled = on;
        if (listener) listener.enabled = on;

        if (!IsOwner)
        {
            enabled = false; // don't run camera math on non-owners
            return;
        }

        // Light default angle
        pitch = Mathf.Clamp(20f, minPitch, maxPitch);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!IsOwner || cam == null || move == null) return;

        // Read look from movement script (already filled by input)
        var li = move.lookInput;

        // Mouse vs stick feel
        bool usingMouse = UsingMouse();
        float sx = usingMouse ? mouseSensitivity : (stickSensitivity * Time.deltaTime);
        float sy = sx * (invertY ? 1f : -1f);

        yaw   += li.x * sx;
        pitch  = Mathf.Clamp(pitch + li.y * sy, minPitch, maxPitch);

        // Orbit offset
        Vector3 baseOff = new Vector3(0f, orbitHeight, -orbitDistance);
        Quaternion rot  = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 off     = rot * baseOff;

        // Smooth follow
        Vector3 desired = transform.position + off;
        cam.transform.position = Vector3.Lerp(cam.transform.position, desired, Time.deltaTime * followSmooth);
        AimAt(cam.transform, transform.position);
    }

    bool UsingMouse()
    {
        var m = UnityEngine.InputSystem.Mouse.current;
        if (m == null) return false;
        return m.delta.ReadValue().sqrMagnitude > 1e-6f;
    }

    void AimAt(Transform t, Vector3 target)
    {
        var r = Quaternion.LookRotation(target - t.position, Vector3.up);
        t.rotation = Quaternion.Slerp(t.rotation, r, Time.deltaTime * followSmooth);
    }
}

