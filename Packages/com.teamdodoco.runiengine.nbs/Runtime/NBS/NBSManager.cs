using RuniEngine.Resource.Texts;
using System.Collections.Generic;
using System.IO;

namespace RuniEngine.NBS
{
    public static class NBSManager
    {
        /// <summary>
        /// NBS 파일을 불러옵니다
        /// </summary>
        /// <param name="path">
        /// NBS 파일의 경로
        /// </param>
        /// <returns>
        /// 불러온 NBS 파일 클래스
        /// </returns>
        public static NBSFile? ReadNBSFile(string path)
        {
            using FileStream fileStream = File.OpenRead(path);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            bool isOld = binaryReader.ReadInt16() != 0;
            if (isOld)
            {
                Debug.ForceLogError(LanguageLoader.TryGetText("nbs_manager.warning.old_version").Replace("{version}", 0.ToString()));
                return null;
            }
            /*NBS Version*/ byte version = binaryReader.ReadByte();
            if (version < 4)
            {
                Debug.ForceLogError(LanguageLoader.TryGetText("nbs_manager.warning.old_version").Replace("{version}", version.ToString()));
                return null;
            }
            else if (version > 5)
                Debug.ForceLogWarning(LanguageLoader.TryGetText("nbs_manager.warning.new_version").Replace("{version}", version.ToString()));
            /*Vanilla instrument count*/
            byte vanillaInstrumentCount = binaryReader.ReadByte();
            /*Song Length*/ short songLength = binaryReader.ReadInt16();
            /*Layer count*/ short layerCount = binaryReader.ReadInt16();
            for (int i = 0; i < 4; i++)
            {
                int length = 0;
                for (int j = 0; j < 4; j++)
                    length += binaryReader.ReadByte();

                binaryReader.ReadChars(length);
            }
            /*Song tempo*/ short tickTempo = binaryReader.ReadInt16();
            /*Auto-saving*/ binaryReader.ReadByte();
            /*Auto-saving duration*/ binaryReader.ReadByte();
            /*Time signature*/ byte timeSignature = binaryReader.ReadByte();
            /*Minutes spent*/ binaryReader.ReadInt32();
            /*Left-clicks*/ binaryReader.ReadInt32();
            /*Right-clicks*/ binaryReader.ReadInt32();
            /*Note blocks added*/ binaryReader.ReadInt32();
            /*Note blocks removed*/ binaryReader.ReadInt32();
            {
                int length = 0;
                for (int i = 0; i < 4; i++)
                    length += binaryReader.ReadByte();

                binaryReader.ReadChars(length);
            }
            /*Loop on/off*/ binaryReader.ReadByte(); //if (binaryReader.ReadByte() == 1) nbsFile.loop = true; else nbsFile.loop = false;
            /*Max loop count*/ binaryReader.ReadByte();
            /*Loop start tick*/ short loopStartTick = binaryReader.ReadInt16();

            /*Jumps to the next tick*/
            short tick;
            short tick2 = -1; //틱의 시작은 -1이여야합니다

            List<NBSNote> nbsNotes = new List<NBSNote>();
            while ((tick = binaryReader.ReadInt16()) != 0)
            {
                tick2 += tick;
                List<NBSNoteMetaData> nbsNoteMetaDatas = new List<NBSNoteMetaData>();

                /*Jumps to the next layer*/
                short layerIndex;
                short layerIndex2 = -1; //레이어도 마찬가지
                while ((layerIndex = binaryReader.ReadInt16()) != 0)
                {
                    layerIndex2 += layerIndex;
                    NBSNoteMetaData nbsNoteMetaData = new NBSNoteMetaData
                    (
                        layerIndex2,
                        /*Note block instrument*/ binaryReader.ReadByte(),
                        /*Note block key*/ binaryReader.ReadByte(),
                        /*Note block velocity*/ binaryReader.ReadByte(),
                        /*Note block panning*/ binaryReader.ReadByte(),
                        /*Note block pitch*/ binaryReader.ReadInt16()
                    );

                    nbsNoteMetaDatas.Add(nbsNoteMetaData);
                }

                nbsNotes.Add(new NBSNote(tick2, nbsNoteMetaDatas.ToArray()));
            }

            NBSLayer[] nbsLayers = new NBSLayer[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                string layerName;
                {
                    int length = 0;
                    for (int j = 0; j < 4; j++)
                        length += binaryReader.ReadByte();

                    layerName = new string(binaryReader.ReadChars(length));
                }

                NBSLayer nbsLayer = new NBSLayer
                (
                    layerName,
                    binaryReader.ReadByte(),
                    /*Layer volume*/ binaryReader.ReadByte(),
                    /*Layer stereo*/ binaryReader.ReadByte()
                );

                nbsLayers[i] = nbsLayer;
            }

            /*Custom instruments*/; NBSCustomInstrument[] customInstruments = new NBSCustomInstrument[binaryReader.ReadByte()];
            for (int i = 0; i < customInstruments.Length; i++)
            {
                /*Instrument name*/
                string instrumentName;
                {
                    int length = 0;
                    for (int j = 0; j < 4; j++)
                        length += binaryReader.ReadByte();

                    instrumentName = new string(binaryReader.ReadChars(length));
                }
                /*Sound file*/
                {
                    int length = 0;
                    for (int j = 0; j < 4; j++)
                        length += binaryReader.ReadByte();

                    binaryReader.ReadChars(length);
                }
                /*Sound key*/ byte soundKey = binaryReader.ReadByte();
                /*Press piano key*/ binaryReader.ReadByte();

                customInstruments[i] = new NBSCustomInstrument(instrumentName, soundKey);
            }

            return new NBSFile(songLength, tickTempo, loopStartTick, timeSignature, vanillaInstrumentCount, nbsNotes.ToArray(), nbsLayers, customInstruments);
        }
    }
}