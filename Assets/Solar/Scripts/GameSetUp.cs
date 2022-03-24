using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetUp : MonoBehaviour {
	public enum StartCondition { InShip, OnBody }

	public StartCondition startCondition;
	public CelestialBody startBody;

	/*void Start () {
		PlayerController player = FindObjectOfType<PlayerController> ();
		Vector3 pointAbovePlanet = startBody.transform.position + Vector3.right * startBody.radius * 1.1f;
		player.transform.position = pointAbovePlanet;
		player.SetVelocity (startBody.initialVelocity);
		
		}*/
	}
