using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

class MyFirstClass
{
	static void error(string msg)
	{
		Console.Error.WriteLine("[-] Error: " + msg);
		Environment.Exit(1);
	}

	static void usage()
	{
		string name = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
		Console.WriteLine("Usage: " + name + " <options>");
		Console.WriteLine("Options:");
		Console.WriteLine("  -u, --url\t\tServer url.");
		Console.WriteLine("  -f, --file\t\tMain file.");
		Console.WriteLine("  -b, --bytes\t\tMax bytes per run. (default: 50)");
		Environment.Exit(0);
	}

	static void proc(string url, string post_data)
	{
		WebClient client = new WebClient();
		ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
		client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
		byte[] responseBytes = client.UploadData(url, "POST", System.Text.Encoding.UTF8.GetBytes(post_data));
		string responseBody = System.Text.Encoding.UTF8.GetString(responseBytes);
	}

	static string encode(byte[] data)
	{
		string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
		StringBuilder encoded = new StringBuilder();
		int bits = 0;
		int value = 0;

		foreach (byte b in data)
		{
			value = (value << 8) | b;
			bits += 8;

			while (bits >= 5)
			{
				encoded.Append(base32Chars[(value >> (bits - 5)) & 0x1F]);
				bits -= 5;
			}
		}

		if (bits > 0)
		{
			value <<= (5 - bits);
			encoded.Append(base32Chars[value & 0x1F]);
		}

		// Add padding if necessary
		while (encoded.Length % 8 != 0)
			encoded.Append('=');

		return encoded.ToString();
	}

	static List<string> Split(string str, int bytes)
    {
        List<string> strings = new List<string>();

        if (str.Length > bytes)
        {
            for (int i = 0; i < str.Length; i += bytes)
            {
                int sLen = Math.Min(bytes, str.Length - i);
                strings.Add(str.Substring(i, sLen));
            }
        }
        else strings.Add(str);
        return strings;
    }

	static void Main(string []args)
	{
		string url = "", file = "", contents = "";
		int bytes = 50;

		Console.CancelKeyPress += (sender, e) =>
        {
        	Console.WriteLine("\n[-] Exiting...");
            Environment.Exit(1);
        };

		if (args.Length < 4 || args.Length > 7)
			usage();

		for (int i = 0; i < args.Length; i++)
		{
			if (args[i] == "-h" || args[i] == "--help")
				usage();

			else if (args[i] == "-u" || args[i] == "--url")
			{
				try{url = args[i+1];}
				catch (IndexOutOfRangeException){error("-u/--url expected a proper https url or atleast an ip.");}
				if (!url.StartsWith("http"))
					url = "https://" + url;
			}

			else if (args[i] == "-f" || args[i] == "--file")
			{
				try
				{
					file = Path.GetFileName(args[i+1]);
					contents = encode(File.ReadAllBytes(args[i+1]));
				}
				catch (IndexOutOfRangeException){error("-f/--file expected a proper file.");}
				catch (FileNotFoundException){error("File not found.");}
			}

			else if (args[i] == "-b" || args[i] == "--bytes")
			{
				try
				{
					if (!int.TryParse(args[i+1], out bytes))
						throw new IndexOutOfRangeException();
				}
				catch (IndexOutOfRangeException){error("-b/--bytes expected a integer.");}
			}
		}

		float p = 1.0F;
		List<string> data = Split(contents, bytes);
		Console.Write("[>] Starting Transmission...[ENTER]");
		Console.ReadKey();
		Console.WriteLine();
		foreach (string buf in data)
		{
			string post_data = "file=" + file + "&data=" + buf;
			proc(url, post_data);
			Console.Write("[>] Progress: " + (float) Math.Round(p / data.Count * 100, 2) + "%\r");
			p++;
		}
	}
}
