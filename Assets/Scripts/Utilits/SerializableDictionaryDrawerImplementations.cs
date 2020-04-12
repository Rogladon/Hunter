using UnityEngine;
using UnityEngine.UI;
 
using UnityEditor;

#if UNITY_EDITOR

// ---------------
//  int => Animator 
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(IntAnimatorDictionary))]
public class IntAnimatorDictionaryDrawer : SerializableDictionaryDrawer<int, RuntimeAnimatorController> {
	protected override SerializableKeyValueTemplate<int, RuntimeAnimatorController> GetTemplate() {
		return GetGenericTemplate<SerializableIntAnimatorTemplate>();
	}
}
internal class SerializableIntAnimatorTemplate : SerializableKeyValueTemplate<int, RuntimeAnimatorController> { }


#endif
