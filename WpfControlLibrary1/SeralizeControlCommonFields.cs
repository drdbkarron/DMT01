using System;

namespace WpfControlLibrary1
{
	[Serializable ( )]
	public class SeralizeControlCommonFields
	{
		public String ControlClass;
		public String ControlName;
		public String SaveStateFileName;
		public Boolean UpdatedFromXmlFiles;
		//public Object Parent;
		public SeralizeControlCommonFields ( )
		{
			this . ControlClass = string . Empty;
			this . ControlName = string . Empty;
			this . SaveStateFileName = string . Empty;
			this . UpdatedFromXmlFiles = false;
			this . UpdatedFromXmlFiles = false;
		}
	}
  }

