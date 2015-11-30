﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierSitesFileShuffler
{
	public class ViewFile
	{
		public string Extension { get; set; }
		public string SourceLocation { get; set; }
		public double FileSize { get; set; }
		public string PartNo { get; set; }
		public string CopySite { get; set; }
		public string FileName { get; set; }
		public bool SiteFound { get; set; }
		public string Supplier { get; set; }
		public string Version { get; set; }
		public string Status { get; set; }


		public ViewFile()
		{

		}
	}
}
