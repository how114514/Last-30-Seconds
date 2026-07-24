using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UpgradeStage))]
public class UpgradeStageDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Find parent dimension to read its type
        var dimType = FindParentDimensionType(property);

        var priceProp  = property.FindPropertyRelative("price");
        var floatProp  = property.FindPropertyRelative("floatValue");
        var intProp    = property.FindPropertyRelative("intValue");
        var boolProp   = property.FindPropertyRelative("boolValue");

        int index = GetArrayIndex(property);
        label.text = $"Stage {index}";

        EditorGUI.BeginProperty(position, label, property);

        float lineH = EditorGUIUtility.singleLineHeight;
        float pad   = 2f;
        float y     = position.y;

        // Value — only the relevant one
        switch (dimType)
        {
            case DimensionType.Float:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineH), floatProp, new GUIContent("Value"));
                break;
            case DimensionType.Int:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineH), intProp, new GUIContent("Value"));
                break;
            case DimensionType.Bool:
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineH), boolProp, new GUIContent("Value"));
                break;
        }
        y += lineH + pad;

        // Price
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineH), priceProp);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight + 2f) * 2f;
    }

    /// <summary>Path like "attackDamage.stages.Array.data[0]" → find UpgradeDimension.type.</summary>
    private DimensionType FindParentDimensionType(SerializedProperty property)
    {
        var path = property.propertyPath;
        int dot = path.IndexOf('.');
        if (dot <= 0) return DimensionType.Float;

        string dimName = path.Substring(0, dot);
        var dimProp = property.serializedObject.FindProperty(dimName);
        if (dimProp == null) return DimensionType.Float;

        var typeProp = dimProp.FindPropertyRelative("type");
        return (DimensionType)typeProp.enumValueIndex;
    }

    private int GetArrayIndex(SerializedProperty property)
    {
        var path = property.propertyPath;
        int start = path.LastIndexOf('[');
        int end   = path.LastIndexOf(']');
        if (start < 0 || end < 0) return 0;
        return int.Parse(path.Substring(start + 1, end - start - 1));
    }
}
