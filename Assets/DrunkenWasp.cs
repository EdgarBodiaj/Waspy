using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenWasp : MonoBehaviour
{
	public enum State { Fly, Hover, Inspect, Land, Idle, Gone };

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

	private float waitUntil = 0f;

	private Vector3 seekPosition;
    private Vector3 waspVelocity;
	private Vector3 currentTarget;
	private State state = State.Hover;
    
    //List of Target Points
    private List<GameObject> targets = new List<GameObject>();
    //List of Walk areas
    private List<GameObject> planes = new List<GameObject>();
	private List<GameObject> spawns = new List<GameObject>();

	private AudioSource buzzing;

    // Start is called before the first frame update
    void Start()
    {
		waspSkeleton = GetComponent<Animator>();
		buzzing = GetComponent<AudioSource>();
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
		for (int i = 0; i < GameObject.FindGameObjectsWithTag("Spawn").Length; i++)
        {
            //Add all targets to list
            spawns.Add(GameObject.FindGameObjectsWithTag("Spawn")[i]);
        }

		if(spawns.Count > 0) {
			var index = Random.Range(0, spawns.Count);
			transform.position = spawns[index].transform.position;
		}

		target = null;
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
			waitUntil = Time.time + hoverTime;
			state = State.Hover;
			Hover();
			return;
		}

		// TODO Collisions

		// Move seekPosition
		Vector3 seekVelocity = (currentTarget - seekPosition).normalized * ForwardVelocity;
        seekPosition += seekVelocity * Time.deltaTime;

		Float();
		transform.forward = (currentTarget - transform.position).normalized;
	}

	void Hover() {
		if(Time.time > waitUntil) {
			if(target == null) {
				var nearestDistance = float.MaxValue;
				GameObject nearest = null;
				foreach (var targetItem in targets)
				{
					var distance = (transform.position - targetItem.transform.position).magnitude;
					if(distance < nearestDistance) {
						nearest = targetItem;
						nearestDistance = distance;
					}
				}
				if(nearest == null) {
					foreach (var targetItem in spawns)
					{
						var distance = (transform.position - targetItem.transform.position).magnitude;
						if(distance < nearestDistance) {
							nearest = targetItem;
							nearestDistance = distance;
						}
					}
				}
				target = nearest;
			}
			var offsetTarget = target.transform.position + (target.transform.up.normalized * hoverDistance);
			if(currentTarget == offsetTarget) {
				if(target.tag == "Spawn") {
					state = State.Gone;
					return;
				} else {
					state = State.Land;
					Land();
					return;
				}
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
		if(target != null) {
			transform.forward = (target.transform.position - transform.position).normalized;
		}
	}

	void Float(float reduce = 1) {
		// TODO Collisions
		waspSkeleton.SetBool("Fly", true);
		waspSkeleton.SetBool("Finish", false);		
		if(!buzzing.isPlaying) {
			buzzing.Play();
		}
		float timing = Time.time * swerveSpeed;
		float offsetX = (Mathf.Sin(timing + Random.Range(-randomness, randomness)) * swerveAmount * reduce);
		float offsetY = (Mathf.Sin(timing + Random.Range(-randomness, randomness)) * swerveAmount * reduce);
		float offsetZ = (Mathf.Cos(timing + Random.Range(-randomness, randomness)) * swerveAmount * reduce);
		Vector3 offset = new Vector3(offsetX, offsetY, offsetZ); 
		Vector3 swerveTarget = seekPosition + offset;

        var desiredVelocity = swerveTarget - transform.position;
        desiredVelocity = desiredVelocity.normalized * MaxVelocity;

        var steering = desiredVelocity - waspVelocity;
        steering = Vector3.ClampMagnitude(steering, MaxForce);
        steering /= Mass;

        waspVelocity = Vector3.ClampMagnitude(waspVelocity + steering, MaxVelocity);

		var seekDist = (transform.position - seekPosition).magnitude;
		var velDist = waspVelocity.magnitude * Time.deltaTime;
		if(target != null && target.transform.position == seekPosition && velDist > seekDist) {
			velDist = waspVelocity.magnitude * reduce * Time.deltaTime;
			if(velDist > seekDist) {
				transform.position = seekPosition;		
			} else {
				transform.position += waspVelocity * reduce * Time.deltaTime;		
			}
		} else {
        	transform.position += waspVelocity * Time.deltaTime;
		}

        Debug.DrawRay(transform.position, waspVelocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
	}

	void Land() {
		if((transform.position - target.transform.position).magnitude < 0.01) {
			state = State.Idle;
			waitUntil = Time.time + hoverTime + Random.Range(-randomness, randomness);
			Idle();
			return;			
		}

		currentTarget = target.transform.position;
		seekPosition = target.transform.position;
		float dist = (target.transform.position - transform.position).magnitude;
		float reduce = Mathf.Clamp01(dist / hoverDistance) / 5;
		Float(reduce);

		transform.up = Vector3.RotateTowards(transform.up, target.transform.up, rotateSpeed * Time.deltaTime, 0f);
	}

	void Idle() {
		waspSkeleton.SetBool("Fly", false);
		waspSkeleton.SetBool("Finish", true);
		if(buzzing.isPlaying) {
			buzzing.Stop();
		}

		if(Time.time > waitUntil) {
			currentTarget = target.transform.position + (target.transform.up.normalized * hoverDistance * 3);
			targets.Remove(target);
			target = null;
			state = State.Fly;
			Fly();
			return;
		}
	}
}
