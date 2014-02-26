import argparse #apt-get install python-argparse
import sys
import os
from postgresdb import Database
from log import Log
from bagconfig import BAGConfig
from decimal import *

class ArgParser(argparse.ArgumentParser):
     def error(self, message):
        print (message)
        self.print_help()
        sys.exit(2)

class BagPreprocessor():
    def __init__(self):
        # All arguments required to be able to run configure configreader
        parser = ArgParser(description='bag-extract, commandline tool voor het extraheren en inlezen van BAG bestanden',
            epilog="Configureer de database in extract.conf of geef eigen versie van extract.conf via -f of geef parameters via commando regel expliciet op")
        parser.add_argument('-f', '--config', metavar='<bestand>', help='gebruik dit configuratiebestand i.p.v. extract.conf')
        parser.add_argument('-a', '--config2', metavar='<bestand>', help='gebruik dit configuratiebestand i.p.v. extract.conf')
        parser.add_argument('-c', '--dbinit', action='store_true', help='verwijdert (DROP TABLE) alle tabellen en maakt (CREATE TABLE) nieuwe tabellen aan')
        parser.add_argument('-d', '--database', metavar='<naam>', help='geef naam van de database')
        parser.add_argument('-s', '--schema', metavar='<naam>', help='geef naam van het database schema')
        parser.add_argument('-q', '--query', metavar='<bestand>', help='voer database bewerkingen uit met opgegeven SQL bestand')
        parser.add_argument('-e', '--extract', metavar='<naam>', help='importeert of muteert de database met gegeven BAG-bestand of -directory')
        parser.add_argument('-H', '--host', metavar='<hostnaam of -adres>', help='verbind met de database op deze host')
        parser.add_argument('-U', '--username', metavar='<naam>', help='verbind met database met deze gebruikersnaam')
        parser.add_argument('-p', '--port', metavar='<poort>', help='verbind met database naar deze poort')
        parser.add_argument('-W', '--password', metavar='<paswoord>', help='gebruikt dit wachtwoord voor database gebruiker')
        parser.add_argument('-w', '--no-password', action='store_true', help='gebruik geen wachtwoord voor de database verbinding')
        parser.add_argument('-v', '--verbose', action='store_true', help='toon uitgebreide informatie tijdens het verwerken')
        parser.add_argument('-D', '--dbinitcode', action='store_true', help='createert een lijst met statements om het DB script aan te passen')


        # Initialiseer
        self.args = parser.parse_args()

        # Initialize singleton Log object so we can use one global instance
        Log(self.args)

        self.database = None


    def processData(self):
        self.connectDatabase(True);

        vbos = self.getVBOs()
        vboHeightDict = {}
        vbosIds = []
        for vbo in vbos:
            vbosIds.append(vbo[0])
            vboHeightDict[vbo[0]] = vbo[1]

        buildingVBOs = self.getVBOPandenByIdentificationList(vbosIds)
        buildingIds = []
        totalAreaDict = {}
        for vboBuilding in buildingVBOs:
            if totalAreaDict.has_key(vboBuilding[1]):
                totalAreaDict[vboBuilding[1]] += vboHeightDict[vboBuilding[0]]
            else:
                totalAreaDict[vboBuilding[1]] = vboHeightDict[vboBuilding[0]]

            buildingIds.append(vboBuilding[1])

        buildings = self.getPandenByIdentificationList(buildingIds);

        buildingObjectList = []
        for building in buildings:
            gmlBase64String = building[1].encode('base64')
            buildingObjectList.append(BuildingInfo(building[0], gmlBase64String, building[2], totalAreaDict[building[0]], building[3]))

        # Write mode creates a new file or overwrites the existing content of the file.
        # Write mode will _always_ destroy the existing contents of a file.
        try:
            # This will create a new file or **overwrite an existing file**.
            f = open("buildings.xml", "w")
            try:
                xml = "<?xml version='1.0' encoding='UTF-8'?>\n";
                xml += "<buildings>\n"
                for building in  buildingObjectList:
                    xml += building.toXML()

                xml += "</buildings>"

                f.write(xml) # Write a string to a file
            finally:
                f.close()

        except IOError:
            pass

    def connectDatabase(self, bag2=False):
        BAGConfig(self.args, bag2)

        self.database = Database()
        self.database.verbind()

    def getVBOs(self):
        sql = "Select identificatie, oppervlakteverblijfsobject " \
              "FROM verblijfsObject"

        return self.database.selectQuery(sql)

    def getVBOPandenByIdentificationList(self, idsVBO):
        sql = "Select identificatie, gerelateerdpand " \
              "FROM verblijfsobjectpandactueel " \
              "WHERE identificatie IN %s"

        return self.database.selectQuery(sql, [tuple(idsVBO)])

    def getPandenByIdentificationList(self, pandIds):
        sql = "Select identificatie, st_asgml(geovlak), st_area(geovlak), st_npoints(geovlak) " \
              "FROM pand " \
              "WHERE identificatie IN %s"

        return self.database.selectQuery(sql, [tuple(pandIds)])

class BuildingInfo():
    def __init__(self, identification, gmlBse64, baseSurfaceArea, totalSurfaceArea, numPoints):
        self.gmlBase64String = gmlBse64;
        self.identification = identification;
        self.baseSurfaceArea = baseSurfaceArea
        self.totalSurfaceArea = totalSurfaceArea
        self.numPoints = numPoints;

        # Neem aan hoogte van 2.5m per verdieping
        self.height = (totalSurfaceArea / Decimal(baseSurfaceArea)) * Decimal(2.5)

    def toXML(self):
        return "    <Building id='%d'>\n" \
               "        <height>%s</height>\n" \
               "        <gmlbase64>%s</gmlbase64>\n" \
               "    </Building>\n" % (self.identification, format(self.height, '.2f'), self.gmlBase64String)

if __name__ == "__main__":
    bagFilter = BagPreprocessor()
    bagFilter.processData()