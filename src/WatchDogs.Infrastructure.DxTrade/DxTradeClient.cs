using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DxTrade;

public class DxTradeClient
{
    private readonly IDxTradeAuthenticator _dxTradeAuthenticator;
    private readonly Uri _DxtradeWebSocketURI;

    public DxTradeClient(IDxTradeAuthenticator dxTradeAuthenticator)
    {
        _dxTradeAuthenticator = dxTradeAuthenticator;

        _DxtradeWebSocketURI = new Uri($"wss://dxtrade.ftmo.com/client/connector?X-Atmosphere-tracking-id=0&X-Atmosphere-" +
            $"Framework=2.3.2-javascript&X-Atmosphere-Transport=websocket&X-Atmosphere-TrackMessageSize=true&Content-" +
            $"Type=text/x-gwt-rpc;%20charset=UTF-8&X-atmo-protocol=true&sessionState=dx-new&guest-mode=false");
    }
    public async Task EstablishWebSocketConnectionAsync(string sessionToken)
    {
        //string sessionToken = _dxTradeAuthenticator.AuthenticateAsync().Result;

        using (var ws = new ClientWebSocket())
        {
            ws.Options.SetRequestHeader("Cookie", sessionToken);

            await ws.ConnectAsync(_DxtradeWebSocketURI, CancellationToken.None);

            byte[] buffer = new byte[256];

            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    HandleMessage(buffer, result.Count);
                }
            }
        }
    }
    private static void HandleMessage(byte[] buffer, int count)
    {
        Console.WriteLine($"Received {BitConverter.ToString(buffer, 0, count)}");
    }
}
