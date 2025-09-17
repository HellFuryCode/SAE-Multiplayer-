using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarry : MonoBehaviour
{
    public bool beenGrabbed { get; private set; }
    public Transform GrabParent { get; private set; }
    public float escapeNeeded = 12f; //amount of mashing to escape
    public float mashDecayPerSEC = 4f; //meter falls if you stop fighting

    public Transform currentGrabber;
    public Collider grabberMainCollider;

    private Rigidbody rb;
    private Collider col;
    private PlayerScript_Multi movement;
    private float mashMeter;

    public float MashFill01 => Mathf.Clamp01(escapeNeeded <= 0f ? 0f : (mashMeter/ escapeNeeded));

    public event Action OnGrabbed;
    public event Action OnReasled;
   
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        movement = GetComponent<PlayerScript_Multi>();
    }

    private void Update()
    {
        if (!beenGrabbed) return;
        {
            mashMeter = Mathf.Max(0f, mashMeter - mashDecayPerSEC * Time.deltaTime);

            if (mashMeter >= escapeNeeded)
            {
                TheEscape();
            }
        }
    }

    public void OnMash(InputValue v)
    {
        if (!beenGrabbed)
        {
            return;
        }

        if (!v.isPressed)
        {
            return;
        }

        mashMeter += 1f;
    }

    public void EnterGrab(Transform parent, Transform grabber, Collider grabberColliderToIgnore)
    {
        if (beenGrabbed)
        {
            return;
        }

        beenGrabbed = true;
        mashMeter = 0f;
        GrabParent = parent;
        currentGrabber = grabber;
        grabberMainCollider = grabberColliderToIgnore;

        rb.isKinematic = true;
        col.isTrigger = true;
        transform.SetParent(parent, worldPositionStays: true);

        if (grabberMainCollider)
        {
            Physics.IgnoreCollision(col, grabberMainCollider, true);
        }

        if (movement)
        {
            movement.enabled = true;
        }

        OnGrabbed.Invoke();

          }

    public void ExitGrab(Vector3 tossVelocity)
    {
        if (!beenGrabbed)
        {
            return;
        }

        transform.SetParent(null, worldPositionStays: true);
        rb.isKinematic = false;
        col.isTrigger = false;

        if (grabberMainCollider)
        {
            Physics.IgnoreCollision(col, grabberMainCollider, false);
        }

        rb.linearVelocity = tossVelocity;

        beenGrabbed = false;
        GrabParent = null;
        currentGrabber = null;
        grabberMainCollider = null;
        mashMeter = 0f;

        OnReasled?.Invoke();
          }


    public void TheEscape()
    {
        Vector3 away = Vector3.up * 2;
        if (currentGrabber) away += (transform.position - currentGrabber.position).normalized * 3f;

        ExitGrab(away);
    }
}
