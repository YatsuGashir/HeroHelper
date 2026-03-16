using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using Core.Effects;

namespace Editor
{
    [CustomPropertyDrawer(typeof(GameEffectBase), true)]
    public class GameEffectDrawer : PropertyDrawer
    {
        private static List<Type> _cachedTypes;

        private static List<Type> EffectTypes
        {
            get
            {
                if (_cachedTypes == null)
                {
                    _cachedTypes = AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t =>
                            typeof(GameEffectBase).IsAssignableFrom(t) &&
                            !t.IsAbstract &&
                            !t.IsInterface)
                        .OrderBy(t => t.Name)
                        .ToList();
                }

                return _cachedTypes;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            object obj = property.managedReferenceValue;

            if (obj == null)
            {
                DrawAddButton(position, property);
                property.serializedObject.ApplyModifiedProperties();
                return;
            }

            DrawEffect(position, property, obj);

            property.serializedObject.ApplyModifiedProperties();
        }

        private void DrawAddButton(Rect position, SerializedProperty property)
        {
            GUIStyle style = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };

            if (GUI.Button(position, "➕ Добавить эффект", style))
            {
                ShowTypeSelectionMenu(property);
            }
        }

        private void DrawEffect(Rect position, SerializedProperty property, object obj)
        {
            string typeName = obj.GetType().Name.Replace("Effect", "");

            Rect headerRect = new Rect(position.x, position.y, position.width, 20);

            property.isExpanded = EditorGUI.Foldout(
                new Rect(headerRect.x, headerRect.y, headerRect.width - 60, 20),
                property.isExpanded,
                $"⚡ {typeName}",
                true);

            Rect changeButton = new Rect(headerRect.x + headerRect.width - 60, headerRect.y, 30, 18);
            Rect removeButton = new Rect(headerRect.x + headerRect.width - 30, headerRect.y, 30, 18);

            if (GUI.Button(changeButton, "↺"))
            {
                ShowTypeSelectionMenu(property);
            }

            if (GUI.Button(removeButton, "X"))
            {
                property.managedReferenceValue = null;
                return;
            }

            if (!property.isExpanded)
                return;

            EditorGUI.indentLevel++;

            SerializedProperty prop = property.Copy();
            int depth = prop.depth;

            float yOffset = 22;

            if (prop.NextVisible(true))
            {
                do
                {
                    if (prop.depth <= depth)
                        break;

                    if (prop.name == "m_Type")
                        continue;

                    float h = EditorGUI.GetPropertyHeight(prop, true);

                    Rect fieldRect = new Rect(
                        position.x,
                        position.y + yOffset,
                        position.width,
                        h);

                    EditorGUI.PropertyField(fieldRect, prop, true);

                    yOffset += h + 2;

                } while (prop.NextVisible(false));
            }

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            object obj = property.managedReferenceValue;

            if (obj == null)
                return 22;

            if (!property.isExpanded)
                return 20;

            float height = 22;

            SerializedProperty prop = property.Copy();
            int depth = prop.depth;

            if (prop.NextVisible(true))
            {
                do
                {
                    if (prop.depth <= depth)
                        break;

                    if (prop.name == "m_Type")
                        continue;

                    height += EditorGUI.GetPropertyHeight(prop, true) + 2;

                } while (prop.NextVisible(false));
            }

            return height;
        }

        private void ShowTypeSelectionMenu(SerializedProperty property)
        {
            GenericMenu menu = new GenericMenu();

            foreach (var type in EffectTypes)
            {
                string name = ObjectNames.NicifyVariableName(type.Name.Replace("Effect", ""));

                menu.AddItem(new GUIContent(name), false, () =>
                {
                    SetType(property, type);
                });
            }

            menu.ShowAsContext();
        }

        private void SetType(SerializedProperty property, Type type)
        {
            property.serializedObject.Update();

            try
            {
                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
            }
            catch (Exception e)
            {
                Debug.LogError($"Effect create error: {e}");
            }
        }
    }
}