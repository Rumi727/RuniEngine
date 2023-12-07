#nullable enable
using RuniEngine.Booting;
using RuniEngine.NBS;
using RuniEngine.Pooling;
using RuniEngine.Resource;
using RuniEngine.Resource.Sounds;
using RuniEngine.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

namespace RuniEngine.Sounds
{
    public sealed class NBSPlayer : SoundPlayerBase
    {
        public override SoundData? soundData => nbsData;
        public override SoundMetaDataBase? soundMetaData => nbsMetaData;

        public NBSData? nbsData { get; private set; }
        public NBSMetaData? nbsMetaData { get; private set; }
        public NBSFile? nbsFile => nbsMetaData?.nbsFile;



        public override double time
        {
            get
            {
                if (nbsFile == null)
                    return 0;
                
                return tick * 0.05d / (nbsFile.tickTempo * 0.0005f);
            }
            set
            {
                if (nbsFile == null)
                    return;

                if (time != value)
                {
                    tick = value * (nbsFile.tickTempo * 0.0005f) * 20;
                    TimeChangedEventInvoke();
                }
            }
        }

        public double tick
        {
            get => _tick;
            set
            {
                if (nbsFile == null)
                    return;

                if (tick != value)
                {
                    _tick = value;
                    _index = nbsFile.nbsNotes.Select((d, i) => new { d.delayTick, index = i }).MinBy(x => (x.delayTick - value).Abs()).index;
                    
                    TimeChangedEventInvoke();
                }
            }
        }
        double _tick;

        public int index
        {
            get => _index;
            set
            {
                if (nbsFile == null)
                    return;

                if (index != value)
                {
                    _index = value;
                    _tick = nbsFile.nbsNotes[value].delayTick;
                    
                    TimeChangedEventInvoke();
                }
            }
        }
        int _index;

        public override double length => nbsFile != null ? tickLength / (nbsFile.tickTempo * 0.01f) : 0;

        public double tickLength => nbsFile != null ? nbsFile.songLength : 0;



        public bool allLayerLock { get; private set; } = false;



        bool isDisposable = false;



        void Update()
        {
            if (nbsFile == null)
            {
                if (isDisposable)
                    Remove();

                return;
            }
            
            if (!isPaused && realTempo != 0)
            {
                _tick += Kernel.deltaTimeDouble * (nbsFile.tickTempo * 0.01f) * realTempo;

                Loop();
                SetIndex();
                GetAudioDataToMonoAndInvoke();
            }

            if (isDisposable && (tick < 0 || tick > tickLength))
                Remove();
        }



        public override bool Refresh()
        {
            NBSData? nbsData = NBSLoader.SearchNBSData(key, nameSpace);
            if (nbsData == null || nbsData.nbses.Length <= 0)
                return false;

            NBSMetaData? nbsMetaData = nbsData.nbses[Random.Range(0, nbsData.nbses.Length)];
            if (nbsMetaData == null)
                return false;

            NBSFile? nbsFile = nbsMetaData.nbsFile;
            if (nbsFile == null)
                return false;

            allLayerLock = nbsFile.nbsLayers.Any((b) => b.layerLock == 2);

            this.nbsData = nbsData;
            this.nbsMetaData = nbsMetaData;

            _index = nbsFile.nbsNotes.Select((d, i) => new { d.delayTick, index = i }).MinBy(x => (x.delayTick - tick).Abs()).index;
            return true;
        }

        public override void Play()
        {
            if (!isActiveAndEnabled)
                return;

            Stop();
            if (!Refresh())
                return;

            base.Play();
        }

        public override void Stop()
        {
            base.Stop();

            nbsData = null;
            nbsMetaData = null;

            _tick = 0;
            _index = 0;
            allLayerLock = false;
        }



        public static NBSPlayer? PlayNBS(string key, string nameSpace = "", float volume = 1, bool loop = false, float pitch = 1, float tempo = 1, float panStereo = 0, Transform? parent = null) => InternalPlayNBS(key, nameSpace, volume, loop, pitch, tempo, panStereo, parent, false, Vector3.zero, 0, 16);

        public static NBSPlayer? PlayNBS(string key, string nameSpace, float volume, bool loop, float pitch, float tempo, float panStereo, Transform? parent, Vector3 position, float minDistance = 0, float maxDistance = 16) => InternalPlayNBS(key, nameSpace, volume, loop, pitch, tempo, panStereo, parent, true, position, minDistance, maxDistance);

        static NBSPlayer? InternalPlayNBS(string key, string nameSpace, float volume, bool loop, float pitch, float tempo, float panStereo, Transform? parent, bool spatial, Vector3 position, float minDistance, float maxDistance)
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();
            ResourceDataNotLoadedException.Exception();

            ResourceManager.SetDefaultNameSpace(ref nameSpace);

            NBSPlayer? nbsPlayer = (NBSPlayer?)ObjectPoolingManager.ObjectCreate("nbs_player.prefab", parent).monoBehaviour;
            if (nbsPlayer == null)
                return null;

            nbsPlayer.key = key;
            nbsPlayer.nameSpace = nameSpace;

            nbsPlayer.volume = volume;
            nbsPlayer.loop = loop;
            nbsPlayer.pitch = pitch;
            nbsPlayer.tempo = tempo;

            nbsPlayer.panStereo = panStereo;
            nbsPlayer.spatial = spatial;

            nbsPlayer.minDistance = minDistance;
            nbsPlayer.maxDistance = maxDistance;

            nbsPlayer.transform.localPosition = position;

            nbsPlayer.isDisposable = true;
            nbsPlayer.Play();

            return nbsPlayer;
        }

        void SetIndex()
        {
            if (nbsFile == null || index < 0 || index >= nbsFile.nbsNotes.Length)
                return;

            NBSNote nbsNote = nbsFile.nbsNotes[index];
            if (realTempo >= 0)
            {
                if (index >= 0 && index < nbsFile.nbsNotes.Length && nbsNote.delayTick < tick)
                {
                    InfiniteLoopDetector.Run();
                    
                    AudioPlay();
                    _index++;
                }
            }
            else
            {
                if (index >= 0 && index < nbsFile.nbsNotes.Length && nbsNote.delayTick >= tick)
                {
                    InfiniteLoopDetector.Run();
                    
                    AudioPlay();
                    _index--;
                }
            }
        }




        void AudioPlay()
        {
            if (nbsFile == null)
                return;

            NBSNote nbsNote = nbsFile.nbsNotes[index];
            for (int i = 0; i < nbsNote.nbsNoteMetaDatas.Length; i++)
            {
                NBSNoteMetaData nbsNoteMetaData = nbsNote.nbsNoteMetaDatas[i];
                NBSLayer nbsLayer = nbsFile.nbsLayers[nbsNoteMetaData.layerIndex];
                if (nbsLayer.layerLock != 0 && !allLayerLock)
                    continue;
                else if (nbsLayer.layerLock != 2 && allLayerLock)
                    continue;

                float pitch = 2f.Pow((nbsNoteMetaData.key - 45) * 0.0833333333f) * 1.059463f.Pow(nbsNoteMetaData.pitch * 0.01f);
                float volume = nbsNoteMetaData.velocity * 0.01f * (nbsLayer.layerVolume * 0.01f);
                float panStereo = ((nbsNoteMetaData.panning - 100) * 0.01f) + ((nbsLayer.layerStereo - 100) * 0.01f);
                
                string blockType = "block.note_block.";
                switch (nbsNoteMetaData.instrument)
                {
                    case 0:
                        blockType += "harp";
                        break;
                    case 1:
                        blockType += "bass";
                        break;
                    case 2:
                        blockType += "bassdrum";
                        break;
                    case 3:
                        blockType += "snare";
                        break;
                    case 4:
                        blockType += "hat";
                        break;
                    case 5:
                        blockType += "guitar";
                        break;
                    case 6:
                        blockType += "flute";
                        break;
                    case 7:
                        blockType += "bell";
                        break;
                    case 8:
                        blockType += "chime";
                        break;
                    case 9:
                        blockType += "xylophone";
                        break;
                    case 10:
                        blockType += "iron_xylophone";
                        break;
                    case 11:
                        blockType += "cow_bell";
                        break;
                    case 12:
                        blockType += "didgeridoo";
                        break;
                    case 13:
                        blockType += "bit";
                        break;
                    case 14:
                        blockType += "banjo";
                        break;
                    case 15:
                        blockType += "pling";
                        break;
                }

                AudioPlayer? audioPlayer;
                if (spatial)
                    audioPlayer = AudioPlayer.PlayAudio(blockType, "nbs", volume * this.volume, false, pitch * realPitch, pitch * realPitch, panStereo + this.panStereo, transform, Vector3.zero, minDistance, maxDistance);
                else
                    audioPlayer = AudioPlayer.PlayAudio(blockType, "nbs", volume * this.volume, false, pitch * realPitch, pitch * realPitch, panStereo + this.panStereo, transform);

                allPlayingAudios.Add(audioPlayer);
            }
        }

        void Loop()
        {
            if (nbsMetaData == null || nbsFile == null)
                return;

            if (loop)
            {
                bool isLooped = false;
                while (tempo >= 0 && tick > tickLength)
                {
                    _tick -= tickLength - nbsMetaData.loopStartTick;
                    isLooped = true;
                }

                while (tempo < 0 && tick < 0)
                {
                    _tick += tickLength - nbsMetaData.loopStartTick;
                    isLooped = true;
                }

                if (isLooped)
                {
                    _index = nbsFile.nbsNotes.Select((d, i) => new { d.delayTick, index = i }).MinBy(x => (x.delayTick - _tick).Abs()).index;
                    LoopedEventInvoke();
                }
            }
        }

        readonly List<AudioPlayer?> allPlayingAudios = new List<AudioPlayer?>();
        float[] audioDatas = new float[0];
        float[] tempDatas = new float[0];
        void GetAudioDataToMonoAndInvoke()
        {
            AudioSettings.GetDSPBufferSize(out int bufferLength, out _);

            if (tempDatas.Length != bufferLength)
                tempDatas = new float[bufferLength];
            if (audioDatas.Length != bufferLength)
                audioDatas = new float[bufferLength];

            for (int i = 0; i < audioDatas.Length; i++)
                audioDatas[i] = 0;

            for (int i = 0; i < allPlayingAudios.Count; i++)
            {
                AudioPlayer? audioPlayer = allPlayingAudios[i];
                if (audioPlayer == null || audioPlayer.isRemoved || audioPlayer.audioSource == null)
                {
                    allPlayingAudios.RemoveAt(i);
                    i--;

                    continue;
                }

                for (int j = 0; j < audioPlayer.channels; j++)
                {
                    audioPlayer.audioSource.GetOutputData(tempDatas, j);

                    for (int k = 0; k < tempDatas.Length; k++)
                        audioDatas[k] += tempDatas[k] / audioPlayer.channels;
                }
            }

            OnAudioFilterRead(audioDatas, AudioLoader.systemChannels);
        }
    }
}
