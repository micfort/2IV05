using OpenTK;
using OpenTK.Graphics;
using micfort.GHL;
using micfort.GHL.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CG_2IV05.Common;

namespace CG_2IV05.Visualize
{
    public class VBOLoader
    {
        private ConcurrentPriorityQueue<NodeWithData, int> loadQueue;
		public List<NodeWithData> VBOList { get; set; }

	    private micfort.GHL.ConsumerThread<NodeWithData> thread;
	    private OpenTK.Graphics.GraphicsContext context;

		public VBOLoader(List<NodeWithData> vboList)
        {
            loadQueue = new ConcurrentPriorityQueue<NodeWithData, int>(new PriorityQueue<NodeWithData, int>());
            this.VBOList = vboList;
			thread = new ConsumerThread<NodeWithData>(loadQueue, Handler);
			thread.ExceptionHandling = ExceptionHandling.ReportErrorContinue;
        }

	    private void Handler(NodeWithData data)
	    {
			if(context == null)
			{
				INativeWindow window = new NativeWindow();
				context = new GraphicsContext(GraphicsMode.Default, window.WindowInfo);
				context.MakeCurrent(window.WindowInfo);
			}
			data.loadNodeFromDisc();
		    lock (VBOList)
		    {
				VBOList.Add(data);    
		    }
	    }

	    public void Start()
        {
            thread.Start();
        }

        public void Stop()
        {
            thread.Stop();
        }

        public void enqueueNode(NodeWithData node, int priority = 0)
        {
            loadQueue.Enqueue(node, priority);
        }
    }
}
