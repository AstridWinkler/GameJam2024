using UnityEngine;
using logiked.source.utilities;
using logiked.source.manager;
using logiked.source.types;
using logiked.source.extentions;
using logiked.source.attributes;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using logiked.editor;
using logiked.source.editor;
using UnityEditor;
#endif



namespace logiked.audio
{

    [System.Serializable]
    public class GameSound
    {


        #region Fields


        [SerializeField] protected bool useFileReference = false;

        [SerializeField] protected GameSoundFile sourceFile;

        [SerializeField] protected bool overrideFileVolume = false;
        [SerializeField] protected bool overrideFilePitch = false;



        // [SerializeField] string name;
        [NonReorderable]
        [SerializeField] protected AudioClip[] clips = new AudioClip[1];

        //Genere des bugs tres interessants
        //[RangeVfloat(0,99)]
        [SerializeField] protected vfloat volume = new vfloat(1, 0);//Assignement ne fonctionne pas

        //[RangeVfloat(1,2)]
        [SerializeField] protected vfloat pitch = new vfloat(1, 0);//Assignement ne fonctionne pas

        [SerializeField] Vector2 test = Vector2.one;
        #endregion



        #region Accesseurs publiques

        /// <summary>
        /// Retourne si le fichier est utilisé & utilisable.
        /// </summary>
        public bool IsFileValid => IsFileUsed && sourceFile != null;
        /// <summary>
        /// Si un fichier est utilisé
        /// </summary>
        public bool IsFileUsed => useFileReference;


        // public string Name => name;

        public AudioClip GetAClip => IsFileUsed ? sourceFile?.Sound.GetAClip : clips[Random.Range(0, clips.Length)];


        /// <summary>
        /// retourne le nombre de clips
        /// </summary>
        public AudioClip[] ClipList => IsFileUsed ? sourceFile?.Sound.clips.ToArray() : clips.ToArray();


        /// <summary>
        /// retourne le nombre de clips
        /// </summary>
        public int ClipCount => IsFileUsed ? (sourceFile ? sourceFile.Sound.ClipCount : 0) : clips.Length;

        /// <summary>
        /// retourne le volume randomisé de ce GameSound.
        /// </summary>
        public float VolumeAuto => IsFileValid && !overrideFileVolume ? sourceFile.Sound.volume : volume;

        /// <summary>
        /// retourne le pitch randomisé de ce GameSound.
        /// </summary>
        public float PitchAuto => IsFileValid && !overrideFilePitch ? sourceFile.Sound.pitch : pitch;

        /// <summary>
        /// Check si il n'y a aucun AudioClip à jouer sur ce son 
        /// </summary>
        public bool IsEmpty => ClipCount == 0 || (ClipCount == 1 && GetAClip == null);


        public string name
        {
            get
            {
                if (IsFileValid) return sourceFile.name;

                if (clips == null || clips.Length == 0 || clips[0] == null) return "null";
                return clips[0].name;
            }
        }

        #endregion


        #region Accesseurs Internal

        /// <summary>
        /// Accesseur pour le volume
        /// </summary>
        internal vfloat Volume { get => volume; set => volume = value; }
        /// <summary>
        /// Accesseur pour le pitch
        /// </summary>
        internal vfloat Pitch { get => pitch; set => pitch = value; }

        #endregion


        /// <summary>
        /// Copie du son sans les membres
        /// </summary>
        /// <returns>Nouveau son copié</returns>
        /*
        public GameSound CopySound()
        {
            return (GameSound)this.MemberwiseClone();
        }*/




        private void QuickPlayGenerate(Vector3 position, System.Action<GameAudioSource> func)
        {
            if (clips == null || clips.Length == 0)
                return;

            GameObject obj = new GameObject($"audioSource_{name}");
            obj.transform.SetParent(BaseGameManager.TempInstParent);
            obj.transform.position = position;

            obj.SetActive(false);//Pour stop le awake
            GameAudioSource p = obj.AddComponent<GameAudioSource>();

            func.Invoke(p);

            p.gameObject.SetActive(true);//Go awake
            p.PlaySound(this);

            var sound = p.CurrentlyPlaying;
            if (sound != null)
                obj.AddComponent<SimpleDestroyAfter>().time = sound.length / p.CurrentUsedPitch + 0.5f;


        }


        /// <summary>
        /// Joue le son à la position donnée
        /// </summary>
        /// <param name="position">La position du son</param>
        public void QuickPlay(Vector3 position, float range = 15)
        {
            QuickPlayGenerate(position, m =>
            {
                m.AudioSourceType = GameAudioSource.SoundSourceType.Generated;
                m.maxDistance = range;
            });
        }


        /// <summary>
        /// Joue le son à la position donnée
        /// </summary>
        /// <param name="position">La position du son</param>
        /// <param name="databaseSourePresset">Le presset d'audio source dans la Bdd</param>
        public void QuickPlay(Vector3 position, int databaseSourePresset)
        {
            QuickPlayGenerate(position, p =>
            {

                p.AudioSourceType = GameAudioSource.SoundSourceType.Presset;
                p.sourcePresset = databaseSourePresset;
            });
        }


        #region Constructeurs

        public GameSound(vfloat volume, vfloat pitch, params AudioClip[] clips)
        {
            useFileReference = false;
            this.clips = clips.ToArray();

            this.volume = volume;
            this.pitch = pitch;
        }

        public GameSound(params AudioClip[] clips) : this (new vfloat(1, 0), new vfloat(1, 0), clips)
        {                   
        
           /* try
            {*/
                this.volume = new vfloat(1, LogikedPlugin_AudioSystem.Instance.DefaultGameSoundVolumeVariation);
                this.pitch = new vfloat(1, LogikedPlugin_AudioSystem.Instance.DefaultGameSoundPitchVariation);
            /*}catch(System.Exception e)
            {
              //  Debug.Log("Architecture à revoir");
             //   Debug.LogException(e);
            }*/

            
        }


        #endregion





    }



#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(GameSound))]
    public class Property_GameSound : PropertyDrawer
    {
        private SerializedProperty currentProp;
        private SerializedProperty clips;
        private SerializedProperty volume;
        private SerializedProperty pitch;


        private SerializedProperty useFileReference;
        private SerializedProperty sourceFile;
        private SerializedProperty overrideFileVolume;
        private SerializedProperty overrideFilePitch;


        private bool open;
        GameSound sound;
        bool useFileRef;
        GameSoundFile file;
        GameSoundFile File
        {

            get
            {
                if (!useFileRef) return null;

                if (file == null)
                {
                    file = sourceFile.objectReferenceValue as GameSoundFile;
                }
                return file;
            }
        }


    private void OnStart(SerializedProperty property)
        {

            volume = currentProp.FindPropertyRelative(nameof(volume));
            pitch = currentProp.FindPropertyRelative(nameof(pitch));
            clips = currentProp.FindPropertyRelative(nameof(clips));
            useFileReference = currentProp.FindPropertyRelative(nameof(useFileReference));
            sourceFile = currentProp.FindPropertyRelative(nameof(sourceFile));
            overrideFileVolume = currentProp.FindPropertyRelative(nameof(overrideFileVolume));
            overrideFilePitch = currentProp.FindPropertyRelative(nameof(overrideFilePitch));
            
            if(sound.Volume.value == sound.Volume.variation && sound.Volume.value == 0 && sound.Pitch.value == sound.Pitch.variation && sound.Pitch.value == 0)
            {
                //Not set by unity by default
                sound.Volume = new vfloat(1,LogikedPlugin_AudioSystem.Instance.DefaultGameSoundVolumeVariation);
                sound.Pitch = new vfloat(1, LogikedPlugin_AudioSystem.Instance.DefaultGameSoundPitchVariation);
                property.serializedObject.Update();
            }
    

    }


        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rectIcon = new Rect(position.x + position.width - 20, position.y + 2, 18, 20);
            Rect rectIcon2 = new Rect(position.x + position.width - 38, position.y + 2, 18, 20);
            Rect rect1 = new Rect(position.x, position.y, position.width - 40, position.height);
            string fieldName = label.text;

            currentProp = property;

            if (volume == null)
            {
                sound = property.GetValue<GameSound>();
                OnStart(property);
            }

            GUIStyle unityStyle = GUI.skin.GetStyle("Label");
            unityStyle.richText = true;

            if (clips.arraySize == 0)
            {
                clips.arraySize++;
                property.serializedObject.ApplyModifiedProperties();
            }

            


            //label = new GUIContent(label.text);// + ((clips.arraySize > 1) ? "[{1} clips]".Format(label.text, clips.arraySize) : "")) ;// + " <Color=magenta><size=8> [sound]</size></Color>", "This string can be translated.");


            //  if (clips.arraySize > 1)
            //      GUILayout.BeginVertical("box");

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

             useFileRef = useFileReference.boolValue;

            void DrawPrefix()
            {
                StringBuilder end = new StringBuilder();

          

                end.Append("<b>");
                end.Append(fieldName);
                end.Append("</b> ");

                int c;

                if (sound.Volume.value == 0)
                {
                    end.Append("<color=\"red\">");
                    end.Append($"[volume=0]");
                    end.Append("</color>");
                }
                else if (sound.Pitch.value == 0)
                {
                    end.Append("<color=\"red\">");
                    end.Append($"[pitch=0]");
                    end.Append("</color>");
                }else
                    if ((c = sound.ClipCount) > 0)
                    end.Append($"[{c} clips]");

       
                EditorGUI.PrefixLabel(rect1, new GUIContent(end.ToString()), GUILogiked.Styles.Text_FieldRich);
            }


            if (!useFileRef)
            {
                if (clips.arraySize != 0)
                {
                    DrawPrefix();
                    EditorGUI.PropertyField(rect1, clips.GetArrayElementAtIndex(0), new GUIContent(" "));
                }

                if (open)
                {
                    EditorGUI.indentLevel++;

                    for (int i = 1; i < clips.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(clips.GetArrayElementAtIndex(i), new GUIContent($"variant {i}"));
                    }

                    EditorGUI.indentLevel--;
                }
            }

            else
            {

                DrawPrefix();

                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(rect1, sourceFile, new GUIContent(" "));
                if (EditorGUI.EndChangeCheck())
                {
                    file = null;
                    property.serializedObject.ApplyModifiedProperties();

 

                    if (sourceFile.objectReferenceValue != null)
                    {
                        if(!overrideFileVolume.boolValue)
                        sound.Volume = File.Sound.Volume;

                        if (!overrideFilePitch.boolValue)
                        sound.Pitch = File.Sound.Pitch;

                        property.serializedObject.Update();
                    }
                    else
                    {
 
                        //   sound.Volume = new vfloat(1, 0.1f);
                        //   sound.Pitch = new vfloat(1, 0.1f);
                    }
                }

            }


            //   if (clips.arraySize > 1)
            //       GUILayout.EndVertical();


            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;


            if (open)
            {
                GUI.color = new Color(0.2f, 0.2f, 0.2f);
                GUILayout.BeginVertical("box");
                GUI.color = Color.white;
                //    GUILayout.Label("<b><size=11><Color=magenta>Content</color></size></b>", unityStyle);


                //EditorGUILayout.PropertyField(clips);
    


                if (useFileRef  && File != null)
                {
                    EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(overrideFileVolume);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.serializedObject.ApplyModifiedProperties();
                        sound.Volume = File.Sound.Volume;
                    }
                    GUI.enabled = overrideFileVolume.boolValue;
                } 
                EditorGUILayout.PropertyField(volume);
                GUI.enabled = true;


                if (useFileRef && File != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(overrideFilePitch);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.serializedObject.ApplyModifiedProperties();
                        sound.Pitch = File.Sound.Pitch;
                    }
                    GUI.enabled = overrideFilePitch.boolValue;
                }
                EditorGUILayout.PropertyField(pitch);
                GUI.enabled = true;

                    GUILayout.Space(5);


                if (!useFileRef)
                {

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Variant Clip"))
                    {
                        clips.arraySize++;
                    }

                    GUI.enabled = clips.arraySize > 1;
                    if (GUILayout.Button("Remove Last Clip"))
                    {
                        clips.arraySize--;
                    }

                GUI.enabled = true;
                GUILayout.EndHorizontal();
                }
            }



            // Draw label
            //  position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label, unityStyle);







            GUILogiked.Panels.GUIDrawEditorIcon(() =>
            {
                open = !open; GUI.FocusControl(null);
            }, GUILogiked.Panels.EditorIconType.Gear, rectIcon, "Options du son" );


            GUILogiked.Panels.GUIDrawEditorIcon(() =>
            {
                useFileReference.boolValue = !useFileReference.boolValue;            
            }, GUILogiked.Panels.EditorIconType.FolderStar, rectIcon2, "Piocher les AudioClips et les settings dans un autre fichier ?");


            GUI.enabled = true;






            // EditorGUI.Popup(rectChoseTag, 0, ss);

            // Set indent back to what it was

            EditorGUI.EndProperty();

            EditorGUI.indentLevel = indent;

            if (open)
            {
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }
            else
            {
                if (clips.arraySize > 1)
                {
                    GUILayout.Space(8);
                }
            }
            
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18;
        }


    }
#endif




}