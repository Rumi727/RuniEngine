#nullable enable
using System.Collections.Generic;
using System.Text;

namespace RuniEngine.NBS
{
    public class NBSFile
    {
        public short songLength { get; } = 0;
        public short tickTempo { get; } = 0;
        //public bool loop { get; set; } = false;
        public short loopStartTick { get; } = 0;

        public NBSNote[] nbsNotes { get; }
        public NBSLayer[] nbsLayers { get; }

        public NBSFile(short songLength, short tickTempo, short loopStartTick, NBSNote[] nbsNotes, NBSLayer[] nbsLayers)
        {
            this.songLength = songLength;
            this.tickTempo = tickTempo;
            this.loopStartTick = loopStartTick;
            this.nbsNotes = nbsNotes;
            this.nbsLayers = nbsLayers;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (nbsNotes == null)
                stringBuilder.Append("null");
            else
            {
                for (int i = 0; i < nbsNotes.Length; i++)
                    stringBuilder.Append(nbsNotes[i]);
            }

            StringBuilder stringBuilder2 = new StringBuilder();
            if (nbsLayers == null)
                stringBuilder.Append("null");
            else
            {
                for (int i = 0; i < nbsLayers.Length; i++)
                    stringBuilder.Append(nbsLayers[i]);
            }

            return $"(songLength:{songLength}, tickTempo:{tickTempo}, loopStartTick:{loopStartTick}, nbsNotes:{stringBuilder}, nbsLayers:{stringBuilder2})";
        }
    }
}
