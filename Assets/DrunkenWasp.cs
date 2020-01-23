using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenWasp : MonoBehaviour
{
	public float randomness = 1f;
	// adjust these to taste:
	public float swerveAmount = 3f;
	public float swerveSpeed = 2f; 
	//public GameObject seeker;
	public GameObject target;
	public Animator waspSkeleton;

	//public GameObject test;

	// distance from target at which swerving starts to reduce
	public float thresholdDist = 1f;  

   	public float Mass = 1f;
    public float MaxVelocity = 4f;
    public float MaxForce = 2f;
	public float ForwardVelocity = 1;

	private Vector3 seekPosition;
    private Vector3 waspVelocity;

    // Start is called before the first frame update
    void Start()
    {
		waspSkeleton = gameObject.GetComponent<Animator>();
		waspVelocity = Vector3.zero;
		seekPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(KeyCode.DownArrow))
			waspSkeleton.SetBool("Fly", false);
		else
			waspSkeleton.SetBool("Fly", true);
	}

	void FixedUpdate()
	{
		// Move seekPosition
		Vector3 seekVelocity = (target.transform.position - seekPosition).normalized * ForwardVelocity;
        seekPosition += seekVelocity * Time.deltaTime;

		float dist = (target.transform.position - seekPosition).magnitude;
		float reduce = Mathf.Clamp01(dist / thresholdDist);
		float timing = Time.time * swerveSpeed;
		float offsetX = (Mathf.Sin(timing + Random.Range(-randomness, randomness)) * swerveAmount * reduce);
		float offsetY = Random.Range(-randomness, randomness);
		float offsetZ = (Mathf.Cos(timing + Random.Range(-randomness, randomness)) * swerveAmount * reduce);
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
        transform.forward = (target.transform.position - transform.position).normalized;

        Debug.DrawRay(transform.position, waspVelocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
	}
}
