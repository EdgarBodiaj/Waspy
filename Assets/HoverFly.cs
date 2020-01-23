using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverFly : MonoBehaviour
{
	public enum State { Fly, Hover, Idle };

	public float randomness = 1f;
	public GameObject target;

	// distance from target at which swerving starts to reduce
	public float thresholdDist = 1f;  

   	public float Mass = 1f;
    public float MaxVelocity = 4f;
    public float MaxForce = 2f;

	public float flyDistance = 5f;
	public float hoverDistance = 1f;

	public State state = State.Fly;

    private Vector3 velocity;
	private Vector3 currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void FixedUpdate()
	{
		if(state == State.Fly) {
			Fly();
		} else if(state == State.Hover) {

		} else if(state == State.Idle) {

		}
	}

	void Fly() {
		if(currentTarget == null) 
		{
			float distance = (target.transform.position - transform.position).magnitude;
			if(distance <= hoverDistance) {
				state = State.Hover;
				Hover();
				return;
			}
			else if(distance <= flyDistance) 
			{
				currentTarget = target.transform.position;
			} else {
				currentTarget = (target.transform.position - transform.position).normalized * hoverDistance;
				currentTarget = Quaternion.Euler(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), 0) * currentTarget;
			}
		} 

		if((currentTarget - transform.position).magnitude < hoverDistance) {
			// TODO Inspect
			state = State.Hover;
			Hover();
			return;
		}
		Vector3 desiredVelocity = (currentTarget - transform.position).normalized * MaxVelocity;
        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce);
        steering /= Mass;

        velocity = Vector3.ClampMagnitude(velocity + steering, MaxVelocity);
        transform.position += velocity * Time.deltaTime;
        transform.forward = (currentTarget - transform.position).normalized;

        Debug.DrawRay(transform.position, velocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
	}

	void Hover() {
		//
	}
}
