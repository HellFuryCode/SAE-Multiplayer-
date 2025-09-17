
using UnityEngine;

public class CamFollow2Players : MonoBehaviour
{
    public enum SplitMode { Auto, AlwaysSingle, AlwaysSplit }  //for multple scene use so it can be resused and the vibe of the scene wont change
    public enum SplitAxis { VerticalLR, HorizontalTB } // VerticalLR = left/right (like Lego games), HorizontalTB = top/bottom (like COD)

    [Header("Players")]
    public Transform player1;
    public Transform player2;

    [Header("Cameras")]
    public Camera camA;     // p1
    public Camera camB;     // p2

    [Header("Behavior of the camera")]
    public SplitMode mode = SplitMode.Auto;
    public SplitAxis axis = SplitAxis.VerticalLR;
    public float splitDistance = 12f;  //Camera follow offset relative to target

    public float followSmooth = 6f;

    [Header("Single View Settings")]
    public float singlePullbackFactor = 0.75f;
    public float singleMinDist = 6f;
    public float singleMaxDist = 28f;


    [Header("UI Divider")]
    public RectTransform divider;         
    public float dividerWidth = 4f;       // pixels to keep it constant

    [Header("Orbit")]
    public float mouseSensitivity = 0.12f;
    public float stickSensitivity = 120f;
    public bool inveretY = false;
    public float minPitch = -30f;
    public float maxPitch = 70f;
    public float orbitDistance = 10f;
    public float orbitHeight = 6f;


   private  bool isSplit;
    // Transform _midProxyA;
    // Transform _midProxyB;

    float yawA, pitchA, yawB, pitchB;
    PlayerScript_Multi p1Move, p2Move;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; //very much needed
    }
    void Awake()
    {
        // Create cameras if not assigned 
        if (!camA) camA = CreateChildCamera("SplitCam_A");
        if (!camB) camB = CreateChildCamera("SplitCam_B");


        var alA = camA.GetComponent<AudioListener>() ?? camA.gameObject.AddComponent<AudioListener>();
        var alB = camB.GetComponent<AudioListener>();
        if (alB) Destroy(alB);

        SetSingleView();    // Start in single view



        p1Move = player1 ? player1.GetComponent<PlayerScript_Multi>() : null;
        p2Move = player2 ? player2.GetComponent<PlayerScript_Multi>() : null;

        if (p1Move) p1Move.Camera = camA.transform;
        if (p2Move) p2Move.Camera = camB.transform;

        yawA = yawB = 0F;
         pitchA = pitchB = Mathf.Clamp(20f, minPitch, maxPitch); //sslight tilt downn
    }

    Camera CreateChildCamera(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var cam = go.AddComponent<Camera>();

        // URP-friendly defaults so says the unity manuals
        cam.clearFlags = CameraClearFlags.Skybox; 
        cam.cullingMask = ~0;                     // Everything make sure its everything, becuase if its TransparentXR mask itll lose its shit
        cam.depth = 0;
        cam.rect = new Rect(0, 0, 1, 1);

        cam.allowHDR = true;
        cam.allowMSAA = true;
        cam.targetDisplay = 0;
        return cam;
    }

    void LateUpdate()
    {
        if (!player1 || !player2) return;

        float dist = Vector3.Distance(player1.position, player2.position);
        bool shouldSplit = mode == SplitMode.AlwaysSplit || (mode == SplitMode.Auto && dist > splitDistance);

        if (shouldSplit != isSplit)
        {
            isSplit = shouldSplit;
            if (isSplit) SetSplitView();
            else SetSingleView();
        }

        UpdateOrbitInput();

        if (isSplit) UpdateSplitCameras();
        else UpdateSingleCamera(dist);
    }


  
    void SetSplitView()  //making it look good
    {
        // Two cameras, two rects
        if (axis == SplitAxis.VerticalLR) //Lego
        {
            camA.rect = new Rect(0f, 0f, 0.5f, 1f); // left
            camB.rect = new Rect(0.5f, 0f, 0.5f, 1f); // right
        }
        else // HorizontalTB //COD
        {
            camA.rect = new Rect(0f, 0.5f, 1f, 0.5f); // top
            camB.rect = new Rect(0f, 0f, 1f, 0.5f);   // bottom
        }

        camA.enabled = true;
        camB.enabled = true;

        if (divider)
        {
            divider.gameObject.SetActive(true);
            if (axis == SplitAxis.VerticalLR)
            {
                divider.anchorMin = new Vector2(0.5f, 0f);
                divider.anchorMax = new Vector2(0.5f, 1f);
                 divider.sizeDelta = new Vector2(dividerWidth, 0f);
                divider.anchoredPosition = Vector2.zero;
            }
            else
            {
                divider.anchorMin = new Vector2(0f, 0.5f);
                 divider.anchorMax = new Vector2(1f, 0.5f);
                divider.sizeDelta = new Vector2(0f, dividerWidth);
                divider.anchoredPosition = Vector2.zero;
            }
        }
    }

    void SetSingleView() //duh
    {
        camA.rect = new Rect(0f, 0f, 1f, 1f);
        camA.enabled = true;

        camB.enabled = false;
        camB.rect = new Rect(0f, 0f, 1f, 1f);

        if (divider) divider.gameObject.SetActive(false);
    }

    private void UpdateOrbitInput()
    {
  
        //p1
        if (p1Move != null)
        {
            var li = p1Move.lookInput;
            float sx = UseMouse() ? mouseSensitivity : (stickSensitivity * Time.deltaTime);
            float sy = sx * (inveretY ? 1f : -1f);

            yawA += li.x * sx;
            pitchA = Mathf.Clamp(pitchA + li.y * sy, minPitch, maxPitch);
        }

        //P2
        if (p2Move != null)
        {
            var li = p2Move.lookInput;
            float sx = UseMouse() ? mouseSensitivity : (stickSensitivity * Time.deltaTime);
            float sy = sx * (inveretY ? 1f : -1f);

            yawB += li.x * sx;
            pitchB = Mathf.Clamp(pitchB + li.y * sy, minPitch, maxPitch);
        }
    }

    private bool UseMouse()
    {
        var m = UnityEngine.InputSystem.Mouse.current;
        if (m == null) return false;

        var d = m.delta.ReadValue();
        return d.sqrMagnitude > 0.000001f;
    }

   private Vector3 OrbitOffset(float pitchDeg, float yawDeg)
    {
        var baseOffset = new Vector3(0f, orbitHeight, -orbitDistance);
        var rot = Quaternion.Euler(pitchDeg, yawDeg, 0f);
        return rot * baseOffset;
    }

    //logic for following players
    private void UpdateSplitCameras()
    {
        //player 1
        var offA = OrbitOffset(pitchA, yawA);
        Vector3 desiredA = player1.position + offA;
        camA.transform.position = Vector3.Lerp(camA.transform.position, desiredA, Time.deltaTime * followSmooth);
        AimAt(camA.transform, player1.position);

        //player 2
        var offB = OrbitOffset(pitchB, yawB);
        Vector3 desiredB = player2.position + offB;
        camB.transform.position = Vector3.Lerp(camB.transform.position, desiredB, Time.deltaTime * followSmooth);
        AimAt(camB.transform, player2.position);
    }

    void UpdateSingleCamera(float dist)   // Follow midpoint of the players and pull back a bit as they separate
    {
        Vector3 mid = (player1.position + player2.position) * 0.5f;

        var off = OrbitOffset(pitchA, yawA); 
      
        float t = Mathf.InverseLerp(singleMinDist, singleMaxDist, dist);
        t = Mathf.Clamp01(t);
        Vector3 extraPull = off.normalized * (orbitDistance * singlePullbackFactor * t);

        Vector3 desired = mid + off + extraPull;
          
        camA.transform.position = Vector3.Lerp(camA.transform.position, desired, Time.deltaTime * followSmooth);
        AimAt(camA.transform, mid);
    }

 

    void AimAt(Transform camTf, Vector3 lookPos)
    {
        var rot = Quaternion.LookRotation(lookPos - camTf.position, Vector3.up);
        camTf.rotation = Quaternion.Slerp(camTf.rotation, rot, Time.deltaTime * followSmooth);
    }
}

