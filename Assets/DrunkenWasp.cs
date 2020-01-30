using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenWasp : MonoBehaviour
{
	public enum State { Fly, Hover, Land, Idle };

	public float randomness = 2f;
	public float swerveAmount = 3f;
	public float swerveSpeed = 2f; 
	public GameObject target;
	public Animator waspSkeleton;

	public float hoverTime = 3f;
	private float hoverWait = 0f;

	public float flyDistance = 5f;

   	public float Mass = 1f;
    public float MaxVelocity = 4f;
    public float MaxForce = 2f;
	public float ForwardVelocity = 1;
	public float rotateSpeed = 2f;

	private Vector3 seekPosition;
    private Vector3 waspVelocity;
	private Vector3 currentTarget;
	private State state = State.Hover;
    
    //List of Target Points
    List<GameObject> targets = new List<GameObject>();
    //List of Walk areas
    List<GameObject> planes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
		waspSkeleton = gameObject.GetComponent<Animator>();
		waspVelocity = Vector3.zero;
		seekPosition = transform.position;
        //targets.Add();
        Debug.Log(GameObject.FindGameObjectsWithTag("Target").Length);
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Target").Length; i++)
        {
            //Add all targets to list
            targets.Add(GameObject.FindGameObjectsWithTag("Target")[i]);
        }
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Plane").Length; i++)
        {
            //Add all targets to list
            planes.Add(GameObject.FindGameObjectsWithTag("Plane")[i]);
        }
        Debug.Log(targets.Count);
        Debug.Log(planes.Count);
    }

	void FixedUpdate()
	{
		if(state == State.Fly) {
			Fly();
		} else if(state == State.Hover) {
			Hover();
		} else if(state == State.Land) {
			Land();
		} else if(state == State.Idle) {
			Idle();
		}
	}

	void Fly()  {
		// Close enough to hover?
		if((currentTarget - seekPosition).magnitude < 1) {
			seekPosition = currentTarget;
			hoverWait = Time.time + hoverTime;
			state = State.Hover;
			Debug.Log("Hover");
			Hover();
			return;
		}

		// Move seekPosition
		Vector3 seekVelocity = (currentTarget - seekPosition).normalized * ForwardVelocity;
        seekPosition += seekVelocity * Time.deltaTime;

		Float();
	}

	void Hover() {
		if(Time.time > hoverWait) {
			if(currentTarget == target.transform.position) {
				state = State.Land;
				Debug.Log("Land");
				Land();
				return;
			} else {
				float distance = (target.transform.position - transform.position).magnitude;
				if(distance <= flyDistance) 
				{
					currentTarget = target.transform.position;
				} else {
					currentTarget = transform.position + ((target.transform.position - transform.position).normalized * flyDistance);
					currentTarget = currentTarget + new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));
					//currentTarget = Quaternion.Euler(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), 0) * currentTarget;
					Debug.Log(currentTarget);
				}
				state = State.Fly;
				Debug.Log("Fly");
				Fly();
				return;
			}
		}

		Float();
	}

	void Float() {
		waspSkeleton.SetBool("Fly", true);
		float timing = Time.time * swerveSpeed;
		float offsetX = (Mathf.Sin(timing + Random.Range(-randomness, randomness)) * swerveAmount);// * reduce);
		float offsetY = (Mathf.Sin(timing + Random.Range(-randomness, randomness)) * swerveAmount);
		float offsetZ = (Mathf.Cos(timing + Random.Range(-randomness, randomness)) * swerveAmount);// * reduce);
		Vector3 offset = new Vector3(offsetX, offsetY, offsetZ); 
		Vector3 swerveTarget = seekPosition + offset;
		//test.transform.position = swerveTarget;

        var desiredVelocity = swerveTarget - transform.position;
        desiredVelocity = desiredVelocity.normalized * MaxVelocity;

        var steering = desiredVelocity - waspVelocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce);
        steering /= Mass;

        waspVelocity = Vector3.ClampMagnitude(waspVelocity + steering, MaxVelocity);
        transform.position += waspVelocity * Time.deltaTime;

		if(state == State.Fly || currentTarget == target.transform.position) {
        	transform.forward = (currentTarget - transform.position).normalized;
		}

        Debug.DrawRay(transform.position, waspVelocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
	}

	void Land() {
		waspSkeleton.SetBool("Fly", false);
	}

	void Idle() {
		waspSkeleton.SetBool("Finish", true);
	}
}
