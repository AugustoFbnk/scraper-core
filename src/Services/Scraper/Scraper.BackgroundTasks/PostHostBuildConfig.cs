using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Scraper.BackgroundTasks.Options;

namespace Scraper.BackgroundTasks
{
    internal class PostHostBuildConfig
    {
        readonly FirebaseOptions _firebaseOptions;
        internal PostHostBuildConfig(IOptions<FirebaseOptions> firebaseOptions)
        {
            if (firebaseOptions?.Value == null)
                throw new ArgumentNullException(nameof(FirebaseOptions));

            _firebaseOptions = firebaseOptions.Value;
        }

        internal void Setup()
        {
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(_firebaseOptions.CredentialFile),
                ProjectId = _firebaseOptions.ProjectId
            });
        }
    }
}
