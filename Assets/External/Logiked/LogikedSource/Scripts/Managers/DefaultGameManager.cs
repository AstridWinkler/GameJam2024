using logiked;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using logiked.source.extentions;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace logiked.source.manager {

    /// <summary>
    /// Game manager par défaut pour tous vos projets
    /// </summary>

    [AddComponentMenu("Logiked/DefaultGameManager")]
    public class DefaultGameManager : BaseGameManager
    {
        [SerializeField]
        private List<IBaseManager> managerOrderedList  = new List<IBaseManager>();



        protected override void InitManager()
        {

            for (int i = 0; i < managerOrderedList.Count; i++)
            {
                managerOrderedList[i].Initialization();
            }
        }


        void RefreshList()
        {
            var managerList = GetComponentsInChildren<IBaseManager>();

            var toAdd = managerList.SkipWhile(m => managerOrderedList.Contains(m) || m == this);
            var toRemove = managerOrderedList.SkipWhile(m => managerList.Contains(m) && m != this);


            managerOrderedList.RemoveAll(m => toRemove.Contains(m));
            managerOrderedList.AddRange(toAdd);
            managerOrderedList = managerOrderedList.Distinct().ToList();

        }

        protected override void UpdateGameManager()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                RefreshList();
#endif
        }




#if UNITY_EDITOR
        [CustomEditor(typeof(DefaultGameManager))]
        public class EditorDefaultGameManager : Editor
        {

            private Type[] AvailableManagersClasses
            {
                get
                {
                    if (availableManagersClasses == null)
                    {
                        var assbs = AppDomain.CurrentDomain.GetAssemblies().Where(m => m.FullName.Contains("Assembly") || m.GetName().Name.Contains("Logiked"));

                        availableManagersClasses = assbs.SelectMany(m => m
                                    .GetTypes()
                                    .Where(t => t.BaseType != null &&
                                                t.BaseType.IsGenericType &&
                                                t.IsAbstract == false &&
                                                t.BaseType.GetGenericTypeDefinition() == typeof(BaseManager<>))).ToArray();


                    }
                    return availableManagersClasses;
                }
            }

            private Type[] availableManagersClasses = null;




            public override void OnInspectorGUI()
            {
                var thisManager = (target as DefaultGameManager);
                base.OnInspectorGUI();

                GUILayout.Space(10);

                if (AvailableManagersClasses != null)
                {
                    var result = AvailableManagersClasses.Where(m => thisManager.managerOrderedList.FirstOrDefault(component => component.GetType() == m) == null).ToArray();


                    GUI.enabled = result.Length > 0;
                    if (GUILayout.Button("Add Manager"))
                    {
                        ShowGenerateManager(result);
                    }
                    GUI.enabled = true;

                }
            }


            public void ShowGenerateManager(Type[] types)
            {
                var thisManager = (target as DefaultGameManager);
                GenericMenu menu = new GenericMenu();



             



                // result.ord(m => m.Assembly.FullName.Contains("Logiked") );




                var logikedLst = types.Where(m => m.Assembly.FullName.Contains("Logiked")).ToArray();
                var others = types.Where(m => !m.Assembly.FullName.Contains("Logiked")).ToArray();
                Type selType;


                if (logikedLst.Length > 0)
                {
                    menu.AddDisabledItem(new GUIContent("==== Logiked ===="));
     
                    for (int i = 0; i < logikedLst.Length; i++)
                    {
                        selType = logikedLst[i];
                        menu.AddItem(new GUIContent(selType.Name), false, m =>
                   {
                       Undo.AddComponent(thisManager.gameObject, (Type)m);
                   }, selType);
                    }

                menu.AddSeparator("/");
                }


                if (others.Length > 0)
                {
                    menu.AddDisabledItem(new GUIContent("==== Others ===="));


                    for (int i = 0; i < others.Length; i++)
                    {
                        selType = others[i];
                        menu.AddItem(new GUIContent(selType.Name), false, m =>
                        {
                            Undo.AddComponent(thisManager.gameObject, (Type)m);
                        }, selType);
                    }

                }


                menu.ShowAsContext();


            }
        }


#endif
    }



}

