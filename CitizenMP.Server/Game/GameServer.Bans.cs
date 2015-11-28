using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizenMP.Server.Game
{
    partial class GameServer
    {
        private struct BanDetails
        {
            public string Reason { get; set; }
            public long Expiration { get; set; }
        }

        private ConcurrentDictionary<string, BanDetails> m_bannedIdentifiers = new ConcurrentDictionary<string, BanDetails>();

        public void BanIdentifier(string identifier, string reason)
        {
            BanDetails details = new BanDetails();
            details.Reason = reason;
            details.Expiration = Time.CurrentTime + (30 * 60 * 1000);

            m_bannedIdentifiers.AddOrUpdate(identifier, details, (key, oldValue) => details);
        }

        public bool IsIdentifierBanned(string identifier, out string reason)
        {
            // default reason to the empty string
            reason = string.Empty;

            // get a ban
            BanDetails banDetails;

            if (m_bannedIdentifiers.TryGetValue(identifier, out banDetails))
            {
                // if the ban hasn't expired yet, return 'true' and add the reason
                if (banDetails.Expiration >= Time.CurrentTime)
                {
                    reason = banDetails.Reason;

                    return true;
                }
            }

            return false;
        }
    }
}
