namespace BotV1.hlt
{
    public class Metadata
    {
        private string[] metadata;
        private int index = 0;

        public Metadata(string[] metadata)
        {
            this.metadata = metadata;
        }

        public string Pop()
        {
            return metadata[index++];
        }

        public bool IsEmpty()
        {
            return index == metadata.Length;
        }
    }
}
