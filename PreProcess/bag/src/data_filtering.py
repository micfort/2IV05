import argparse #apt-get install python-argparse
import sys
import os
from postgresdb import Database
from log import Log
from bagconfig import BAGConfig

class ArgParser(argparse.ArgumentParser):
     def error(self, message):
        print (message)
        self.print_help()
        sys.exit(2)

class BagFilter():
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

    def filterBagData(self):
        self.connectDatabase()

        # Get list of addresses of VBOs in Eindhoven
        adressen = self.getEindhovenAdressen(True)
        print "Retrieved Adressen"
        # List of all VBO ids
        idsVBO = [row[0] for row in adressen]

        # Get vbo's and panden eindhoven
        vbos = self.getVBOByIdentificationList(idsVBO)
        vboPanden = self.getVBOPandenByIdentificationList(idsVBO)

        # Clear idsVBO for memory space
        idsVBO = []
        print "Retrieved VBOs"

        panden = [row[6] for row in vboPanden]
        panden = self.getPandenByIdentificationList(panden)
        print "Retrieved Panden"

        self.database.close()

        self.connectDatabase(True)

        #database.insertMultiple("adres", adressen)
        self.database.insertMultiple("verblijfsobject", vbos)
        self.database.insertMultiple("verblijfsobjectpand", vboPanden)
        self.database.insertMultiple("pand", panden)

        self.database.close()

    def connectDatabase(self, bag2=False):
        BAGConfig(self.args, bag2)

        self.database = Database()
        self.database.verbind()

    def getEindhovenAdressen(self, vboIdsOnly=False):

        # Get list adressen of VBOs in Eindhoven
        if vboIdsOnly:
            sql = "SELECT adresseerbaarobject " \
                                  "FROM adres " \
                                  "WHERE woonplaatsnaam LIKE 'Eindhoven'"
        else:
            sql = "SELECT * " \
                  "FROM adres " \
                  "WHERE woonplaatsnaam LIKE 'Eindhoven'"

        return self.database.selectQuery(sql)

    def getVBOByIdentificationList(self, idsVBO):
        sql = "Select * " \
              "FROM verblijfsobjectactuuelalles " \
              "WHERE identificatie IN %s"

        return self.database.selectQuery(sql, [tuple(idsVBO)])

    def getVBOPandenByIdentificationList(self, idsVBO):
        sql = "Select * " \
              "FROM verblijfsobjectpandactueel " \
              "WHERE identificatie IN %s"

        return self.database.selectQuery(sql, [tuple(idsVBO)])

    def getPandenByIdentificationList(self, pandIds):
        sql = "Select * " \
              "FROM pandactueelalles " \
              "WHERE identificatie IN %s"

        return self.database.selectQuery(sql, [tuple(pandIds)])


if __name__ == "__main__":
    bagFilter = BagFilter()
    bagFilter.filterBagData()

