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

            UserWithKey(ldClient);
            UserBuilder(ldClient);
        }

        private static void UserWithKey(ILdClient ldClient)
        {
            var defaultValue = false;
            var userId = "cc04e5b8-b483-47fe-8ce6-0098487e91a8";
            var featureName = "sweet-feature-name-1";

            var ldUser = User.WithKey(userId);
            var allowed = ldClient.BoolVariation(featureName, ldUser, defaultValue);

            Console.WriteLine("CheckByUserKey: userId={0} returned allowed={1}", userId, allowed);
        }

        private static void UserBuilder(ILdClient ldClient)
        {
            var defaultValue = false;
            var userId = Guid.NewGuid().ToString().ToLowerInvariant(); // this is not validated in LD
            var cityId = "ea2b6efc-020d-4469-a993-1bd5baf78adc";
            var regionId = "nz";
            var featureName = "sweet-feature-name-2";

            var ldUser = User
                .Builder(userId)
                .Custom("CityId", cityId)
                .Custom("RegionId", regionId)
                .Build();

            var allowed = ldClient.BoolVariation(featureName, ldUser, defaultValue);

            Console.WriteLine("CheckByCustom: cityId={0} with regionId={1} returned allowed={2}",
                cityId,
                regionId,
                allowed);
        }
    }
}
