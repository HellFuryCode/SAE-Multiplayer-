using UnityEngine;

public class Movement : MonoBehaviour 
{
    public float MoveSpeed = 10.0f;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate () 
	{
        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveH, 0.0f, moveV);

        if (player == this.gameObject)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                transform.Translate(movement * MoveSpeed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.LookRotation(movement);
            }
        } 
        else
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(movement * MoveSpeed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.LookRotation(movement);
            }
        }
    }
}
