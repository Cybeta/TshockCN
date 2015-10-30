﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using System.Web;
using TerrariaApi.Server;

namespace TShockAPI
{
	public class StatTracker
	{
		private bool failed;
		private bool initialized;
		public StatTracker()
		{
			
		}

		public void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				ThreadPool.QueueUserWorkItem(SendUpdate);
			}
		}

		private void SendUpdate(object info)
		{
			Thread.Sleep(1000*60*15);
			var data = new JsonData
			{
				port = Terraria.Netplay.ListenPort,
				currentPlayers = TShock.Utils.ActivePlayers(),
				maxPlayers = TShock.Config.MaxSlots,
				systemRam = 0,
				systemCPUClock = 0,
				version = TShock.VersionNum.ToString(),
				terrariaVersion = Terraria.Main.versionNumber2,
				mono = ServerApi.RunningMono
			};

			var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data);
			var encoded = HttpUtility.UrlEncode(serialized);
			var uri = String.Format("http://stats.tshock.co/publish/{0}", encoded);
			var client = (HttpWebRequest)WebRequest.Create(uri);
			client.Timeout = 5000;
			try
			{
				using (var resp = TShock.Utils.GetResponseNoException(client))
				{
					if (resp.StatusCode != HttpStatusCode.OK)
					{
						throw new IOException("Server did not respond with an OK.");
					}

					failed = false;
				}
			}
			catch (Exception e)
			{
				if (!failed)
				{
					TShock.Log.ConsoleError("StatTracker Exception: {0}", e);
					failed = true;
				}
			}

			ThreadPool.QueueUserWorkItem(SendUpdate);
		}
	}

	public struct JsonData
	{
		public int port;
		public int currentPlayers;
		public int maxPlayers;
		public int systemRam;
		public int systemCPUClock;
		public string version;
		public string terrariaVersion;
		public bool mono;
	}
}
