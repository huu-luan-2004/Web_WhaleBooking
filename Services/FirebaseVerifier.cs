using FirebaseAdmin;
using FirebaseAdmin.Auth;

namespace Web_WhaleBooking.Services;

public interface IFirebaseVerifier
{
    Task<FirebaseToken> VerifyAsync(string idToken, CancellationToken ct = default);
}

public class FirebaseVerifier : IFirebaseVerifier
{
    private readonly string _projectId;

    public FirebaseVerifier(IConfiguration config)
    {
        // Initialize Firebase app once if not already
        _projectId = config.GetValue<string>("Firebase:ProjectId") ?? throw new InvalidOperationException("Firebase:ProjectId not configured");
        EnsureAppInitialized(_projectId);
    }

    private static readonly object _sync = new();
    private static void EnsureAppInitialized(string projectId)
    {
        if (FirebaseApp.DefaultInstance != null) return;
        lock (_sync)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                // For VerifyIdTokenAsync, Admin SDK uses public certs and does not require credentials.
                // Initialize with only ProjectId to avoid ADC requirement locally.
                FirebaseApp.Create(new AppOptions { ProjectId = projectId });
            }
        }
    }

    public Task<FirebaseToken> VerifyAsync(string idToken, CancellationToken ct = default)
    {
        return FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken, ct);
    }
}
