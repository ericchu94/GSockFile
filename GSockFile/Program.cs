using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace GSockFile
{
	class MainClass
	{
		public static void Main (string[] args)
		{	
			if (args.Length != 1) {
				Console.Error.WriteLine ("{0}: missing file operand", System.AppDomain.CurrentDomain.FriendlyName);
				return;
			}

			Console.Write ("IP: ");
			var ip = Console.ReadLine ();

			SendFile (ip, args [0]);

			Console.Write ("Press Enter to exit...");
			Console.ReadLine ();
		}

		private static void SendFile (string ip, string file)
		{
			var client = new TcpClient ();			
			try {
				Console.Write ("Connecting... ");
				client.Connect (ip, 5000);
				Console.WriteLine ("Done");
			} catch (Exception e) {
				Console.Error.WriteLine ();
				Console.Error.WriteLine ("Failed to connect: {0}", e);
				return;
			}

			using (var stream = client.GetStream ())
			using (var writer = new BinaryWriter (stream)) {

				var info = new FileInfo (file);
				try {
					Console.Write ("Sending file length... ");
					writer.Write (IPAddress.HostToNetworkOrder (info.Length));
					Console.WriteLine ("Done");
				} catch (Exception e) {
					Console.Error.WriteLine ();
					Console.Error.WriteLine ("Failed to send file length: {0}", e);
					return;
				}

				try {
					Console.Write ("Sending file... ");
					using (var fileStream = info.OpenRead ()) {
						fileStream.CopyTo (stream);
					}
					Console.WriteLine ("Done");
				} catch (Exception e) {
					Console.Error.WriteLine ();
					Console.Error.WriteLine ("Failed to send file: {0}", e);
					return;
				}
			}
			client.Close ();
		}
	}
}
