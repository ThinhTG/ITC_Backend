namespace ITC.Repositories.ModelViews.KeyModels
{
    public class KeyPairResponseModel
    {
        public string? id {  get; set; }

        public string? PublicKey { get; set; }

        public string? PrivateKey { get; set; }

        public DateTimeOffset? CreateTime { get; set; }
    }
}
