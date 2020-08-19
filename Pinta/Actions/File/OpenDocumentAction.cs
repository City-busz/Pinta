﻿// 
// OpenDocumentAction.cs
//  
// Author:
//       Jonathan Pobst <monkey@jpobst.com>
// 
// Copyright (c) 2010 Jonathan Pobst
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Pinta.Core;
using Gtk;

namespace Pinta.Actions
{
	class OpenDocumentAction : IActionHandler
	{
		#region IActionHandler Members
		public void Initialize ()
		{
			PintaCore.Actions.File.Open.Activated += Activated;
		}

		public void Uninitialize ()
		{
			PintaCore.Actions.File.Open.Activated -= Activated;
		}
		#endregion

		private void Activated (object sender, EventArgs e)
		{
			using var fcd = new Gtk.FileChooserDialog(Translations.GetString("Open Image File"), PintaCore.Chrome.MainWindow,
								FileChooserAction.Open, Pinta.Core.GtkExtensions.DialogButtonsCancelOpen());

			// Add image files filter
			FileFilter ff = new FileFilter ();
            foreach (var format in PintaCore.System.ImageFormats.Formats) {
				if (!format.IsWriteOnly ()) {
					foreach (var ext in format.Extensions)
						ff.AddPattern (string.Format("*.{0}", ext));
				}
            }

			ff.Name = Translations.GetString ("Image files");
			fcd.AddFilter (ff);

			FileFilter ff2 = new FileFilter ();
			ff2.Name = Translations.GetString ("All files");
			ff2.AddPattern ("*.*");
			fcd.AddFilter (ff2);

            fcd.SetCurrentFolder (PintaCore.System.GetDialogDirectory ());
			fcd.SelectMultiple = true;

			fcd.AddImagePreview ();

			int response = fcd.Run ();

			if (response == (int)Gtk.ResponseType.Ok) {
				PintaCore.System.LastDialogDirectory = fcd.CurrentFolder;

				foreach (var file in fcd.Filenames)
					if (PintaCore.Workspace.OpenFile (file, fcd))
						RecentManager.Default.AddFull (fcd.Uri, PintaCore.System.RecentData);
			}
		}
	}
}
