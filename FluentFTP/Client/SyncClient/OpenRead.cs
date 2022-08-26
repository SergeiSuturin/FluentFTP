﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using FluentFTP.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace FluentFTP {
	public partial class FtpClient {

		/// <summary>
		/// Opens the specified file for reading
		/// </summary>
		/// <param name="path">The full or relative path of the file</param>
		/// <param name="type">ASCII/Binary</param>
		/// <param name="restart">Resume location</param>
		/// <param name="checkIfFileExists">Only set this to false if you are SURE that the file does not exist. If true, it reads the file size and saves it into the stream length.</param>
		/// <returns>A stream for reading the file on the server</returns>
		//[Obsolete("OpenRead() is obsolete, please use Download() or DownloadFile() instead", false)]
		public virtual Stream OpenRead(string path, FtpDataType type = FtpDataType.Binary, long restart = 0, bool checkIfFileExists = true) {
			return OpenRead(path, type, restart, checkIfFileExists ? 0 : -1);
		}

		/// <summary>
		/// Opens the specified file for reading
		/// </summary>
		/// <param name="path">The full or relative path of the file</param>
		/// <param name="type">ASCII/Binary</param>
		/// <param name="restart">Resume location</param>
		/// <param name="fileLen">
		/// <para>Pass in a file length if known</para>
		/// <br> -1 => File length is irrelevant, do not attempt to determine it</br>
		/// <br> 0  => File length is unknown, try to determine it</br>
		/// <br> >0 => File length is KNOWN. No need to determine it</br>
		/// </param>
		/// <returns>A stream for reading the file on the server</returns>
		//[Obsolete("OpenRead() is obsolete, please use Download() or DownloadFile() instead", false)]
		public virtual Stream OpenRead(string path, FtpDataType type, long restart, long fileLen) {
			// verify args
			if (path.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", nameof(path));
			}

			path = path.GetFtpPath();
			m_path = path;

			LogFunc(nameof(OpenRead), new object[] { path, type, restart, fileLen });

			var client = this;
			FtpDataStream stream = null;
			long length = 0;

			lock (m_lock) {

				length = fileLen == 0 ? client.GetFileSize(path) : fileLen;

				client.SetDataType(type);
				stream = client.OpenDataStream("RETR " + path, restart);
			}

			if (stream != null) {
				if (length > 0) {
					stream.SetLength(length);
				}

				if (restart > 0) {
					stream.SetPosition(restart);
				}
			}

			return stream;
		}

#if ASYNC

		/// <summary>
		/// Opens the specified file for reading asynchronously
		/// </summary>
		/// <param name="path">The full or relative path of the file</param>
		/// <param name="type">ASCII/Binary</param>
		/// <param name="restart">Resume location</param>
		/// <param name="checkIfFileExists">Only set this to false if you are SURE that the file does not exist. If true, it reads the file size and saves it into the stream length.</param>
		/// <param name="token">The token that can be used to cancel the entire process</param>
		/// <returns>A stream for reading the file on the server</returns>
		//[Obsolete("OpenReadAsync() is obsolete, please use DownloadAsync() or DownloadFileAsync() instead", false)]
		public virtual Task<Stream> OpenReadAsync(string path, FtpDataType type = FtpDataType.Binary, long restart = 0,
			bool checkIfFileExists = true, CancellationToken token = default(CancellationToken)) {
			return OpenReadAsync(path, type, restart, checkIfFileExists ? 0 : -1, token);
		}


		/// <summary>
		/// Opens the specified file for reading asynchronously
		/// </summary>
		/// <param name="path">The full or relative path of the file</param>
		/// <param name="type">ASCII/Binary</param>
		/// <param name="restart">Resume location</param>
		/// <param name="fileLen">
		/// <para>Pass in a file length if known</para>
		/// <br> -1 => File length is irrelevant, do not attempt to determine it</br>
		/// <br> 0  => File length is unknown, try to determine it</br>
		/// <br> >0 => File length is KNOWN. No need to determine it</br>
		/// </param>
		/// <param name="token">The token that can be used to cancel the entire process</param>
		/// <returns>A stream for reading the file on the server</returns>
		//[Obsolete("OpenReadAsync() is obsolete, please use DownloadAsync() or DownloadFileAsync() instead", false)]
		public virtual async Task<Stream> OpenReadAsync(string path, FtpDataType type, long restart, long fileLen, CancellationToken token = default(CancellationToken)) {
			// verify args
			if (path.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", nameof(path));
			}

			path = path.GetFtpPath();
			m_path = path;

			LogFunc(nameof(OpenReadAsync), new object[] { path, type, restart, fileLen });

			var client = this;
			FtpDataStream stream = null;
			long length = 0;

			length = fileLen == 0 ? await client.GetFileSizeAsync(path, -1, token) : fileLen;

			await client.SetDataTypeAsync(type, token);
			stream = await client.OpenDataStreamAsync("RETR " + path, restart, token);

			if (stream != null) {
				if (length > 0) {
					stream.SetLength(length);
				}

				if (restart > 0) {
					stream.SetPosition(restart);
				}
			}

			return stream;
		}

#endif
	}
}
