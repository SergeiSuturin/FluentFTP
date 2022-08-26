﻿using System;
using FluentFTP.Helpers;
using System.Threading;
using FluentFTP.Client.Modules;
using System.Threading;
using System.Threading.Tasks;

namespace FluentFTP {
	public partial class FtpClient {

		/// <summary>
		/// Deletes a file on the server
		/// </summary>
		/// <param name="path">The full or relative path to the file</param>
		public void DeleteFile(string path) {
			FtpReply reply;

			// verify args
			if (path.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", "path");
			}

			lock (m_lock) {
				path = path.GetFtpPath();

				LogFunc(nameof(DeleteFile), new object[] { path });

				if (!(reply = Execute("DELE " + path)).Success) {
					throw new FtpCommandException(reply);
				}
			}
		}

#if ASYNC
		/// <summary>
		/// Deletes a file from the server asynchronously
		/// </summary>
		/// <param name="path">The full or relative path to the file</param>
		/// <param name="token">The token that can be used to cancel the entire process</param>
		public async Task DeleteFileAsync(string path, CancellationToken token = default(CancellationToken)) {
			FtpReply reply;

			// verify args
			if (path.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", "path");
			}

			path = path.GetFtpPath();

			LogFunc(nameof(DeleteFileAsync), new object[] { path });

			if (!(reply = await ExecuteAsync("DELE " + path, token)).Success) {
				throw new FtpCommandException(reply);
			}
		}
#endif

	}
}