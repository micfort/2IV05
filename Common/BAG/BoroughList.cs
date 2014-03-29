using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using OpenTK;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.BAG
{
    public class BoroughList
    {
        private Dictionary<String, List<Borough>> boroughs;
        private List<Borough> allBoroughs;
        private const string BOROUGH_FILENAME = "Boroughs.xml";
        private alglib.kdtree tree;

        public BoroughList()
        {
            boroughs = new Dictionary<string, List<Borough>>();
            allBoroughs = new List<Borough>();
        }

        public void loadBoroughLocations(HyperPointSerializable<float> center)
        {
            Console.Out.WriteLine("Reading Boroughs file: {0}", BOROUGH_FILENAME);
            using (Stream stream = File.OpenRead(BOROUGH_FILENAME))
            {
                XmlReader reader = XmlReader.Create(stream);
                while (reader.Read())
                {
                    if (reader.Name == "Borough")
                    {
                        Borough borough = Borough.loadBorough(reader);
                        borough.UpdateLocationByCenter(center.Value);
                        if (boroughs.ContainsKey(borough.Province))
                        {
                            boroughs[borough.Province].Add(borough);
                        }
                        else
                        {
                            boroughs.Add(borough.Province, new List<Borough>() { borough });
                        }
                        allBoroughs.Add(borough);
                    }
                }
            }

            allBoroughs.OrderBy(b => b.Name);
            initBoroughKDTree();
        }

        private void initBoroughKDTree()
        {
            double[,] points = new double[allBoroughs.Count(), 2];
            int[] tags = new int[allBoroughs.Count()];
            for (int i = 0; i < allBoroughs.Count(); i++)
            {
                points[i,0] = allBoroughs[i].Location.X;
                points[i,1] = allBoroughs[i].Location.Y;
                tags[i] = i;
            }
            alglib.kdtreebuildtagged(points, tags, allBoroughs.Count(), 2, 0, 0, out tree);
        }

        public List<String> getProvinces()
        {
            return boroughs.Keys.ToList();
        }

        public IEnumerable<Borough> getAllBoroughs()
        {
            return allBoroughs;
        }

        public IEnumerable<Borough> getBoroughsByProvince(string province)
        {
            return boroughs[province];
        }

        public IEnumerable<Borough> searchBoroughs(string filter, string province = "")
        {
            if(province.Equals(""))
            {
                return allBoroughs.Where(b => b.Name.ToLower().Contains(filter));
            }
            else
            {
                return boroughs[province].Where(b => b.Name.ToLower().Contains(filter));
            }
        }

        public String findNearestBorough(Vector3 position)
        {
            double[] point = new double[] { position.X, position.Y};
            int[] closestIndexes = new int[1];
            int k = alglib.kdtreequeryknn(tree, point, 1, false);
            if(k > 0)
            {
                alglib.kdtreequeryresultstags(tree, ref closestIndexes);
                return allBoroughs[closestIndexes[0]].Name;
            }

            return "";
        }
    }
}
