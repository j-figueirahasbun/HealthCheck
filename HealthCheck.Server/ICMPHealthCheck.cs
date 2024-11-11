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
        // ! The code has been adapted to use these as parameter
        private readonly string Host;
        private readonly int HealthyRoundTripTime;
       




        public ICMPHealthCheck(string host, int healthyRoundTripTime)
        {
            Host = host;
            HealthyRoundTripTime = healthyRoundTripTime;
        }



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

                        var msg = $"ICMP to {Host} took {reply.RoundtripTime} ms.";


                        return (reply.RoundtripTime > HealthyRoundTripTime)
                            ? HealthCheckResult.Degraded(msg)
                            : HealthCheckResult.Healthy(msg);
                    default:
                        var err = $"ICMP to {Host} failed: {reply.Status}.";
                        return HealthCheckResult.Unhealthy(err);
                }
            }

            catch (Exception ex)
            {

                var err = $"ICMP failed: {ex.Message}.";

                return HealthCheckResult.Unhealthy(err);
            }
        }
    }
}
