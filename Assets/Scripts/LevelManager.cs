using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance { get; private set;}

	public int BonusCutoffSeconds;
	public int BonusSecondMultiplier;
	private int _savedPoints;

	public Player Player { get; private set;}
	public CameraController Camera   { get; private set;}

	public TimeSpan RunningTime {get {return DateTime.UtcNow - _started;}}
	public int CurrentTimeBonus { 
		get {
			var secondDifference = (int)(BonusCutoffSeconds - RunningTime.TotalSeconds);
			return Mathf.Max (0, secondDifference) * BonusSecondMultiplier;
		}
	}

	private List<Checkpoint> _checkpoints;
	private int _currentCheckpointIndex;
	private DateTime _started;


	public Checkpoint DebugSpawn;
	


	public void Awake()
	{
		_savedPoints = GameManager.Instance.Points;
		Instance = this;
	}

	public void Start()
	{
		_checkpoints = FindObjectsOfType<Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
		_currentCheckpointIndex = _checkpoints.Count > 0 ? 0 : -1;
		//Debug.Log ("cp " + _checkpoints.Count + " " + _currentCheckpointIndex);
		//Debug.Log ("cp0 " + _checkpoints[0]);
		//Debug.Log ("cp1 " + _checkpoints[1]);
		//Debug.Log ("cp2 " + _checkpoints[2]);
		Player = FindObjectOfType<Player>();
		Camera = FindObjectOfType<CameraController> ();
		_started = DateTime.UtcNow;

		var listeners = FindObjectsOfType<MonoBehaviour> ().OfType<IPlayerRespawnListener> ();
		foreach (var listener in listeners) {
			for (var i = _checkpoints.Count - 1; i >= 0; i--) {
				var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoints [i].transform.position.x;
				if (distance < 0)
					continue;
				_checkpoints [i].AssignObjectToCheckpoint (listener);
				break;

			}
		}

		//#if UNITY_EDITOR
//		if (DebugSpawn != null)
//			DebugSpawn.SpawnPlayer (Player);
//		else if (_currentCheckpointIndex != -1)
//			_checkpoints [_currentCheckpointIndex].SpawnPlayer (Player);
//#else
		if (_currentCheckpointIndex != -1)
			_checkpoints [_currentCheckpointIndex].SpawnPlayer (Player);
//#endif

	}

	public void Update()
	{
		var isAtLastCheckpoint = _currentCheckpointIndex + 1 >= _checkpoints.Count;
		if (isAtLastCheckpoint)
			return;

		var distanceToNextCheckpoint = _checkpoints [_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;
		if (distanceToNextCheckpoint >= 0)
			return;
		
		_checkpoints [_currentCheckpointIndex].PlayLeftCheckpoint ();
		_currentCheckpointIndex++;
		_checkpoints [_currentCheckpointIndex].PlayerHitCheckpoint ();
		Debug.Log ("add " + CurrentTimeBonus);
		GameManager.Instance.AddPoints (CurrentTimeBonus);
		_savedPoints = GameManager.Instance.Points;
		_started = DateTime.UtcNow;

	}

	public void GotoNextLevel(string levelName)
	{
		StartCoroutine (GotoNextLevelCo (levelName));
	}

	private IEnumerator GotoNextLevelCo(string levelName)
	{
		Player.FinishLevel ();
		GameManager.Instance.AddPoints (CurrentTimeBonus);

		FloatingText.Show (string.Format ("Level Complete!"),  
			"CheckPointText",
			new CenterTextPositioner (0.2f)); 
		yield return new WaitForSeconds (1f);
		FloatingText.Show (string.Format ("{0} Points!", GameManager.Instance.Points),  
			"CheckPointText",
			new CenterTextPositioner (0.1f)); 
		yield return new WaitForSeconds (5f);
		if (string.IsNullOrEmpty(levelName))
			SceneManager.LoadScene ("StartScreen");
		else
			SceneManager.LoadScene(levelName);
	}

	public void KillPlayer()
	{
		StartCoroutine (KillPlayerCo());
	}

	private IEnumerator KillPlayerCo()
	{
		Player.Kill ();
		Camera.IsFollowing = false;
		yield return new WaitForSeconds (2f);
		Camera.IsFollowing = true;

		if (_currentCheckpointIndex != -1)
			_checkpoints [_currentCheckpointIndex].SpawnPlayer (Player);
		_started = DateTime.UtcNow;
		GameManager.Instance.ResetPoints (_savedPoints);

	}


}

