using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace GL_ESPALDA
{
	/// <summary>
	/// Summary description for frmEspaldaMain.
	/// </summary>
	public class frmEspaldaMain : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// To do
		/// enhance parser to include directories (started but still under test)
		/// file associations, open file with (started but still under test)
		/// console application : javac, ... (to check see above point) if yes no new window get output
		/// tab with maximum common string doesn't include search for commands
		/// copyfile, movefile (2 arguments???)
		/// </summary>
		
		#region VARIABLES
		private Graphics form_graphics;
		private SolidBrush text_brush;
		private float text_x;
		private float text_y;
		private float height_between_command_lines;
		private float height_between_output_lines;
		private int max_lines_number;
		private int max_visible_lines_number;
		private EspaldaLine[] espalda_lines;
		private int line_number;
		private int visible_start_line_number;

		private String[] commands;

		private ASCIIEncoding ascii_encoding;

		private bool Alt_Gr;
		
		private String line_start;

		private int get_line_number;
		private System.Windows.Forms.VScrollBar vScrollBar;

		private Keys last_key;

		private EspaldaLineParser espalda_line_parser;

		int new_scroll_value;

		private String program_version;

		public class YesNoQuestion
		{
			private String yes_no_question_command_string;
			private Object yes_no_question_arguments;

			public String YesNoQuestionCommandString
			{
				get
				{	return yes_no_question_command_string;	}
			}
			public Object YesNoQuestionArguments
			{
				get
				{	return yes_no_question_arguments;	}
			}
			public YesNoQuestion (String s, Object o)
			{
				yes_no_question_command_string = s;
				yes_no_question_arguments = o;
			}
		}
		private YesNoQuestion yes_no_question_object;
		#endregion
		
		#region CONSTRUCTOR
		public frmEspaldaMain()
		{
			program_version = "2.2.2";
			
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			text_brush = new SolidBrush (Color.White);
			text_x = 1;
			text_y = 25;

			height_between_command_lines = 30;
			height_between_output_lines = 30;

			String current_dir = Environment.CurrentDirectory;
			current_dir = current_dir.Replace ("\\", "/");
			line_start = current_dir.Substring (0, 3) + ">";
			max_lines_number = 2500;
			max_visible_lines_number = ((int)this.ClientSize.Height - (int)text_y) / 30 + 1;
			espalda_lines = new EspaldaLine[max_lines_number];
			InitializeLinesText ();
			line_number = 1;
			visible_start_line_number = 1;

			ascii_encoding = new ASCIIEncoding();

			get_line_number = -1;

			last_key = Keys.None;

			vScrollBar.Minimum = 0;
			vScrollBar.Maximum = max_lines_number;

			espalda_line_parser = new EspaldaLineParser ();

			commands = new String [22];
			commands[0] = "exit";
			commands[1] = "bye";
			commands[2] = "quit";
			commands[3] = "cls";
			commands[4] = "date";
			commands[5] = "time";
			commands[6] = "dt";
			commands[7] = "min";
			commands[8] = "pwd";
			commands[9] = "trem";
			commands[10] = "sysinfo";
			commands[11] = "dir";
			commands[12] = "file";
			commands[13] = "ls";
			commands[14] = "cd";
			commands[15] = "help";
			commands[16] = "halt";
			commands[17] = "reboot";
			commands[18] = "title";
			commands[19] = "delfile";
			commands[20] = "gldoors";
			commands[21] = "hrm";
			Array.Sort (commands);

			yes_no_question_object = null;
			
			// Double Buffering
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
		}
		#endregion

		#region Dispose
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmEspaldaMain));
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// vScrollBar
			// 
			this.vScrollBar.Cursor = System.Windows.Forms.Cursors.Hand;
			this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar.Enabled = false;
			this.vScrollBar.LargeChange = 20;
			this.vScrollBar.Location = new System.Drawing.Point(676, 0);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(16, 317);
			this.vScrollBar.TabIndex = 1;
			this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
			// 
			// frmEspaldaMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(692, 317);
			this.Controls.Add(this.vScrollBar);
			this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "frmEspaldaMain";
			this.Text = "GL-ESPALDA";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmEspaldaMain_KeyDown);
			this.Resize += new System.EventHandler(this.frmEspaldaMain_Resize);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmEspaldaMain_Paint);
			this.ResumeLayout(false);

		}
		#endregion

		#region MAIN
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmEspaldaMain());
		}
		#endregion MAIN

		#region DLL
		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
		public static extern short GetKeyState(int keyCode);
		#endregion

		#region FORM_KEYDOWN
		private void frmEspaldaMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			bool CapsLock = (((ushort) GetKeyState(0x14 /*VK_CAPITAL*/)) & 0xffff) != 0;
			bool NumLock  = (((ushort) GetKeyState(0x90 /*VK_NUMLOCK*/)) & 0xffff) != 0;
			bool up_character = (CapsLock && !e.Shift) || (!CapsLock && e.Shift);

			if (e.KeyCode == Keys.Menu && e.Control)
			{
				Alt_Gr = true;
				return;
			}

			if ((e.KeyCode != Keys.PageUp)
				&& (e.KeyCode != Keys.PageDown)
				&& (line_number > visible_start_line_number + max_visible_lines_number)
				)
			{
				visible_start_line_number = line_number - max_visible_lines_number + 1;
			}
			
			String string_to_draw = "";
			if ((e.KeyValue >= 65) && (e.KeyValue <= 90))
			{
				//
				if (yes_no_question_object != null)
				{
					if (e.KeyValue == 89)
					{
						if (yes_no_question_object.YesNoQuestionCommandString == "Halt")
						{
							EspaldaWindowsShutdown sh = new EspaldaWindowsShutdown ();
							sh.Halt ();
						}
						else if (yes_no_question_object.YesNoQuestionCommandString == "Reboot")
						{
							EspaldaWindowsShutdown sh = new EspaldaWindowsShutdown ();
							sh.Reboot ();
						}
						else if (yes_no_question_object.YesNoQuestionCommandString == "delfile")
						{
							String[] files = (String[])yes_no_question_object.YesNoQuestionArguments;
							foreach (String file in files)
							{
								File.Delete (file);
							}
							NewOutputLine (String.Format ("{0} fichier(s) supprimé(s).", files.Length));

							// New line is visible
							LineVisibilityCheck ();

							// Open a new command line
							line_number++;
							espalda_lines[line_number - 1].TEXT = line_start;
							espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.NONE_LINE;
							string_to_draw = "line_end";
				
							get_line_number = line_number - 2;
						}
					}
				}
				else
				{
					// Une lettre
					string_to_draw = ascii_encoding.GetString(new Byte[1]{Convert.ToByte (e.KeyValue)});
					string_to_draw = (CapsLock)?((e.Shift)?string_to_draw.ToLower ():string_to_draw):((!e.Shift)?string_to_draw.ToLower ():string_to_draw);
				}
			}
			else if (e.KeyCode == Keys.Space)
			{
				string_to_draw = " ";
			}
			else if (e.KeyCode == Keys.Enter)
			{
				if (espalda_lines[line_number - 1].TEXT != "")
					espalda_lines[line_number - 1].TEXT = espalda_lines[line_number - 1].TEXT.Substring (0, espalda_lines[line_number - 1].TEXT.Length - 1);
				LineVisibilityCheck ();

				espalda_line_parser.ParseLine (espalda_lines[line_number - 1].TEXT.Substring (line_start.Length));
				if (espalda_line_parser.COMPONENTS.Count == 0)
				{}
				else if ((((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "exit")
					|| (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "bye")
					|| (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "quit")
					)
				{
					ByeCommand ();
					return;
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "cd")
				{
					string_to_draw = CdCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "cd /")
				{
					CdantislashantislashCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "cd..")
				{
					string_to_draw = CddotdotCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "cls")
				{
					ClsCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "date")
				{
					DateCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "delfile")
				{
					DeleteFileCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "dir")
				{
					DirCommand ();	
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "dt")
				{
					DtCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "halt")
				{
					HaltCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "help")
				{
					HelpCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "hrm")
				{
					HrmCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "file")
				{
					FileCommand ();	
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "glatomic")
				{
					GLAtomicCommand ();	
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "gldoors")
				{
					GLDoorsCommand ();	
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "ls")
				{
					LsCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "min")
				{
					MinCommand ();
				}
				else if ((((((String)espalda_line_parser.COMPONENTS[0]).ToLower ()).Length == 2) && ((((String)espalda_line_parser.COMPONENTS[0]).ToLower ()).Substring (1) == ":"))
					|| (((((String)espalda_line_parser.COMPONENTS[0]).ToLower ()).Length == 3) && ((((String)espalda_line_parser.COMPONENTS[0]).ToLower ()).Substring (1) == ":/"))
					)
				{
					NewDriveLetter ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "pwd")
				{
					PwdCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "reboot")
				{
					RebootCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "sysinfo")
				{
					SysinfoCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "time")
				{
					TimeCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "title")
				{
					TitleCommand ();
				}
				else if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () == "trem")
				{
					TremCommand ();
				}
				else
				{
					ExecutableCommand ();
				}

				CheckEndOfLines ();

				line_number++;
				espalda_lines[line_number - 1].TEXT = line_start;
				espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.NONE_LINE;
				string_to_draw = "line_end";
				
				get_line_number = line_number - 2;
			}
			else if (e.KeyCode == Keys.Down)
			{
				// get next string
				// add to get_line_number
				int add_to_get_line_number = 1;
				if (last_key == Keys.Up)
					add_to_get_line_number = 2;

				while ((get_line_number + add_to_get_line_number < espalda_lines.Length)
					&& (espalda_lines[get_line_number + add_to_get_line_number].TYPE != EspaldaLine.EspaldaLineType.COMMAND_LINE)
					&& (espalda_lines[get_line_number + add_to_get_line_number].TEXT != "")
					)
				{
					get_line_number += add_to_get_line_number;
					if (add_to_get_line_number == 2)
						add_to_get_line_number--;
				}

				if (get_line_number + add_to_get_line_number < espalda_lines.Length)
				{
					if (espalda_lines[get_line_number + add_to_get_line_number].TEXT != "")
					{
						get_line_number += add_to_get_line_number;
						espalda_lines[line_number - 1].TEXT = line_start;
						espalda_lines[line_number - 1].TEXT += espalda_lines[get_line_number].TEXT.Substring (espalda_lines[get_line_number].TEXT.LastIndexOf (">") + 1);
						espalda_lines[line_number - 1].TEXT += "_";
						string_to_draw = "line_end";
					}
					else
					{
						espalda_lines[line_number - 1].TEXT = line_start;
						espalda_lines[line_number - 1].TEXT += "_";
						string_to_draw = "line_end";
					}

				}
			}
			else if (e.KeyCode == Keys.Up)
			{
				// get previous string
				if (last_key == Keys.Down)
					get_line_number--;
				while (get_line_number >= 0 && espalda_lines[get_line_number].TYPE != EspaldaLine.EspaldaLineType.COMMAND_LINE)
				{
					get_line_number--;
				}
				
				if (get_line_number >= 0)
				{
					espalda_lines[line_number - 1].TEXT = line_start;
					espalda_lines[line_number - 1].TEXT += espalda_lines[get_line_number].TEXT.Substring (espalda_lines[get_line_number].TEXT.LastIndexOf (">") + 1);
					espalda_lines[line_number - 1].TEXT += "_";
					string_to_draw = "line_end";
					get_line_number--;
				}
			}
			else if (e.KeyValue == 8)
			{
				string_to_draw = "Backspace";
			}
			else if (e.KeyCode == Keys.Oemcomma)
			{
				if (up_character)
					string_to_draw = "?";
				else
					string_to_draw = ",";
			}
			else if (e.KeyCode == Keys.OemPeriod)
			{
				if (up_character)
					string_to_draw = ".";
				else
					string_to_draw = ";";
			}
			else if (e.KeyCode == Keys.OemQuestion)
			{
				if (up_character)
					string_to_draw = "/";
				else
					string_to_draw = ":";
			}
			else if (e.KeyCode == Keys.Oem8)
			{
				if (up_character)
					string_to_draw = "§";
				else
					string_to_draw = "!";
			}
			else if (e.KeyCode == Keys.OemPipe)
			{
				if (up_character)
					string_to_draw = "µ";
				else
					string_to_draw = "*";
			}
			else if (e.KeyCode == Keys.Oemtilde)
			{
				if (up_character)
					string_to_draw = "%";
				else
					string_to_draw = "ù";
			}
			else if (e.KeyCode == Keys.OemSemicolon)
			{
				if (up_character)
					string_to_draw = "£";
				else
					string_to_draw = "$";
			}
			else if (e.KeyCode == Keys.OemCloseBrackets)
			{
				if (up_character)
					string_to_draw = "¨";
				else
					string_to_draw = "^";
			}
			else if (e.KeyCode == Keys.OemQuotes)
			{
				if (up_character)
					string_to_draw = "";
				else
					string_to_draw = "²";
			}
			else if (e.KeyCode == Keys.D1)
			{
				if (up_character)
					string_to_draw = "1";
				else
					string_to_draw = "&";
			}
			else if (e.KeyCode == Keys.D2)
			{
				if (Alt_Gr)
					string_to_draw = "~";
				else if (up_character)
					string_to_draw = "2";
				else
					string_to_draw = "é";
			}
			else if (e.KeyCode == Keys.D3)
			{
				if (Alt_Gr)
					string_to_draw = "#";
				else if (up_character)
					string_to_draw = "3";
				else
					string_to_draw = "\"";
			}
			else if (e.KeyCode == Keys.D4)
			{
				if (Alt_Gr)
					string_to_draw = "{";
				else if (up_character)
					string_to_draw = "4";
				else
					string_to_draw = "'";
			}
			else if (e.KeyCode == Keys.D5)
			{
				if (Alt_Gr)
					string_to_draw = "[";
				else if (up_character)
					string_to_draw = "5";
				else
					string_to_draw = "(";
			}
			else if (e.KeyCode == Keys.D6)
			{
				if (Alt_Gr)
					string_to_draw = "|";
				else if (up_character)
					string_to_draw = "6";
				else
					string_to_draw = "-";
			}
			else if (e.KeyCode == Keys.D7)
			{
				if (Alt_Gr)
					string_to_draw = "`";
				else if (up_character)
					string_to_draw = "7";
				else
					string_to_draw = "é";
			}
			else if (e.KeyCode == Keys.D8)
			{
				if (Alt_Gr)
					string_to_draw = "\\";
				else if (up_character)
					string_to_draw = "8";
				else
					string_to_draw = "_";
			}
			else if (e.KeyCode == Keys.D9)
			{
				if (Alt_Gr)
					string_to_draw = "^";
				else if (up_character)
					string_to_draw = "9";
				else
					string_to_draw = "ç";
			}
			else if (e.KeyCode == Keys.D0)
			{
				if (Alt_Gr)
					string_to_draw = "@";
				else if (up_character)
					string_to_draw = "0";
				else
					string_to_draw = "à";
			}
			else if (e.KeyCode == Keys.OemOpenBrackets)
			{
				if (Alt_Gr)
					string_to_draw = "]";
				else if (up_character)
					string_to_draw = "°";
				else
					string_to_draw = ")";
			}
			else if (e.KeyCode == Keys.Oemplus)
			{
				if (Alt_Gr)
					string_to_draw = "}";
				else if (up_character)
					string_to_draw = "+";
				else
					string_to_draw = "=";
			}
			else if (e.KeyCode == Keys.OemOpenBrackets)
			{
				if (Alt_Gr)
					string_to_draw = "]";
				else if (up_character)
					string_to_draw = "°";
				else
					string_to_draw = ")";
			}
			else if (e.KeyCode == Keys.NumPad1)
			{
				if (NumLock)
					string_to_draw = "1";
			}
			else if (e.KeyCode == Keys.NumPad2)
			{
				if (NumLock)
					string_to_draw = "2";
			}
			else if (e.KeyCode == Keys.NumPad3)
			{
				if (NumLock)
					string_to_draw = "3";
			}
			else if (e.KeyCode == Keys.NumPad4)
			{
				if (NumLock)
					string_to_draw = "4";
			}
			else if (e.KeyCode == Keys.NumPad5)
			{
				if (NumLock)
					string_to_draw = "5";
			}
			else if (e.KeyCode == Keys.NumPad6)
			{
				if (NumLock)
					string_to_draw = "6";
			}
			else if (e.KeyCode == Keys.NumPad7)
			{
				if (NumLock)
					string_to_draw = "7";
			}
			else if (e.KeyCode == Keys.NumPad8)
			{
				if (NumLock)
					string_to_draw = "8";
			}
			else if (e.KeyCode == Keys.NumPad9)
			{
				if (NumLock)
					string_to_draw = "9";
			}
			else if (e.KeyCode == Keys.NumPad0)
			{
				if (NumLock)
					string_to_draw = "0";
			}
			else if (e.KeyCode == Keys.Decimal)
			{
				if (NumLock)
					string_to_draw = ".";
			}
			else if (e.KeyCode == Keys.Add)
			{
				if (NumLock)
					string_to_draw = "+";
			}
			else if (e.KeyCode == Keys.Subtract)
			{
				if (NumLock)
					string_to_draw = "-";
			}
			else if (e.KeyCode == Keys.Multiply)
			{
				if (NumLock)
					string_to_draw = "*";
			}
			else if (e.KeyCode == Keys.Divide)
			{
				if (NumLock)
					string_to_draw = "/";
			}
			else if (e.KeyCode == Keys.F10)
			{
				Application.Exit ();
			}
			else if (e.KeyCode == Keys.PageUp)
			{
				if (visible_start_line_number > 1)
				{
					visible_start_line_number--;
					this.Invalidate ();
				}
			}
			else if (e.KeyCode == Keys.PageDown)
			{
				if (visible_start_line_number > 0)
				{
					visible_start_line_number++;
					this.Invalidate ();
				}
			}
			else if (e.KeyCode == Keys.Tab)
			{
				try
				{
					string_to_draw = TabPressed ();
				}
				catch
				{}
			}
					
			if (string_to_draw != "")
			{
				if (string_to_draw != "Backspace")
				{
					if (string_to_draw == "line_end")
						string_to_draw = "";
					string_to_draw += "_";
					if (espalda_lines[line_number - 1].TEXT.Length > line_start.Length)
						espalda_lines[line_number - 1].TEXT = espalda_lines[line_number - 1].TEXT.Substring (0, espalda_lines[line_number - 1].TEXT.Length - 1);
					espalda_lines[line_number - 1].TEXT += string_to_draw;
				}
				else
				{
					if (espalda_lines[line_number - 1].TEXT.Length > line_start.Length + 1)
					{
						if (espalda_lines[line_number - 1].TEXT != "")
							espalda_lines[line_number - 1].TEXT = espalda_lines[line_number - 1].TEXT.Substring (0, espalda_lines[line_number - 1].TEXT.Length - 2);
						espalda_lines[line_number - 1].TEXT += "_";
					}
				}
				this.Invalidate ();
			}

			last_key = e.KeyCode;
			if (Alt_Gr)
				Alt_Gr = false;

			// if not enter pressed, then command shouldn't exist
			if ((yes_no_question_object != null) && (e.KeyCode != Keys.Enter))
				yes_no_question_object = null;
		}

		#endregion

		#region COMMANDS
		private void ByeCommand ()
		{
			NewOutputLine (line_start + "Au revoir!!");
			Application.Exit ();
		}

		private String CdCommand ()
		{
			String string_to_draw = "";
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			if (espalda_line_parser.COMPONENTS.Count > 1)
			{
				// cd directory
				// find directory
				String dir = "";
				for (int i = 1; i < espalda_line_parser.COMPONENTS.Count; i++)
				{
					if (dir != "")
						dir += espalda_line_parser.SEPARATORS[i - 1];
					dir += (String)espalda_line_parser.COMPONENTS[i];
				}
				if (dir.Substring (dir.Length - 1, 1) == "/")
					dir = dir.Substring (0, dir.Length - 1);

				if (dir == "..")
				{
					// cd..
					// return to previous directory
					line_start = line_start.Substring (0, line_start.LastIndexOf ("/"));
					if (line_start.IndexOf ("/") == -1)
						line_start += "/";
					line_start += ">";
					string_to_draw = "line_end";
				}
				else if (dir == "/")
				{
					// return to root (drive)
					line_start = line_start.Substring (0, 2) + "/>";
				}
				else
				{
					String start_dir = line_start.Substring (0, line_start.Length - 1);
					dir = GoToDir (start_dir, dir);
										
					if (dir != "")
					{
						line_start = dir + ">";
						string_to_draw = "line_end";
					}
					else
					{
						NewOutputLine ("Répertoire incorrect");
					}
													
				}
			}

			return string_to_draw;
		}

		private void CdantislashantislashCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			// return to root (drive)
			line_start = line_start.Substring (0, 2) + "/>";
		}

		private String CddotdotCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			// return to previous directory
			line_start = line_start.Substring (0, line_start.LastIndexOf ("/"));
			if (line_start.IndexOf ("/") == -1)
				line_start += "/";
			line_start += ">";
			return "line_end";
		}

		private void ClsCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;

			visible_start_line_number = line_number + 1;	
		}

		private void DateCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			DateTime now = DateTime.Now;
			NewOutputLine (String.Format ("{0:00}/{1:00}/{2:0000}", now.Day, now.Month, now.Year));
		}

		private void DeleteFileCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;

			if (espalda_line_parser.COMPONENTS.Count == 1)
			{
				NewOutputLine ("Spécifier ce qu'il faut supprimer.");
				return;
			}

			String dir = "";
			for (int i = 1; i < espalda_line_parser.COMPONENTS.Count; i++)
			{
				if (i > 1)
					dir += espalda_line_parser.SEPARATORS[i - 1];
				dir += (String)espalda_line_parser.COMPONENTS[i];
			}
			if (dir.Substring (dir.Length - 1, 1) == "/")
				dir = dir.Substring (0, dir.Length - 1);

			String[] files = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), dir);

			if (files.Length == 0)
			{
				NewOutputLine ("Rien à supprimer.");
				return;
			}

			if (files.Length == 1)
				NewOutputLine (String.Format ("Supprimer un fichier? 'y' pour continuer, autre touche pour annuler."));
			else
				NewOutputLine (String.Format ("Supprimer {0} fichiers? 'y' pour continuer, autre touche pour annuler.", files.Length));
			
			this.yes_no_question_object = new YesNoQuestion ("delfile", files);
		}

		private void DirCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;

			if (espalda_line_parser.COMPONENTS.Count == 2)
			{
				if ((String)espalda_line_parser.COMPONENTS[1] == "-h")
				{
					// display help
					NewOutputLine ("Syntaxe : dir -h -l");
					NewOutputLine ("-h : aide");
					NewOutputLine ("-l : liste");
					return;
				}
			}

			ArrayList switches = new ArrayList ();
			String search_criteria = "";
			bool stop_adding_to_search_criteria = false;
			if (espalda_line_parser.COMPONENTS.Count > 1)
			{
				for (int index = 1; index < espalda_line_parser.COMPONENTS.Count; index++)
				{	
					if ((String)espalda_line_parser.SEPARATORS[index - 1] == " " && ((String)espalda_line_parser.COMPONENTS[index]).Length == 2 && ((String)espalda_line_parser.COMPONENTS[index]).Substring (0, 1) == "-")
					{
						switches.Add ((String)espalda_line_parser.COMPONENTS[index]);
						if (search_criteria != "")
							stop_adding_to_search_criteria = true;
					}
					else if (!stop_adding_to_search_criteria)
					{
						if (search_criteria != "")
							search_criteria += espalda_line_parser.SEPARATORS[index - 1];
						search_criteria += (String)espalda_line_parser.COMPONENTS[index];
					}
				}
				if (search_criteria.Length > 0 && search_criteria.Substring (search_criteria.Length - 1, 1) == "/")
					search_criteria = search_criteria.Substring (0, search_criteria.Length - 1);
				search_criteria += "*";
			}
			else
				search_criteria = "*";

			String[] dirs = Directory.GetDirectories (line_start.Substring (0, line_start.Length - 1), search_criteria);

			// Sort
			Array.Sort (dirs);

			bool list = false;
			for (int sw_index = 0; sw_index < switches.Count; sw_index++)
			{
				if ((String)switches[sw_index] == "-l")
				{
					list = true;
				}
			}

			int in_row = 2;
			int dirs_number = 0;
			String dir = "";
			for (int d = 0; d < dirs.Length; d++)
			{
				dir = dirs[d];
				dir = dir.Replace ("\\", "/");
				if ((dir.Substring (dir.LastIndexOf ("/") + 1).ToLower () != "recycled") &&
					(dir.Substring (dir.LastIndexOf ("/") + 1).ToLower () != "recycler") &&
					(dir.Substring (dir.LastIndexOf ("/") + 1).ToLower () != "system volume information")
					)
				{
					dirs_number++;
					if (!list)
					{
						if (in_row < 2)
						{
							espalda_lines[line_number - 1].TEXT += "\t" + "[" + dir.Substring (dir.LastIndexOf ("/") + 1) + "]";
							in_row++;
						}
						else
						{
							NewOutputLine ("[" + dir.Substring (dir.LastIndexOf ("/") + 1) + "]");
							in_row = 0;
						}
					}
					else
					{
						// One by line
						NewOutputLine ("[" + dir.Substring (dir.LastIndexOf ("/") + 1) + "]");
					}
				}
			}

			String s = "";
			if (dirs_number != 1)
				s = String.Format ("{0} répertoire(s)", dirs_number);
			else
				s = "1 répertoire";
			NewOutputLine (s);
		}

		private void DtCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			DateTime now = DateTime.Now;
			NewOutputLine (String.Format ("{0:00}/{1:00}/{2:0000} {3:00}h{4:00}m{5:00}s", now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second));
		}

		private void ExecutableCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;

			// Vérifier si extension .txt et fichier existe
			String file_name = "", extension = "";
			for (int i = 0; i < espalda_line_parser.COMPONENTS.Count; i++)
			{
				if (i > 0)
					file_name += espalda_line_parser.SEPARATORS[i - 1];
				file_name += (String)espalda_line_parser.COMPONENTS[i];
			}
			if (file_name.IndexOf (".") != -1)
				extension = file_name.Substring (file_name.LastIndexOf (".") + 1);
			extension = extension.ToLower ();
			if (extension != "exe")
			{
				String openwith = "";
				if ((openwith = GetOpenWith (extension)) != "")
				{
					String[] temp = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), file_name);
					if (temp.Length == 1)
					{
						OpenFile (line_start.Substring (0, line_start.Length - 1), openwith, file_name);
						return;
					}
				}
			}

			// Vérfier si executable à lancer
			bool executable = false;
			String environment_path = Environment.GetEnvironmentVariable ("Path");
			String executable_name = ((String)espalda_line_parser.COMPONENTS[0]).ToLower ();
			String executable_directory = "";
			String[] files;
			String fl = "";

			// Vérifier tous les paramètres, si un répertoire ou fichier aller à ce fichier
			if (espalda_line_parser.COMPONENTS.Count > 1)
			{
				String dir = "";
				for (int i = 0; i < espalda_line_parser.COMPONENTS.Count; i++)
				{
					if (i > 0)
						dir += espalda_line_parser.SEPARATORS[i - 1];
					dir += (String)espalda_line_parser.COMPONENTS[i];
					if (((String)espalda_line_parser.COMPONENTS[i]).IndexOf (".") != -1)
						break;
				}
				if (dir.Substring (dir.Length - 1, 1) == "/")
					dir = dir.Substring (0, dir.Length - 1);
				String[] dirs = new String[0];
				if (dir.IndexOf (":/") == -1)
					dir = GoToDir (line_start.Substring (0, line_start.Length - 1), dir);
				else
					dir = GoToDir (dir.Substring (0, dir.IndexOf (":/") + 2), dir.Substring (dir.IndexOf (":/") + 2));
				
				if (dir != "")
				{
					dir = dir.Substring (0, 1).ToUpper () + dir.Substring (1);
					dir = dir.Replace ("\\", "/");
					line_start = dir + ">";
					return;
				}
				else
				{
					files = new String[0];
					extension = "";
					if (dir.IndexOf (".") != -1)
					{
						extension = dir.Substring (dir.LastIndexOf (".") + 1);
					}
					if ((extension.ToLower () == "exe") || (extension.ToLower () == "bat"))
					{
						if (dir.IndexOf (":/") == -1)
							files = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), dir);
						else
						{
							files = Directory.GetFiles (dir.Substring (0, dir.IndexOf (":/") + 2), dir.Substring (dir.IndexOf (":/") + 2));
						}
						if (files.Length == 1)
						{
							// Execute
							fl = files[0].Replace ("\\", "/");
							executable = true;
							executable_name = fl.Substring (fl.LastIndexOf ("/") + 1);
							executable_directory = fl.Substring (0, fl.LastIndexOf ("/"));
						}
					}
					else
					{
						// Find with .exe or .bat
						dir += ".exe";
						if (dir.IndexOf (":\\") == -1)
							files = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), dir);
						else
						{
							files = Directory.GetFiles (dir.Substring (0, dir.IndexOf (":/") + 2), dir.Substring (dir.IndexOf (":/") + 2));
						}
						if (files.Length == 1)
						{
							// Execute
							fl = files[0].Replace ("\\", "/");
							executable = true;
							executable_name = fl.Substring (fl.LastIndexOf ("/") + 1);
							executable_directory = fl.Substring (0, fl.LastIndexOf ("/"));
						}
						else
						{
							dir = dir.Substring (dir.LastIndexOf ("."));
							dir += ".bat";
							if (dir.IndexOf (":/") == -1)
								files = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), dir);
							else
							{
								files = Directory.GetFiles (dir.Substring (0, dir.IndexOf (":/") + 2), dir.Substring (dir.IndexOf (":/") + 2));
							}
							if (files.Length == 1)
							{
								// Execute
								fl = files[0].Replace ("\\", "/");
								executable = true;
								executable_name = fl.Substring (fl.LastIndexOf ("/") + 1);
								executable_directory = fl.Substring (0, fl.LastIndexOf ("/"));
							}
						}
					}
					
				}
				
			}

			if (!executable)
			{
				if (executable_name.IndexOf ("/") != -1)
				{
					executable_directory = executable_name.Substring (0, executable_name.LastIndexOf ("/"));
					executable_name = executable_name.Substring (executable_name.LastIndexOf ("/") + 1);
					if (executable_name.IndexOf (".") == -1)
					{
						files = Directory.GetFiles (executable_directory, executable_name + executable_name + "*.exe");
						if (files.Length >= 1)
							executable_name += ".exe";
						else
						{
							files = Directory.GetFiles (executable_directory, executable_name + executable_name + "*.bat");
							if (files.Length >= 1)
								executable_name += ".bat";
						}
					}
					executable = true;
				}
				else if (executable_name.LastIndexOf (".") != -1)
					executable_name = executable_name.Substring (0, executable_name.LastIndexOf ("."));
			}
       
			if (!executable)
			{
				// Search exe in current directory
				files = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), executable_name + "*.exe");
				foreach (String file in files)
				{
					fl = file.Replace ("\\", "/");
					fl = fl.Substring (fl.LastIndexOf ("/") + 1).ToLower ();
					if (fl.LastIndexOf (".") != -1)
						fl = fl.Substring (0, fl.LastIndexOf ("."));
					if (executable_name.ToLower () == fl)
					{
						executable = true;
						executable_name += ".exe";
						executable_directory = line_start.Substring (0, line_start.Length - 1);
						break;
					}
				}
			}

			if (!executable)
			{
				// Search bat in current directory
				files = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), executable_name + "*.bat");
				foreach (String file in files)
				{
					fl = file.Replace ("\\", "/");
					fl = fl.Substring (fl.LastIndexOf ("/") + 1).ToLower ();
					if (fl.LastIndexOf (".") != -1)
						fl = fl.Substring (0, fl.LastIndexOf ("."));
					if (executable_name.ToLower () == fl)
					{
						executable = true;
						executable_name += ".bat";
						executable_directory = line_start.Substring (0, line_start.Length - 1);
						break;
					}
				}
			}

			if (!executable)
			{
				// Search in path
				int index = 0;
				String dir = "";
				while (!executable && (index != -1))
				{
					if (index > 0)
					{
						if (environment_path.IndexOf (";", index + 1) != -1)
							dir = environment_path.Substring (index + 1, environment_path.IndexOf (";", index + 1) - index - 1);
						else
							dir = environment_path.Substring (index + 1);
					}
					else
						dir = environment_path.Substring (index, environment_path.IndexOf (";"));
					if (dir == "")
					{
						index = environment_path.IndexOf (";", index + 1);
						continue;
					}
					files = Directory.GetFiles (dir, executable_name + "*.exe");
					foreach (String file in files)
					{
						fl = file.Replace ("\\", "/");
						fl = fl.Substring (fl.LastIndexOf ("/") + 1).ToLower ();
						if (fl.LastIndexOf (".") != -1)
							fl = fl.Substring (0, fl.LastIndexOf ("."));
						if (executable_name.ToLower () == fl)
						{
							executable = true;
							executable_name += ".exe";
							executable_directory = line_start.Substring (0, line_start.Length - 1);
							break;
						}
					}
					if (!executable)
					{
						// Search bat in current directory
						files = Directory.GetFiles (dir, executable_name + "*.bat");
						foreach (String file in files)
						{
							fl = file.Replace ("\\", "/");
							fl = fl.Substring (fl.LastIndexOf ("/") + 1).ToLower ();
							if (fl.LastIndexOf (".") != -1)
								fl = fl.Substring (0, fl.LastIndexOf ("."));
							if (executable_name.ToLower () == fl)
							{
								executable = true;
								executable_name += ".bat";
								executable_directory = line_start.Substring (0, line_start.Length - 1);
								break;
							}
						}
					}
					index = environment_path.IndexOf (";", index + 1);
				}
			}

			if (executable)
			{
				extension = "";
				if (executable_name.LastIndexOf (".") != -1)
					extension = executable_name.Substring (executable_name.LastIndexOf (".") + 1).ToLower ();
				if ((extension == "exe") || (extension == "bat"))
				{
					Process p = null;
					try
					{
						p = new Process();
						p.StartInfo.WorkingDirectory = executable_directory;
						p.StartInfo.FileName = executable_name;

						if (espalda_line_parser.COMPONENTS.Count > 1)
						{
							String temp = "";
							String exec_temp = executable_name.Substring (0, executable_name.LastIndexOf (".")).ToLower ();
							int i = 0;
							for (; i < espalda_line_parser.COMPONENTS.Count; i++)
							{
								if (temp != "")
									temp += espalda_line_parser.SEPARATORS[i - 1];
								temp += (String)espalda_line_parser.COMPONENTS[i];
								if (temp.IndexOf (".") != -1)
									temp = temp.Substring (0, temp.LastIndexOf ("."));
								// w/o extension
								if (temp.Substring (temp.LastIndexOf ("/") + 1).ToLower () == exec_temp.ToLower ())
								{
									i++;
									break;
								}
							}
							for (; i < espalda_line_parser.COMPONENTS.Count; i++)
							{
								p.StartInfo.Arguments += (String)espalda_line_parser.COMPONENTS[i];
							}
						}
													
						p.StartInfo.CreateNoWindow = true;
								
						//p.StartInfo.UseShellExecute = false;
						//p.StartInfo.RedirectStandardOutput = true;

						p.Start ();

						//Console.WriteLine (p.StandardOutput.ReadToEnd());
						//p.WaitForExit ();
					}
					catch (Exception ex)
					{
						Console.WriteLine("Exception Occurred :{0},{1}", 
							ex.Message,ex.StackTrace.ToString());
					}
				}
				else
				{
					NewOutputLine ("Pas commande ni .exe ni .bat");
				}
			}
			else
			{
				NewOutputLine ("Pas commande ni .exe ni .bat");
			}
		}

		private void FileCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;

			if (espalda_line_parser.COMPONENTS.Count == 2)
			{
				if ((String)espalda_line_parser.COMPONENTS[1] == "-h")
				{
					// display help
					NewOutputLine ("Syntaxe : file -h -l -p");
					NewOutputLine ("-h : aide");
					NewOutputLine ("-l : liste");
					NewOutputLine ("-p : détails");
					return;
				}
			}

			bool list = false;
			bool properties = false;

			ArrayList switches = new ArrayList ();
			String search_criteria = "";
			bool stop_adding_to_search_criteria = false;
			if (espalda_line_parser.COMPONENTS.Count > 1)
			{
				for (int index = 1; index < espalda_line_parser.COMPONENTS.Count; index++)
				{	
					if ((String)espalda_line_parser.SEPARATORS[index - 1] == " " && ((String)espalda_line_parser.COMPONENTS[index]).Length == 2 && ((String)espalda_line_parser.COMPONENTS[index]).Substring (0, 1) == "-")
					{
						switches.Add ((String)espalda_line_parser.COMPONENTS[index]);
						if (search_criteria != "")
							stop_adding_to_search_criteria = true;
					}
					else if (!stop_adding_to_search_criteria)
					{
						if (search_criteria != "")
							search_criteria += espalda_line_parser.SEPARATORS[index - 1];
						search_criteria += (String)espalda_line_parser.COMPONENTS[index];
					}
				}
				if (search_criteria.Length > 0 && search_criteria.Substring (search_criteria.Length - 1, 1) == "/")
					search_criteria = search_criteria.Substring (0, search_criteria.Length - 1);
				search_criteria += "*";
			}
			else
				search_criteria = "*";
					                    
			String[] dirs = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), search_criteria);

			// Sort
			Array.Sort (dirs);

			for (int sw_index = 0; sw_index < switches.Count; sw_index++)
			{
				if ((String)switches[sw_index] == "-l")
				{
					list = true;
				}
				if ((String)switches[sw_index] == "-p")
				{
					properties = true;
				}
			}

			int in_row = 2;
			int files = 0;
			long files_size = 0;
			String s = "", text = "";
			String dir = "";
			for (int d = 0; d < dirs.Length; d++)
			{
				dir = dirs[d];
				dir = dir.Replace ("\\", "/");
				files++;
				if (!list)
				{
					if (in_row < 2)
					{
						espalda_lines[line_number - 1].TEXT += "     " + dir.Substring (dir.LastIndexOf ("/") + 1);
						in_row++;
						if (properties)
						{
							FileInfo fi = new FileInfo(dir);
							files_size += fi.Length;
						}
					}
					else
					{
						NewOutputLine (dir.Substring (dir.LastIndexOf ("/") + 1));
						if (properties)
						{
							FileInfo fi = new FileInfo(dir);
							files_size += fi.Length;
						}
						in_row = 0;
					}
				} // fin de if (!list)
				else
				{
					if (properties)
					{
						FileInfo fi = new FileInfo(dir);
						DateTime creation_time = fi.LastWriteTime;
						s = String.Format ("{0:00}/{1:00}/{2:0000} {3:00}h{4:00}",
							creation_time.Day, creation_time.Month, creation_time.Year,
							creation_time.Hour, creation_time.Minute);
						text = s;
						text = text.PadRight (22, '_');
						s = String.Format ("{0}", fi.Length);
						files_size += fi.Length;
						s = ProcessNumberIntoSpaces (s);
						text = text.PadRight (32 - s.Length, '_');
						text += s;
						text = text.PadRight (40, '_');
						text += dir.Substring (dir.LastIndexOf ("/") + 1);
					}
					else
						text = dir.Substring (dir.LastIndexOf ("/") + 1);
						
					NewOutputLine (text);
				}	// fin de else de if (!list)

			} // fin de foreach (String dir in dirs)
					
			s = String.Format ("{0} fichier(s)",
				files);
			text = s;
			if (properties)
			{
				s = String.Format ("{0}",
					files_size);
				s = ProcessNumberIntoSpaces (s);
				s += " octets";
				text += "     " + s;
			}
			NewOutputLine (text);
		}

		private void GLAtomicCommand ()
		{
			Process p = null;
			try
			{
				p = new Process();
				p.StartInfo.WorkingDirectory = "D:/slf/dotnet/gatomic/bin/release";
				p.StartInfo.FileName = "gatomicplay.exe";

				p.StartInfo.CreateNoWindow = false;
								
				p.Start ();

				MinCommand ();

			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception Occurred :{0},{1}", 
					ex.Message,ex.StackTrace.ToString());
			}
		}

		private void GLDoorsCommand ()
		{
			Process p = null;
			try
			{
				p = new Process();
				p.StartInfo.WorkingDirectory = "D:/slf/dotnet/gldoors2/bin/release";
				p.StartInfo.FileName = "gldoors2.exe";

				p.StartInfo.CreateNoWindow = false;
								
				//p.StartInfo.UseShellExecute = false;
				//p.StartInfo.RedirectStandardOutput = true;

				p.Start ();

				//Console.WriteLine (p.StandardOutput.ReadToEnd());
				//p.WaitForExit ();

				Application.Exit ();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception Occurred :{0},{1}", 
					ex.Message,ex.StackTrace.ToString());
			}
		}

		private void HaltCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;

			NewOutputLine ("Arreter le PC? 'y' pour continuer, autre touche pour annuler");

			yes_no_question_object = new YesNoQuestion ("Halt", null);
		}

		private void HelpCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			int in_row = 0;
			String str = "";
			for (int index = 0; index < commands.Length; index++)
			{
				if (in_row > 0 && in_row < 3)
				{
					str = str.PadRight (20 * in_row, '_');
					str += commands[index];
					in_row++;
				}
				else
				{
					if (str != "")
						NewOutputLine (str);
					str = commands[index];
					in_row = 1;
				}

			}
			if (str != "")
				NewOutputLine (str);
		}

		private void HrmCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;

			DateTime today = DateTime.Now;

			TimeSpan one_day = new TimeSpan (1, 0, 0, 0);

			int hours = 0, total_hours = 0;

			today += one_day;
			while (today.Day < 30)
			{
				if (today.Day == 25)
				{}
				else if (today.DayOfWeek == DayOfWeek.Friday)
					hours += 7;
				else if ((today.DayOfWeek >= DayOfWeek.Monday) && (today.DayOfWeek <= DayOfWeek.Thursday))
					hours += 8;

				today += one_day;
			}

			today = DateTime.Now;

			total_hours = hours;
			if (today.Day == 25)
			{}
			else if (today.DayOfWeek == DayOfWeek.Friday)
				total_hours += 7;
			else if ((today.DayOfWeek >= DayOfWeek.Monday) && (today.DayOfWeek <= DayOfWeek.Thursday))
				total_hours += 8;

			int today_hours = 18;
			if (today.DayOfWeek == DayOfWeek.Friday)
				today_hours = 17;
			today_hours = today_hours - today.Hour;
			if (today.Hour < 13)
				today_hours -= 1;

			hours += today_hours;

			String output = "";
			float x = 1 - ((float) hours / (float) total_hours);
			x *= 100;
			output = String.Format ("{0}-{1:0.00}", hours, x);
			NewOutputLine (output);
		}

		private void LsCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			
			if (espalda_line_parser.COMPONENTS.Count == 2)
			{
				if ((String)espalda_line_parser.COMPONENTS[1] == "-h")
				{
					// display help
					NewOutputLine ("Syntaxe : ls -h -l -p");
					NewOutputLine ("-h : aide");
					NewOutputLine ("-l : liste");
					NewOutputLine ("-p : détails");
					return;
				}
			}

			bool list = false;
			bool properties = false;
					
			ArrayList switches = new ArrayList ();
			String search_criteria = "";
			bool stop_adding_to_search_criteria = false;
			if (espalda_line_parser.COMPONENTS.Count > 1)
			{
				for (int index = 1; index < espalda_line_parser.COMPONENTS.Count; index++)
				{	
					if ((String)espalda_line_parser.SEPARATORS[index - 1] == " " && ((String)espalda_line_parser.COMPONENTS[index]).Length == 2 && ((String)espalda_line_parser.COMPONENTS[index]).Substring (0, 1) == "-")
					{
						switches.Add ((String)espalda_line_parser.COMPONENTS[index]);
						if (search_criteria != "")
							stop_adding_to_search_criteria = true;
					}
					else if (!stop_adding_to_search_criteria)
					{
						if (search_criteria != "")
							search_criteria += espalda_line_parser.SEPARATORS[index - 1];
						search_criteria += (String)espalda_line_parser.COMPONENTS[index];
					}
				}
				if (search_criteria.Length > 0 && search_criteria.Substring (search_criteria.Length - 1, 1) == "/")
					search_criteria = search_criteria.Substring (0, search_criteria.Length - 1);
				search_criteria += "*";
			}
			else
				search_criteria = "*";

			for (int sw_index = 0; sw_index < switches.Count; sw_index++)
			{
				if ((String)switches[sw_index] == "-l")
				{
					list = true;
				}
				if ((String)switches[sw_index] == "-p")
				{
					properties = true;
				}
			}

			String[] dirs = Directory.GetDirectories (line_start.Substring (0, line_start.Length - 1), search_criteria);

			// Sort
			Array.Sort (dirs);

			int in_row = 2;
			int dirs_number = 0;
			String dir = "";
			for (int d = 0; d < dirs.Length; d++)
			{
				dir = dirs[d];dir = dir.Replace ("\\", "/");
				if ((dir.Substring (dir.LastIndexOf ("/") + 1).ToLower () != "recycled") &&
					(dir.Substring (dir.LastIndexOf ("/") + 1).ToLower () != "recycler") &&
					(dir.Substring (dir.LastIndexOf ("/") + 1).ToLower () != "system volume information")
					)
				{
					dirs_number++;
					if (!list)
					{
						if (in_row < 2)
						{
							espalda_lines[line_number - 1].TEXT += "\t" + "[" + dir.Substring (dir.LastIndexOf ("/") + 1) + "]";
							in_row++;
						}
						else
						{
							NewOutputLine ("[" + dir.Substring (dir.LastIndexOf ("/") + 1) + "]");
							in_row = 0;
						}
					}
					else
					{
						// One by line
						NewOutputLine ("[" + dir.Substring (dir.LastIndexOf ("/") + 1) + "]");
					}

				}
			}

			dirs = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), search_criteria);

			// Sort
			Array.Sort (dirs);

			int files = 0;
			long files_size = 0;
			in_row = 2;
			String s = "", text = "";
			dir = "";
			for (int d = 0; d < dirs.Length; d++)
			{
				dir = dirs[d];dir = dir.Replace ("\\", "/");
				files++;
				if (!list)
				{
					if (in_row < 2)
					{
						espalda_lines[line_number - 1].TEXT += "     " + dir.Substring (dir.LastIndexOf ("/") + 1);
						in_row++;
					}
					else
					{
						NewOutputLine (dir.Substring (dir.LastIndexOf ("/") + 1));
						in_row = 0;
					}
				}
				else
				{
					if (properties)
					{
						FileInfo fi = new FileInfo(dir);
						DateTime creation_time = fi.LastWriteTime;
						s = String.Format ("{0:00}/{1:00}/{2:0000} {3:00}h{4:00}",
							creation_time.Day, creation_time.Month, creation_time.Year,
							creation_time.Hour, creation_time.Minute);
						text = s;
						text = text.PadRight (22, '_');
						s = String.Format ("{0}", fi.Length);
						files_size += fi.Length;
						s = ProcessNumberIntoSpaces (s);
						text = text.PadRight (32 - s.Length, '_');
						text += s;
						text = text.PadRight (40, '_');
						text += dir.Substring (dir.LastIndexOf ("/") + 1);
					}
					else
						text = dir.Substring (dir.LastIndexOf ("/") + 1);
					NewOutputLine (text);
				}
			}
					
			if (dirs_number != 1)
				s = String.Format ("{0} répertoire(s)", dirs_number);
			else
				s = "1 répertoire";
			NewOutputLine (s);
					
			s = String.Format ("{0} fichier(s)",
				files);
			text = s;
			if (properties)
			{
				s = String.Format ("{0}",
					files_size);
				s = ProcessNumberIntoSpaces (s);
				s += " octets";
				text += "     " + s;
			}
			NewOutputLine (text);
		}

		private void MinCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;

			// Minimize
			this.WindowState = FormWindowState.Minimized;
			// and clear
			visible_start_line_number = line_number + 1;
		}

		private void NewDriveLetter ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			// nouveau disque
			if (((String)espalda_line_parser.COMPONENTS[0]).ToLower () != line_start.Substring (0, 2))
			{
				// pas le meme
				String drive_letter = (((String)espalda_line_parser.COMPONENTS[0]).ToLower ()).Substring (0, 1);
				drive_letter = drive_letter.ToUpper () + ":\\";
				// lettre valide?
				String[] drives = Directory.GetLogicalDrives ();
				bool found = false;
				foreach (String drive in drives)
				{
					if (drive.ToUpper () == drive_letter)
					{
						drive_letter = drive_letter.Replace ("\\", "/");
						line_start = drive_letter + ">";
						found = true;
						break;
					}
				}
				if (!found)
				{
					NewOutputLine ("Lecteur introuvable");
				}
			}
		}

		private void PwdCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			NewOutputLine (line_start.Substring (0, line_start.Length - 1));
		}

		private void RebootCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;

			NewOutputLine ("Redémarrer le PC? 'y' pour continuer, autre touche pour annuler");

			yes_no_question_object = new YesNoQuestion ("Reboot", null);
		}
		
		private void SysinfoCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			String str = "";
			str = Environment.MachineName;
			OperatingSystem sys = Environment.OSVersion;
			str += "            " + sys.ToString ();
			NewOutputLine (str);
		}

		private void TimeCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			DateTime now = DateTime.Now;
			NewOutputLine (String.Format ("{0:00}h{1:00}m{2:00}s", now.Hour, now.Minute, now.Second));
		}

		private void TitleCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			if (espalda_line_parser.COMPONENTS.Count == 1)
				this.Text = "GL-ESPALDA";
			else
			{
				String t = "";
				for (int index = 1; index < espalda_line_parser.COMPONENTS.Count; index++)
				{
					if (index > 1)
						t += espalda_line_parser.SEPARATORS[index - 1];
					t += espalda_line_parser.COMPONENTS[index];
				}
				this.Text = t;
			}
		}

		private void TremCommand ()
		{
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.COMMAND_LINE;
			String to_time = "1800";
			bool remains = false;
			bool time_set = false;
			if (espalda_line_parser.COMPONENTS.Count > 1)
			{
				for (int index = 1; index < espalda_line_parser.COMPONENTS.Count; index++)
				{
					if (((String)espalda_line_parser.COMPONENTS[index]).ToLower () == "-r")
					{
						remains = true;
					}
					else
					{
						String time = (String)espalda_line_parser.COMPONENTS[index];
						if (time.Length == 2)
						{
							try
							{
								Convert.ToInt32 (time);
								time += "00";
								to_time = time;
								time_set = true;
							}
							catch
							{}
						}
						else if (time.Length == 4)
						{
							try
							{
								Convert.ToInt32 (time);
								to_time = time;
								time_set = true;
							}
							catch
							{}
						}
					} // fin de else de if (((String)espalda_line_parser.COMPONENTS[index]).ToLower () == "/r")

					if (remains && time_set)
						break;

				} // Fin de for
						
			} // Fin de if (espalda_line_parser.COMPONENTS.Count > 1)
					
			DateTime start = DateTime.Now;
			DateTime end = new DateTime (start.Year, start.Month, start.Day, Convert.ToInt32 (to_time.Substring (0, 2)), Convert.ToInt32 (to_time.Substring (2)), 0);
			TimeSpan diff = end - start;

			if (remains)
			{
				if (diff.Ticks < 0)
					NewOutputLine ("++++++++++++++++++++++++++++++++++++++++++++++++++");
				else
					NewOutputLine ("----------------------------------------------------------------");
			}
			else
			{
				if (diff.Ticks < 0)
					NewOutputLine (String.Format ("-{0:00}h{1:00}m{2:00}", diff.Hours, Math.Abs (diff.Minutes), Math.Abs (diff.Seconds)));
				else
					NewOutputLine (String.Format ("{0:00}h{1:00}m{2:00}", diff.Hours, diff.Minutes, diff.Seconds));
			}
		}

		private bool isCommand (String arg)
		{
			for (int i = 0;i < commands.Length; i++)
			{
				if (arg.ToLower () == commands[i].ToLower ())
				{
					return true;
				}
			}
			return false;
		}
		#endregion

		#region TAB_PRESSED
		private String TabPressed ()
		{
			String string_to_draw = "";
			
			espalda_line_parser.ParseLine (espalda_lines[line_number - 1].TEXT.Substring (line_start.Length));
			if (espalda_line_parser.COMPONENTS.Count > 0)
			{
				// Guess
				String search_criteria = "";
				if (espalda_line_parser.COMPONENTS.Count == 1)
				{
					search_criteria = (String)espalda_line_parser.COMPONENTS[0];
					// Commande
					if (search_criteria == "_")
					{}
					else if (search_criteria.Length >= 3 && search_criteria.Substring (1, 1) == ":")
					{
						String drive_letter = search_criteria.ToLower ().Substring (0, 1);
						drive_letter = drive_letter.ToUpper () + ":\\";
						// lettre valide?
						String[] drives = Directory.GetLogicalDrives ();
						bool found = false;
						foreach (String drive in drives)
						{
							if (drive.ToUpper () == drive_letter)
							{
								found = true;
								break;
							}
						}
						if (found)
						{
							String dir = "";
							if (search_criteria.IndexOf ("/") != -1)
								dir = search_criteria.Substring (0, search_criteria.IndexOf ("/")) + "/";
							else
								dir = search_criteria.Substring (0, 2) + "/";
							search_criteria = search_criteria.Substring (0, search_criteria.Length - 1);
							if (search_criteria.IndexOf ("/") != -1)
								search_criteria = search_criteria.Substring (search_criteria.IndexOf ("/") + 1);
							search_criteria += "*";
							String[] dirs = Directory.GetDirectories (dir, search_criteria);
							String[] files = Directory.GetFiles (dir, search_criteria);
							if (dirs.Length == 1 && files.Length == 0)
								string_to_draw = dirs[0].Substring (dirs[0].IndexOf ("/") + 1).Substring (search_criteria.Length - 1) + "/";
							else if (dirs.Length == 0 && files.Length == 1)
								string_to_draw = files[0].Substring (files[0].IndexOf ("/") + 1).Substring (search_criteria.Length - 1) + " ";
							else
							{
								// Trouver les charactères communs
								String common_string = CommonStringBetweenTwoArrays (files, dirs);
								if (common_string.Length > search_criteria.Length - 1)
									string_to_draw = common_string.Substring (search_criteria.Length - 1);
							}
						}
					}
					else
					{
						search_criteria = search_criteria.Substring (0, search_criteria.Length - 1);
						search_criteria = search_criteria.TrimStart ();
						search_criteria += "*";
						// chercher dans la liste des commandes
						String[] cmds = GetCommands (search_criteria.Substring (0, search_criteria.Length - 1));
						// chercher d'abord dans le répertoire courant
						String[] files = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), search_criteria);
						String[] dirs = Directory.GetDirectories (line_start.Substring (0, line_start.Length - 1), search_criteria);
						if ((files.Length > 0) || (cmds.Length > 0) || (dirs.Length > 0))
						{
							if (files.Length == 1 && cmds.Length == 0 && dirs.Length == 0)
							{
								if (line_start.Length == 4)
									string_to_draw = files[0].Substring (files[0].IndexOf ("/") + 1).Substring (line_start.Length - 4 + search_criteria.Length - 1) + " ";
								else
									string_to_draw = files[0].Substring (files[0].IndexOf ("/") + 1).Substring (line_start.Length - 4 + search_criteria.Length) + " ";
							}
							else if (files.Length == 0 && cmds.Length == 1 && dirs.Length == 0)
								string_to_draw = cmds[0].Substring (search_criteria.Length - 1) + " ";
							else if (files.Length == 0 && cmds.Length == 0 && dirs.Length == 1)
								string_to_draw = dirs[0].Substring (dirs[0].IndexOf ("/") + 1).Substring (search_criteria.Length - 1) + "/";
							else if (files.Length == 0 && (cmds.Length >= 1 || dirs.Length >= 1))
							{
								// Trouver les charactères communs
								int index;
								int length = line_start.Length;
								if (length == 4)
									length--;
								for (index = 0; index < files.Length; index++)
								{
									files[index] = files[index].Substring (length);
								}
								String common_string = CommonStringBetweenTwoArrays (cmds, files);
								String common_string2 = CommonStringBetweenTwoArrays (cmds, dirs);
								common_string = CommonString (common_string, common_string2);
								if (common_string.Length > search_criteria.Length - 1)
									string_to_draw = common_string.Substring (search_criteria.Length - 1);
								
							}
							else if (files.Length > 0 && cmds.Length == 0 && dirs.Length == 0)
							{
								// Trouver les charactères communs
								int index;
								int length = line_start.Length;
								if (length == 4)
									length--;
								for (index = 0; index < files.Length; index++)
								{
									files[index] = files[index].Substring (length);
								}
								String common_string = CommonStringBetweenTwoArrays (cmds, files);
								if (common_string.Length > search_criteria.Length - 1)
									string_to_draw = common_string.Substring (search_criteria.Length - 1);
								else
								{
									string_to_draw = espalda_lines[line_number - 1].TEXT;
									string_to_draw = string_to_draw.Substring (line_start.Length);
									string_to_draw = string_to_draw.Substring (0, string_to_draw.Length - 1);
									espalda_lines[line_number - 1].TEXT = line_start + "file " + search_criteria + ".*_";
									frmEspaldaMain_KeyDown (null, new KeyEventArgs (Keys.Enter));
								}
							}
							else if (files.Length > 0 && cmds.Length == 0 && dirs.Length > 0)
							{
								// Trouver les charactères communs
								int index;
								int length = line_start.Length;
								if (length == 4)
									length--;
								for (index = 0; index < files.Length; index++)
								{
									files[index] = files[index].Substring (length);
								}
								for (index = 0; index < dirs.Length; index++)
								{
									dirs[index] = dirs[index].Substring (length);
								}
								String common_string = CommonStringBetweenTwoArrays (dirs, files);
								if (common_string.Length > search_criteria.Length - 1)
									string_to_draw = common_string.Substring (search_criteria.Length - 1);
								else
								{
									string_to_draw = espalda_lines[line_number - 1].TEXT;
									string_to_draw = string_to_draw.Substring (line_start.Length);
									string_to_draw = string_to_draw.Substring (0, string_to_draw.Length - 1);
									espalda_lines[line_number - 1].TEXT = line_start + "ls " + search_criteria + ".*_";
									frmEspaldaMain_KeyDown (null, new KeyEventArgs (Keys.Enter));
								}
							}
						}	// fin de if ((files.Length > 0) || (cmds.Length > 0) || (dirs.Length > 0))
						else
						{
							// chercher dans les autres répertoires du Path mais seulement les fichiers
							String environment_path = Environment.GetEnvironmentVariable ("Path");
								
							// Search in path
							int index = 0;
							String dir = "";
							int files_number = 0;
							String file = "";
							while (index != -1)
							{
								if (index > 0)
								{
									if (environment_path.IndexOf (";", index + 1) != -1)
										dir = environment_path.Substring (index + 1, environment_path.IndexOf (";", index + 1) - index - 1);
									else
										dir = environment_path.Substring (index + 1);
								}
								else
									dir = environment_path.Substring (index, environment_path.IndexOf (";"));
								files = Directory.GetFiles (dir, search_criteria);
								files_number += files.Length;
								if (files_number > 1)
									break;
								else if (files_number == 1)
								{
									file = files[0];
									file = files[0].Replace ("\\", "/");
									file = file.Substring (file.LastIndexOf ("/") + 1).Substring (search_criteria.Length - 1);
								}
								index = environment_path.IndexOf (";", index + 1);
							}
							if (files_number == 1)
								string_to_draw = file + " ";
						}	// end of else of if ((files.Length > 0) || (cmds.Length > 0))
					}	// end of else of if (search_criteria == "_")
				}	// end of if (espalda_line_parser.COMPONENTS.Count == 1)
				else
				{
					int start_index = 1;
					if (!isCommand ((String)espalda_line_parser.COMPONENTS[0]))
					{
						start_index = 0;
					}
					search_criteria = "";
					for (int k = start_index; k < espalda_line_parser.COMPONENTS.Count; k++)
					{
						if (search_criteria != "")
							search_criteria += espalda_line_parser.SEPARATORS[k - 1];
						search_criteria += (String)espalda_line_parser.COMPONENTS[k];
					}
					// Fichiers ou répertoires
					if ((search_criteria == "_") || (search_criteria == "/_"))
					{}
					else if (search_criteria.Length >= 3 && search_criteria.Substring (1, 1) == ":")
					{
						String drive_letter = search_criteria.ToLower ().Substring (0, 1);
						drive_letter = drive_letter.ToUpper () + ":\\";
						// lettre valide?
						String[] drives = Directory.GetLogicalDrives ();
						bool found = false;
						foreach (String drive in drives)
						{
							if (drive.ToUpper () == drive_letter)
							{
								found = true;
								break;
							}
						}
						if (found)
						{
							String dir = "";
							if (search_criteria.IndexOf ("/") != -1)
								dir = search_criteria.Substring (0, search_criteria.IndexOf ("/")) + "/";
							else
								dir = search_criteria.Substring (0, 2) + "/";
							search_criteria = search_criteria.Substring (0, search_criteria.Length - 1);
							if (search_criteria.IndexOf ("/") != -1)
								search_criteria = search_criteria.Substring (search_criteria.IndexOf ("/") + 1);
							search_criteria += "*";
							String[] dirs = Directory.GetDirectories (dir, search_criteria);
							String[] files = Directory.GetFiles (dir, search_criteria);
							if (dirs.Length == 1 && files.Length == 0)
								string_to_draw = dirs[0].Substring (dirs[0].IndexOf ("/") + 1).Substring (search_criteria.Length - 1) + "/";
							else if (dirs.Length == 0 && files.Length == 1)
								string_to_draw = files[0].Substring (files[0].IndexOf ("/") + 1).Substring (search_criteria.Length - 1) + " ";
							else if ((dirs.Length > 0) || (files.Length > 0))
							{
								// Trouver les charactères communs
								String common_string = CommonStringBetweenTwoArrays (files, dirs);
								if (common_string.Length > search_criteria.Length - 1)
									string_to_draw = common_string.Substring (search_criteria.Length - 1);
							}
						}
					}
					else
					{
						search_criteria = search_criteria.Substring (0, search_criteria.Length - 1);
						search_criteria = search_criteria.TrimStart ();
						search_criteria += "*";
						String[] dirs = Directory.GetDirectories (line_start.Substring (0, line_start.Length - 1), search_criteria);
						String[] files = Directory.GetFiles (line_start.Substring (0, line_start.Length - 1), search_criteria);
						if (dirs.Length == 1 && files.Length == 0)
						{
							if (line_start.Length == 4)
								string_to_draw = dirs[0].Substring (dirs[0].IndexOf ("/") + 1).Substring (line_start.Length - 4 + search_criteria.Length - 1) + "/";
							else
								string_to_draw = dirs[0].Substring (dirs[0].IndexOf ("/") + 1).Substring (line_start.Length - 4 + search_criteria.Length) + "/";
						}
						else if (dirs.Length == 0 && files.Length == 1)
							string_to_draw = files[0].Substring (files[0].IndexOf ("/") + 1).Substring (search_criteria.Length - 1) + " ";
						else if (dirs.Length == 0 && files.Length == 0)
						{
							// chercher dans les autres répertoires du Path mais seulement les fichiers
							String environment_path = Environment.GetEnvironmentVariable ("Path");
							// Search in path
							int index = 0;
							String dir = "";
							int files_number = 0;
							String file = "";
							while (index != -1)
							{
								if (index > 0)
								{
									if (environment_path.IndexOf (";", index + 1) != -1)
										dir = environment_path.Substring (index + 1, environment_path.IndexOf (";", index + 1) - index - 1);
									else
										dir = environment_path.Substring (index + 1);
								}
								else
									dir = environment_path.Substring (index, environment_path.IndexOf (";"));
								files = Directory.GetFiles (dir, search_criteria);
								files_number += files.Length;
								if (files_number > 1)
									break;
								else if (files_number == 1)
								{
									file = files[0];
									file = files[0].Replace ("\\", "/");
									file = file.Substring (file.LastIndexOf ("/") + 1).Substring (search_criteria.Length - 1);
								}
								index = environment_path.IndexOf (";", index + 1);
							}
							if (files_number == 1)
								string_to_draw = file + " ";
						}
						else	// else de if (dirs.Length == 1 && files.Length == 0)
						{
							// Trouver les charactères communs
							String common_string = CommonStringBetweenTwoArrays (files, dirs);
							if (line_start.Length != 4)
								common_string = common_string.Substring (line_start.Length - 4 + 1);
							if (common_string.Length > search_criteria.Length - 1)
								string_to_draw = common_string.Substring (search_criteria.Length - 1);
						}
					}
				}
			}

			return string_to_draw;
		}
		
		#endregion

		#region FORM_PAINT
		private void frmEspaldaMain_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			form_graphics = e.Graphics;
			form_graphics.DrawString ("Bienvenue a GL-ESPALDA!! version " + program_version, this.Font, text_brush, text_x, text_y - 20);

			int start_line_number = visible_start_line_number;
			float temp_text_y = text_y;
			for (int i = 0; i < max_visible_lines_number && start_line_number <= espalda_lines.Length; i++)
			{
				if (espalda_lines[start_line_number - 1].TEXT != "")
				{
					if (i > 0)
					{
						if (espalda_lines[start_line_number - 1].TYPE == EspaldaLine.EspaldaLineType.COMMAND_LINE)
							temp_text_y += height_between_command_lines;
						else if (espalda_lines[start_line_number - 1].TYPE == EspaldaLine.EspaldaLineType.OUTPUT_LINE)
							temp_text_y += height_between_output_lines;
						else
							temp_text_y += height_between_command_lines;
					}
					form_graphics.DrawString (espalda_lines[start_line_number - 1].TEXT, this.Font, text_brush, text_x, temp_text_y);
										
					start_line_number++;
				}
				else
					break;
			}
		}
		#endregion

		#region FORM_RESIZE
		private void frmEspaldaMain_Resize(object sender, System.EventArgs e)
		{
			max_visible_lines_number = ((int)this.ClientSize.Height - (int)text_y) / 30;
		}
		#endregion

		#region GoToDir
		private String GoToDir (String start_dir, String dir)
		{
			try
			{
				String temp_dir = dir.Replace ("\\", "/");

				if (start_dir.Substring (start_dir.Length - 1, 1) != "/")
					dir = start_dir += "/";
				else
					dir = start_dir;
			
				bool exists = true;
				String[] dirs = new String[0];
				String dir_to_find = "";
				int index = 0;

				index = temp_dir.IndexOf ("/");
				while (index != -1 && exists)
				{
					dir_to_find = temp_dir.Substring (0, index);
				
					dirs = Directory.GetDirectories (start_dir, dir_to_find);

					if (dirs.Length == 1)
					{
						dir += dirs[0].Substring (dirs[0].LastIndexOf ("/") + 1) + "/";
					}
					else
						exists = false;

					start_dir += dirs[0].Substring (dirs[0].LastIndexOf ("/") + 1) + "/";
					if (start_dir.Substring (start_dir.Length - 1, 1) != "/")
						start_dir += "/";

					temp_dir = temp_dir.Substring (index + 1);
					index = temp_dir.IndexOf ("/");
				}
			
				if (exists)
				{
					if (start_dir.Substring (start_dir.Length - 1, 1) != "/")
						start_dir += "/";
					dir_to_find = temp_dir;
					dirs = Directory.GetDirectories (start_dir, dir_to_find);

					if (dirs.Length == 1)
					{
						dir += dirs[0].Substring (dirs[0].LastIndexOf ("/") + 1);
					}
					else
						exists = false;
				}
			
				if (exists)
					return dir;
				else
					return "";
			}
			catch
			{
				return "";
			}
		}
		#endregion

		#region GENERAL_FUNCTIONS
		private void CheckEndOfLines ()
		{
			if (line_number >= espalda_lines.Length)
			{
				// lines
				for (int index = 0; index < espalda_lines.Length - 1; index++)
				{
					espalda_lines[index].TEXT = espalda_lines[index + 1].TEXT;
					espalda_lines[index].TYPE = espalda_lines[index + 1].TYPE;
				}
				espalda_lines[espalda_lines.Length - 1].TEXT = "";
				espalda_lines[espalda_lines.Length - 1].TYPE = EspaldaLine.EspaldaLineType.NONE_LINE;
				line_number = espalda_lines.Length - 1;
			}
		}

		private String CommonString (String str1, String str2)
		{
			int length = (str1.Length >= str2.Length)?str2.Length:str1.Length;
			String common_string = "";
			for (int index = 0; index < length; index++)
			{
				if (str1.Substring (index, 1).ToLower () == str2.Substring (index, 1).ToLower ())
				{
					common_string += str1.Substring (index, 1).ToLower ();
				}
				else
					break;
			}
			return common_string;
		}

		private String CommonStringBetweenTwoArrays (String[] arr1, String[] arr2)
		{
			String common_string = "";
			int ind = 0;
			if (arr1.Length > 0)
			{
				arr1[0] = arr1[0].Replace ("\\", "/");
				if (arr1[0].IndexOf ("/") != -1)
					common_string = arr1[0].Substring (arr1[0].IndexOf ("/") + 1);
				else
					common_string = arr1[0];
				while (ind + 1 < arr1.Length)
				{
					arr1[ind + 1] = arr1[ind + 1].Replace ("\\", "/");
					if (arr1[ind + 1].IndexOf ("/") != -1)
						common_string = CommonString (common_string, arr1[ind + 1].Substring (arr1[ind + 1].IndexOf ("/") + 1));
					else
						common_string = CommonString (common_string, arr1[ind + 1]);
					ind++;
				}
				ind = -1;
			}
			else
			{
				arr2[0] = arr2[0].Replace ("\\", "/");
				if (arr2[0].IndexOf ("/") != -1)
					common_string = arr2[0].Substring (arr2[0].IndexOf ("/") + 1);
				else
					common_string = arr2[0];
				ind = 0;
			}
			while (ind + 1 < arr2.Length)
			{
				arr2[ind + 1] = arr2[ind + 1].Replace ("\\", "/");
				if (arr2[ind + 1].IndexOf ("/") != -1)
					common_string = CommonString (common_string, arr2[ind + 1].Substring (arr2[ind + 1].IndexOf ("/") + 1));
				else
					common_string = CommonString (common_string, arr2[ind + 1]);
				ind++;
			}

			return common_string;
		}

		private String[] GetCommands (String search_criteria)
		{
			if (search_criteria == "")
				return commands;

			ArrayList arr = new ArrayList ();
			for (int index = 0; index < commands.Length; index++)
			{
				if ((commands[index].Length >= search_criteria.Length) && (commands[index].Substring (0, search_criteria.Length).ToLower () == search_criteria))
				{
					arr.Add (commands[index]);
				}
			}

			String[] ret_value = new String[arr.Count];
			for (int k = 0; k < arr.Count; k++)
				ret_value[k] = (String)arr[k];

			return ret_value;
		}

		private String GetOpenWith (String extension)
		{
			///
			/// Open with
			/// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.log
			/// If exists and value 'Application' and not null, then open with value data.
			/// Else
			/// HKEY_CLASSES_ROOT\.log
			/// Value by default
			/// HKEY_CLASSES_ROOT\(value by default)\shell
			/// Value by default, if null open\command else value\command
			/// HKEY_CLASSES_ROOT\(value by default)\shell\open\command
			/// =HKEY_CLASSES_ROOT\txtfile\shell\open\command
			/// value by default
			///
			String ret_value = "";

			RegistryKey key = Registry.CurrentUser;
			
			RegistryKey subkey = key.OpenSubKey ("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\." + extension);

			if (subkey != null)
			{
				String s = (String)subkey.GetValue("Application");

				subkey.Close ();
				if ((s != null) && (s != ""))
				{
					ret_value = s;
					return ret_value;
				}
			}

			key = Registry.ClassesRoot;
			subkey = key.OpenSubKey ("." + extension);

			if (subkey != null)
			{
				String s = (String)subkey.GetValue("");
				subkey.Close ();

				if ((s != null) && (s != ""))
				{
					String openarg = "open";
					String new_key_value = s;

					if (new_key_value != "")
					{
						subkey = key.OpenSubKey (new_key_value + "\\shell");
						if (subkey != null)
						{
							s = (String)subkey.GetValue("");
							subkey.Close ();
						
							if ((s != null) && (s != ""))
								openarg = s;
						}

						subkey = key.OpenSubKey (new_key_value + "\\shell\\" + openarg + "\\command");
						if (subkey != null)
						{
							s = (String)subkey.GetValue("");
							subkey.Close ();
						
							if ((s != null) && (s != ""))
								ret_value = s;
						}
					}
				}

			}

			return ret_value;
		}

		private void InitializeLinesText ()
		{
			for (int i = 0; i < max_lines_number; i++)
			{
				espalda_lines[i] = new EspaldaLine ();
				espalda_lines[i].TEXT = "";
				espalda_lines[i].TYPE = EspaldaLine.EspaldaLineType.NONE_LINE;
			}
			espalda_lines[0].TEXT = line_start + "_";
		}

		private void LineVisibilityCheck ()
		{
			if (line_number + 1 >= visible_start_line_number + max_visible_lines_number)
			{
				visible_start_line_number++;
				if (vScrollBar.Value < espalda_lines.Length - 1)
					vScrollBar.Value++;
				else
					vScrollBar.Value = espalda_lines.Length;
			}
		}

		private void NewOutputLine (String text)
		{
			CheckEndOfLines ();
			line_number++;
			espalda_lines[line_number - 1].TEXT = text;
			espalda_lines[line_number - 1].TYPE = EspaldaLine.EspaldaLineType.OUTPUT_LINE;
			LineVisibilityCheck ();
		}

		private void OpenFile (String directory, String openwith, String file)
		{
			Process p = null;
			try
			{
				p = new Process();
				p.StartInfo.WorkingDirectory = directory;
				
				String executable = "";
				if (openwith.IndexOf ("\"") != -1)
				{
					executable = openwith.Substring (1);
					executable = executable.Substring (0, executable.IndexOf ("\""));
				}
				else if (openwith.IndexOf ("%") != -1)
				{
					executable = openwith.Substring (0, openwith.IndexOf ("%"));
				}
				else
				{
					executable = openwith;
				}

				p.StartInfo.FileName = executable;

				p.StartInfo.Arguments = file;
																	
				p.Start ();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception Occurred :{0},{1}", 
					ex.Message,ex.StackTrace.ToString());
			}
		}

		private String ProcessNumberIntoSpaces (String str)
		{
			String s = str;
			int l = 0;
			while (l < s.Length)
			{
				if (s.Length - l > 3)
					s = s.Substring (0, s.Length - 3 * (l / 3 + 1) - (l / 3)) + " " + s.Substring (s.Length - 3 * (l / 3 + 1) - (l / 3));
				l += 3 + (l / 3);
			}
			return s;
		}

		#endregion

		#region CONTROLS_EVENTS
		private void vScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.Type == ScrollEventType.SmallIncrement)
			{
				new_scroll_value = e.NewValue;
			}
			else if (e.Type == ScrollEventType.SmallDecrement)
			{
				new_scroll_value = e.NewValue;
			}
			else if (e.Type == ScrollEventType.LargeIncrement)
			{
				new_scroll_value = e.NewValue / (max_visible_lines_number / 2);
			}
			else if (e.Type == ScrollEventType.LargeDecrement)
			{
				new_scroll_value = e.NewValue / (max_visible_lines_number / 2);
			}
			else if (e.Type == ScrollEventType.EndScroll)
			{
				visible_start_line_number = new_scroll_value + 1;
				this.Invalidate ();
				new_scroll_value = visible_start_line_number;
			}
		}

		#endregion

	}
}
