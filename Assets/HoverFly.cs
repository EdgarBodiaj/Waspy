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

	public float flyDistance = 3f;
	public float hoverDistance = 1f;

	private State state = State.Hover;

	public float hoverTime = 2f;
	public int hoverMoves = 2;

    private Vector3 velocity;
	private Vector3 currentTarget;
	private float hoverWait = 0f;
	private int hoverCount = 0;

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
		//Debug.Log(state);
		if(state == State.Fly) {
			Fly();
		} else if(state == State.Hover) {
			Hover();
		} else if(state == State.Idle) {

		}
	}

	void Fly() {
		Debug.Log((currentTarget - transform.position).magnitude);
		var distance = (currentTarget - transform.position).magnitude;
		if(distance < hoverDistance) {
			// TODO Inspect
			state = State.Hover;
			hoverWait = Time.time + hoverTime;
			hoverCount = hoverMoves;
			Hover();
			return;
		}
		Vector3 desiredVelocity = (currentTarget - transform.position).normalized * MaxVelocity;
        var steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce);
        steering /= Mass;

        velocity = Vector3.ClampMagnitude(velocity + steering, MaxVelocity);
		if((velocity.magnitude * Time.deltaTime) > distance) {
			transform.position = currentTarget;
		} else {
			transform.position += velocity * Time.deltaTime;
			transform.forward = (currentTarget - transform.position).normalized;
		}

        Debug.DrawRay(transform.position, velocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
	}

	void Hover() {
		velocity = Vector3.zero;
		if(Time.time > hoverWait) {
			if(hoverCount > 0) {
				hoverCount--;
				hoverWait = Time.time + hoverTime;
				transform.position += new Vector3(Random.Range(-.2f,.2f),Random.Range(-.2f,.2f),Random.Range(-.2f,.2f));
			} else {
				float distance = (target.transform.position - transform.position).magnitude;
				if(distance <= hoverDistance) {
					state = State.Idle;
					return;
				}
				else if(distance <= flyDistance) 
				{
					currentTarget = target.transform.position;
				} else {
					currentTarget = transform.position + ((target.transform.position - transform.position).normalized * flyDistance);
					Debug.Log(transform.position);
					Debug.Log(currentTarget);
					Debug.Log(target.transform.position);
					currentTarget = currentTarget + new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));
					//currentTarget = Quaternion.Euler(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), 0) * currentTarget;
					Debug.Log(currentTarget);
				}
				state = State.Fly;
			}
		}
	}
}
