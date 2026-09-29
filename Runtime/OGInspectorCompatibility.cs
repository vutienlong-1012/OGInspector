namespace OGInspector
{
    public class SerializedMonoBehaviour : UnityEngine.MonoBehaviour
    {
    }

    public class SerializedScriptableObject : UnityEngine.ScriptableObject
    {
    }
}

namespace OGInspector.Serialization
{
    public static class NamespaceMarker { }
}

namespace OGInspector.Utilities
{
}

#if UNITY_EDITOR
namespace OGInspector.Editor
{
    using UnityEditor;
    using UnityEngine;
}

namespace OGInspector.Utilities.Editor
{
    using UnityEditor;
    using UnityEngine;
}
#endif
