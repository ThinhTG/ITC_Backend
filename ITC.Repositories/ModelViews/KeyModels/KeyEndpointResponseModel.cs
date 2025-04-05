namespace ITC.Repositories.ModelViews.KeyModels
{
    public class KeyEndpointResponseModel
    {
        public string? Id { get; set; }

        public string? KeyPairId { get; set; }

        public string? ClaimType { get; set; }

        public string? ClaimValue { get; set;}

        public DateTimeOffset? CreateTime { get; set; }
    }
}
