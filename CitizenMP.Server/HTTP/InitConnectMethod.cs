using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using uhttpsharp;
using uhttpsharp.Headers;
using Newtonsoft.Json.Linq;

namespace CitizenMP.Server.HTTP
{
    static class InitConnectMethod
    {
        public static Func<IHttpHeaders, IHttpContext, Task<JObject>> Get(Configuration config, Game.GameServer gameServer)
        {
            return async (headers, context) =>
            {
                var result = new JObject();

                var name = headers.GetByName("name");
                var guid = headers.GetByName("guid");

                string protocol = null;
                
                headers.TryGetByName("protocol", out protocol);
                
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(guid))
                {
                    result["error"] = "fields missing";

                    return result;
                }

                if (string.IsNullOrEmpty(protocol))
                {
                    protocol = "1";
                }

                // check the protocol version
                uint protocolNum;

                if (!uint.TryParse(protocol, out protocolNum))
                {
                    result["error"] = "invalid protocol version";

                    return result;
                }

                if (config.Imports != null && config.Imports.Count > 0)
                {
                    if (protocolNum < 2)
                    {
                        result["error"] = "Your client is too old to support imported resources. Please update to a cleanliness client or higher.";

                        return result;
                    }
                }

                IEnumerable<string> clientIdentifiers;

                // authentication
                if (!gameServer.Configuration.DisableAuth)
                {
                    try
                    { string authTicket;

                        if (!headers.TryGetByName("authTicket", out authTicket))
                        {
                            result["authID"] = (0x110000100000001).ToString();

                            return result;
                        }

                        var validationAddress = ((IPEndPoint)context.RemoteEndPoint).Address;

                        if (validationAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            validationAddress = IPAddress.Parse("192.168.1.1"); // as these are whitelisted in NP code
                        }

                        // validate using the HTTP endpoint
                        var platformPort = gameServer.Configuration.PlatformPort;

                        if (platformPort == 0)
                        {
                            platformPort = 3035;
                        }

                        // create a request
                        var httpClient = HttpWebRequest.CreateHttp(string.Format("http://{0}:{1}/ticket/validate", gameServer.Configuration.PlatformServer, platformPort));
                        httpClient.Method = "POST";
                        httpClient.ContentType = "application/json";

                        // get the request stream
                        var requestStream = await httpClient.GetRequestStreamAsync();

                        // create the request object
                        var requestObject = new JObject();
                        requestObject["npid"] = "0x" + long.Parse(guid).ToString("X16");
                        requestObject["clientIP"] = validationAddress.ToString();
                        requestObject["ticket"] = authTicket;

                        // write the encoded object
                        var writer = new StreamWriter(requestStream);
                        await writer.WriteAsync(requestObject.ToString(Newtonsoft.Json.Formatting.None));
                        writer.Close();

                        // get the response from the server
                        var response = await httpClient.GetResponseAsync();
                        using (var responseStream = response.GetResponseStream())
                        {
                            var reader = new StreamReader(responseStream);
                            var responseObject = JObject.Parse(await reader.ReadToEndAsync());

                            if (!responseObject["valid"].Value<bool>())
                            {
                                result["error"] = "Platform rejected authentication: " + responseObject["error"].Value<string>();

                                return result;
                            }

                            JToken jsonValue;

                            try
                            {
                                jsonValue = JToken.Parse(responseObject["identifiers"].Value<string>());
                            }
                            catch (FileLoadException)
                            {
                                result["error"] = "Failed to parse identifier string.";

                                return result;
                            }

                            var identifiers = new List<string>();

                            if (jsonValue.Type == JTokenType.Array)
                            {
                                var array = (JArray)jsonValue;

                                foreach (var identifierToken in array.Children())
                                {
                                    if (identifierToken.Type == JTokenType.Array)
                                    {
                                        var identifierArray = (JArray)identifierToken;

                                        identifiers.Add(string.Format("{0}:{1}", identifierArray[0], identifierArray[1]));
                                    }
                                }
                            }

                            clientIdentifiers = identifiers;
                        }
                    }
                    catch (Exception e)
                    {
                        result["error"] = "Error during authentication: " + e.Message;

                        return result;
                    }
                }
                else
                {
                    // as there's no other authentication source, we'd guess
                    clientIdentifiers = new[] { string.Format("ip:{0}", ((IPEndPoint)context.RemoteEndPoint).Address) };
                }

                var client = new Client();
                client.Token = TokenGenerator.GenerateToken();
                client.Name = name;
                client.Guid = ulong.Parse(guid).ToString("x16");
                client.Identifiers = clientIdentifiers;
                client.ProtocolVersion = protocolNum;
                client.Touch();

                if (ClientInstances.Clients.ContainsKey(guid))
                {
                    gameServer.DropClient(ClientInstances.Clients[guid], "Duplicate GUID");
                }

                ClientInstances.AddClient(client);

                result["token"] = client.Token;
                result["protocol"] = Game.GameServer.PROTOCOL_VERSION;

                return result;
            };
        }
    }
}
