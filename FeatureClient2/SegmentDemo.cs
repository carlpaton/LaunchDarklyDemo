using System.Text;
using System.Text.Json;
using static FeatureClient2.LaunchDarklyDto;

namespace FeatureClient2
{
    public class SegmentDemo
    {
        public async Task Start() 
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "api-00000000-0000-0000-0000-000000000000");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var userId = "e3dbd64b-f51d-4aff-a4e5-f334960b9045";
            var segment = await GetSegment(httpClient, jsonOptions);            
            var operations = GetPatchOperation(userId, segment);

            await UpdateSegment(operations, userId, httpClient, jsonOptions);
        }

        /// <summary>
        /// https://apidocs.launchdarkly.com/tag/Segments/#operation/patchSegment
        /// </summary>
        /// <param name="operations"></param>
        /// <param name="userId"></param>
        /// <param name="httpClient"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static async Task UpdateSegment(List<LdPatchOperation> operations, string userId, HttpClient httpClient, JsonSerializerOptions options)
        {
            if (!operations.Any())
                return;

            var body = new LdPatchSegmentPayload()
            {
                Patch = operations,
                Comment = $"Removing userId : {userId}"
            };
            var content = JsonSerializer.Serialize(body, options);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(GetSegmentUri(), stringContent);
            response.EnsureSuccessStatusCode();
        }

        private static List<LdPatchOperation> GetPatchOperation(string userId, Segment segment)
        {
            var firstRule = segment.Rules.First();
            var patchOperations = new List<LdPatchOperation>();

            for (int clauseIndex = 0; clauseIndex < firstRule.Clauses.Count; clauseIndex++)
            {
                var currentClause = firstRule.Clauses[clauseIndex];
                var guidsInClause = currentClause.Values.ToHashSet();
                var exists = guidsInClause.Contains(userId);

                if (exists)
                {
                    var idIndex = currentClause.Values.IndexOf(userId);
                    patchOperations.Add(new LdPatchOperation()
                    {
                        Op = "remove",
                        Path = $"/rules/0/clauses/{clauseIndex}/values/{idIndex}",
                    });
                }
            }

            return patchOperations;
        }

        private static async Task<Segment> GetSegment(HttpClient httpClient, JsonSerializerOptions options)
        {
            var response = await httpClient.GetAsync(GetSegmentUri());
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Segment>(responseBody, options);
        }

        /// <summary>
        /// https://apidocs.launchdarkly.com/tag/Segments#operation/getSegment
        /// </summary>
        /// <returns></returns>
        private static string GetSegmentUri()
        {
            var projectKey = "default";
            var environmentKey = "production";
            var segmentKey = "sweet-segment-3";
            var baseUrl = "https://app.launchdarkly.com/api/v2";

            return $"{baseUrl}/segments/{projectKey}/{environmentKey}/{segmentKey}";
        }
    }
}
