using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.BAG
{
    public class Borough
    {
        private String name;
        public String Name { get { return name; } }

        private String province;
        public String Province { get { return province; } }

        private HyperPoint<float> location;
        public HyperPoint<float> Location { get { return location; } }

        public Borough(string name, string province, HyperPoint<float> location)
        {
            this.name = name;
            this.province = province;
            this.location = location;
        }

        public override string ToString()
        {
            return name;
        }

        public static Borough loadBorough(XmlReader reader)
        {
            String name = "";
            String province = "";
            HyperPoint<float> location = new HyperPoint<float>(0,0,0);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "Name")
                    {
                        name = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name == "Province")
                    {
                        province = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name == "LocationX")
                    {
                        location.X = reader.ReadElementContentAsFloat();
                    }
                    else if (reader.Name == "LocationY")
                    {
                        location.Y = reader.ReadElementContentAsFloat();
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "Borough")
                    {
                        return new Borough(name, province, location);
                    }
                }
            }
            return null;
        }

        public void UpdateLocationByCenter(HyperPoint<float> center)
        {
            this.location -= new HyperPoint<float>(center, 0);
        }
    }
}
