using System;

namespace GL_ESPALDA
{
	/// <summary>
	/// Summary description for EspaldaLine.
	/// </summary>
	public class EspaldaLine
	{
		public enum EspaldaLineType {	NONE_LINE = 0, COMMAND_LINE, OUTPUT_LINE};

		private String text;
		public String TEXT
		{
			get
			{	return text;	}
			
			set
			{	text = value;	}
		}

		private EspaldaLineType type;
		public EspaldaLineType TYPE
		{
			get
			{	return type;	}

			set
			{	type = value;	}
		}

		public EspaldaLine()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public EspaldaLine(String tx, EspaldaLineType tp)
		{
			TEXT = tx;
			TYPE = tp;
		}
	}
}
