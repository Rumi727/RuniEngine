#nullable enable
using RuniEngine;
using RuniEngine.Inputs;
using RuniEngine.Resource.Sounds;
using RuniEngine.Rhythms;
using RuniEngine.Sounds;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    Vector2 rotation = Vector2.zero;

    void OnEnable() => rhythm = new RhythmWatch();
    void OnDisable()
    {
        if (rhythm != null)
        {
            rhythm.Dispose();
            rhythm = null;
        }
    }

    [System.Serializable]
    public struct Asdf
    {
        public float wasans;
        public Asdf2 asdf;

        [System.Serializable]
        public struct Asdf2
        {
            public float wasans2;
        }
    }

    RhythmWatch? rhythm;
    public TMP_Text? text;
    public SoundPlayerBase? soundPlayer;
    public double lastBeat = 0;
    public Asdf rectCorner;
    public SerializableDictionary<string, SerializableDictionary<string, int>> a = new();
    public List<string> lists = new();
    void Update()
    {
        float speed;
        if (InputManager.GetKey(KeyCode.LeftControl))
            speed = 0.25f * Kernel.fpsUnscaledSmoothDeltaTime;
        else
            speed = 0.125f * Kernel.fpsUnscaledSmoothDeltaTime;

        {
            Vector3 motion = Vector3.zero;
            Vector3 rotation = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

            if (InputManager.GetKey(KeyCode.A))
                motion -= transform.right * speed;
            if (InputManager.GetKey(KeyCode.D))
                motion += transform.right * speed;
            if (InputManager.GetKey(KeyCode.S))
                motion -= transform.forward * speed;
            if (InputManager.GetKey(KeyCode.W))
                motion += transform.forward * speed;
            if (InputManager.GetKey(KeyCode.LeftShift))
                motion -= transform.up * speed;
            if (InputManager.GetKey(KeyCode.Space))
                motion += transform.up * speed;

            transform.position += motion;
            transform.localEulerAngles = rotation;
        }

        if (InputManager.GetKey(KeyCode.Mouse0))
        {
            Vector2 rotation = InputManager.pointerPositionDelta * 0.5f;
            this.rotation += new Vector2(-rotation.y, rotation.x);
        }

        transform.localEulerAngles = rotation;

        if (text != null && rhythm != null && soundPlayer != null)
        {
            double outBeat = 0;
            BeatBPMPairList.BPM bpm = rhythm.bpms?.GetValue(rhythm.currentBeat, out outBeat) ?? new(60);

            int beat = (rhythm.currentBeat - outBeat).Repeat(bpm.timeSignatures).FloorToInt();
            
            rhythm.soundPlayer = soundPlayer;
            text.text = $"Beat : {rhythm.currentBeat:0.00}\nTime : {rhythm.currentTime.ToTime()}";

            if (lastBeat != beat)
            {
                text.rectTransform.anchoredPosition = new Vector2(0, 5);

                if (beat == 0)
                    AudioPlayer.PlayAudio("block.note_block.hat", NBSLoader.nbsNameSpace, 1, false, 1.25f, 1.25f, 0, transform);
                else
                    AudioPlayer.PlayAudio("block.note_block.hat", NBSLoader.nbsNameSpace, 1, false, 1, 1, 0, transform);
            }

            text.rectTransform.anchoredPosition = text.rectTransform.anchoredPosition.Lerp(Vector2.zero, 0.1f * Kernel.fpsDeltaTime);

            lastBeat = beat;
        }
    }
}
