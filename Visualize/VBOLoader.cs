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
        private ConcurrentPriorityQueue<Node, int> nodeQueue;
        private ConcurrentBag<VBO> vboBag;

        private Thread thread;

        private bool running = false;
        
        public VBOLoader(ConcurrentBag<VBO> vboBag)
        {
            nodeQueue = new ConcurrentPriorityQueue<Node, int>(new PriorityQueue<Node, int>());
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
                if (nodeQueue.Count > 0)
                {
                    Node nextNode = nodeQueue.Dequeue();

                    VBO oldVBO;
                    bool taken = vboBag.TryTake(out oldVBO);
                    if (taken)
                    {
                        oldVBO.LoadData(nextNode.ReadRawData());
                        vboBag.Add(oldVBO);
                    }
                    else
                    {
                        nodeQueue.Enqueue(nextNode, 0);
                    }
                }
            }
        }

        public void enqueueNode(Node node, int priority = 0)
        {
            nodeQueue.Enqueue(node, priority);
        }
    }
}
