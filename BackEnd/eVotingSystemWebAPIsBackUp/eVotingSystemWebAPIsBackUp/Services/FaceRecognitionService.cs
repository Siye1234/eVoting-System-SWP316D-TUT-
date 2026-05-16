using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace eVotingSystemWebAPIsBackUp.Services
{
    public class FaceRecognitionService
    {
        private readonly AmazonRekognitionClient _client;

        public FaceRecognitionService()
        {
            _client = new AmazonRekognitionClient(
                "AKIASUGXLNJAMIVFTJO6",
                "alXG8UnoDVPjNq6iJOzghTwowIV2Wtw8w2nyWZJ9",
                RegionEndpoint.USEast1
            );
        }

        public async Task<float?> CompareFaces(byte[] sourceImage, byte[] targetImage)
        {
            var request = new CompareFacesRequest
            {
                SourceImage = new Image
                {
                    Bytes = new MemoryStream(sourceImage)
                },
                TargetImage = new Image
                {
                    Bytes = new MemoryStream(targetImage)
                },
                SimilarityThreshold = 80F
            };

            var response = await _client.CompareFacesAsync(request);

            if (response.FaceMatches.Count > 0)
            {
                return response.FaceMatches[0].Similarity;
            }

            return null;
        }
    }
}