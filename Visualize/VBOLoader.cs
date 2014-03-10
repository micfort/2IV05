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
    class VBOLoader
    {
        private ConcurrentPriorityQueue<NodeLoadItem, int> loadQueue;
        private ConcurrentBag<VBO> vboBag;

        private Thread thread;

        private bool running = false;
        
        public VBOLoader(ConcurrentBag<VBO> vboBag)
        {
            loadQueue = new ConcurrentPriorityQueue<NodeLoadItem, int>(new PriorityQueue<NodeLoadItem, int>());
            this.vboBag = vboBag;
            thread = new Thread(run);
        }

        public Thread start()
        {
            if (!thread.IsAlive)
            {
                running = true;
                thread.Start();
            }
            return thread;
        }

        public void stop(bool waitForStop = false)
        {
            if (thread.IsAlive)
            {
                running = false;
                if (waitForStop)
                    thread.Join();
            }
        }

        public void run()
        {
            while (running)
            {
                if (loadQueue.Count > 0)
                {
                    NodeLoadItem nextNode = loadQueue.Dequeue();
                    VBO reloadedVBO = nextNode.loadNodeFromDisc();
                    vboBag.Add(reloadedVBO);
                }
            }
        }

        public void enqueueNode(NodeLoadItem node, int priority = 0)
        {
            loadQueue.Enqueue(node, priority);
        }
    }
}
