using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsMovement : MonoBehaviour 
{
	// public Transform target;

	// public float moveSpeed = 6.0;
	// public float rotationSpeed = 1.0;
	
	// private float minDistance = 5;
	// private float safeDistance = 60;
	
	// enum AIState {Idle, Seek, Flee, Arrive, Pursuit, Evade}
	// public AIState currentState = Idle;
	
	// function Update () {
	// 	switch(currentState){
	// 		case AIState.Idle:
	// 			break;
	// 		case AIState.Seek:
	// 			Seek();
	// 			break;
	// 		case AIState.Flee:
	// 			Flee();
	// 			break;
	// 		case AIState.Arrive:
	// 			Arrive();
	// 			break;
	// 		case AIState.Pursuit:
	// 			Pursuit();
	// 			break;
	// 		case AIState.Evade:
	// 			Evade();
	// 			break;
	// 	}
	// }
	
	// function Seek () {
	// 	Vector3 direction = target.position - transform.position;
	// 	direction.y = 0;
	
	// 	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
	
	// 	if(direction.magnitude > minDistance) {
	// 		Vector3 moveVector = direction.normalized * moveSpeed * Time.deltaTime;
	// 		transform.position += moveVector;
	// 	}
	// }
	
	// function Flee () : void{
	// 	Vector3 direction = transform.position - target.position;
	// 	direction.y = 0;
	
	// 	if(direction.magnitude < safeDistance){
	// 		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
	// 		Vector3 moveVector = direction.normalized * moveSpeed * Time.deltaTime;
	// 		transform.position += moveVector;
	// 	}
	// }
	
	// function Arrive () {
	// 	Vector3 direction = target.position - transform.position;
	// 	direction.y = 0;
	
	// 	float distance = direction.magnitude;
	// 	float decelerationFactor = distance / 5;
	// 	float speed = moveSpeed * decelerationFactor;
	
	// 	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
	
	// 	Vector3 moveVector = direction.normalized * Time.deltaTime * speed;
	// 	transform.position += moveVector;
	// }
	
	// function Pursuit () {
	// 	int iterationAhead = 30;
	
	// 	var targetSpeed = target.gameObject.GetComponent(Move).instantVelocity;
	
	// 	Vector3 targetFuturePosition = target.transform.position + (targetSpeed * iterationAhead);
	// 	Vector3 direction = targetFuturePosition - transform.position;
	// 	direction.y = 0;
	
	// 	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
	
	// 	if(direction.magnitude > minDistance) {
	// 		Vector3 moveVector = direction.normalized * moveSpeed * Time.deltaTime;
	// 		transform.position += moveVector;
	// 	}
	// }
	
	// function Evade () {
	// 	int iterationAhead = 30;
	// 	var targetSpeed = target.gameObject.GetComponent(Move).instantVelocity;
	
	// 	Vector3 targetFuturePosition = target.position + (targetSpeed * iterationAhead);
	// 	Vector3 direction = transform.position - targetFuturePosition;
	// 	direction.y = 0;
	
	// 	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
	
	// 	if(direction.magnitude < safeDistance{
	// 		Vector3 moveVector= direction.normalized * moveSpeed * Time.deltaTime;
	// 		transform.position += moveVector;
	// 	}
	// }
}
