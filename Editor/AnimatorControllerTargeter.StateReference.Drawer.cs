using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moths.Animations.Internal 
{

    [CustomPropertyDrawer(typeof(AnimatorControllerTargeter.StateReference))]
    public class StateReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            PropertyField target = new PropertyField(property.FindPropertyRelative("target"), "");
            PropertyField state = new PropertyField(property.FindPropertyRelative("state"), "");

            target.style.width = Length.Percent(50);
            state.style.flexGrow = 1;

            root.style.display = DisplayStyle.Flex;
            root.style.flexDirection = FlexDirection.Row;


            root.Add(target);
            root.Add(state);

            return root;

        }
    }
}