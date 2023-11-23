#nullable enable
using System.Text;

namespace RuniEngine.NBS
{
    public class NBSNote
    {
        public short delayTick { get; } = 0;
        public NBSNoteMetaData[] nbsNoteMetaDatas { get; }

        public NBSNote(short delayTick, NBSNoteMetaData[] nbsNoteMetaDatas)
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
                for (int i = 0; i < nbsNoteMetaDatas.Length; i++)
                    stringBuilder.Append(nbsNoteMetaDatas[i]);
            }

            return $"(delayTick:{delayTick}, nbsNoteMetaDatas:{stringBuilder})";
        }
    }
}
