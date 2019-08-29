namespace MedicalNlpSkillsetFunctions.HocrGenerator
{
    public class OcrImageMetadata
    {
        public OcrLayoutText HandwrittenLayoutText { get; set; }
        public OcrLayoutText LayoutText { get; set; }
        public string ImageStoreUri { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
