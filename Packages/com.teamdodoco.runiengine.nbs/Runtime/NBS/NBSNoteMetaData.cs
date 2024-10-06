namespace RuniEngine.NBS
{
    public readonly struct NBSNoteMetaData
    {
        public readonly short layerIndex;

        public readonly byte instrument;
        public readonly byte key;
        public readonly byte velocity;
        public readonly byte panning;
        public readonly short pitch;

        public NBSNoteMetaData(short layerIndex, byte instrument, byte key, byte velocity, byte panning, short pitch)
        {
            this.layerIndex = layerIndex;

            this.instrument = instrument;
            this.key = key;
            this.velocity = velocity;
            this.panning = panning;
            this.pitch = pitch;
        }

        public override string ToString() => $"(layerIndex:{layerIndex}, instrument:{instrument}, key:{key}, velocity:{velocity}, panning:{panning}, pitch:{pitch})";
    }
}
