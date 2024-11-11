using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.NetworkInformation;

namespace HealthCheck.Server
{
    //We implement the IhealthCheckInterface, with its CheckHealthAsync method.
    //As this is an official .NET way to deal with health checks there is already an interface available.

    public class ICMPHealthCheck : IHealthCheck
    {
        //Host is set to a non-routable IP adress, which seems rather awkward.
        //This is done for demonstration purposes so that we will be able to simulate an "unhealthy" scenario.
        //Normally, Host and HealthyRoundtripTime variables sould be passed as parameters so that we can set them programmatically.
        private readonly string Host = $"10.0.0.0";
        private readonly int HealthyRoundTripTime = 300;
       
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                //Here the ping variable has been defined using the using keyword. This declaration reduces nesting and makes more
                //readable code. 
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(Host);
                switch (reply.Status)
                {
                    //This code handles the different scenarios we want:
                    //Healthy, if the PING request gets a successful reply withing the round trip time of 300 ms or less
                    //Degraded, if the PING request gets a successful reply with a round trip time greater than 300 ms
                    //Unhealthy, if the PING request fails or an Exception is thrown
                    case IPStatus.Success:
                        return (reply.RoundtripTime > HealthyRoundTripTime)
                            ? HealthCheckResult.Degraded()
                            : HealthCheckResult.Healthy();
                    default:
                        return HealthCheckResult.Unhealthy();
                }
            }

            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
