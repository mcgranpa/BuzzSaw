using UnityEngine;


public interface IFloatingTextPositioner
{
	bool GetPosition(ref Vector2 position, GUIContent cnntent, Vector2 size);
}

