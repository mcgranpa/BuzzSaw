using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortParticleSystem : MonoBehaviour {
	private ParticleSystemRenderer _particles;
	public string LayerName = "Particles";


	void Start () {
		_particles = GetComponent<ParticleSystemRenderer> ();
		_particles.sortingLayerName = LayerName;

	}

}







