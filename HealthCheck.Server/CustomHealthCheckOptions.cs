using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using System.Text.Json;

namespace HealthCheck.Server
{
    //We override the standard class which outputs only one word with our own custom class
    //This way we can change its ResponseWriter property to make it output whatever we want..
    public class CustomHealthCheckOptions : HealthCheckOptions
    {
        public CustomHealthCheckOptions() : base()
        {
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            ResponseWriter = async (c, r) =>
            {
                c.Response.ContentType = MediaTypeNames.Application.Json;
                c.Response.StatusCode = StatusCodes.Status200OK;

                //We want to output a custom JSON containin useful information for the checks
                var result = JsonSerializer.Serialize(new
                {
                    checks = r.Entries.Select(e => new
                    {
                        //The identifying string we provided when adding the check to the HealthChecks Middleware "ICMP_01" etc.
                        name = e.Key,

                        //The whole duration of the single check
                        responseTime = e.Value.Duration.TotalMilliseconds,
                        //The individual status of a check, not the status of the whole HealthCheck
                        status = e.Value.Status.ToString(),
                        //The custom inormative message configured earlier on in the ICMPHealthCheck clss
                        description = e.Value.Description
                    }),

                    //The JSON file, in addition t the above array wil also have these properties:
                    //Boolean sum of all the inner checks statuses, thus unhealthy if at least one host is unhealthy
                    totalStatus = r.Status,
                    //The whole duration of all checks together
                    totalResponseTime = r.TotalDuration.TotalMilliseconds,

                }, jsonSerializerOptions);

                await c.Response.WriteAsync(result);

            };
        }
    }
}
