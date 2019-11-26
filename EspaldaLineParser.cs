using System;
using System.Collections;

namespace GL_ESPALDA
{
	/// <summary>
	/// Summary description for EspaldaLineParser.
	/// </summary>
	public class EspaldaLineParser
	{
		private String[] separators_char;
		private String[] components_start_char;
		private ArrayList components;
		private ArrayList separators;
		public ArrayList COMPONENTS
		{
			get
			{	return components;	}

		}
		public ArrayList SEPARATORS
		{
			get
			{	return separators;	}

		}

		public EspaldaLineParser()
		{
			separators_char = new String [1];
			separators_char[0] = " ";
			components_start_char = new String[1];
			components_start_char[0] = "-";
		}

		public bool ParseLine (String line)
		{
			components = new ArrayList ();
			separators = new ArrayList ();
			
			if (line.Length == 0)	// nothing in the line
				return true;
						
			String word = "";
			String previous_character = "";

			while (line.Length > 0)
			{
				if (line.Substring (0, 1) != separators_char[0])
				{
					if ((line.Substring (0, 1) != components_start_char[0])
						)
					{
						// continue word
						word += line.Substring (0, 1);
					}
					else
					{
						// add old word
						if (word != "")
							components.Add (word);
						// start new
						if (previous_character != " ")
							separators.Add ("");
						word = line.Substring (0, 1);
					}
				}
				else
				{
					separators.Add (line.Substring (0, 1));
					// add word to list
					components.Add (word);
					word = "";
				}
				previous_character = line.Substring (0, 1);
				line = line.Substring (1);
			}
			if (word != "")
				components.Add (word);

			return true;
		}

		
	}
}
