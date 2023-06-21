using LaunchDarkly.Sdk;
using Microsoft.Extensions.DependencyInjection;
using System;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace FeatureClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = ContainerConfiguration.Configure();
            var ldClient = provider.GetService<ILdClient>();

            UserWithKey(ldClient, "cc04e5b8-b483-47fe-8ce6-0098487e91a8", "sweet-feature-name-1");
            UserWithKey(ldClient, "e3dbd64b-f51d-4aff-a4e5-f334960b9045", "sweet-feature-name-3");
            UserBuilder(ldClient);
        }

        private static void UserWithKey(ILdClient ldClient, string userId, string featureName)
        {
            var defaultValue = false;
            var context = Context.Builder(userId);
            var allowed = ldClient.BoolVariation(featureName, context.Build(), defaultValue);

            Console.WriteLine("CheckByUserKey: featureName={0} userId={1} returned allowed={2}", featureName, userId, allowed);
        }

        private static void UserBuilder(ILdClient ldClient)
        {
            var defaultValue = false;
            var userId = Guid.NewGuid().ToString().ToLowerInvariant(); // this is not validated in LD
            var cityId = "ea2b6efc-020d-4469-a993-1bd5baf78adc";
            var regionId = "nz";
            var featureName = "sweet-feature-name-2";

            var context = Context
                .Builder(userId)
                .Set("CityId", cityId)
                .Set("RegionId", regionId);

            var allowed = ldClient.BoolVariation(featureName, context.Build(), defaultValue);

            Console.WriteLine("CheckByCustom: cityId={0} with regionId={1} returned allowed={2}",
                cityId,
                regionId,
                allowed);
        }
    }
}
