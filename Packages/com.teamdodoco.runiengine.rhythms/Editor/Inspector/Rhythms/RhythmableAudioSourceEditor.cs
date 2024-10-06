using RuniEngine.Rhythms;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor.Inspector.Sounds
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RhythmableAudioSource))]
    public class RhythmableAudioSourceEditor : CustomInspectorBase<RhythmableAudioSource>
    {
        [SerializeField] AudioClip? hitsound;

        float y = 0;
        int lastBeat = 0;
        public override void OnInspectorGUI()
        {
            UseProperty("_rhythmOffset", TryGetText("inspector.rhythmable.rhythmOffset"));

            Space();

            {
                string propertyName = "_bpm";

                EditorGUI.BeginChangeCheck();

                try
                {
                    SerializedProperty? tps = serializedObject.FindProperty(propertyName);

                    tps.Next(true);
                    EditorGUILayout.PropertyField(tps);

                    tps.Next(true);
                    EditorGUILayout.PropertyField(tps);
                }
                catch
                {
                    GUILayout.Label(TryGetText("inspector.property_none").Replace("{name}", propertyName));
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    TargetsInvoke(x => x.bpm = x.bpm);

                    lastBeat = 0;
                }
            }

            if (target == null || targets == null || targets.Length > 1)
                return;

            DrawLine();

            {
                Rect backgroundRect = EditorGUILayout.BeginVertical();

                double time = target.time - target.rhythmOffset;
                double bpm = target.bpm.bpm;
                double currentBeat = time * (bpm / 60d);
                double timeSignatures = currentBeat.Repeat(target.bpm.timeSignatures);

                DrawText("gui.beat", currentBeat.ToString());
                DrawText("gui.time", $"{time.ToTimeString()} ({time})");
                DrawText("gui.time_signatures", timeSignatures.ToString());

                void DrawText(string label, string content)
                {
                    Rect rect = EditorGUILayout.GetControlRect();
                    rect.y -= y;

                    GUI.Label(rect, $"{TryGetText(label)} : {content}");
                }

                if (Kernel.isPlaying && target.audioSource != null && (target.audioSource.isPlaying || y != 0))
                {
                    y = y.Lerp(0, Kernel.unscaledDeltaTime * ((float)bpm.Clamp(150) / 60f));
                    if (y < 0.5f)
                        y = 0;

                    bool pass = false;
                    if (target.audioSource != null)
                    {
                        if (target.audioSource.pitch >= 0)
                            pass = currentBeat.FloorToInt() > lastBeat;
                        else
                            pass = currentBeat.FloorToInt() < lastBeat;

                        pass |= currentBeat.FloorToInt().Distance(lastBeat) >= 0.1f;
                    }

                    if (Event.current.type == EventType.Repaint && pass)
                    {
                        bool firstBeat = (int)timeSignatures == 0;

                        if (firstBeat)
                            y = 8;
                        else
                            y = 4;

                        if (backgroundRect.Contains(Event.current.mousePosition))
                        {
                            if (firstBeat)
                                PlayClip(hitsound, 0.3f, 1.25f);

                            PlayClip(hitsound, 0.3f);
                        }

                        lastBeat = currentBeat.FloorToInt();
                    }

                    Repaint();
                }
                else
                    y = 0;

                EditorGUILayout.EndVertical();
            }
        }

        static void PlayClip(AudioClip? clip, float volume = 1, float pitch = 1)
        {
            if (!Kernel.isPlaying || clip == null)
                return;

            GameObject gameObject = new GameObject(nameof(RhythmableAudioSourceEditor) + ".Hitsound")
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;

            audioSource.Play();

            Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
        }
    }
}
