﻿using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BattleIAserver
{
    public class OneDisplay
    {
        private WebSocket webSocket = null;
        public Guid ClientGuid { get; }
        public bool MustRemove = false;

        public OneDisplay(WebSocket webSocket)
        {
            this.webSocket = webSocket;
            ClientGuid = Guid.NewGuid();
        }

        public async Task WaitReceive()
        {
            // 1 - on attend la première data du client
            // qui doit etre son GUID

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = null;
            try
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch (Exception err)
            {
                MustRemove = true;
                Console.WriteLine($"[VIEWER ERROR] {err.Message}");
                try
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, "[DISPLAY] Error waiting data", CancellationToken.None);
                }
                catch (Exception) { }
                return;
            }
            while (!result.CloseStatus.HasValue)
            {
                if (result.Count < 1)
                {
                    MustRemove = true;
                    await webSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, "[DISPLAY] Missing data in answer", CancellationToken.None);
                    return;
                }

                string command = System.Text.Encoding.UTF8.GetString(buffer, 0, 1);
                Console.WriteLine($"[DISPLAY] Received command '{command}'");
                if (command == "Q")
                {
                    MustRemove = true;
                    webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"[DISPLAY CLOSING] receive {command}", CancellationToken.None);
                    return;
                }
                if (command != "M")
                {
                    MustRemove = true;
                    await webSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, $"[DISPLAY ERROR] Not the right answer, waiting M#, receive {command}", CancellationToken.None);
                    return;
                }
                /*if (result.Count < 1)
                {
                    MustRemove = true;
                    await webSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, "Missing data in answer 'D'", CancellationToken.None);
                    return;
                }*/
                await SendMapInfo();

                try
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                catch (Exception err)
                {
                    MustRemove = true;
                    System.Diagnostics.Debug.WriteLine($"[DISPLAY ERROR] {err.Message}");
                    try
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, "[DISPLAY] Error waiting data", CancellationToken.None);
                    }
                    catch (Exception) { }
                    return;
                }
            }
            MustRemove = true;
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public async Task SendMapInfo()
        {
            var buffer = new byte[5 + MainGame.Settings.MapWidth * MainGame.Settings.MapHeight];
            buffer[0] = System.Text.Encoding.ASCII.GetBytes("M")[0];
            buffer[1] = (byte)MainGame.Settings.MapWidth;
            buffer[2] = (byte)(MainGame.Settings.MapWidth >> 8);
            buffer[3] = (byte)MainGame.Settings.MapHeight;
            buffer[4] = (byte)(MainGame.Settings.MapHeight >> 8);
            int index = 5;
            for (int j = 0; j < MainGame.Settings.MapHeight; j++)
                for (int i = 0; i < MainGame.Settings.MapWidth; i++)
                    buffer[index++] = (byte)MainGame.TheMap[i, j];
            /*foreach(OneClient oc in MainGame.AllBot)
            {
                buffer[5 + oc.bot.X +( oc.bot.Y * MainGame.MapWidth)] = (byte)CaseState.Ennemy;
            }*/
            try
            {
                Console.WriteLine("[DISPLAY] Sending MAPINFO");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception err)
            {
                Console.WriteLine($"[DISPLAY ERROR] {err.Message}");
                MustRemove = true;
            }
        }


        public async Task SendMovePlayer(byte x1, byte y1, byte x2, byte y2)
        {
            var buffer = new byte[5];
            buffer[0] = System.Text.Encoding.ASCII.GetBytes("P")[0];
            buffer[1] = x1;
            buffer[2] = y1;
            buffer[3] = x2;
            buffer[4] = y2;
            try
            {
                System.Diagnostics.Debug.WriteLine("[DISPLAY] Sending move player");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine($"[DISPLAY ERROR] {err.Message}");
                MustRemove = true;
            }
        }

        public async Task SendRemovePlayer(byte x1, byte y1)
        {
            var buffer = new byte[3];
            buffer[0] = System.Text.Encoding.ASCII.GetBytes("R")[0];
            buffer[1] = x1;
            buffer[2] = y1;
            try
            {
                System.Diagnostics.Debug.WriteLine("[DISPLAY] Sending remove player");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine($"[DISPLAY ERROR] {err.Message}");
                MustRemove = true;
            }
        }

        public async Task SendClearCase(byte x1, byte y1)
        {
            var buffer = new byte[3];
            buffer[0] = System.Text.Encoding.ASCII.GetBytes("C")[0];
            buffer[1] = x1;
            buffer[2] = y1;
            try
            {
                System.Diagnostics.Debug.WriteLine("[DISPLAY] Sending clear case");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine($"[DISPLAY ERROR] {err.Message}");
                MustRemove = true;
            }
        }

        public async Task SendAddEnergy(byte x1, byte y1)
        {
            var buffer = new byte[3];
            buffer[0] = System.Text.Encoding.ASCII.GetBytes("E")[0];
            buffer[1] = x1;
            buffer[2] = y1;
            try
            {
                System.Diagnostics.Debug.WriteLine("[DISPLAY] Sending add energy");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine($"[DISPLAY ERROR] {err.Message}");
                MustRemove = true;
            }
        }

        public async Task SendAddBullet(byte x1, byte y1, byte direction, UInt16 duration)
        {
            var buffer = new byte[6];
            buffer[0] = System.Text.Encoding.ASCII.GetBytes("B")[0];
            buffer[1] = x1;
            buffer[2] = y1;
            buffer[3] = direction;
            buffer[4] = (byte)duration;
            buffer[5] = (byte)(duration >> 8);
            try
            {
                System.Diagnostics.Debug.WriteLine("[DISPLAY] Sending add bullet");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine($"[DISPLAY ERROR] {err.Message}");
                MustRemove = true;
            }
        }

        public async Task SendPlayerShield(byte x1, byte y1, byte s1, byte s2)
        {
            var buffer = new byte[5];
            buffer[0] = System.Text.Encoding.ASCII.GetBytes("S")[0];
            buffer[1] = x1;
            buffer[2] = y1;
            buffer[3] = s1;
            buffer[4] = s2;
            try
            {
                System.Diagnostics.Debug.WriteLine("[DISPLAY] Sending player shield");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine($"[DISPLAY ERROR] {err.Message}");
                MustRemove = true;
            }
        }

        public async Task SendPlayerCloak(byte x1, byte y1, byte s1, byte s2)
        {
            var buffer = new byte[5];
            buffer[0] = System.Text.Encoding.ASCII.GetBytes("H")[0];
            buffer[1] = x1;
            buffer[2] = y1;
            buffer[3] = s1;
            buffer[4] = s2;
            try
            {
                System.Diagnostics.Debug.WriteLine("[DISPLAY] Sending player cloak");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine($"[DISPLAY ERROR] {err.Message}");
                MustRemove = true;
            }
        }

    }
}
