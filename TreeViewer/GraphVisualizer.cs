using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace TreeViewer
{
	public enum Renderer
	{
		//Dot=1,
		Default,
		Neato,
		Fdp,
		Twopi,
		Circo,
	}

	class GraphVisualiser : IDotEngine
	{
		private string _graphvizBin = @"GraphViz\bin\dot.exe";
		private string _outputType = "png";
		private Renderer _rendererType = Renderer.Default;

		public Renderer RendererType
		{
			get { return _rendererType; }
			set { _rendererType = value; }
		}

		public string GraphvizBin
		{
			get { return _graphvizBin; }
			set { _graphvizBin = value; }
		}

		public string OutputType
		{
			get { return _outputType; }
			set { _outputType = value; }
		}

		public bool OutputFile { get; set; }

		public Image OutputImage { get; private set; }

		#region Implementation of IDotEngine

		public string Run(GraphvizImageType imageType, string dot, string outputFileName)
		{
			string outputFileNameWithExt = string.Format("{0}.{1}", outputFileName, _outputType);
			string parameters = "";

			parameters = string.Format("{0} -T{1}", parameters, _outputType);
			if (OutputFile == true)
			{
				parameters = string.Format("{0} \"-o{1}\"", parameters, outputFileNameWithExt);
			}
			if (RendererType != Renderer.Default)
			{
				parameters = string.Format("{0} -K{1}", parameters, Enum.GetName(typeof(Renderer), RendererType).ToLower());
			}

			ProcessStartInfo pinfo = new ProcessStartInfo(GraphvizBin, parameters);
			pinfo.UseShellExecute = false;
			pinfo.RedirectStandardInput = true;
			pinfo.RedirectStandardOutput = true;
			pinfo.RedirectStandardError = true;
			pinfo.WindowStyle = ProcessWindowStyle.Hidden;
			pinfo.CreateNoWindow = true;
			Process gvizProc = Process.Start(pinfo);
			gvizProc.StandardInput.Write(dot);
			gvizProc.StandardInput.Close();

			if(OutputFile == false)
			{
				Image image = Image.FromStream(gvizProc.StandardOutput.BaseStream);
				OutputImage = image;
			}
			string error = gvizProc.StandardError.ReadToEnd();
			//if (!string.IsNullOrWhiteSpace(error))
			//	throw new Exception(error);
			gvizProc.WaitForExit();
			return string.Empty;
		}

		#endregion

	}
}
