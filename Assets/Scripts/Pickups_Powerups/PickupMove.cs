using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMove : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody rb;

    // Update is called once per frame
    void Update()
    {
        // Move to the left
        Move(-1.0f, 0.0f);

        // If we have gone offscreen, destroy this object
        if (transform.position.x <= -12.0f)
        {
            Destroy(this.gameObject);
        }
    }

    public void Move(float xMove, float zMove)
    {
        // Create Vector3 direction to move in
        Vector3 direction = new Vector3(xMove, 0.0f, zMove);

        // Move the rigid body
        rb.MovePosition(transform.position + direction.normalized * moveSpeed * Time.deltaTime);
    }
}
