#if UNITY_EDITOR

using logiked.source.utilities;
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using logiked.source.extentions;
using System.Linq;

namespace logiked.source.editor
{


    public static class GUILogiked
    {


        public static class Styles
        {

            /// <summary>
            /// Update THe Main class when editor reload.
            /// </summary>
           // [InitializeOnLoad]
           // public class GUILogiked_StyleUpdate { static GUILogiked_StyleUpdate() { RefreshStyles(); } }


            public static GUIStyle Button_Magenta
            {
                get
                {
                    if (button_Magenta == null)
                    {
                        button_Magenta = new GUIStyle(GUI.skin.button);
                        button_Magenta.normal.textColor = Color.magenta;
                        button_Magenta.fontStyle = FontStyle.Bold;

                    }
                    return button_Magenta;
                }
            }
            private static GUIStyle button_Magenta;

            public static GUIStyle Button_NoBackgroundButton
            {
                get
                {
                    if (button_NoBackgroundButton == null)
                    {
                        button_NoBackgroundButton = new GUIStyle(GUI.skin.FindStyle("IconButton"));
                       /* button_NoBackgroundText.onActive.textColor = GUI.skin.button.onActive.textColor;
                        button_NoBackgroundText.onFocused.textColor = GUI.skin.button.onFocused.textColor + Color.white*0.2f;
                        button_NoBackgroundText.onHover.textColor = GUI.skin.button.onHover.textColor - Color.white * 0.2f;
                        button_NoBackgroundText.onNormal.textColor = GUI.skin.button.onNormal.textColor;*/

                    }
                    return button_NoBackgroundButton;
                }
            }

            private static GUIStyle button_NoBackgroundButton;


            public static GUIStyle BaseText { get
                {
                    if (text == null)
                    {
                        text = new GUIStyle(GUI.skin.label);
                        text.padding.top = 2;
                        text.wordWrap = true;
                        text.alignment = TextAnchor.MiddleLeft;
                    }
                    return text;
                }
            }
            private static GUIStyle text;

            public static GUIStyle Text_Little { get
                {
                    if (text_Little == null)
                    {
                        text_Little = new GUIStyle(BaseText);
                        text_Little.fontSize = 10;
                        text_Little.alignment = TextAnchor.MiddleCenter;
                    }
                    return text_Little;
                }
            }
            private static GUIStyle text_Little;

            public static GUIStyle Text_Magenta { get
                {
                    if (text_Magenta == null)
                    {
                        text_Magenta = new GUIStyle(BaseText);
                        text_Magenta.normal.textColor = Color.magenta;
                    }
                    return text_Magenta;
                }
            }
            private static GUIStyle text_Magenta;

 
            public static GUIStyle Text_Green { get
                {
                    if (text_Green == null)
                    {
                        text_Green = new GUIStyle(BaseText);
                        text_Green.normal.textColor = Color.green;
                    }
                    return text_Green;
                }
            }
            private static GUIStyle text_Green;

            public static GUIStyle Text_Bold
            {
                get
                {
                    if (text_Bold == null)
                    {
                        text_Bold = new GUIStyle(BaseText);
                        text_Bold.fontStyle = FontStyle.Bold;
                        text_Bold.wordWrap = false;
                    }
                    return text_Bold;
                }
            }
            private static GUIStyle text_Bold;


            public static GUIStyle Text_BoldCentered
            {
                get
                {
                    if (text_BoldCentered == null)
                    {
                        text_BoldCentered = new GUIStyle(Text_Bold);
                        text_BoldCentered.alignment = TextAnchor.MiddleCenter;
                    }
                    return text_BoldCentered;
                }
            }
            private static GUIStyle text_BoldCentered;


            public static GUIStyle Text_BigBold { get
                {
                    if (text_BigBold == null)
                    {
                        text_BigBold = new GUIStyle(BaseText);
                        text_BigBold.alignment = TextAnchor.MiddleLeft;
                        text_BigBold.fontSize = 14;
                        text_BigBold.fontStyle = FontStyle.Bold;
                    }
                    return text_BigBold;
                }
            }
            private static GUIStyle text_BigBold;

            public static GUIStyle Text_Big { get
                {
                    if (text_Big == null)
                    {
                        text_Big = new GUIStyle(BaseText);
                        text_Big.fontSize = 18;
                        text_Big.alignment = TextAnchor.MiddleLeft;
                    }
                    return text_Big;
                }
            }
            private static GUIStyle text_Big;

            public static GUIStyle Text_BlackBold { get
                {
                    if (text_BlackBold == null)
                    {
                        text_BlackBold = new GUIStyle(Text_BigBold);
                        text_BlackBold.normal.textColor = Color.black;

                    }
                    return text_BlackBold;
                }
            }
            private static GUIStyle text_BlackBold;

            public static GUIStyle Text_BlackRich { get
                {
                    if (text_BlackRich == null)
                    {
                        text_BlackRich = new GUIStyle();
                        text_BlackRich.richText = true;
                        text_BlackRich.alignment = TextAnchor.MiddleLeft;

                    }
                    return text_BlackRich;
                }
            }
            private static GUIStyle text_BlackRich;

            public static GUIStyle Box_OpaqueWhite
            {
                get
                {
                    if (box_OpaqueWhite == null)
                    {

                        box_OpaqueWhite = new GUIStyle();
                        box_OpaqueWhite.border = new RectOffset(5, 5, 5, 5);
                        box_OpaqueWhite.padding = new RectOffset(5, 5, 5, 5);
                        box_OpaqueWhite.margin = new RectOffset(0, 0, 0, 0);
                        box_OpaqueWhite.normal.background = EditorGUIUtility.whiteTexture;
        

                    }
                    return box_OpaqueWhite;
                }
            }
            private static GUIStyle box_OpaqueWhite;
          

            public static GUIStyle Box_Border 
            {
                get
                {
                    if (box_Border == null)
                    {
                        box_Border = new GUIStyle(EditorStyles.helpBox);     
                    }
                    return box_Border;
                }
            }
            private static GUIStyle box_Border;

            /*
            [Obsolete("Texture are unload when enter playmode. they need to be stored as asset.")]
            private static Texture2D GetBorderTexture(string base64)
            {
                var tex = new Texture2D(1, 1);
                tex.LoadImage(Convert.FromBase64String(base64));
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.filterMode = FilterMode.Bilinear;
                tex.Apply();
                tex.EncodeToPNG();
                return tex;
            }*/

            static Texture2D FindBorderTexture(string path)
            {
                var assets = AssetDatabase.FindAssets("t:texture2d " + path);
                if (assets.Length > 0) return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(assets[0]));
                return null;
            }

                 


            public static GUIStyle Box_OpaqueWindowWhite
            {
                get
                {
                    if (box_OpaqueWindowWhite == null)
                    {
          
                        box_OpaqueWindowWhite = new GUIStyle(EditorStyles.helpBox);
                        box_OpaqueWindowWhite.border = new RectOffset(8,8,8,8);
                        box_OpaqueWindowWhite.padding = new RectOffset(6, 6, 6, 6);

                        box_OpaqueWindowWhite.onNormal = new GUIStyleState(); 
                        box_OpaqueWindowWhite.onNormal.textColor = Color.white;
                    }

                    if (box_OpaqueWindowWhite.onNormal.background == null)
                    {
                        box_OpaqueWindowWhite.onNormal.background = FindBorderTexture("logiked_GuiStyleBorder1");
                        box_OpaqueWindowWhite.normal = box_OpaqueWindowWhite.onNormal;
                    }



                    return box_OpaqueWindowWhite;
                }
            }
            private static GUIStyle box_OpaqueWindowWhite;


            public static GUIStyle Box_OpaqueWindowDark
            {
                get
                {
                    if (box_OpaqueWindowDark == null)
                    {
                        box_OpaqueWindowDark = new GUIStyle(Box_OpaqueWindowWhite);
                        box_OpaqueWindowDark.name = "DarkWin";

                        box_OpaqueWindowDark.onNormal.background = FindBorderTexture("logiked_GuiStyleBorder2"); ;
                        box_OpaqueWindowDark.normal = box_OpaqueWindowDark.onNormal;
                    }                            

                    return box_OpaqueWindowDark;
                }
            }
            private static GUIStyle box_OpaqueWindowDark;







            public static GUIStyle Box_DragAndDrop
            {
                get
                {
                    if (box_DragAndDrop == null)
                    {
                        box_DragAndDrop = new GUIStyle(Box_OpaqueWindowDark);
                        box_DragAndDrop.fontStyle = FontStyle.Bold;
                        box_DragAndDrop.alignment =  TextAnchor.MiddleCenter;
                        box_DragAndDrop.fontSize =  14;

                        box_DragAndDrop.onHover = box_DragAndDrop.onNormal;
                        box_DragAndDrop.onHover.textColor = Color.green;
                        box_DragAndDrop.onFocused.textColor = Color.green;

                    }
                    return box_DragAndDrop;
                }
            }
            private static GUIStyle box_DragAndDrop;






            public static GUIStyle Box_Warning1
            {
                get
                {
                    if (box_warning1 == null)
                    {
                        box_warning1 = new GUIStyle(EditorStyles.helpBox);
                        box_warning1.normal.textColor = Color.yellow;
                        box_warning1.hover.textColor = Color.yellow;
                        box_warning1.onFocused.textColor = Color.yellow;
                        box_warning1.onActive.textColor = Color.yellow;
                    }
                    return box_warning1;
                }
            }
            private static GUIStyle box_warning1;

            public static GUIStyle Box_HelpBox1
            {
                get
                {
                    if (box_HelpBox1 == null)
                    {
                        box_HelpBox1 = new GUIStyle(EditorStyles.helpBox);
                        box_HelpBox1.normal.textColor = Color.gray + Color.white/3f;
                    }
                    return box_HelpBox1;
                }
            }
            private static GUIStyle box_HelpBox1;

            public static GUIStyle Box_ErrorBox1
            {
                get
                {
                    if (box_ErrorBox1 == null)
                    {
                        box_ErrorBox1 = new GUIStyle(EditorStyles.helpBox);
                        box_ErrorBox1.normal.textColor = Color.red;
                        box_ErrorBox1.hover.textColor = Color.red;
                        box_ErrorBox1.onFocused.textColor = Color.red;
                        box_ErrorBox1.onActive.textColor = Color.red;
                    }
                    return box_ErrorBox1;
                }
            }
            private static GUIStyle box_ErrorBox1;



            public static GUIStyle Box_ArraySectionBox
            {
                get
                {
                    if (box_ArraySectionBox == null)
                    {

                        box_ArraySectionBox = new GUIStyle(EditorStyles.helpBox);
                        box_ArraySectionBox.fontSize = 15;
                        box_ArraySectionBox.fontStyle = FontStyle.Bold;
                        box_ArraySectionBox.alignment = TextAnchor.MiddleCenter;
                        box_ArraySectionBox.padding = new RectOffset(1, 1, 1, 1);
                        box_ArraySectionBox.margin = new RectOffset(2, 2, 5,6);

                        box_ArraySectionBox.normal.textColor =  Color.white;
                        box_ArraySectionBox.border =   new RectOffset();

                    }
                    return box_ArraySectionBox;
                }
            }
            private static GUIStyle box_ArraySectionBox;







            //Getter parce que sinon ca crash au chargement
            private static GUIStyle text_FieldRich;
            public static GUIStyle Text_FieldRich
            {
                get
                {
                    if (text_FieldRich == null)
                    {
                        text_FieldRich = new GUIStyle(EditorStyles.label);
                        text_FieldRich.richText = true;
                    }
                    return text_FieldRich;
                }
            }


        }
       
        public static class StylesNodeBox
        {

            /// <summary>
            /// Update THe Main class when editor reload.
            /// </summary>
           /*
            [InitializeOnLoad]
            public class GUILogiked_StyleUpdate { static GUILogiked_StyleUpdate() { RefreshStyles(); } }
            private static void RefreshStyles(){}
           */

            private static GUIStyle nodeStyleNormal=null;
            private static GUIStyle nodeStyleBlueSelected = null;
            private static GUIStyle nodeStyleLime = null;
            private static GUIStyle nodeStyleLimeSelected = null;
            private static GUIStyle nodeStyleYellow = null;
            private static GUIStyle nodeStyleYellowSelected = null;
            private static GUIStyle nodeStyleGreen = null;
            private static GUIStyle nodeStyleGreenSelected = null;
            private static GUIStyle nodeStyleOrange = null;
            private static GUIStyle nodeStyleOrangeSelected = null;


            private static GUIStyle nodeStyleRed = null;
            private static GUIStyle nodeStyleRedSelected = null;

            #region Accessors
            public static GUIStyle NodeStyleDefault
            {
                get
                {
                    if (nodeStyleNormal == null)
                    { 
                        nodeStyleNormal = new GUIStyle();
                        nodeStyleNormal.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
                        nodeStyleNormal.border = new RectOffset(12, 12, 12, 12);
                        nodeStyleNormal.normal.textColor = Color.white;
                        nodeStyleNormal.alignment = TextAnchor.MiddleCenter;
                        nodeStyleNormal.fontSize = 14;
                    }
                    return nodeStyleNormal;
                }
            }

            public static GUIStyle NodeStyleDefaultSelected
            {
                get
                {
                    if (nodeStyleBlueSelected == null)
                    {
                        nodeStyleBlueSelected = new GUIStyle(NodeStyleDefault);
                        nodeStyleBlueSelected.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
                    }
                    return nodeStyleBlueSelected;
                }
            }
            public static GUIStyle NodeStyleLime
            {
                get
                {
                    if (nodeStyleLime == null)
                    {
                        nodeStyleLime = new GUIStyle(NodeStyleDefault);
                        nodeStyleLime.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
                    }
                    return nodeStyleLime;
                }
            }


            public static GUIStyle NodeStyleLimeSelected
            {
                get
                {
                    if (nodeStyleLimeSelected == null)
                    {
                        nodeStyleLimeSelected = new GUIStyle(NodeStyleDefault);
                        nodeStyleLimeSelected.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;
                    }
                    return nodeStyleLimeSelected;
                }
            }

            public static GUIStyle NodeStyleYellow
            {
                get
                {
                    if (nodeStyleYellow == null)
                    {
                        nodeStyleYellow = new GUIStyle(NodeStyleDefault);
                        nodeStyleYellow.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node4.png") as Texture2D;
                    }
                    return nodeStyleYellow;
                }
            }

            public static GUIStyle NodeStyleYellowSelected
            {
                get
                {
                    if (nodeStyleYellowSelected == null)
                    {
                        nodeStyleYellowSelected = new GUIStyle(NodeStyleDefault);
                        nodeStyleYellowSelected.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node4 on.png") as Texture2D;
                    }
                    return nodeStyleYellowSelected;
                }
            }


            public static GUIStyle NodeStyleGreen
            {
                get
                {
                    if (nodeStyleGreen == null)
                    {
                        nodeStyleGreen = new GUIStyle(NodeStyleDefault);
                        nodeStyleGreen.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
                    }
                    return nodeStyleGreen;
                }
            }
            public static GUIStyle NodeStyleGreenSelected
            {
                get
                {
                    if (nodeStyleGreenSelected == null)
                    {
                        nodeStyleGreenSelected = new GUIStyle(NodeStyleDefault);
                        nodeStyleGreenSelected.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3 on.png") as Texture2D;
                    }
                    return nodeStyleGreenSelected;
                }
            }



            public static GUIStyle NodeStyleOrange
            {
                get
                {
                    if (nodeStyleOrange == null)
                    {
                        nodeStyleOrange = new GUIStyle(NodeStyleDefault);
                        nodeStyleOrange.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node5.png") as Texture2D;
                    }
                    return nodeStyleOrange;
                }
            }
            public static GUIStyle NodeStyleOrangeSelected
            {
                get
                {
                    if (nodeStyleOrangeSelected == null)
                    {
                        nodeStyleOrangeSelected = new GUIStyle(NodeStyleDefault);
                        nodeStyleOrangeSelected.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node5 on.png") as Texture2D;
                    }
                    return nodeStyleOrangeSelected;
                }
            }




            public static GUIStyle NodeStyleRed
            {
                get
                {
                    if (nodeStyleRed == null)
                    {
                        nodeStyleRed = new GUIStyle(NodeStyleDefault);
                        nodeStyleRed.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6.png") as Texture2D;
                    }
                    return nodeStyleRed;
                }
            }
            public static GUIStyle NodeStyleRedSelected
            {
                get
                {
                    if (nodeStyleRedSelected == null)
                    {
                        nodeStyleRedSelected = new GUIStyle(NodeStyleDefault);
                        nodeStyleRedSelected.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6 on.png") as Texture2D;
                    }
                    return nodeStyleRedSelected;
                }
            }


            #endregion


        }

        public static class Panels
        {
            public enum EditorIconType
            {
                Gear,
                GearWhite,
                Unity,
                Folder,
                FolderWhite,
                RemoveCross,
                AddItem,
                ArrowUp,
                Save,
                ConfirmChecked,
                UnityIcon,
                Import,
                FolderStar,
                Audio,
                AudioPlay,
                AudioStop,
                TimerClock,
                SpeedClock,
                Play,
                PlayStep,
                Pause,
                AnimatorLink,
                SplitBar,
                EditPen,
                EditDotFill,
                EditDotFrame,
                EditDots,
                Error,
                Warning,
                BubbleInfo,
                LockerWhite,
                LockerDark,
                NodeConnectBlendTree,
                NodeConnectBubble1,
                NodeConnectBubble2,
                NodeConnectBubble3,
                NodeConnectBubble4,
                NodeConnectBubble5,
                NodeConnectBubble6,
                NodeConnectBubble7,
            };


            /// <summary>
            /// Draws an editor icon button
            /// </summary>
            /// <param name="menu">The displayed menu when button is clicked</param>
            public static void GUIDrawEditorIcon(GenericMenu menu, EditorIconType type = EditorIconType.Gear, string tooltip = "")
            {
                GUIDrawEditorIcon(() => menu.ShowAsContext(), type, tooltip);
            }
            public static void GUIDrawEditorIcon(Action act, EditorIconType type = EditorIconType.Gear, string tooltip = "")
            {
                //IconList :
                //https://gist.github.com/MattRix/c1f7840ae2419d8eb2ec0695448d4321
                //https://github.com/halak/unity-editor-icons


                var buttonRect = EditorGUILayout.GetControlRect(false, 16f, GUILayout.MaxWidth(16f));
                GUIDrawEditorIcon(act, type, buttonRect, tooltip);
            }



            private static Dictionary<EditorIconType, GUIContent> IconDictionary = new Dictionary<EditorIconType, GUIContent>();
            private static GUIStyle IconButtonStyle;
        
            public static GUIContent GetIconContent(EditorIconType icon)
            {
                string val = "_Popup";
                switch (icon)
                {
                    case EditorIconType.Gear: val = "_Popup"; break;
                    case EditorIconType.GearWhite: val = "d__Popup"; break;
                    case EditorIconType.Unity: val = "BuildSettings.SelectedIcon"; break;
                    case EditorIconType.Folder: val = "Folder Icon"; break;
                    case EditorIconType.FolderWhite: val = "d_Folder Icon"; break;
                    case EditorIconType.RemoveCross: val = "P4_DeletedLocal"; break;
                    case EditorIconType.AddItem: val = "P4_AddedRemote"; break;
                    case EditorIconType.ArrowUp: val = "UpArrow"; break;
                    case EditorIconType.Save: val = "SaveActive"; break;
                    case EditorIconType.ConfirmChecked: val = "Installed@2x"; break;
                    case EditorIconType.UnityIcon: val = "BuildSettings.Editor"; break;
                    case EditorIconType.Import: val = "Import-Available@2x"; break;
                    case EditorIconType.FolderStar: val = "d_FolderFavorite Icon"; break;
                    case EditorIconType.Audio: val = "d_Profiler.Audio@2x"; break;
                    case EditorIconType.AudioPlay: val = "d_PlayButton On@2x"; break;
                    case EditorIconType.AudioStop: val = "d_PreMatQuad@2x"; break;
                    case EditorIconType.TimerClock: val = "UnityEditor.ProfilerWindow@2x"; break;
                    case EditorIconType.SpeedClock: val = "d_SpeedScale"; break;
                    case EditorIconType.PlayStep: val = "d_StepButton On@2x"; break;
                    case EditorIconType.Pause: val = "d_PauseButton On@2x"; break;
                    case EditorIconType.Play: val = "d_PlayButton On@2x"; break;
                    case EditorIconType.AnimatorLink: val = "d_AnimatorController Icon"; break;
                    case EditorIconType.SplitBar: val = "d_VerticalSplit"; break;
                    case EditorIconType.EditPen: val =  "d_editicon.sml"; break;
                    case EditorIconType.EditDotFill: val = "DotFill"; break;
                    case EditorIconType.EditDotFrame: val = "DotFrame"; break;
                    case EditorIconType.EditDots: val = "d__Menu@2x"; break;
                    case EditorIconType.Error: val = "console.erroricon"; break;
                    case EditorIconType.Warning: val = "d_console.warnicon@2x"; break;
                    case EditorIconType.BubbleInfo: val = "console.infoicon"; break;
                    case EditorIconType.LockerWhite: val = "d_AssemblyLock"; break;
                    case EditorIconType.LockerDark: val = "AssemblyLock"; break;

                        
                    case EditorIconType.NodeConnectBlendTree: val = "BlendTree Icon"; break;
                    case EditorIconType.NodeConnectBubble1: val = "sv_icon_dot1_pix16_gizmo"; break;
                    case EditorIconType.NodeConnectBubble2: val = "sv_icon_dot2_pix16_gizmo"; break;
                    case EditorIconType.NodeConnectBubble3: val = "sv_icon_dot3_pix16_gizmo"; break;
                    case EditorIconType.NodeConnectBubble4: val = "sv_icon_dot4_pix16_gizmo"; break;
                    case EditorIconType.NodeConnectBubble5: val = "sv_icon_dot5_pix16_gizmo"; break;
                    case EditorIconType.NodeConnectBubble6: val = "sv_icon_dot6_pix16_gizmo"; break;
                    case EditorIconType.NodeConnectBubble7: val = "sv_icon_dot7_pix16_gizmo"; break;
                }

                if (!IconDictionary.ContainsKey(icon))
                    IconDictionary.Add(icon, EditorGUIUtility.IconContent(val));

              return IconDictionary[icon];
            }



            /// <summary>
            /// Dessine un petit bouton d'édition à coté d'un élement d'une liste, afin de pouvoir la modifier
            /// </summary>
            /// <param name="baseArray">La liste à modifier</param>
            /// <param name="selectedId">L'index dessiné</param>
            /// <param name="onPerformed">Le callback une fois l'action effectuée, avec la liste modifiée</param>
            /// <param name="allowDupplicate">Autoriser la duplication d'éléments ?</param>
            /// <returns>Le bouton à t-il été pressé ?</returns>
            public static bool DrawArrayElementContextButton(IList baseArray, int selectedId, Action<IList> onPerformed, bool allowDupplicate = true)
            {
                //Generic menu defition for each element

                if (baseArray == null) return false;

                IList list_result = null;



                GenericMenu arrayAction = new GenericMenu();



                void Callback()
                {
                    if (baseArray.GetType().Is<Array>())
                        onPerformed(list_result.CastListToArray());
                    else
                        onPerformed(list_result);
                }

                void FillResult()
                {
                    Type listElementType = baseArray.GetType().GetGenericArrayElementType();
                    Type listType = typeof(List<>).MakeGenericType(listElementType);
                    list_result = (IList)Activator.CreateInstance(listType);
                    foreach (var elem in baseArray)
                        list_result.Add(elem);
                }


                if (allowDupplicate)
                {
                    arrayAction.AddItem(new GUIContent("Duplicate array element"), false, () =>
                    {
                        GUI.FocusControl(null);
                        list_result.Insert(selectedId, list_result[selectedId]);
                        Callback();
                    });
                }

                arrayAction.AddItem(new GUIContent("Delete array element"), false, () =>
                {
                    GUI.FocusControl(null);
                    list_result.RemoveAt(selectedId);
                    Callback();
                });
                arrayAction.AddSeparator("");

                arrayAction.AddItem(new GUIContent("Move up"), () =>
                  {
                      GUI.FocusControl(null);
                      object e = list_result[selectedId - 1];
                      list_result[selectedId - 1] = list_result[selectedId];
                      list_result[selectedId] = e;
                      Callback();
                  }, selectedId == 0);

                arrayAction.AddItem(new GUIContent("Move down"), () =>
               {
                   GUI.FocusControl(null);
                   object e = list_result[selectedId + 1];
                   list_result[selectedId + 1] = list_result[selectedId];
                   list_result[selectedId] = e;
                   Callback(); ;
               }, selectedId == baseArray.Count - 1);




                bool pressed = false;

                GUILogiked.Panels.GUIDrawEditorIcon(() =>
                {
                    FillResult();
                    arrayAction.ShowAsContext();
                    pressed = true;
                }, GUILogiked.Panels.EditorIconType.EditDots);
                return pressed;

            }


            private static Action NullAct = () => { };

            public static void GUIDrawEditorIcon(EditorIconType type, Rect rect, string tooltip = "")
            {
                GUIDrawEditorIcon(NullAct, type, rect, tooltip);
            }

            public static void GUIDrawEditorIcon(EditorIconType type, string tooltip = "")
            {
                var buttonRect = EditorGUILayout.GetControlRect(false, 16f, GUILayout.MaxWidth(16f));
                GUIDrawEditorIcon(type, buttonRect, tooltip);
            }


            public static void GUIDrawEditorIcon(Action act, EditorIconType type, Rect rect, string tooltip ="")
            {
                var popupIcon = GetIconContent(type);
                popupIcon = new GUIContent(popupIcon.image, tooltip);

                if (IconButtonStyle == null)
                {
                    IconButtonStyle = GUI.skin.FindStyle("IconButton");
                    IconButtonStyle.clipping = TextClipping.Overflow;
                    IconButtonStyle.alignment = TextAnchor.MiddleCenter;
                    IconButtonStyle.fixedHeight = 0;
                    IconButtonStyle.fixedWidth = 0;
                    IconButtonStyle.richText = true;
                }

                var popupStyle = IconButtonStyle;

                if (act == NullAct)
                {
                    popupStyle = GUIStyle.none;          
                }




                EditorGUIUtility.SetIconSize(rect.size);

                if (GUI.Button(rect, popupIcon, popupStyle))
                {
                    act();                   
                }
                EditorGUIUtility.SetIconSize(Vector2.zero);


            }



            public enum ImageFitMode  {Fit, ClampInside}

            /// <summary>
            /// Draws the sprite on the screen at pos Dest
            /// </summary>
            public static void DrawSprite(Sprite sprite, Rect dest, ImageFitMode fitMode = ImageFitMode.Fit)
            {
                if (sprite == null) return;
                Rect sourceRect = new Rect(sprite.rect);
                sourceRect.xMin /= sprite.texture.width;
                sourceRect.xMax /= sprite.texture.width;
                sourceRect.yMin /= sprite.texture.height;
                sourceRect.yMax /= sprite.texture.height;

                Rect converted = dest;

                switch (fitMode)
                {

                    case ImageFitMode.ClampInside:

                        var spriteRect = sprite.rect;

                        if (dest.width / spriteRect.width > dest.height / spriteRect.height){
                            converted.height = dest.height; 
                            converted.width = (spriteRect.width / spriteRect.height ) * converted.height;
                        }
                        else {
                            converted.width = dest.width;
                            converted.height = (spriteRect.height / spriteRect.width) * converted.width;
                        }
                        converted.center = dest.center;
                        break;
                }




                GUI.DrawTextureWithTexCoords(converted, sprite.texture, sourceRect);
            }


            /// <summary>
            /// Creer un GenericMenu à partir d'une liste de chaines
            /// </summary>
            public static GenericMenu GenericMenuFromStrings(IEnumerable<string> list, Action<string> callback, string checkedValue = null)
            {   
                GenericMenu end = new GenericMenu();
                foreach (var e in list)
                    end.AddItem(new GUIContent(e), checkedValue != null && e == checkedValue, () => callback(e));
                return end;
            }

            /// <summary>
            /// Creer un GenericMenu à partir d'une liste de chaines
            /// </summary>
            public static GenericMenu GenericMenuFromStrings(IEnumerable<string> list, Action<int> callback, int checkedValue = -1)
            {
                GenericMenu end = new GenericMenu();
                string e;
                for (int i = 0; i < list.Count(); i++)
                {
                    e = list.ElementAt(i);
                    end.AddItem(new GUIContent(e), i == checkedValue, (x) => callback((int)x), i);
                }
                return end;
            }



        }
    }
}




#endif