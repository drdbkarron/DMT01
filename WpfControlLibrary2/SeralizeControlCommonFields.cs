using System;

namespace DMT01
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
			ControlClass = string . Empty;
			ControlName = string . Empty;
			SaveStateFileName = string . Empty;
			UpdatedFromXmlFiles = false;
			UpdatedFromXmlFiles = false;
		}
	}

  }
 
