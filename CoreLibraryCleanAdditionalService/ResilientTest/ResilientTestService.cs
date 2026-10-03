namespace Core.Library.Clean.AdditionalService.ResilientTest
{
    public class ResilientTestService
    {
        //e-exception, t-timeout, a-default
        public async Task InjectIssueDelayExceptionNone(string action, CancellationToken cancellationToken)
        {
            switch (action)
            {
                case "e":
                    throw new Exception("RAJESH NANDHAN RESILIENT TEST SERVICE FAIL PRODUCER EXCEPTION");

                case "t":
                    // Deliberately take 25 seconds, but honor cancellation.
                    await Task.Delay(TimeSpan.FromSeconds(45), cancellationToken);
                    break;
                default:
                    return;
            }
        }
    }
}
