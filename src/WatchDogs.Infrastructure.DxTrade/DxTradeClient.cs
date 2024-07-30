using Contracts;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Infrastructure.DxTrade;
public class DxTradeClient
{
    private readonly Dictionary<string, string> cookies;
    private readonly DxTradeConnectionOptions _dxTradeConnectionOptions;

    public DxTradeClient(Dictionary<string, string> cookies, IOptions<DxTradeConnectionOptions> options)
    {
        _dxTradeConnectionOptions = options.Value;

        this.cookies = cookies;
    }

    public void EstablishHandshake()
    {
        var cookieString = string.Join("; ", cookies.Select(kv => $"{kv.Key}={kv.Value}"));
        var headers = new Dictionary<string, string>
        {
            { "Cookie", cookieString }
        };

        var wsUrl = $"wss://dxtrade.{_dxTradeConnectionOptions.Vendor}.com/client/connector?X-Atmosphere-tracking-id=0&X-Atmosphere-Framework=2.3.2-javascript&X-Atmosphere-Transport=websocket&X-Atmosphere-TrackMessageSize=true&Content-Type=text/x-gwt-rpc;%20charset=UTF-8&X-atmo-protocol=true&sessionState=dx-new&guest-mode=false";

        using (var ws = new ClientWebSocket())
        {
            foreach (var header in headers)
            {
                ws.SetCookie(new WebSocketSharp.Net.Cookie(header.Key, header.Value));
            }

            try
            {
                ws.ConnectAsync(wsUrl, );
                Console.WriteLine("WebSocket connection established.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("WebSocket Error: " + ex.Message);
            }
            finally
            {
                ws.Close();
                Console.WriteLine("WebSocket connection closed.");
            }
        }
    }
}