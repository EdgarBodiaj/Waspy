using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenWasp : MonoBehaviour
{
	public enum State { Fly, Hover, Inspect, Land, Idle };

	public float randomness = 2f;
	public float swerveAmount = 3f;
	public float swerveSpeed = 2f; 

	public float hoverTime = 3f;

	public float flyDistance = 5f;
	public float hoverDistance = 1f;

   	public float Mass = 1f;
    public float MaxVelocity = 4f;
    public float MaxForce = 2f;
	public float ForwardVelocity = 1;
	public float rotateSpeed = 2f;


	private GameObject target;
	private Animator waspSkeleton;

	private float hoverWait = 0f;

	private Vector3 seekPosition;
    private Vector3 waspVelocity;
	private Vector3 currentTarget;
	private State state = State.Hover;
    
    //List of Target Points
    private List<GameObject> targets = new List<GameObject>();
    //List of Walk areas
    private List<GameObject> planes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
		waspSkeleton = gameObject.GetComponent<Animator>();
		waspVelocity = Vector3.zero;
		seekPosition = transform.position;

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
		
		if(targets.Count > 0) {
			var index = Random.Range(0, targets.Count);
			target = targets[index];
		}
        Debug.Log(targets.Count);
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
		if((currentTarget - seekPosition).magnitude < hoverDistance) {
			seekPosition = currentTarget;
			hoverWait = Time.time + hoverTime;
			state = State.Hover;
			Hover();
			return;
		}

		// Move seekPosition
		Vector3 seekVelocity = (currentTarget - seekPosition).normalized * ForwardVelocity;
        seekPosition += seekVelocity * Time.deltaTime;

		Float();
		transform.forward = (currentTarget - transform.position).normalized;
	}

	void Hover() {
		if(Time.time > hoverWait) {
			var offsetTarget = target.transform.position + (target.transform.up.normalized * hoverDistance);
			if(currentTarget == offsetTarget) {
				state = State.Land;
				Land();
				return;
			} else {
				float distance = (target.transform.position - transform.position).magnitude;
				if(distance <= flyDistance) 
				{
					currentTarget = offsetTarget;
				} else {
					currentTarget = transform.position + ((target.transform.position - transform.position).normalized * flyDistance);
					currentTarget = currentTarget + new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));
					//currentTarget = Quaternion.Euler(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), 0) * currentTarget;
					//Debug.Log(currentTarget);
				}
				state = State.Fly;
				Fly();
				return;
			}
		}

		Float();
		transform.forward = (target.transform.position - transform.position).normalized;
	}

	void Float() {
		waspSkeleton.SetBool("Fly", true);
		waspSkeleton.SetBool("Finish", false);		
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

        Debug.DrawRay(transform.position, waspVelocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
	}

	void Land() {
		waspSkeleton.SetBool("Fly", true);
		if((transform.position - target.transform.position).magnitude < 0.01) {
			state = State.Land;
			Idle();
			return;			
		}

		float dist = (target.transform.position - transform.position).magnitude;
		float reduce = Mathf.Clamp01(dist / hoverDistance);
        var desiredVelocity = target.transform.position - transform.position + (new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), Random.Range(-randomness, randomness)) * reduce);
        desiredVelocity = desiredVelocity.normalized * (MaxVelocity / 10);

        var steering = desiredVelocity - waspVelocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce);
        steering /= Mass;

        waspVelocity = Vector3.ClampMagnitude(waspVelocity + steering, (MaxVelocity / 10));
        transform.position += waspVelocity * Time.deltaTime;

        Debug.DrawRay(transform.position, waspVelocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);

		transform.up = Vector3.RotateTowards(transform.up, target.transform.up, rotateSpeed * Time.deltaTime, 0f);
	}

	void Idle() {
		waspSkeleton.SetBool("Fly", false);
		waspSkeleton.SetBool("Finish", true);
	}
}
