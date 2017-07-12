using System;
using System.Collections.Generic;
using System.Windows;

namespace ClipboardTool
{
	public class ClipboardListener
	{
		private readonly ICollection<string> _texts = new List<string>();

		public Action<string> Add;

		public IEnumerable<string> Texts => _texts;

		public void CheckClipboard()
		{
			var text = Clipboard.GetText();
			if (string.IsNullOrEmpty(text))
				return;

			text = text.Trim();

			if (!_texts.Contains(text))
			{
				_texts.Add(text);
				Add(text);
			}
		}

		public void Clear()
		{
			_texts.Clear();
		}
	}
}
