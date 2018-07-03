using System;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    protected Vector2 TargetVelocity;

    private Vector2 _velocity;
    private ContactFilter2D _contactFilter;
    private Vector2 move;

    private void Start()
    {
        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        _contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        TargetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {
    }

    private void FixedUpdate()
    {
        move = TargetVelocity * Time.deltaTime;
        transform.localPosition = new Vector2(transform.localPosition.x + move.x, transform.localPosition.y + move.y);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        move = RoundPosition(move * 100) / 100;
        if (Math.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            transform.localPosition = new Vector2(
                transform.localPosition.x - move.x, transform.localPosition.y + move.y
            );
        }

        if (Math.Abs(Input.GetAxis("Vertical")) > 0)
        {
            transform.localPosition = RoundPosition(new Vector2(
                transform.localPosition.x + move.x, transform.localPosition.y - move.y
            ));
        }
    }

    /// <summary>
    /// Round position 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private static Vector2 RoundPosition(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }
}