#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
 
public class SpeakerAsset
{
	[MenuItem("Assets/Create/StoryBuilder/Speaker")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<Speaker> ();
	}
}
#endif