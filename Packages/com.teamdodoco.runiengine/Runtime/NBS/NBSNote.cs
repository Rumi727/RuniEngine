#nullable enable
using System.Collections.Generic;
using System.Text;

namespace RuniEngine.NBS
{
    public class NBSNote
    {
        public short delayTick { get; } = 0;
        public List<NBSNoteMetaData> nbsNoteMetaDatas { get; }

        public NBSNote(short delayTick, List<NBSNoteMetaData> nbsNoteMetaDatas)
        {
            this.delayTick = delayTick;
            this.nbsNoteMetaDatas = nbsNoteMetaDatas;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (nbsNoteMetaDatas == null)
                stringBuilder.Append("null");
            else
            {
                for (int i = 0; i < nbsNoteMetaDatas.Count; i++)
                    stringBuilder.Append(nbsNoteMetaDatas[i]);
            }

            return $"(delayTick:{delayTick}, nbsNoteMetaDatas:{stringBuilder})";
        }
    }
}
