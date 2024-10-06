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
    [ExecuteAlways]
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
                
                return _internalTick * 0.05d / (nbsFile.tickTempo * 0.0005);
            }
            set
            {
                if (nbsFile == null)
                    return;

                if (time != value)
                {
                    internalTick = value * (nbsFile.tickTempo * 0.0005) * 20;
                    TimeChangedEventInvoke();
                }
            }
        }

        public double tick
        {
            get => _internalTick / (nbsFile?.tickTempo * 0.0005) ?? 0;
            set
            {
                if (nbsFile == null)
                    return;

                internalTick = value * (nbsFile.tickTempo * 0.0005);
            }
        }

        public double internalTick
        {
            get => _internalTick;
            set
            {
                if (nbsFile == null)
                    return;

                if (internalTick != value)
                {
                    _internalTick = value;
                    _index = nbsFile.nbsNotes.Select((d, i) => new { d.delayTick, index = i }).MinBy(x => (x.delayTick - value).Abs()).index;
                    
                    TimeChangedEventInvoke();
                }
            }
        }
        double _internalTick;

        public int index
        {
            get => _index;
            set
            {
                if (nbsFile == null)
                    return;

                value = value.Clamp(0, nbsFile.nbsNotes.Length - 1);

                if (_index != value)
                {
                    _index = value;
                    _internalTick = nbsFile.nbsNotes[value].delayTick;
                    
                    TimeChangedEventInvoke();
                }
            }
        }
        int _index;

        public override double length => nbsFile != null ? tickLength / 20d : 0;

        public double tickLength => (internalTickLength / (nbsFile?.tickTempo * 0.0005)) ?? 0;
        public int internalTickLength => ((nbsFile?.songLength / (nbsFile?.timeSignature * 4f) ?? 0).CeilToInt() * ((nbsFile?.timeSignature * 4f) ?? 0)).RoundToInt();



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
            
            if (isPlaying && !isPaused && realTempo != 0)
            {
                _internalTick += Kernel.unscaledDeltaTimeDouble * (nbsFile.tickTempo * 0.01) * realTempo;

                Loop();
                SetIndex();
                GetAudioDataToMonoAndInvoke();
            }

            if (isDisposable && !loop && (_internalTick < 0 || _internalTick > internalTickLength || !isPlaying))
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

            _index = nbsFile.nbsNotes.Select((d, i) => new { d.delayTick, index = i }).MinBy(x => (x.delayTick - _internalTick).Abs()).index;
            return true;
        }

        public override void Play()
        {
            if (!isActiveAndEnabled)
                return;

            Stop();
            if (!Refresh())
                return;

            if (tempo < 0)
                _internalTick = internalTickLength;

            base.Play();
        }

        public override void Stop()
        {
            base.Stop();

            nbsData = null;
            nbsMetaData = null;

            _internalTick = 0;
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

            NBSPlayer? nbsPlayer = ObjectPoolingManager.ObjectClone<NBSPlayer>("NBS Player", NBSLoader.nbsNameSpace, parent);
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
            if (nbsFile == null || _index < 0 || _index >= nbsFile.nbsNotes.Length)
                return;

            NBSNote nbsNote = nbsFile.nbsNotes[_index];
            if (realTempo >= 0)
            {
                if (_index >= 0 && _index < nbsFile.nbsNotes.Length && nbsNote.delayTick < internalTick)
                {
                    //InfiniteLoopDetector.Run();

                    if (pitch != 0)
                        AudioPlay();

                    _index++;
                }
            }
            else
            {
                if (_index >= 0 && _index < nbsFile.nbsNotes.Length && nbsNote.delayTick >= internalTick)
                {
                    //InfiniteLoopDetector.Run();

                    if (pitch != 0)
                        AudioPlay();

                    _index--;
                }
            }
        }




        void AudioPlay()
        {
            if (nbsFile == null)
                return;

            NBSNote nbsNote = nbsFile.nbsNotes[_index];
            for (int i = 0; i < nbsNote.nbsNoteMetaDatas.Length; i++)
            {
                NBSNoteMetaData nbsNoteMetaData = nbsNote.nbsNoteMetaDatas[i];
                NBSLayer nbsLayer = nbsFile.nbsLayers[nbsNoteMetaData.layerIndex];
                if (nbsLayer.layerLock != 0 && !allLayerLock)
                    continue;
                else if (nbsLayer.layerLock != 2 && allLayerLock)
                    continue;

                string nameSpace;
                string instrumentName;
                int customInstrumentKey = 0;
                if (nbsNoteMetaData.instrument < nbsFile.vanillaInstrumentCount)
                {
                    nameSpace = NBSLoader.nbsNameSpace;
                    instrumentName = "block.note_block.";

                    switch (nbsNoteMetaData.instrument)
                    {
                        case 0:
                            instrumentName += "harp";
                            break;
                        case 1:
                            instrumentName += "bass";
                            break;
                        case 2:
                            instrumentName += "bassdrum";
                            break;
                        case 3:
                            instrumentName += "snare";
                            break;
                        case 4:
                            instrumentName += "hat";
                            break;
                        case 5:
                            instrumentName += "guitar";
                            break;
                        case 6:
                            instrumentName += "flute";
                            break;
                        case 7:
                            instrumentName += "bell";
                            break;
                        case 8:
                            instrumentName += "chime";
                            break;
                        case 9:
                            instrumentName += "xylophone";
                            break;
                        case 10:
                            instrumentName += "iron_xylophone";
                            break;
                        case 11:
                            instrumentName += "cow_bell";
                            break;
                        case 12:
                            instrumentName += "didgeridoo";
                            break;
                        case 13:
                            instrumentName += "bit";
                            break;
                        case 14:
                            instrumentName += "banjo";
                            break;
                        case 15:
                            instrumentName += "pling";
                            break;
                    }
                }
                else if (nbsNoteMetaData.instrument - nbsFile.vanillaInstrumentCount < nbsFile.customInstrumentKeys.Length)
                {
                    int index = nbsNoteMetaData.instrument - nbsFile.vanillaInstrumentCount;
                    NBSCustomInstrument customInstrument = nbsFile.customInstrumentKeys[index];

                    nameSpace = this.nameSpace;

                    instrumentName = customInstrument.name;
                    customInstrumentKey = customInstrument.key - 45;
                }
                else
                    continue;

                double pitch = 2d.Pow((nbsNoteMetaData.key + customInstrumentKey - 45) * 0.0833333333) * 2d.Pow(nbsNoteMetaData.pitch * 0.01 * 0.0833333333);
                float volume = nbsNoteMetaData.velocity * 0.01f * (nbsLayer.layerVolume * 0.01f);

                float layerStereo = (nbsLayer.layerStereo - 100) * 0.01f;
                float panStereo = ((nbsNoteMetaData.panning - 100) * 0.01f).Lerp(layerStereo, layerStereo.Abs());

                volume *= this.volume;
                pitch *= realPitch;
                panStereo = panStereo.Lerp(this.panStereo, this.panStereo.Abs());
                
                AudioPlayer? audioPlayer;
                if (spatial)
                    audioPlayer = AudioPlayer.PlayAudio(instrumentName, nameSpace, volume, false, pitch, pitch, panStereo, transform, Vector3.zero, minDistance, maxDistance);
                else
                    audioPlayer = AudioPlayer.PlayAudio(instrumentName, nameSpace, volume, false, pitch, pitch, panStereo, transform);

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
                while (tempo >= 0 && internalTick > internalTickLength)
                {
                    _internalTick -= internalTickLength - nbsMetaData.loopStartTick;
                    isLooped = true;
                }

                while (tempo < 0 && internalTick < nbsMetaData.loopStartTick)
                {
                    _internalTick += internalTickLength - nbsMetaData.loopStartTick;
                    isLooped = true;
                }

                if (isLooped)
                {
                    _index = nbsFile.nbsNotes.Select((d, i) => new { d.delayTick, index = i }).MinBy(x => (x.delayTick - _internalTick).Abs()).index;
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
