/**
 * Copyright (c) 2011-2012, Jean-Philippe Steinmetz
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of Jean-Philippe Steinmetz nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL JEAN-PHILIPPE STEINMETZ BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using UnityEngine;
using System.Collections;

/**
 * The CameraController, when applied to a camera, will follow a target from a distance.
 * 
 * @author Jean-Philippe Steinmetz
 * @author David Housky
 */
public class CameraController : MonoBehaviour
{
	/**
	 * The transform that describes the boundary that the camera should never move outside of.
	 */
	public Transform bounds = null;
	
	/**
	 * The distance in 3D space to maintain from the target.
	 */
	public Vector3 distanceFromTarget = new Vector3(0,1,-10);
	/// <summary>
	/// The distance from the target before the crab dies
	/// </summary>
	private Vector3 distanceFromTargetBeforeDeath;
	
	/// <summary>
	/// The position of the camera when the level starts
	/// </summary>
	private Vector3 initialPosition;
	/// <summary>
	/// The rotation of the camera when the level starts
	/// </summary>
	private Quaternion initialRotation;
	/// <summary>
	/// What the camera is looking at when the level starts
	/// </summary>
	private Transform initialLookAt;
	
	public Vector3 rotationFromTarget = new Vector3(0, 0, 0);
	
	/**
	 * The target that the camera will face. If set to null the camera will face the last
	 * known lookAt target.
	 */
	public Transform lookAt = null;
	
	public Transform target = null;
	
	public bool smoothX = true;
	public bool smoothY = true;
	public bool smoothZ = true;
	
	private Vector3 offsetPosition = Vector3.zero;
	
	private Vector3 targetPosition = Vector3.zero;
	
	private Vector3 offsetRotation = Vector3.zero;
	
	private float smoothStartTime = -1.0f;
	
	public float smoothTimeLength = 1.0f;
	
	/// <summary>
	/// Smooth time length when the level starts
	/// </summary>
	private float initialSmoothTimeLength;
	
	private bool firstUpdate = true;
	
	public Transform cameraShake = null;
	
	private float cameraShakeWeight = 0.0f;
	
	public bool useCurrentAsInitial = true;
	
	public bool ignoreTimeScale = true;
	
	
	public Transform CameraShake
	{
		get
		{
			return cameraShake;
		}
		set
		{
			cameraShake = value;
		}
	}
	
	
	/**
	 * The target that the camera will follow.
	 */
	public Transform Target
	{
		get
		{ 
			return target;
		}
		set
		{
			smoothStartTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
			target = value;
		}
	}
	
	public Vector3 DistanceFromTarget
	{
		get
		{ 
			return distanceFromTarget;
		}
		set
		{
			smoothStartTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
			distanceFromTarget = value;
		}
	}
	
	public Vector3 RotationFromTarget
	{
		get
		{ 
			return rotationFromTarget;
		}
		set
		{
			smoothStartTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
			rotationFromTarget = value;
		}
	}
	
	/**
	 * The speed at which to smooth the camera's movement when following the target.
	 */
	public float SmoothTimeLength
	{
		get
		{
			return smoothTimeLength;
		}
		set
		{
			smoothStartTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
			smoothTimeLength = value;
		}
	}

	
	
	private void Start()
	{
		smoothStartTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
		if (!useCurrentAsInitial)
		{
			this.transform.position = Target.position + distanceFromTarget;
			this.transform.rotation = Quaternion.Euler( rotationFromTarget );
		}
	}
	
	
	
	/**
	 * Recieves message to change camera's target
	 * 
	 * @param newTarget		New target for the camera to move with
	 * */
	private void OnChangeTarget( Transform newTarget )
	{
		
		Target = newTarget;
	}
	
	/**
	 * Starts a camera shake, with position and rotation amounts, for a given amount of time
	 * If amount is Vector3.zero, shake of that type will not apply 
	 * */
	public void ShakeCamera( Vector3 posAmount, Vector3 rotAmount, float time )
	{
		//positional shake
		if (posAmount != Vector3.zero)
		{
			iTween.ShakePosition( cameraShake.gameObject, iTween.Hash("amount", posAmount,
			                                                         "time", time,
			                                                         "islocal", true,
			                                                          "onupdate", "OnUpdateShakePosition",
			                                                          "onupdatetarget", this.gameObject));
		}
		
		//rotational shake
		if (rotAmount != Vector3.zero)
		{
			iTween.ShakeRotation( cameraShake.gameObject, iTween.Hash("amount", rotAmount,
			                                                         "time", time,
			                                                          "onupdate", "OnUpdateShakeRotation",
			                                                          "onupdatetarget", this.gameObject));
		}
		
		//tween weight from 1 to 0, to smooth effect of shake
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 1.0f,
		                                            "to", 0.0f,
		                                            "time", time,
		                                            "easeType", iTween.EaseType.easeInCirc,
		                                            "onupdate", "OnUpdateCameraShake",
		                                            "onupdatetarget", this.gameObject,
		                                            "oncomplete", "OnCompleteCameraShake",
		                                            "oncompletetarget", this.gameObject) );
	}
	
	/**
	 * Update method for camera shake weight tween
	 * */
	private void OnUpdateCameraShake( float weight )
	{
		cameraShakeWeight = weight;
	}
	
	/**
	 * Complete method for camera shake weight tween.
	 * Resets position and rotation data of shake object
	 * */
	private void OnCompleteCameraShake()
	{
		cameraShake.localPosition = Vector3.zero;
		cameraShake.localRotation = Quaternion.identity;
	}

	
	
	private float sCurve( float valA, float valB, float percent )
	{
		return Mathf.Lerp( valA, valB, percent / Mathf.Sqrt( 1 + percent * percent ) );
	}
	
	private Vector3 sCurve( Vector3 valA, Vector3 valB, float percent )
	{
		return new Vector3( sCurve( valA.x, valB.x, percent), sCurve( valA.y, valB.y, percent), sCurve( valA.z, valB.z, percent)  );
	}
	
	
	private void LateUpdate()
	{
		// Determine the new ideal position the camera should be. This is based upon the target's
		// current position and then offset the specified distance.
		if (!Target)
			return;
		
		
		
		Vector3 newPosition = Target.position + distanceFromTarget;
		
		
		
		if (newPosition != this.transform.position)
		{
			//if first update, set start time to now:
			if (firstUpdate)
			{
				targetPosition = Target.position;
				
				smoothStartTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
				
				offsetPosition = this.transform.position - Target.position;
				offsetRotation = this.transform.rotation.eulerAngles;
				if (!useCurrentAsInitial)
				{
					offsetPosition = distanceFromTarget;
					offsetRotation = rotationFromTarget;
				}
				offsetRotation.x = offsetRotation.x % 180;
				offsetRotation.y = offsetRotation.y % 180;
				offsetRotation.z = offsetRotation.z % 180;
				
				initialSmoothTimeLength = smoothTimeLength;
				initialPosition = this.transform.position;
				initialRotation = this.transform.rotation;
				initialLookAt = lookAt;
				//distanceFromTargetBeforeDeath = distanceFromTarget;
				Debug.Log("Offset Rotation: " + offsetRotation);
				
				firstUpdate = false;
			}
			
			// Interpolate from the current position to the newly desired position
			float timeLeft = (smoothStartTime + smoothTimeLength) - ( (ignoreTimeScale ? Time.realtimeSinceStartup : Time.time));
			timeLeft = Mathf.Clamp( timeLeft, 0, float.MaxValue );
			float percent = 1.0f - Mathf.Clamp01( timeLeft / smoothTimeLength );
			
			/*if (percent == 0.0f)
				Debug.Log("done tweening camera");*/
			
			if (!ignoreTimeScale)
			{
				percent *= Time.timeScale;
			}
			
			//always smooth offsets
			offsetPosition = sCurve( offsetPosition, distanceFromTarget, percent );
			
			offsetRotation = sCurve( offsetRotation, rotationFromTarget, percent );
			
			//smooth specified axes of target:
			if (smoothX)
			{
				targetPosition.x = sCurve( targetPosition.x, Target.position.x, percent );
			}
			else
			{
				targetPosition.x = Target.position.x;
			}
			if (smoothY)
			{
				targetPosition.y = sCurve( targetPosition.y, Target.position.y, percent );
			}
			else
			{
				targetPosition.y = Target.position.y;
			}
			if (smoothZ)
			{
				targetPosition.z = sCurve( targetPosition.z, Target.position.z, percent );
			}
			else
			{
				targetPosition.z = Target.position.z;
			}
			
			
			if (cameraShake)
			{
				//add in camera shake position
				offsetPosition += cameraShake.localPosition * cameraShakeWeight;
				
				//add in camera shake rotation
				offsetRotation += cameraShake.localRotation.eulerAngles * cameraShakeWeight;
			}
			
			newPosition = targetPosition + offsetPosition;
			
			// If applicable, make sure the camera never moves outside of the boundary
			if (bounds)
			{
				SphereCollider sCol = bounds.GetComponent<SphereCollider>();
				BoxCollider bCol = bounds.GetComponent<BoxCollider>();
				
				if (sCol)
				{
					float[] scales = { bounds.localScale.x, bounds.localScale.y, bounds.localScale.z };
					float radius = Mathf.Max( scales );
					
					Vector3 distFromCenter = newPosition - bounds.position;
					if (distFromCenter.magnitude > radius)
					{
						distFromCenter = distFromCenter.normalized * radius;
					}
					newPosition = distFromCenter + bounds.position;
				} 
				if (bCol)
				{
					float xMin = bounds.position.x - (bounds.localScale.x / 2);
					float xMax = bounds.position.x + (bounds.localScale.x / 2);
					float yMin = bounds.position.y - (bounds.localScale.y / 2);
					float yMax = bounds.position.y + (bounds.localScale.y / 2);
					float zMin = bounds.position.z - (bounds.localScale.z / 2);
					float zMax = bounds.position.z + (bounds.localScale.z / 2);
					
					if (newPosition.x < xMin) newPosition.x = xMin;
					else if (newPosition.x > xMax) newPosition.x = xMax;
					
					if (newPosition.y < yMin) newPosition.y = yMin;
					else if (newPosition.y > yMax) newPosition.y = yMax;
					
					if (newPosition.z < zMin) newPosition.z = zMin;
					else if (newPosition.z > zMax) newPosition.z = zMax;
				}
			}
			
			this.transform.position = newPosition;
			
			this.transform.rotation = Quaternion.Euler( offsetRotation );
			
		}
		
		// Orient the camera to look at the look at target if applicable
		if (lookAt)
		{
			this.transform.rotation = Quaternion.LookRotation( (lookAt.position - this.transform.position).normalized );
		}
	}
	
	protected bool Reset() 
	{
		//DistanceFromTarget = distanceFromTargetBeforeDeath;
		lookAt = initialLookAt;
		firstUpdate = true;
		smoothStartTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
		SmoothTimeLength = initialSmoothTimeLength;
		this.transform.position = initialPosition;
		this.transform.rotation = initialRotation;
		return true;
	}
}
