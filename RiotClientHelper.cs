using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


public sealed class RiotClientHelper
{
    private static readonly RiotClientHelper instance = new RiotClientHelper();

    public static RiotClientHelper Instance
    {
        get { return instance; }
    }

    private string lockFilePort;
    private string lockFilePassword;
    private string accessToken;
    private string entitlement;
    private string authorization;
    private string playerID;
    private string base64Chat;

    private RiotClientHelper() { }

    public async Task GetAccessTokenAsync()
    {
        if (accessToken == null)
        {
            string lockFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Config", "lockfile");

            // Open the lockfile with FileShare.ReadWrite option to allow concurrent access
            using (FileStream fileStream = new FileStream(lockFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string lockFileContent = await reader.ReadToEndAsync();

                    string[] lockFileParts = lockFileContent.Split(':');
                    lockFilePort = lockFileParts[2];
                    lockFilePassword = lockFileParts[3];

                    string message = "riot:" + lockFilePassword;
                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                    base64Chat = Convert.ToBase64String(messageBytes);

                    string url = $"https://127.0.0.1:{lockFilePort}/entitlements/v1/token";

                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        // Disable SSL verification
                        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Chat}");
                            try
                            {
                                HttpResponseMessage response = await client.GetAsync(url);
                                response.EnsureSuccessStatusCode();
                                string responseContent = await response.Content.ReadAsStringAsync();

                                // Process the response
                                dynamic j = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);

                                entitlement = j.token;
                                authorization = j.accessToken;
                                playerID = j.subject;
                                accessToken = responseContent;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred during the request: " + ex.Message);
                            }
                        }
                    }
                   


                }
            }
        }
    }

    public string GetEntitlement()
    {
        return entitlement;
    }

    public string GetAuthorization()
    {
        return authorization;
    }

    public string GetPlayerID()
    {
        return playerID;
    }

    public string GetBase64Chat()
    {
        return base64Chat;
    }

    public string GetLockFilePort()
    {
        return lockFilePort;
    }

    public async Task<string> GetPreMatchId()
    {
        var playerID = GetPlayerID();
        var entitlement = GetEntitlement();
        var authorization = GetAuthorization();

        var url = $"https://glz-eu-1.eu.a.pvp.net/pregame/v1/players/{playerID}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", entitlement);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authorization}");

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var matchID = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse)["MatchID"].ToString();
                return matchID;
            }

            throw new Exception("Failed to retrieve prematch ID");
        }
    }
    public async Task<string> GetPartyId(int index)
    {
        var url = $"https://glz-eu-1.eu.a.pvp.net/parties/v1/players/{Instance.GetPlayerID()}";

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", Instance.GetEntitlement());
            httpClient.DefaultRequestHeaders.Add("X-Riot-ClientVersion", "release-05.12-15-804337");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Instance.GetAuthorization()}");

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var j = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var currentPartyId = j["CurrentPartyID"].ToString();
                return index == 1 ? currentPartyId : currentPartyId + "@ares-parties.eu2.pvp.net";
            }

            throw new Exception("Failed to retrieve party ID");
        }
    }

    public async Task<string> GetPrematchId()
    {
        string Player_ID = RiotClientHelper.instance.GetPlayerID();
        var url = $"https://glz-eu-1.eu.a.pvp.net/pregame/v1/players/{Player_ID}";

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", Instance.GetEntitlement());
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Instance.GetAuthorization()}");

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var j = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var matchID = j["MatchID"].ToString();

                return matchID;
            }

            throw new Exception("Failed to retrieve prematch ID");
        }
    }
    public async Task<string> GetCurrentGameId()
    {
        var url = $"https://glz-eu-1.eu.a.pvp.net/core-game/v1/players/{instance.GetPlayerID()}";

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", instance.GetEntitlement());
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {instance.GetAuthorization()}");

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var j = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var currentGameId = j["MatchID"].ToString();
                return currentGameId;
            }

            throw new Exception("Failed to retrieve current game ID");
        }
    }

}
