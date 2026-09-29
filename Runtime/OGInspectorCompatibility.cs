namespace Mobione.MobioneInspector
{
    public class SerializedMonoBehaviour : UnityEngine.MonoBehaviour
    {
    }

    public class SerializedScriptableObject : UnityEngine.ScriptableObject
    {
    }
}

namespace Mobione.Serialization
{
    public static class NamespaceMarker { }
}

namespace Mobione.Utilities
{
}

#if UNITY_EDITOR
namespace Mobione.MobioneInspector.Editor
{
    using UnityEditor;
    using UnityEngine;
}

namespace Mobione.Utilities.Editor
{
    using UnityEditor;
    using UnityEngine;
}
#endif
